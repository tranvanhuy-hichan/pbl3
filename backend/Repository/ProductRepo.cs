using pbl3_web_api.Entities;
using pbl3_web_api.RequestDTO;
using Microsoft.EntityFrameworkCore;

namespace pbl3_web_api.Repository
{
    public class ProductRepo
    {
        private readonly ShopPbl3Context _context;

        public ProductRepo(ShopPbl3Context context)
        {
            _context = context;
        }

        public IQueryable<Product> GetProductsQuery()
        {
            return _context.Products
                        .Include(p => p.Category)
                        .Include(p => p.Brand);
        }

        public async Task<Product?> GetProductById(int id)
        {
            return await _context.Products
                        .Include(p => p.Category)
                        .Include(p => p.Brand)
                        .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        public async Task Add(Product product)
        {
            _context.Products.Add(product);
            await Save();
        }

        public async Task<bool> CheckCategoryAndBrandExist(int categoryId, int brandId)
        {
            var categoryExists = await _context.Categories.AnyAsync(c => c.CategoryId == categoryId);
            var brandExists = await _context.Brands.AnyAsync(b => b.BrandId == brandId);
            return categoryExists && brandExists;
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
