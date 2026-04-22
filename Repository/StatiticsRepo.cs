using Microsoft.EntityFrameworkCore;
using pbl3_web_api.Entities;

namespace pbl3_web_api.Repository
{
    public class StatisticsRepo : IStatisticsRepo
    {
        private readonly ShopPbl3Context _context;

        public StatisticsRepo(ShopPbl3Context context)
        {
            _context = context;
        }

        public Task<int> GetTotalUsers() => _context.Users.CountAsync();

        public Task<int> GetTotalProducts() => _context.Products.CountAsync();

        public Task<int> GetTotalOrders() => _context.Orders.CountAsync();

        public Task<decimal> GetTotalRevenue()
        {
            return _context.Orders
                .Where(o => o.Status == "Delivered")
                .SumAsync(o => o.TotalAmount);
        }

        public Task<int> GetPendingOrders()
        {
            return _context.Orders.CountAsync(o => o.Status == "Pending");
        }
        public IQueryable<User> GetUsersQuery()
        {
            return _context.Users.AsQueryable();
        }
        public Task<int> GetLowStockProducts()
        {
            return _context.Products.CountAsync(p => p.StockQuantity < 10);
        }

        public IQueryable<Order> GetOrdersQuery() => _context.Orders;

        public IQueryable<OrderDetail> GetOrderDetailsQuery() => _context.OrderDetails;

        public IQueryable<Product> GetProductsQuery() => _context.Products;
    }
}
