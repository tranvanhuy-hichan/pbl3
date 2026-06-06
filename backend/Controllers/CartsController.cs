using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pbl3_web_api.DTO;
using System.Security.Claims;
using pbl3_web_api.Service;

namespace pbl3_web_api.Controllers
{
    [Authorize(Roles = "Customer")]
    [Route("api/carts")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartsController(ICartService cartService)
        {
            _cartService = cartService;
        }

        private int? GetUserIdFromClaim()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                    ?? User.FindFirst("sub")?.Value
                    ?? User.FindFirst("userid")?.Value
                    ?? User.FindFirst("UserId")?.Value;

                if (int.TryParse(userIdClaim, out var userId))
                {
                    return userId;
                }
            }

            if (HttpContext.Request.Query.TryGetValue("userId", out var userIdFromQuery) && int.TryParse(userIdFromQuery, out var userIdQuery))
            {
                return userIdQuery;
            }

            return null;
        }

        [HttpGet]
        public async Task<IActionResult> GetCartItems()
        {
            var userId = GetUserIdFromClaim();
            if (!userId.HasValue)
            {
                return BadRequest(new { message = "UserId không xác định. Thêm userId query hoặc xác thực Bearer" });
            }

            var cartItems = await _cartService.GetCartItemsAsync(userId.Value);
            return Ok(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddCartRequest request)
        {
            var userId = GetUserIdFromClaim();
            if (!userId.HasValue)
            {
                return BadRequest(new { message = "UserId không xác định. Thêm userId query hoặc xác thực Bearer" });
            }

            if (request.Quantity <= 0)
            {
                return BadRequest(new { message = "Quantity phải lớn hơn 0" });
            }

            var (isSuccess, message, item) = await _cartService.AddItemAsync(userId.Value, request);
            if (!isSuccess)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message, cartItem = item });
        }

        [HttpPut("{cartId}")]
        public async Task<IActionResult> UpdateCartItem(int cartId, [FromBody] UpdateCartRequest request)
        {
            var userId = GetUserIdFromClaim();
            if (!userId.HasValue)
            {
                return BadRequest(new { message = "UserId không xác định. Thêm userId query hoặc xác thực Bearer" });
            }

            var (isSuccess, message) = await _cartService.UpdateItemAsync(userId.Value, cartId, request);
            if (!isSuccess)
            {
                return BadRequest(new { message });
            }

            return Ok(new { message });
        }

        [HttpDelete("{cartId}")]
        public async Task<IActionResult> DeleteCartItem(int cartId)
        {
            var userId = GetUserIdFromClaim();
            if (!userId.HasValue)
            {
                return BadRequest(new { message = "UserId không xác định. Thêm userId query hoặc xác thực Bearer" });
            }

            var (isSuccess, message) = await _cartService.RemoveItemAsync(userId.Value, cartId);
            if (!isSuccess)
            {
                return NotFound(new { message });
            }

            return Ok(new { message });
        }

        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            var userId = GetUserIdFromClaim();
            if (!userId.HasValue)
            {
                return BadRequest(new { message = "UserId không xác định. Thêm userId query hoặc xác thực Bearer" });
            }

            await _cartService.ClearCartAsync(userId.Value);
            return Ok(new { message = "Xoá toàn bộ giỏ hàng thành công" });
        }
    }
}

