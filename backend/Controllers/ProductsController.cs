using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pbl3_web_api.DTO.Product;
using pbl3_web_api.Service;

namespace pbl3_web_api.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductsController(IProductService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetProduct([FromRoute] int id)
        {
            try
            {
                var product = await _service.GetProductByIdAsync(id);
                return Ok(product);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetProducts(
            int page = 1,
            int pageSize = 12,
            string? search = null,
            int? categoryId = null,
            int? brandId = null,
            decimal? minPrice = null,
            decimal? maxPrice = null)
        {
            try
            {
                var result = await _service.GetProductsAsync(page, pageSize, search, categoryId, brandId, minPrice, maxPrice);

                var data = result.products;

                return Ok(new
                {
                    data,
                    totalCount = result.totalCount,
                    page,
                    pageSize
                });
            }
            catch (Exception ex)
            {
                return  StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public async Task<IActionResult> AdminGetProducts(
            int page = 1,
            int pageSize = 12,
            string? search = null,
            int? categoryId = null,
            int? brandId = null,
            decimal? minPrice = null,
            decimal? maxPrice = null)
        {
            try
            {
                var result = await _service.AdminGetProductsAsync(page, pageSize, search, categoryId, brandId, minPrice, maxPrice);

                var data = result.products;

                return Ok(new
                {
                    data,
                    totalCount = result.totalCount,
                    page,
                    pageSize
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("deleted")]
        public async Task<ActionResult> GetDeletedProducts()
        {
            try
            {
                var products = await _service.GetDeletedProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] AddProductRequest request)
        {
            try
            {
                var productId = await _service.AddProductAsync(request);
                return Created("", new { message = "Product created successfully", productId = productId });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                await _service.DeleteProductAsync(id);
                return Ok(new { message = "Product deleted successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new {message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message }); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/restore")]
        public async Task<IActionResult> RestoreProduct(int id)
        {
            try
            {
                await _service.RestoreProductAsync(id);
                return Ok(new { message = "Product deleted successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> EditProduct(int id, [FromBody] EditProductRequest request)
        {                                           
            try
            {
                await _service.EditProductAsync(id, request);
                return Ok(new { message = "Product updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/image")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadImage(int id, [FromForm] UploadImageRequest request)
        {
            try
            {
                var file = request.File;

                if (file == null || file.Length == 0)
                    return BadRequest("No file uploaded");

                var imageUrl = await _service.UploadProductImageAsync(id, file);

                return Ok(new
                {
                    message = "Upload image success",
                    imageUrl
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


    }
}
