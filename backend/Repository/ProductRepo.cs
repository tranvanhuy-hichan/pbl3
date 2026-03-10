using pbl3_web_api.Entities;
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
            return _context.Products;
        }

        public async Task<Product?> GetProductById(int id)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.ProductId == id);
        }
    }
}
