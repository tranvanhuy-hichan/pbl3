using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using pbl3_web_api.Entities;
using pbl3_web_api.RequestDTO;
using pbl3_web_api.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pbl3_web_api.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _service;

        public ProductsController(ProductService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetProduct(int id)
        {
            try
            {
                var product = await _service.GetProductById(id);
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
                var result = await _service.GetProducts(page, pageSize, search, categoryId, brandId, minPrice, maxPrice);

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

        [HttpPost]
        public async Task<IActionResult> AddProduct(AddProductRequest request)
        {
            try
            {
                var productId = await _service.AddProduct(request);
                return Created("", new { message = "Product created successfully", productId = productId });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new {message = ex.Message});
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                await _service.DeleteProduct(id);
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

        [HttpPut("{id}")]
        public async Task<IActionResult> EditProduct(int id, EditProductRequest request)
        {
            try
            {
                await _service.EditProduct(id, request);
                return Ok(new { message = "Product updated successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch(ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
