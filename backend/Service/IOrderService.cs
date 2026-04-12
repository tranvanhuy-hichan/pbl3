using pbl3_web_api.DTO.Order;

namespace pbl3_web_api.Service
{
    public interface IOrderService
    {
        Task<(IEnumerable<AdminOrdersResponse> orders, int totalCount)> GetOrdersAsync(
            int page,
            int pageSize,
            string? search,
            string? status,
            string? paymentStatus,
            DateTime? fromDate,
            DateTime? toDate,
            string? sortOrder);
        Task<AdminOrderDetailResponse> GetOrderDetailAsync(int orderId);
        Task UpdateOrderStatusAsync(int orderId, string newStatus);
        Task UpdatePaymentStatusAsync(int orderId, string newStatus);
        Task<(IEnumerable<HistoryOrderResponse> order, int totalCount)> GetHistoryOrdersAsync(
            int userId,
            int page,
            int pageSize,
            string? status);
        Task<MyOrderDetailResponse> GetMyOrderDetailAsync(int userId, int orderId);
        Task CancelOrderAsync(int userId, int orderId);
        Task<CreateOrderResponse> CreateOrderAsync(int userId, CreateOrderRequest request);
        Task<byte[]> ExportOrderPdfAsync(int orderId);
    }
}
