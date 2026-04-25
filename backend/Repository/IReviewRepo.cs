using pbl3_web_api.Entities;

namespace pbl3_web_api.Repository
{
    public interface IReviewRepo
    {
        Task<IEnumerable<Review>> GetByProductIdAsync(int productId, int page, int pageSize);
        Task<int> GetTotalCountAsync(int productId);
        Task<Review?> GetByUserAndProductAsync(int userId, int productId);
        Task AddAsync(Review review);
        Task SaveChangesAsync();
    }
}
