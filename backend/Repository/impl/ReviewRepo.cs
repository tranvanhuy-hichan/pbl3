using pbl3_web_api.Data;
using pbl3_web_api.Entities;
using Microsoft.EntityFrameworkCore;

namespace pbl3_web_api.Repository.impl
{
    public class ReviewRepo : IReviewRepo
    {
        private readonly ShopPbl3Context _context;

        public ReviewRepo(ShopPbl3Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Review>> GetByProductIdAsync(int productId, int page, int pageSize)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        public async Task<int> GetTotalCountAsync(int productId)
        {
            return await _context.Reviews
                .Where(r => r.ProductId == productId)
                .CountAsync();
        }

        public async Task<Review?> GetByUserAndProductAsync(int userId, int productId)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.UserId == userId && r.ProductId == productId);
        }

        public async Task AddAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
