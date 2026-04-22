using pbl3_web_api.Entities;

namespace pbl3_web_api.Repository
{
    public interface IStatisticsRepo
    {
        Task<int> GetTotalUsers();
        Task<int> GetTotalProducts();
        Task<int> GetTotalOrders();

        Task<decimal> GetTotalRevenue();
        Task<int> GetPendingOrders();
        Task<int> GetLowStockProducts();
        IQueryable<User> GetUsersQuery();
        IQueryable<Order> GetOrdersQuery();

        IQueryable<OrderDetail> GetOrderDetailsQuery();
        IQueryable<Product> GetProductsQuery();
    }
}
