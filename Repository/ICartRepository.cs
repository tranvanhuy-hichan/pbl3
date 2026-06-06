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
        Task SaveAsync();
    }
}
