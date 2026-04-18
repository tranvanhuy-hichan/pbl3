using pbl3_web_api.DTO.Categories;
using System.Threading.Tasks;
namespace pbl3_web_api.Service
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponse>> GetAllCategoriesAsync();
        Task<CategoryResponse?> GetCategoryByIdAsync(int id);
        Task<int> AddCategoryAsync(AddCategoryRequest request);
        Task<bool> UpdateCategoryAsync(int id, UpdateCategoryRequest request);
        Task<bool> DeleteCategoryAsync(int id);
    }
}