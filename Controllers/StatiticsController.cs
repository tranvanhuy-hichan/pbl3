using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pbl3_web_api.DTO;
using pbl3_web_api.Entities;
using pbl3_web_api.Service;
using pbl3_web_api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PBL3.Controllers
{
    [Route("api/statistics")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService _service;

        public StatisticsController(IStatisticsService service)
        {
            _service = service;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            return Ok(await _service.GetDashboardOverview(from, to));
        }


        [HttpGet("revenue")]
        public async Task<IActionResult> Revenue(DateTime? from, DateTime? to, string groupBy = "month")
        {
            return Ok(await _service.GetRevenue(from, to, groupBy));
        }

        [HttpGet("orders")]
        public async Task<IActionResult> Orders(DateTime? from, DateTime? to)
        {
            return Ok(await _service.GetOrderStatus(from, to));
        }

        [HttpGet("top-products")]
        public async Task<IActionResult> TopProducts(int top = 10, DateTime? from = null, DateTime? to = null)
        {
            return Ok(await _service.GetTopProducts(top, from, to));
        }

        [HttpGet("export")]
        public async Task<IActionResult> ExportRevenue(DateTime? from, DateTime? to)
        {
            var file = await _service.ExportFullExcel(from, to);

            return File(
                file,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"report_{DateTime.Now:yyyyMMddHHmmss}.xlsx"
            );
        }

    }
}
