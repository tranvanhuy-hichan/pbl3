using Microsoft.EntityFrameworkCore;
using pbl3_web_api.Entities;

namespace pbl3_web_api.Service
{
    public class ProductService
    {
        private readonly Repository.ProductRepo _productRepo;

        public ProductService(Repository.ProductRepo productRepo)
        {
            _productRepo = productRepo;
        }

        public async Task<(IEnumerable<Product> products, int totalCount)> GetProducts(
            int page,
            int pageSize,
            string? search,
            int? categoryId,
            int? brandId,
            decimal? minPrice,
            decimal? maxPrice)
        {
            var query = _productRepo.GetProductsQuery();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.ProductName.Contains(search));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId);
            }

            if (brandId.HasValue)
            {
                query = query.Where(p => p.BrandId == brandId);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice);
            }

            var totalCount = await query.CountAsync();

            var products = await query
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (products, totalCount);
        }


        public async Task<Product?> GetProductById(int id)
        {
            return await _productRepo.GetProductById(id);
        }

    }
}
