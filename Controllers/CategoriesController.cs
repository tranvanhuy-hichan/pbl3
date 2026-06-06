using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pbl3_web_api.DTO;
using pbl3_web_api.Service;

namespace pbl3_web_api.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var result = await _categoryService.GetAllCategoriesAsync();
            return Ok(result);
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _categoryService.GetCategoryByIdAsync(id);

            // Nếu Service báo về là null -> Trả về lỗi 404 (đúng theo API Contract)
            if (result == null)
            {
                return NotFound(new { message = $"Không tìm thấy danh mục với ID = {id}" });
            }

            // Nếu tìm thấy -> Trả về data kèm mã 200 OK
            return Ok(result);
        }

        // PUT: api/Categories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryRequest request)
        {
            // Gọi Service thực hiện cập nhật
            var isSuccess = await _categoryService.UpdateCategoryAsync(id, request);

            // Nếu Service trả về false -> Không tìm thấy ID
            if (!isSuccess)
            {
                return NotFound(new { message = $"Không tìm thấy danh mục với ID = {id} để cập nhật" });
            }

            // Nếu thành công -> Trả về 200 OK cùng message theo chuẩn Contract
            return Ok(new { message = "Cập nhật danh mục thành công" });
        }

        // POST: api/Categories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] AddCategoryRequest request)
        {
            // Kiểm tra xem dữ liệu gửi lên có hợp lệ không (ví dụ: thiếu CategoryName)
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Mã 400 Bad Request
            }

            var categoryId = await _categoryService.AddCategoryAsync(request);

            // Trả về mã 201 Created cùng JSON đúng chuẩn bản thiết kế (API Contract)
            return StatusCode(201, new
            {
                message = "Thêm danh mục mới thành công",
                categoryId = categoryId
            });
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var isSuccess = await _categoryService.DeleteCategoryAsync(id);

            // Nếu Service trả về false (không tìm thấy) -> Mã 404
            if (!isSuccess)
            {
                return NotFound(new { message = $"Không tìm thấy danh mục với ID = {id} để xoá" });
            }

            // Nếu xoá thành công -> Mã 200 OK kèm message
            return Ok(new { message = "Xoá danh mục thành công" });
        }
    }
}
