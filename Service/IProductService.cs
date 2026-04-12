using pbl3_web_api.DTO.Product;

namespace pbl3_web_api.Service
{
    public interface IProductService
    {
        Task<(IEnumerable<ProductsResponse> products, int totalCount)> GetProductsAsync(
            int page,
            int pageSize,
            string? search,
            int? categoryId,
            int? brandId,
            decimal? minPrice,
            decimal? maxPrice);
        Task<(IEnumerable<ProductsResponse> products, int totalCount)> AdminGetProductsAsync(
            int page,
            int pageSize,
            string? search,
            int? categoryId,
            int? brandId,
            decimal? minPrice,
            decimal? maxPrice);

        Task<ProductDetailResponse?> GetProductByIdAsync(int id);
        Task<int> AddProductAsync(AddProductRequest request);
        Task DeleteProductAsync(int id);
        Task<List<ProductsResponse>> GetDeletedProductsAsync();
        Task RestoreProductAsync(int id);
        Task EditProductAsync(int id, EditProductRequest request);
    }
}
