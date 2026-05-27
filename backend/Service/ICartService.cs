using pbl3_web_api.DTO;

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
}
