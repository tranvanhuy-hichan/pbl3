using pbl3_web_api.DTO.Brands;
using System.Threading.Tasks;

namespace pbl3_web_api.Service
{
    public interface IBrandService
    {
        Task<IEnumerable<BrandResponse>> GetAllBrandsAsync();
        Task<BrandResponse?> GetBrandByIdAsync(int id);
        Task<int> AddBrandAsync(AddBrandRequest request);
        Task<bool> UpdateBrandAsync(int id, UpdateBrandRequest request);
        Task<bool> DeleteBrandAsync(int id);
    }
}