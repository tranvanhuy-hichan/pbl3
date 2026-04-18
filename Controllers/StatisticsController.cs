using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PBL3;
using pbl3_web_api.Entities;

namespace PBL3.Controllers 
{
    [Route("api/statistics")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private readonly ShopPbl3Context _context;

        public StatisticsController(ShopPbl3Context context)
        {
            _context = context;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardOverview()
        {
            // Đếm tổng số lượng người dùng, sản phẩm, đơn hàng
            var totalUsers = await _context.Users.CountAsync();
            var totalProducts = await _context.Products.CountAsync();
            var totalOrders = await _context.Orders.CountAsync();

            // Tính tổng doanh thu 
            var totalRevenue = await _context.Orders
                                             .Where(o => o.Status == "Delivered")
                                             .SumAsync(o => o.TotalAmount);

            // Đếm số đơn hàng đang chờ xử lý ('Pending')
            var pendingOrders = await _context.Orders
                                              .CountAsync(o => o.Status == "Pending");

            // Đếm số sản phẩm sắp hết hàng (StockQuantity < 10)
            var lowStockProducts = await _context.Products
                                                 .CountAsync(p => p.StockQuantity < 10);

            // Trả về JSON y hệt hợp đồng yêu cầu
            return Ok(new
            {
                totalUsers = totalUsers,
                totalProducts = totalProducts,
                totalOrders = totalOrders,
                totalRevenue = totalRevenue,
                pendingOrders = pendingOrders,
                lowStockProducts = lowStockProducts
            });

        }

        //  Thống kê Doanh thu (GET /api/statistics/revenue)

        [HttpGet("revenue")]
        public async Task<IActionResult> GetRevenue([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] string groupBy = "month")
        {

            var query = _context.Orders.Where(o => o.Status == "Delivered").AsQueryable();

            if (from.HasValue) query = query.Where(o => o.OrderDate >= from.Value);
            if (to.HasValue) query = query.Where(o => o.OrderDate <= to.Value);


            var orders = await query.ToListAsync();

            var chartData = orders.GroupBy(o => {
                if (groupBy == "day") return o.OrderDate.ToString("yyyy-MM-dd");
                if (groupBy == "year") return o.OrderDate.ToString("yyyy");
                return o.OrderDate.ToString("yyyy-MM"); // Mặc định là nhóm theo Tháng
            })
            .Select(g => new {
                label = g.Key,
                revenue = g.Sum(o => o.TotalAmount), // Tổng tiền của nhóm đó
                orderCount = g.Count()               
            })
            .OrderBy(c => c.label) 
            .ToList();

            var totalRevenue = chartData.Sum(c => c.revenue);

            // Trả về đúng khuôn mẫu hợp đồng
            return Ok(new { totalRevenue = totalRevenue, chart = chartData });
        }

        //  Thống kê Trạng thái Đơn hàng (GET /api/statistics/orders)
        [HttpGet("orders")]
        public async Task<IActionResult> GetOrderStatus([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var query = _context.Orders.AsQueryable();

            if (from.HasValue) query = query.Where(o => o.OrderDate >= from.Value);
            if (to.HasValue) query = query.Where(o => o.OrderDate <= to.Value);

            // Bắt đầu đếm từng loại trạng thái
            var total = await query.CountAsync();
            var pending = await query.CountAsync(o => o.Status == "Pending");
            var processing = await query.CountAsync(o => o.Status == "Processing");
            var shipped = await query.CountAsync(o => o.Status == "Shipped");
            var delivered = await query.CountAsync(o => o.Status == "Delivered");
            var cancelled = await query.CountAsync(o => o.Status == "Cancelled");

            return Ok(new
            {
                total = total,
                pending = pending,
                processing = processing,
                shipped = shipped,
                delivered = delivered,
                cancelled = cancelled
            });
        }
       
        //  Top Sản Phẩm Bán Chạy (GET /api/statistics/top-products)

        [HttpGet("top-products")]
        public async Task<IActionResult> GetTopProducts([FromQuery] int top = 10, [FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            // Trộn 3 bảng: Chi tiết đơn hàng + Đơn hàng + Sản phẩm lại với nhau
            var query = from od in _context.OrderDetails
                        join o in _context.Orders on od.OrderId equals o.OrderId
                        join p in _context.Products on od.ProductId equals p.ProductId
                        where o.Status == "Delivered" // Chỉ tính những đơn đã giao tiền trao cháo múc
                        select new { od, o, p };

            // Lọc theo ngày tháng nếu giao diện có yêu cầu
            if (from.HasValue) query = query.Where(x => x.o.OrderDate >= from.Value);
            if (to.HasValue) query = query.Where(x => x.o.OrderDate <= to.Value);

            // Gom nhóm theo từng sản phẩm và cộng dồn số lượng bán ra
            var topProducts = await query
                .GroupBy(x => new { x.p.ProductId, x.p.ProductName })
                .Select(g => new
                {
                    productId = g.Key.ProductId,
                    productName = g.Key.ProductName,
                    totalSold = g.Sum(x => x.od.Quantity), 
                    totalRevenue = g.Sum(x => x.od.Quantity * x.od.UnitPrice) 
                })
                .OrderByDescending(x => x.totalSold) 
                .Take(top) 
                .ToListAsync();

            return Ok(topProducts);
        }
    }
}