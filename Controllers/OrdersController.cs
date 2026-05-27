using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pbl3_web_api.DTO.Order;
using pbl3_web_api.Entities;
using pbl3_web_api.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static NuGet.Packaging.PackagingConstants;

namespace pbl3_web_api.Controllers
{
    [Authorize]
    [Route("api/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _service;

        public OrdersController(IOrderService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Get(
            int page = 1,
            int pageSize = 20,
            string? search = null,
            string? status = null,
            string? paymentStatus = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string? sortOrder = null)
        {
            try
            {
                var (data, totalCount) = await _service.GetOrdersAsync(page, pageSize, search, status, paymentStatus, fromDate, toDate, sortOrder);
                return Ok(new        
                {
                    data = data,
                    totalCount = totalCount,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{orderId}")]
        public async Task<ActionResult> AdminGetOrderDetail(int orderId)
        {
            try
            {
                var orderDetail = await _service.GetOrderDetailAsync(orderId);
                return Ok(orderDetail);
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

        [Authorize(Roles = "Admin")]
        [HttpPatch("{orderId}/status")]
        public async Task<ActionResult> UpdateOrserStutus([FromRoute] int orderId, [FromBody] string newStatus)
        {
            try
            {
                await _service.UpdateOrderStatusAsync(orderId, newStatus);
                return Ok(new { message = "Order status updated successfully.", newStatus = newStatus });

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


        [Authorize(Roles = "Admin")]
        [HttpPatch("{orderId}/payment-status")]
        public async Task<ActionResult> UpdatePaymentStutus([FromRoute] int orderId, [FromBody] string newStatus)
        {
            try
            {
                await _service.UpdatePaymentStatusAsync(orderId, newStatus);
                return Ok(new { message = "Payment status updated successfully.", newStatus = newStatus });

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

        [Authorize(Roles = "Customer")]
        [HttpGet("my")]
        public async Task<ActionResult> GetMyOrders(
            int page = 1,
            int pageSize = 10,
            string? status = null)
        {
              try {
                  int userId = GetUserIdFromToken();

                  var (data, totalCount) = await _service.GetHistoryOrdersAsync(userId, page, pageSize, status);

                  return Ok(new
                  {
                      data = data,
                      totalCount = totalCount
                  });
              } 
              catch (UnauthorizedAccessException ex)
              {
                   return Unauthorized(new { message = ex.Message });
              }
        }

        [Authorize(Roles = "Customer")]
        [HttpGet("my/{orderId}")]
        public async Task<ActionResult> GetMyOrderDetail(int orderId)
        {
            try
            {
                int userId = GetUserIdFromToken();
                var orderDetail = await _service.GetMyOrderDetailAsync(userId, orderId);
                return Ok(orderDetail);
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

        [Authorize(Roles = "Customer")]
        [HttpPatch("my/{orderId}/cancel")]
        public async Task<ActionResult> CancelMyOrder(int orderId)
        {
            try
            {
                int userId = GetUserIdFromToken();
                await _service.CancelOrderAsync(userId, orderId);
                return Ok(new { message = "Order canceled successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Customer")]
        [HttpPost]
        public async Task<ActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            try
            {
                int userId = GetUserIdFromToken();
                var data = await _service.CreateOrderAsync(userId, request);
              
                return Created("", data);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return UnprocessableEntity(new {message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{orderId}/export")]
        public async Task<IActionResult> ExportOrder(int orderId)
        {
            try
            {
                var pdf = await _service.ExportOrderPdfAsync(orderId);

                return File(pdf, "application/pdf", $"order_{orderId}.pdf");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        private int GetUserIdFromToken()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Invalid token");

            return int.Parse(userId);
        }
    }
}
