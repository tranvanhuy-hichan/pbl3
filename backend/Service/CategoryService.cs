using pbl3_web_api.DTO;
using pbl3_web_api.Repository;
using pbl3_web_api.Entities;

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
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;
        public CategoryService(ICategoryRepository repository)
        {
            _repository = repository;
        }
        public async Task<IEnumerable<CategoryResponse>> GetAllCategoriesAsync()
        {
            var Categories = await _repository.GetAllAsync();
            var CategoryResponses = Categories.Select(c => new CategoryResponse
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                Description = c.Description
            });

            return CategoryResponses;
        }

        public async Task<CategoryResponse?> GetCategoryByIdAsync(int id)
        {
            // Bước 1: Nhờ Repository tìm kiếm trong DB
            var category = await _repository.GetByIdAsync(id);

            // Bước 2: Kiểm tra xem có tồn tại không
            if (category == null)
            {
                return null; // Báo hiệu là không tìm thấy
            }

            // Bước 3: Chuyển đổi Entity sang Response (DTO)
            return new CategoryResponse
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                Description = category.Description
            };
        }
        public async Task<int> AddCategoryAsync(AddCategoryRequest request)
        {
            // Chuyển từ DTO sang Entity
            var category = new Category
            {
                CategoryName = request.CategoryName,
                Description = request.Description
            };

            // Gọi Repository để lưu vào DB
            var createdCategory = await _repository.AddAsync(category);

            // Chỉ trả về ID của danh mục vừa tạo theo đúng yêu cầu API Contract
            return createdCategory.CategoryId;
        }
        public async Task<bool> UpdateCategoryAsync(int id, UpdateCategoryRequest request)
        {
            // Bước 1: Tìm danh mục cũ trong DB
            var category = await _repository.GetByIdAsync(id);

            // Nếu không tìm thấy, trả về false để Controller biết
            if (category == null)
            {
                return false;
            }

            // Bước 2: Cập nhật thông tin. Chỉ cập nhật nếu client có gửi dữ liệu lên
            if (!string.IsNullOrEmpty(request.CategoryName))
            {
                category.CategoryName = request.CategoryName;
            }

            if (request.Description != null)
            {
                category.Description = request.Description;
            }

            // Bước 3: Gọi Repository để lưu xuống DB
            await _repository.UpdateAsync(category);

            return true; // Báo hiệu thành công
        }
        public async Task<bool> DeleteCategoryAsync(int id)
        {
            // Bước 1: Tìm danh mục theo ID
            var category = await _repository.GetByIdAsync(id);

            // Bước 2: Nếu không tồn tại -> Trả về false
            if (category == null)
            {
                return false;
            }

            // Bước 3: Nếu tồn tại -> Nhờ Repository xoá và trả về true
            await _repository.DeleteAsync(category);

            return true;
        }
    }
}
