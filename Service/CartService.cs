using pbl3_web_api.DTO;
using pbl3_web_api.Entities;
using pbl3_web_api.Repository;

namespace pbl3_web_api.Service
{
    public interface ICartService
    {
        Task<IEnumerable<CartResponse>> GetCartItemsAsync(int userId);
        Task<(bool IsSuccess, string Message, CartResponse? Item)> AddItemAsync(int userId, AddCartRequest request);
        Task<(bool IsSuccess, string Message)> UpdateItemAsync(int userId, int cartId, UpdateCartRequest request);
        Task<(bool IsSuccess, string Message)> RemoveItemAsync(int userId, int cartId);
        Task ClearCartAsync(int userId);
    }

    public class CartService : ICartService
    {
        private readonly ICartRepository _repository;
        private readonly AppDbContext _context;

        public CartService(ICartRepository repository, AppDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<IEnumerable<CartResponse>> GetCartItemsAsync(int userId)
        {
            var carts = await _repository.GetByUserIdAsync(userId);
            return carts.Select(c => new CartResponse
            {
                CartId = c.CartId,
                ProductId = c.ProductId,
                ProductName = c.Product?.ProductName ?? string.Empty,
                ImageUrl = c.Product?.ImageUrl,
                Price = c.Product?.Price ?? 0,
                Quantity = c.Quantity,
                Subtotal = (c.Product?.Price ?? 0) * c.Quantity
            });
        }

        public async Task<(bool IsSuccess, string Message, CartResponse? Item)> AddItemAsync(int userId, AddCartRequest request)
        {
            if (request.Quantity <= 0)
            {
                return (false, "Quantity phải lớn hơn 0", null);
            }

            var product = await _context.Products.FindAsync(request.ProductId);
            if (product == null || !product.IsActive)
            {
                return (false, "Sản phẩm không tồn tại", null);
            }

            if (product.StockQuantity < request.Quantity)
            {
                return (false, "Số lượng không đủ trong kho", null);
            }

            var cart = await _repository.GetByUserAndProductAsync(userId, request.ProductId);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    AddedAt = DateTime.UtcNow
                };

                await _repository.AddAsync(cart);
            }
            else
            {
                var newQuantity = cart.Quantity + request.Quantity;
                if (product.StockQuantity < newQuantity)
                {
                    return (false, "Số lượng trong giỏ vượt quá tồn kho", null);
                }

                cart.Quantity = newQuantity;
                await _repository.UpdateAsync(cart);
            }

            var added = await _repository.GetByIdAsync(cart.CartId);
            if (added == null)
            {
                return (false, "Lỗi khi thêm sản phẩm vào giỏ", null);
            }

            var response = new CartResponse
            {
                CartId = added.CartId,
                ProductId = added.ProductId,
                ProductName = added.Product?.ProductName ?? string.Empty,
                ImageUrl = added.Product?.ImageUrl,
                Price = added.Product?.Price ?? 0,
                Quantity = added.Quantity,
                Subtotal = (added.Product?.Price ?? 0) * added.Quantity
            };

            return (true, "Thêm sản phẩm vào giỏ thành công", response);
        }

        public async Task<(bool IsSuccess, string Message)> UpdateItemAsync(int userId, int cartId, UpdateCartRequest request)
        {
            if (request.Quantity < 0)
                return (false, "Quantity phải lớn hơn hoặc bằng 0");

            var cart = await _repository.GetByIdAsync(cartId);
            if (cart == null || cart.UserId != userId)
                return (false, "Không tìm thấy sản phẩm trong giỏ");

            var product = await _context.Products.FindAsync(cart.ProductId);
            if (product == null)
                return (false, "Sản phẩm không tồn tại");

            if (request.Quantity == 0)
            {
                await _repository.DeleteAsync(cart);
                return (true, "Xoá sản phẩm khỏi giỏ thành công");
            }

            if (product.StockQuantity < request.Quantity)
                return (false, "Số lượng không đủ trong kho");

            cart.Quantity = request.Quantity;
            await _repository.UpdateAsync(cart);
            return (true, "Cập nhật giỏ hàng thành công");
        }

        public async Task<(bool IsSuccess, string Message)> RemoveItemAsync(int userId, int cartId)
        {
            var cart = await _repository.GetByIdAsync(cartId);
            if (cart == null || cart.UserId != userId)
                return (false, "Không tìm thấy sản phẩm trong giỏ");

            await _repository.DeleteAsync(cart);
            return (true, "Xoá sản phẩm khỏi giỏ thành công");
        }

        public async Task ClearCartAsync(int userId)
        {
            var carts = await _repository.GetByUserIdAsync(userId);
            await _repository.DeleteRangeAsync(carts);
        }
    }
}
