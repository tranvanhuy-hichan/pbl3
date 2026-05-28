using Microsoft.EntityFrameworkCore;
using pbl3_web_api.Entities;

namespace pbl3_web_api.Repository.impl
{
    public class BrandRepo : IBrandRepository
    {
        private readonly ShopPbl3Context _context;

        public BrandRepo(ShopPbl3Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Brand>> GetAllAsync()
        {
            return await _context.Brands.ToListAsync();
        }

        public async Task<Brand?> GetByIdAsync(int id)
        {
            return await _context.Brands.FindAsync(id);
        }

        public async Task<Brand> AddAsync(Brand brand)
        {
            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();
            return brand;
        }

        public async Task UpdateAsync(Brand brand)
        {
            _context.Brands.Update(brand);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Brand brand)
        {
            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();
        }
    }
}
