using pbl3_web_api.DTO.Review;

namespace pbl3_web_api.Service
{
    public interface IReviewService
    {
        Task<(IEnumerable<ReviewResponse> Items, int TotalCount)> GetByProductIdAsync(int productId, int page, int pageSize);
        Task<(bool success, string message)> AddReviewAsync(int userId, ReviewRequest request);
        Task<ReviewResponse?> GetMyReviewAsync(int userId, int productId);
    }
}
