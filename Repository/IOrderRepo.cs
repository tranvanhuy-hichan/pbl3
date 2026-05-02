using Microsoft.EntityFrameworkCore.Storage;
using pbl3_web_api.Entities;

namespace pbl3_web_api.Repository
{
    public interface IOrderRepo
    {
        Task<Order?> GetOrderByIdAsync(int id);
        Task<Order?> GetOrderByIdAndUserIdAsync(int userId, int orderId);
        IQueryable<Order> GetOrdersQuery();
        Task AddOrderAsync(Order order);
        Task<IEnumerable<Cart>> GetCartItemsByUserIdAsync(int userId);
        Task AddOrderDetailAsync(OrderDetail orderDetail);
        Task AddRangeOrderDetailsAsync(IEnumerable<OrderDetail> orderDetails);
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task<bool> HasUserPurchasedProductAsync(int userId, int productId);
        Task SaveAsync();
    }
}
