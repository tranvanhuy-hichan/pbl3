using Microsoft.EntityFrameworkCore;
using pbl3_web_api.DTO.Order;
using pbl3_web_api.Entities;
using pbl3_web_api.Repository;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace pbl3_web_api.Service
{
    public interface IOrderService
    {
        Task<(IEnumerable<AdminOrdersResponse> orders, int totalCount)> GetOrders(
            int page,
            int pageSize,
            string? status,
            int? userId,
            DateTime? fromDate,
            DateTime? toDate);
        Task<AdminOrderDetailResponse> GetOrderDetail(int orderId);
        Task UpdateOrderStatus(int orderId, string newStatus);
        Task<(IEnumerable<HistoryOrderResponse> order, int totalCount)> GetHistoryOrders(
            int userId,
            int page,
            int pageSize,
            string? status);
        Task<MyOrderDetailResponse> GetMyOrderDetail(int userId, int orderId);
        Task CancelOrder(int userId, int orderId);
        Task<CreateOrderResponse> CreateOrder(int userId, CreateOrderRequest request);
    }

    public class OrderService : IOrderService
    {
        public readonly IOrderRepo _repo;
        public OrderService(IOrderRepo repo)
        {
            _repo = repo;
        }

        public async Task<(IEnumerable<AdminOrdersResponse> orders, int totalCount)> GetOrders(
            int page,
            int pageSize,
            string? status,
            int? userId,
            DateTime? fromDate,
            DateTime? toDate)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 12;

            var querys = _repo.GetOrdersQuery();

            if (!string.IsNullOrEmpty(status))
                querys = querys.Where(o => o.Status.Equals(status, StringComparison.OrdinalIgnoreCase));

            if (userId.HasValue)
                querys = querys.Where(o => o.UserId == userId.Value);

            if (fromDate.HasValue)
                querys = querys.Where(o => o.OrderDate >= fromDate.Value);

            if (toDate.HasValue)
                querys = querys.Where(o => o.OrderDate <= toDate.Value);

            var totalCount = await querys.CountAsync();

            var data = await querys
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new AdminOrdersResponse
                {
                    orderId = o.OrderId,
                    userId = o.UserId,
                    userName = o.User.Username,
                    orderDate = o.OrderDate,
                    totalAmount = o.TotalAmount,
                    status = o.Status,
                })
                .ToListAsync();
            return (data, totalCount);
        }

        public async Task<AdminOrderDetailResponse> GetOrderDetail(int orderId)
        {
            if (orderId < 0)
                throw new ArgumentException("OderId must be a non-negative integer.");

            var order = await _repo.GetOrderById(orderId);

            if (order == null)
                throw new KeyNotFoundException("Order not found.");

            List<OrderDetailItem> items = new List<OrderDetailItem>();
            foreach (OrderDetail i in order.OrderDetails)
            {
                var item = new OrderDetailItem
                {
                    productId = i.ProductId,
                    productName = i.Product.ProductName,
                    quantity = i.Quantity,
                    unitPrice = i.UnitPrice,
                    subtotal = i.Subtotal
                };
                items.Add(item);
            }

            var orderDetails = new AdminOrderDetailResponse
            {
                orderId = order.OrderId,
                userId = order.UserId,
                userName = order.User.Username,
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
            return (orderDetails);
        }

        public async Task UpdateOrderStatus(int orderId, string newStatus)
        {
            if (orderId < 0)
                throw new ArgumentException("OderId must be a non-negative integer.");

            var order = await _repo.GetOrderById(orderId);

            if (order == null)
                throw new KeyNotFoundException("Order not found.");

            order.Status = newStatus;
            order.UpdatedAt = DateTime.UtcNow;
            await _repo.Save();
        }

        public async Task<(IEnumerable<HistoryOrderResponse> order, int totalCount)> GetHistoryOrders(
            int userId,
            int page, 
            int pageSize,
            string? status)
        {
            if (page < 0) page = 1;
            if (pageSize < 0) pageSize = 12;

            var querys = _repo.GetOrdersQuery();

            querys = querys.Where(o => o.UserId == userId);

            if (!string.IsNullOrEmpty(status))
                querys = querys.Where(o => o.Status.Equals(status, StringComparison.OrdinalIgnoreCase));

            var totalCount = await querys.CountAsync();

            var data = await querys
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
            return (data, totalCount);
        }

        public async Task<MyOrderDetailResponse> GetMyOrderDetail(int userId, int orderId)
        {
            if (orderId < 0)
                throw new ArgumentException("OderId must be a non-negative integer.");

            if (userId < 0)
                throw new ArgumentException("UserId must be a non-negative integer.");

            var order = await _repo.GetOrderByIdAndUserId(userId, orderId);

            if (order == null)
                throw new KeyNotFoundException("Order not found.");

            List<OrderDetailItem> items = new List<OrderDetailItem>();
            foreach (OrderDetail i in order.OrderDetails)
            {
                var item = new OrderDetailItem
                {
                    productId = i.ProductId,
                    productName = i.Product.ProductName,
                    quantity = i.Quantity,
                    unitPrice = i.UnitPrice,
                    subtotal = i.Subtotal
                };
                items.Add(item);
            }
            var orderDetails = new MyOrderDetailResponse
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
            return (orderDetails);
        }

        public async Task CancelOrder(int userId, int orderId)
        {
            if (orderId < 0)
                throw new ArgumentException("OderId must be a non-negative integer.");

            if (userId < 0)
                throw new ArgumentException("UserId must be a non-negative integer.");

            var order = await _repo.GetOrderByIdAndUserId(userId, orderId);

            if (order == null)
                throw new KeyNotFoundException("Order not found.");

            if (!order.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Only pending orders can be canceled.");

            order.Status = "Cancel";
            order.UpdatedAt = DateTime.UtcNow;
            await _repo.Save();
        }

        public async Task<CreateOrderResponse> CreateOrder(int userId, CreateOrderRequest request)
        {
            if (userId < 0)
                throw new ArgumentException("UserId must be a non-negative integer.");

            if (request == null)
                throw new ArgumentNullException(nameof(request), "Request cannot be null.");

            var items = await _repo.GetCartItemsByUserId(userId);

            if (items == null || !items.Any())
                throw new InvalidOperationException("Cart is empty.");

            var order = new Order
            {
                UserId = userId,
                ShippingAddress = request.shippingAddress,
                PaymentMethod = request.paymentMethod,
                Notes = request.notes,
                OrderDate = DateTime.Now,
                Status = "Pending",
                PaymentStatus = "Unpaid",
                TotalAmount = 0,
                UpdatedAt = DateTime.Now,
            };
    
            await _repo.Add(order);

            foreach (var item in items)
            {
                var orderDetail = new OrderDetail
                {
                    OrderId = order.OrderId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.Price,
                    Subtotal = item.Quantity * item.Product.Price
                };
                order.TotalAmount += orderDetail.Subtotal;
                await _repo.AddOrderDetail(orderDetail);
            }
            await _repo.Save();
            return new CreateOrderResponse
            {
                message = "Order created successfully.",
                orderId = order.OrderId,
                totalAmount = order.TotalAmount,
            };
        }
    }
}
