using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using pbl3_web_api.DTO;
using pbl3_web_api.Entities;
using pbl3_web_api.Repository;

namespace pbl3_web_api.Service
{
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _repository;

        public BrandService(IBrandRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<BrandResponse>> GetAllBrandsAsync()
        {
            var brands = await _repository.GetAllAsync();
            return brands.Select(b => new BrandResponse
            {
                BrandId = b.BrandId,
                BrandName = b.BrandName,
                LogoUrl = b.LogoUrl
            });
        }

        public async Task<BrandResponse?> GetBrandByIdAsync(int id)
        {
            var brand = await _repository.GetByIdAsync(id);
            if (brand == null) return null;
            return new BrandResponse
            {
                BrandId = brand.BrandId,
                BrandName = brand.BrandName,
                LogoUrl = brand.LogoUrl
            };
        }

        public async Task<int> AddBrandAsync(AddBrandRequest request)
        {
            var brand = new Brand
            {
                BrandName = request.BrandName,
                CreatedAt = DateTime.UtcNow,
                LogoUrl = request.LogoUrl
            };

            var created = await _repository.AddAsync(brand);
            return created.BrandId;
        }

        public async Task<bool> UpdateBrandAsync(int id, UpdateBrandRequest request)
        {
            var brand = await _repository.GetByIdAsync(id);
            if (brand == null) return false;

            if (!string.IsNullOrEmpty(request.BrandName)) brand.BrandName = request.BrandName;
            if (request.LogoUrl != null) brand.LogoUrl = request.LogoUrl;

            await _repository.UpdateAsync(brand);
            return true;
        }

        public async Task<bool> DeleteBrandAsync(int id)
        {
            var brand = await _repository.GetByIdAsync(id);
            if (brand == null) return false;
            await _repository.DeleteAsync(brand);
            return true;
        }
    }
}
