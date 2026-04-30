using Azure.Core;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using pbl3_web_api.DTO.Order;
using pbl3_web_api.Entities;
using pbl3_web_api.Repository;
using QuestPDF.Fluent;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace pbl3_web_api.Service
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepo _orderRepo;
        private readonly ICartRepository _cartRepo;
        private readonly IProductRepo _productRepo;
        public OrderService(IOrderRepo orderRepo, ICartRepository cartRepo, IProductRepo productRepo)
        {
            _orderRepo = orderRepo;
            _cartRepo = cartRepo;
            _productRepo = productRepo;
        }

        public async Task<(IEnumerable<AdminOrdersResponse> orders, int totalCount)> GetOrdersAsync(
            int page,
            int pageSize,
            string? search,
            string? status,
            string? paymentStatus,
            DateTime? fromDate,
            DateTime? toDate,
            string? sortOrder)
        {
            var baseQuery = _orderRepo.GetOrdersQuery();

            var filteredQuery = ApplyFilters(baseQuery, search, status, paymentStatus, fromDate, toDate, sortOrder);

            var totalCount = await filteredQuery.CountAsync();

            var data = await GetPageOrdersAsync(filteredQuery, page, pageSize);

            return (data, totalCount);
        }

        public async Task<AdminOrderDetailResponse> GetOrderDetailAsync(int orderId)
        {
            ValidateId(orderId, "OrderId");

            var order = await _orderRepo.GetOrderByIdAsync(orderId)
                ?? throw new KeyNotFoundException("Order not found.");

            return MapToAdminOrderDetailResponse(order);
        }

        public async Task UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            ValidateId(orderId, "OrderId");

            var order = await _orderRepo.GetOrderByIdAsync(orderId)
                ?? throw new KeyNotFoundException("Order not found.");

            order.Status = newStatus;

            if (newStatus == "Delivered")
                order.PaymentStatus = "Paid";

            order.UpdatedAt = DateTime.UtcNow;
            await _orderRepo.SaveAsync();
        }

        public async Task UpdatePaymentStatusAsync(int orderId, string newStatus)
        {
            ValidateId(orderId, "OrderId");

            var order = await _orderRepo.GetOrderByIdAsync(orderId)
                ?? throw new KeyNotFoundException("Order not found.");

            order.PaymentStatus = newStatus;
            order.UpdatedAt = DateTime.UtcNow;
            await _orderRepo.SaveAsync();
        }

        public async Task<(IEnumerable<HistoryOrderResponse> order, int totalCount)> GetHistoryOrdersAsync(
            int userId,
            int page, 
            int pageSize,
            string? status)
        {
            if (page < 0) page = 1;
            if (pageSize < 0) pageSize = 12;

            var baseQuery = _orderRepo.GetOrdersQuery();

            baseQuery = baseQuery.OrderByDescending(s => s.OrderId);

            var filteredQuery = ApplyUserFilters(baseQuery, userId, status);

            var totalCount = await filteredQuery.CountAsync();

            var data = await GetHistoryOrdersPageAsync(filteredQuery, page, pageSize);

            return (data, totalCount);
        }

        public async Task<MyOrderDetailResponse> GetMyOrderDetailAsync(int userId, int orderId)
        {
            ValidateId(orderId, "OrderId");
            ValidateId(userId, "UserId");

            var order = await _orderRepo.GetOrderByIdAndUserIdAsync(userId, orderId)
                ?? throw new KeyNotFoundException("Order not found."); 

            return MapToMyOrderDetailResponse(order);
        }
        
        public async Task CancelOrderAsync(int userId, int orderId)
        {
            ValidateId(orderId, "OrderId");
            ValidateId(userId, "UserId");

            var order = await _orderRepo.GetOrderByIdAndUserIdAsync(userId, orderId)
                ?? throw new KeyNotFoundException("Order not found."); 

            if (!order.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Only pending orders can be canceled.");

            order.Status = "Cancelled";
            order.UpdatedAt = DateTime.UtcNow;
            await _orderRepo.SaveAsync();
        }

        public async Task<CreateOrderResponse> CreateOrderAsync(int userId, CreateOrderRequest request)
        {
            ValidateId(userId, "UserId");

            if (request == null)
                throw new ArgumentNullException(nameof(request), "Request cannot be null.");

            var items = await _orderRepo.GetCartItemsByUserIdAsync(userId);
            Console.WriteLine(items);

            if (items == null || !items.Any())
                throw new InvalidOperationException("Cart is empty.");

            var order = MapToOrder(userId, request);
            order.TotalAmount = request.shippingFee ?? 0;

            using var transaction = await _orderRepo.BeginTransactionAsync();

            try
            {
                await _orderRepo.AddOrderAsync(order); 

                await AddOrderDetailsAsync(order, items); 

                await _orderRepo.SaveAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }


            return new CreateOrderResponse
            {
                message = "Order created successfully.",
                orderId = order.OrderId,
                totalAmount = order.TotalAmount,
            };
        }

        public async Task<byte[]> ExportOrderPdfAsync(int orderId)
        {
            var order = await _orderRepo.GetOrderByIdAsync(orderId)
                ?? throw new KeyNotFoundException("Order not found");

            if (order.Status != "Processing") throw new InvalidOperationException("Đơn hàng không thể xuất pdf");

            var userName = order.User?.FullName ?? "Không có";
            var shippingAddress = order.ShippingAddress ?? "Không có";
            var phoneNumber = order.User?.PhoneNumber ?? "Không có";
            var orderDetails = order.OrderDetails ?? Enumerable.Empty<OrderDetail>();

            var pdf = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);

                    // Header phiếu vận chuyển
                    page.Header().Column(header =>
                    {
                        header.Item().Text("PHIẾU VẬN CHUYỂN").FontSize(18).Bold().AlignCenter();
                        header.Item().Text($"Mã đơn: {order.OrderId}").FontSize(12).AlignCenter();
                        header.Item().Text($"Ngày: {order.OrderDate:dd/MM/yyyy}").FontSize(12).AlignCenter();
                        header.Spacing(5);
                    });

                    // Footer: số trang
                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Trang ");
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });

                    // Nội dung chính
                    page.Content().Column(col =>
                    {
                        col.Spacing(5); // khoảng cách giữa các item

                        col.Item().Text($"Khách hàng: {userName}").FontSize(12);
                        col.Item().Text($"Địa chỉ: {shippingAddress}").FontSize(12);
                        col.Item().Text($"SDT: {phoneNumber}").FontSize(12);

                        col.Item().LineHorizontal(1); // dòng phân cách

                        // Bảng sản phẩm
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();      // Tên sản phẩm
                                columns.ConstantColumn(50);   // Số lượng
                                columns.ConstantColumn(80);   // Giá
                                columns.ConstantColumn(100);  // Thành tiền
                            });

                            // Header bảng
                            table.Header(header =>
                            {
                                header.Cell().Border(1).Padding(5).Text("Sản phẩm").Bold();
                                header.Cell().Border(1).Padding(5).Text("SL").Bold().AlignCenter();
                                header.Cell().Border(1).Padding(5).Text("Giá").Bold().AlignRight();
                                header.Cell().Border(1).Padding(5).Text("Thành tiền").Bold().AlignRight();
                            });

                            // Dữ liệu sản phẩm
                            foreach (var item in orderDetails)
                            {
                                table.Cell().Border(1).Padding(5).Text(item.Product?.ProductName ?? "N/A");
                                table.Cell().Border(1).Padding(5).AlignCenter().Text(item.Quantity.ToString());
                                table.Cell().Border(1).Padding(5).AlignRight().Text((item.Product?.Price ?? 0).ToString("N0") + " VNĐ");
                                table.Cell().Border(1).Padding(5).AlignRight().Text((item.Subtotal).ToString("N0") + " VNĐ");
                            }
                        });

                        col.Item().LineHorizontal(1); // phân cách cuối bảng

                        // Tổng tiền
                        col.Item().AlignRight()
                            .Text($"Tổng: {(order.TotalAmount):N0} VNĐ")
                            .Bold()
                            .FontSize(14);

                        // Trạng thái thanh toán
                        if (order.PaymentStatus == "Unpaid")
                        {
                            col.Item().AlignRight()
                            .Text($"Chưa thanh toán")
                            .Bold()
                            .FontSize(12);
                        }
                        else
                        {
                            col.Item().AlignRight()
                            .Text($"Đã thanh toán")
                            .Bold()
                            .FontSize(12);
                        }
                        col.Item().Text(" ");
                        // Lời cảm ơn
                        col.Item().AlignLeft().Text("Cảm ơn quý khách đã mua hàng!").FontSize(12).Italic();
                    });
                });
            }).GeneratePdf();

            return pdf;
        }

        private IQueryable<Order> ApplyFilters(
            IQueryable<Order> query,
            string? search,
            string? status,
            string? paymentStatus,
            DateTime? fromDate,
            DateTime? toDate,
            string? sortOrder)
        {

            if (!string.IsNullOrWhiteSpace(search))
            {
                var keyword = $"%{search.Trim()}%";

                query = query.Where(p =>
                    EF.Functions.Like(p.OrderId.ToString(), keyword) ||
                    EF.Functions.Like(p.UserId.ToString(), keyword) ||
                    EF.Functions.Like(p.User.PhoneNumber, keyword)
                );
            }

            if (!string.IsNullOrEmpty(status))
                query = query.Where(o => o.Status.ToLower() == status.ToLower());

            if (!string.IsNullOrEmpty(paymentStatus))
                query = query.Where(o => o.PaymentStatus.ToLower() == paymentStatus.ToLower());

            if (fromDate.HasValue)
                query = query.Where(o => o.OrderDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(o => o.OrderDate <= toDate.Value);

            if (!string.IsNullOrEmpty(sortOrder))
            {
                query = sortOrder switch
                {
                    "asc" => query.OrderBy(p => p.OrderDate),
                    "desc" => query.OrderByDescending(p => p.OrderDate),
                    _ => query
                };
            }

            return query;
        }

        private async Task<List<AdminOrdersResponse>> GetPageOrdersAsync(IQueryable<Order> query, int page, int pageSize)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 20;

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new AdminOrdersResponse
                {
                    orderId = o.OrderId,
                    userId = o.UserId,
                    fullName = o.User.FullName,
                    orderDate = o.OrderDate,
                    totalAmount = o.TotalAmount,
                    status = o.Status,
                    paymentStatus = o.PaymentStatus
                })
                .ToListAsync();
        }

        private IQueryable<Order> ApplyUserFilters(IQueryable<Order> query, int userId, string? status)
        {
            query = query.Where(o => o.UserId == userId);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(o => o.Status.ToLower() == status.ToLower());

            return query;
        }

        private async Task<List<HistoryOrderResponse>> GetHistoryOrdersPageAsync(IQueryable<Order> query, int page, int pageSize)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new HistoryOrderResponse
                {
                    orderId = o.OrderId,
                    orderDate = o.OrderDate,
                    totalAmount = o.TotalAmount,
                    status = o.Status,
                    PaymentStatus = o.PaymentStatus
                })
                .ToListAsync();
        }

        private void ValidateId(int id, string name)
        {
            if (id <= 0)
                throw new ArgumentException($"{name} must be a non-negative integer.");
        }

        private AdminOrderDetailResponse MapToAdminOrderDetailResponse(Order order)
        {
            var items = order.OrderDetails.Select(i => new OrderDetailItem
                {
                    imageURL = i.Product.ImageUrl,
                    productId = i.ProductId,
                    productName = i.Product.ProductName,
                    quantity = i.Quantity,
                    unitPrice = i.UnitPrice,
                    subtotal = i.Subtotal,
                    productCode = i.Product.ProductCode,
                })
                .ToList();


            return new AdminOrderDetailResponse
            {
                orderId = order.OrderId,
                userId = order.UserId,
                userName = order.User.FullName,
                phoneNumber = order.User?.PhoneNumber,
                orderDate = order.OrderDate,
                totalAmount = order.TotalAmount,
                status = order.Status,
                shippingAddress = order.ShippingAddress,
                paymentMethod = order.PaymentMethod,
                paymentStatus = order.PaymentStatus,
                notes = order.Notes,
                updatedAt = order.UpdatedAt,
                orderDetails = items
            };
        }


        private MyOrderDetailResponse MapToMyOrderDetailResponse(Order order)
        {
            var items = order.OrderDetails.Select(i => new OrderDetailItem
            {
                productId = i.ProductId,
                productName = i.Product.ProductName,
                quantity = i.Quantity,
                unitPrice = i.UnitPrice,
                subtotal = i.Subtotal,
                imageURL = i.Product.ImageUrl
            }).ToList();

            return new MyOrderDetailResponse
            {
                orderId = order.OrderId,
                orderDate = order.OrderDate,
                totalAmount = order.TotalAmount,
                status = order.Status,
                shippingAddress = order.ShippingAddress,
                paymentMethod = order.PaymentMethod,
                paymentStatus = order.PaymentStatus,
                notes = order.Notes,
                updatedAt = order.UpdatedAt,
                orderDetails = items
            };
        }

        private Order MapToOrder(int userId, CreateOrderRequest request)
        {
            return new Order
            {
                UserId = userId,
                ShippingAddress = request.shippingAddress,
                PaymentMethod = request.paymentMethod,
                Notes = request.notes,
                OrderDate = DateTime.UtcNow,
                Status = "Pending",
                PaymentStatus = "Unpaid",
                TotalAmount = 0,
                UpdatedAt = DateTime.UtcNow,
            };
        }

        private async Task AddOrderDetailsAsync(Order order, IEnumerable<Cart> items)
        {
            var orderDetails = items.Select(item => new OrderDetail
            {
                OrderId = order.OrderId,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.Product.Price,
                Subtotal = item.Quantity * item.Product.Price
            }).ToList();

            order.TotalAmount += orderDetails.Sum(x => x.Subtotal);

            foreach (var item in items)
            {
                if (item.Product.StockQuantity < item.Quantity)
                    throw new InvalidOperationException($"{item.Product.ProductName} không đủ hàng :)))");

                item.Product.StockQuantity -= item.Quantity;
            }
            await _productRepo.UpdateProductsAsync(items.Select(i => i.Product).ToList());
            await _cartRepo.DeleteRange2Async(items);
            await _orderRepo.AddRangeOrderDetailsAsync(orderDetails);
        }
    }
}
