using Microsoft.EntityFrameworkCore;
using pbl3_web_api.Entities;

namespace pbl3_web_api.Repository
{
    public interface IOrderRepo
    {
        Task<List<Order>> GetOrderByUserId(int id);

        Task<Order?> GetOrderById(int id);

        Task<Order?> GetOrderByIdAndUserId(int userId, int orderId);

        IQueryable<Order> GetOrdersQuery();

        Task Add(Order order);

        Task<IEnumerable<Cart>> GetCartItemsByUserId(int userId);

        Task AddOrderDetail(OrderDetail orderDetail);

        Task Save();
    }

    public class OrderRepo : IOrderRepo
    {
        public readonly ShopPbl3Context _context;
        public OrderRepo(ShopPbl3Context context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetOrderByUserId(int id)
        {
            return await _context.Orders
                .Where(o => o.UserId == id)
                .Include(o => o.OrderDetails)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderById(int id)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderId == id);
        }

        public async Task<Order?> GetOrderByIdAndUserId(int userId, int orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);
        }

        public IQueryable<Order> GetOrdersQuery()
        {
            return _context.Orders
                        .Include(p => p.OrderDetails);
        }

        public async Task Add(Order order)
        {
            await _context.Orders.AddAsync(order);
            await Save();
        }

        public async Task<IEnumerable<Cart>> GetCartItemsByUserId(int userId)
        {
            return await _context.Carts
                .Where(ci => ci.UserId == userId)
                .Include(ci => ci.Product)
                .ToListAsync();
        }

        public async Task AddOrderDetail(OrderDetail orderDetail)
        {
            await _context.OrderDetails.AddAsync(orderDetail);
            await Save();
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync(); 
        }
    }
}
