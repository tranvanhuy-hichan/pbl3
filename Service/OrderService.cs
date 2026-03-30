using Azure.Core;
using Microsoft.EntityFrameworkCore;
using pbl3_web_api.DTO.Order;
using pbl3_web_api.Entities;
using pbl3_web_api.Repository;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace pbl3_web_api.Service
{
    public interface IOrderService
    {
        Task<(IEnumerable<AdminOrdersResponse> orders, int totalCount)> GetOrdersAsync(
            int page,
            int pageSize,
            string? status,
            string? paymentStatus,
            int? userId,
            DateTime? fromDate,
            DateTime? toDate);
        Task<AdminOrderDetailResponse> GetOrderDetailAsync(int orderId);
        Task UpdateOrderStatusAsync(int orderId, string newStatus);
        Task<(IEnumerable<HistoryOrderResponse> order, int totalCount)> GetHistoryOrdersAsync(
            int userId,
            int page,
            int pageSize,
            string? status);
        Task<MyOrderDetailResponse> GetMyOrderDetailAsync(int userId, int orderId);
        Task CancelOrderAsync(int userId, int orderId);
        Task<CreateOrderResponse> CreateOrderAsync(int userId, CreateOrderRequest request);
    }

    public class OrderService : IOrderService
    {
        public readonly IOrderRepo _repo;
        public OrderService(IOrderRepo repo)
        {
            _repo = repo;
        }

        public async Task<(IEnumerable<AdminOrdersResponse> orders, int totalCount)> GetOrdersAsync(
            int page,
            int pageSize,
            string? status,
            string? paymentStatus,
            int? userId,
            DateTime? fromDate,
            DateTime? toDate)
        {
            var baseQuery = _repo.GetOrdersQuery();

            var filteredQuery = ApplyFilters(baseQuery, status, paymentStatus, userId, fromDate, toDate);

            var totalCount = await filteredQuery.CountAsync();

            var data = await GetPageOrdersAsync(filteredQuery, page, pageSize);

            return (data, totalCount);
        }

        public async Task<AdminOrderDetailResponse> GetOrderDetailAsync(int orderId)
        {
            ValidateId(orderId, "OrderId");

            var order = await _repo.GetOrderByIdAsync(orderId)
                ?? throw new KeyNotFoundException("Order not found.");

            return MapToAdminOrderDetailResponse(order);
        }

        public async Task UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            ValidateId(orderId, "OrderId");

            var order = await _repo.GetOrderByIdAsync(orderId)
                ?? throw new KeyNotFoundException("Order not found.");

            order.Status = newStatus;
            order.UpdatedAt = DateTime.UtcNow;
            await _repo.SaveAsync();
        }

        public async Task<(IEnumerable<HistoryOrderResponse> order, int totalCount)> GetHistoryOrdersAsync(
            int userId,
            int page, 
            int pageSize,
            string? status)
        {
            if (page < 0) page = 1;
            if (pageSize < 0) pageSize = 12;

            var baseQuery = _repo.GetOrdersQuery();

            var filteredQuery = ApplyUserFilters(baseQuery, userId, status);

            var totalCount = await filteredQuery.CountAsync();

            var data = await GetHistoryOrdersPageAsync(filteredQuery, page, pageSize);

            return (data, totalCount);
        }

        public async Task<MyOrderDetailResponse> GetMyOrderDetailAsync(int userId, int orderId)
        {
            ValidateId(orderId, "OrderId");
            ValidateId(userId, "UserId");

            var order = await _repo.GetOrderByIdAndUserIdAsync(userId, orderId)
                ?? throw new KeyNotFoundException("Order not found."); ;

            return MapToMyOrderDetailResponse(order);
        }
        
        public async Task CancelOrderAsync(int userId, int orderId)
        {
            ValidateId(orderId, "OrderId");
            ValidateId(userId, "UserId");

            var order = await _repo.GetOrderByIdAndUserIdAsync(userId, orderId)
                ?? throw new KeyNotFoundException("Order not found."); ;

            if (!order.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Only pending orders can be canceled.");

            order.Status = "Cancel";
            order.UpdatedAt = DateTime.UtcNow;
            await _repo.SaveAsync();
        }

        public async Task<CreateOrderResponse> CreateOrderAsync(int userId, CreateOrderRequest request)
        {
            ValidateId(userId, "UserId");

            if (request == null)
                throw new ArgumentNullException(nameof(request), "Request cannot be null.");

            var items = await _repo.GetCartItemsByUserIdAsync(userId);

            if (items == null || !items.Any())
                throw new InvalidOperationException("Cart is empty.");

            var order = MapToOrder(userId, request);

            using var transaction = await _repo.BeginTransactionAsync();
            try
            {
                await _repo.AddOrderAsync(order);
                await AddOrderDetailsAsync(order, items);
                await _repo.SaveAsync();

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

        private IQueryable<Order> ApplyFilters(
            IQueryable<Order> query,
            string? status,
            string? paymentStatus,
            int? userId,
            DateTime? fromDate,
            DateTime? toDate)
        {
            if (!string.IsNullOrEmpty(status))
                query = query.Where(o => o.Status.ToLower() == status.ToLower());

            if (!string.IsNullOrEmpty(paymentStatus))
                query = query.Where(o => o.PaymentStatus.ToLower() == paymentStatus.ToLower());

            if (userId.HasValue)
                query = query.Where(o => o.UserId == userId.Value);

            if (fromDate.HasValue)
                query = query.Where(o => o.OrderDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(o => o.OrderDate <= toDate.Value);

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
                subtotal = i.Subtotal
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

            order.TotalAmount = orderDetails.Sum(x => x.Subtotal);

            await _repo.AddRangeOrderDetailsAsync(orderDetails);
        }
    }
}
