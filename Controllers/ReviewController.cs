using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pbl3_web_api.DTO.Review;
using pbl3_web_api.Entities;
using pbl3_web_api.Service;
using System.Security.Claims;

namespace pbl3_web_api.Controllers
{
    [ApiController]
    [Route("api/reviews")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetReviews(
            int productId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 5)
        {
            var (items, totalCount) = await _reviewService.GetByProductIdAsync(productId, page, pageSize);

            return Ok(new
            {
                items,
                totalCount,
                page,
                pageSize
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostReview([FromBody] ReviewRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim);

            var (success, message) = await _reviewService.AddReviewAsync(userId, request);

            if (!success) return BadRequest(new { message });
            return Ok(new { message });
        }

        [HttpGet("{productId}/my-review")]
        [Authorize]
        public async Task<IActionResult> GetMyReview(int productId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized();
            int userId = int.Parse(userIdClaim);

            var review = await _reviewService.GetMyReviewAsync(userId, productId);
            if (review == null) return NotFound();

            return Ok(review);
        }

    }
}
