using pbl3_web_api.Entities;

namespace pbl3_web_api.Repository
{
    public interface IProductRepo
    {
        IQueryable<Product> GetProductsQuery();
        Task<Product?> GetProductByIdAsync(int id);
        Task AddProductAsync(Product product);
        Task<bool> CheckCategoryAndBrandExistAsync(int categoryId, int brandId);
        Task UpdateProductsAsync(List<Product> products);
        Task SaveAsync();
    }
}
