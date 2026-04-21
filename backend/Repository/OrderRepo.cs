using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using pbl3_web_api.Entities;

namespace pbl3_web_api.Repository
{
    public class OrderRepo : IOrderRepo
    {
        private readonly ShopPbl3Context _context;
        public OrderRepo(ShopPbl3Context context)
        {
            _context = context;
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.User)                  
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)  
                .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(o => o.OrderId == id);
        }

        public async Task<Order?> GetOrderByIdAndUserIdAsync(int userId, int orderId)
        {
            return await _context.Orders
                .Include(o => o.User)              
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)       
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);
        }

        public IQueryable<Order> GetOrdersQuery()
        {
            return _context.Orders
                        .Include(o => o.User)
                        .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Product);
        }

        public async Task AddOrderAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await SaveAsync();
        }

        public async Task<IEnumerable<Cart>> GetCartItemsByUserIdAsync(int userId)
        {
            Console.WriteLine(userId);
            return await _context.Carts
                .Where(ci => ci.UserId == userId)
                .Include(ci => ci.Product)
                .ToListAsync();
        }

        public async Task AddOrderDetailAsync(OrderDetail orderDetail)
        {
            await _context.OrderDetails.AddAsync(orderDetail);
            await SaveAsync();
        }

        public async Task AddRangeOrderDetailsAsync(IEnumerable<OrderDetail> orderDetails)
        {
            await _context.OrderDetails.AddRangeAsync(orderDetails);
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync(); 
        }
    }
}
