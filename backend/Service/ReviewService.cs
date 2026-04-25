using pbl3_web_api.DTO.Review;
using pbl3_web_api.Entities;
using pbl3_web_api.Repository;

namespace pbl3_web_api.Service
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepo _reviewRepo;

        public ReviewService(IReviewRepo reviewRepo)
        {
            _reviewRepo = reviewRepo;
        }

        public async Task<(IEnumerable<ReviewResponse> Items, int TotalCount)> GetByProductIdAsync(int productId, int page, int pageSize)
        {
            var reviews = await _reviewRepo.GetByProductIdAsync(productId, page, pageSize);
            var totalCount = await _reviewRepo.GetTotalCountAsync(productId);

            var items = reviews.Select(r => new ReviewResponse
            {
                ReviewId = r.ReviewId,
                UserId = r.UserId,
                UserName = r.User.FullName,
                ProductId = r.ProductId,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            });

            return (items, totalCount);
        }

        public class ReviewException : Exception
        {
            public ReviewException(string message) : base(message) { }
        }

        public async Task AddReviewAsync(int userId, ReviewRequest request)
        {
            if (request.Rating < 1 || request.Rating > 5)
                throw new ReviewException("Rating phải từ 1 đến 5.");

            var existing = await _reviewRepo.GetByUserAndProductAsync(userId, request.ProductId);
            if (existing != null)
                throw new ReviewException("Bạn đã đánh giá sản phẩm này rồi.");

            var review = new Review
            {
                UserId = userId,
                ProductId = request.ProductId,
                Rating = request.Rating,
                Comment = request.Comment,
                CreatedAt = DateTime.UtcNow
            };

            await _reviewRepo.AddAsync(review);
            await _reviewRepo.SaveChangesAsync();
        }

        public async Task<ReviewResponse> GetMyReviewAsync(int userId, int productId)
        {
            var review = await _reviewRepo.GetByUserAndProductAsync(userId, productId);
            if (review == null)
                throw new KeyNotFoundException("Bạn chưa có đánh giá nào cho sản phẩm này.");

            return new ReviewResponse
            {
                ReviewId = review.ReviewId,
                UserId = review.UserId,
                UserName = review.User.Username,
                ProductId = review.ProductId,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt
            };
        }
    }
}
