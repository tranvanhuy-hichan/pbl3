using Microsoft.EntityFrameworkCore;
using pbl3_web_api.Data;
using pbl3_web_api.Entities;

namespace pbl3_web_api.Repository
{
    public interface ICartRepository
    {
        Task<IEnumerable<Cart>> GetByUserIdAsync(int userId);
        Task<Cart?> GetByIdAsync(int cartId);
        Task<Cart?> GetByUserAndProductAsync(int userId, int productId);
        Task<Cart> AddAsync(Cart cart);
        Task UpdateAsync(Cart cart);
        Task DeleteAsync(Cart cart);
        Task DeleteRangeAsync(IEnumerable<Cart> carts);
        Task DeleteRange2Async(IEnumerable<Cart> carts);
    }

    public class CartRepo : ICartRepository
    {
        private readonly AppDbContext _context;

        public CartRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Cart>> GetByUserIdAsync(int userId)
        {
            return await _context.Carts
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task<Cart?> GetByIdAsync(int cartId)
        {
            return await _context.Carts
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.CartId == cartId);
        }

        public async Task<Cart?> GetByUserAndProductAsync(int userId, int productId)
        {
            return await _context.Carts
                .Include(c => c.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);
        }

        public async Task<Cart> AddAsync(Cart cart)
        {
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task UpdateAsync(Cart cart)
        {
            _context.Carts.Update(cart);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Cart cart)
        {
            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRangeAsync(IEnumerable<Cart> carts)
        {
            _context.Carts.RemoveRange(carts);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRange2Async(IEnumerable<Cart> carts)
        {
            _context.Carts.RemoveRange(carts);
        }
    }
}
