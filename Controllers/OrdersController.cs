using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pbl3_web_api.Entities;
using pbl3_web_api.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using pbl3_web_api.DTO.Order;

namespace pbl3_web_api.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _service;

        public OrdersController(IOrderService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get(
            int page,
            int pageSize,
            string? status,
            int? userId,
            DateTime? fromDate,
            DateTime? toDate)
        {
            try
            {
                var orders = await _service.GetOrders(page, pageSize, status, userId, fromDate, toDate);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{orderId}")]
        public async Task<ActionResult> AdminGetOrderDetail(int orderId)
        {
            try
            {
                var orderDetail = await _service.GetOrderDetail(orderId);
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

        [HttpPatch("{orderId}/status")]
        public async Task<ActionResult> UpdateOrserStutus([FromRoute] int orderId, [FromBody] string newStatus)
        {
            try
            {
                await _service.UpdateOrderStatus(orderId, newStatus);
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

        [HttpGet("my")]
        public async Task<ActionResult> GetMyOrders(
            int page,
            int pageSize,
            string? status
            )
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    return Unauthorized("Invalid token");

                int userId = int.Parse(userIdClaim.Value);
                var orders = await _service.GetHistoryOrders(userId, page, pageSize, status);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("my/{orderId}")]
        public async Task<ActionResult> GetMyOrderDetail(int orderId)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    return Unauthorized("Invalid token");

                int userId = int.Parse(userIdClaim.Value);
                var orderDetail = await _service.GetMyOrderDetail(userId, orderId);
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

        [HttpPatch("my/{orderId}/cancel")]
        public async Task<ActionResult> CancelMyOrder(int orderId)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    return Unauthorized("Invalid token");

                int userId = int.Parse(userIdClaim.Value);
                await _service.CancelOrder(userId, orderId);
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

        [HttpPost]
        public async Task<ActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    return Unauthorized("Invalid token");

                int userId = int.Parse(userIdClaim.Value);
                var orderId = await _service.CreateOrder(userId, request);
                return Created("", new { message = "Order created successfully.", orderId = orderId });
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
    }
}
