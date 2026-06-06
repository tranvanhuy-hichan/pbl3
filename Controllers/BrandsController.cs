using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pbl3_web_api.DTO;
using pbl3_web_api.Service;

namespace pbl3_web_api.Controllers
{
    [Route("api/brands")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly IBrandService _brandService;

        public BrandsController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBrands()
        {
            var result = await _brandService.GetAllBrandsAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _brandService.GetBrandByIdAsync(id);
            if (result == null)
            {
                return NotFound(new { message = $"Không tìm thấy thương hiệu với ID = {id}" });
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddBrand([FromBody] AddBrandRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var brandId = await _brandService.AddBrandAsync(request);
            return StatusCode(201, new { message = "Thêm thương hiệu mới thành công", brandId = brandId });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBrandRequest request)
        {
            var isSuccess = await _brandService.UpdateBrandAsync(id, request);
            if (!isSuccess)
            {
                return NotFound(new { message = $"Không tìm thấy thương hiệu với ID = {id} để cập nhật" });
            }
            return Ok(new { message = "Cập nhật thương hiệu thành công" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var isSuccess = await _brandService.DeleteBrandAsync(id);
            if (!isSuccess)
            {
                return NotFound(new { message = $"Không tìm thấy thương hiệu với ID = {id} để xoá" });
            }
            return Ok(new { message = "Xoá thương hiệu thành công" });
        }
    }
}
