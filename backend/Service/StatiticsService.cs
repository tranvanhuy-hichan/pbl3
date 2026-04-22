using pbl3_web_api.Services;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;
using System.IO;
using pbl3_web_api.Repository;

namespace PBL3.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IStatisticsRepo _repo;

        public StatisticsService(IStatisticsRepo repo)
        {
            _repo = repo;
        }

        public async Task<object> GetDashboardOverview(DateTime? from, DateTime? to)
        {
            var userQuery = _repo.GetUsersQuery();
            if (from.HasValue) userQuery = userQuery.Where(u => u.CreatedAt >= from.Value);
            if (to.HasValue) userQuery = userQuery.Where(u => u.CreatedAt <= to.Value);

            var totalUsers = await userQuery.CountAsync();


            var orderQuery = _repo.GetOrdersQuery();
            if (from.HasValue) orderQuery = orderQuery.Where(o => o.OrderDate >= from.Value);
            if (to.HasValue) orderQuery = orderQuery.Where(o => o.OrderDate <= to.Value);

            var totalOrders = await orderQuery.CountAsync();


            var revenue = await orderQuery
                .Where(o => o.Status == "Delivered")
                .SumAsync(o => (decimal?)o.TotalAmount) ?? 0;


            var pendingOrders = await orderQuery
                .CountAsync(o => o.Status == "Pending");


            var totalProducts = await _repo.GetProductsQuery().CountAsync();

            var lowStockProducts = await _repo.GetProductsQuery()
                .CountAsync(p => p.StockQuantity <= 0); 


            return new
            {
                totalUsers,
                totalProducts,
                totalOrders,
                totalRevenue = revenue,
                pendingOrders,
                lowStockProducts
            };
        }


        public async Task<object> GetRevenue(DateTime? from, DateTime? to, string groupBy)
        {
            var query = _repo.GetOrdersQuery().Where(o => o.Status == "Delivered");

            if (from.HasValue) query = query.Where(o => o.OrderDate >= from.Value);
            if (to.HasValue) query = query.Where(o => o.OrderDate <= to.Value);

            var orders = query.ToList();

            var chartData = orders.GroupBy(o =>
            {
                if (groupBy == "day") return o.OrderDate.ToString("yyyy-MM-dd");
                if (groupBy == "year") return o.OrderDate.ToString("yyyy");
                return o.OrderDate.ToString("yyyy-MM");
            })
            .Select(g => new
            {
                label = g.Key,
                revenue = g.Sum(o => o.TotalAmount),
                orderCount = g.Count()
            })
            .OrderBy(x => x.label)
            .ToList();

            return new
            {
                totalRevenue = chartData.Sum(x => x.revenue),
                chart = chartData
            };
        }

        public async Task<object> GetOrderStatus(DateTime? from, DateTime? to)
        {
            var query = _repo.GetOrdersQuery();

            if (from.HasValue) query = query.Where(o => o.OrderDate >= from.Value);
            if (to.HasValue) query = query.Where(o => o.OrderDate <= to.Value);

            return new
            {
                total = await query.CountAsync(),
                pending = await query.CountAsync(o => o.Status == "Pending"),
                processing = await query.CountAsync(o => o.Status == "Processing"),
                shipped = await query.CountAsync(o => o.Status == "Shipped"),
                delivered = await query.CountAsync(o => o.Status == "Delivered"),
                cancelled = await query.CountAsync(o => o.Status == "Cancelled")
            };
        }

        public async Task<object> GetTopProducts(int top, DateTime? from, DateTime? to)
        {
            var query = from od in _repo.GetOrderDetailsQuery()
                        join o in _repo.GetOrdersQuery() on od.OrderId equals o.OrderId
                        join p in _repo.GetProductsQuery() on od.ProductId equals p.ProductId
                        where o.Status == "Delivered"
                        select new { od, o, p };

            if (from.HasValue) query = query.Where(x => x.o.OrderDate >= from.Value);
            if (to.HasValue) query = query.Where(x => x.o.OrderDate <= to.Value);

            var result = query
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
                .ToList();

            return result;
        }

        public async Task<byte[]> ExportFullExcel(DateTime? from, DateTime? to)
        {
            using var workbook = new XLWorkbook();

            var orderQuery = _repo.GetOrdersQuery();

            if (from.HasValue)
                orderQuery = orderQuery.Where(o => o.OrderDate >= from.Value);

            if (to.HasValue)
                orderQuery = orderQuery.Where(o => o.OrderDate <= to.Value);

            var ws1 = workbook.Worksheets.Add("Dashboard");

            var totalUsers = await _repo.GetTotalUsers();
            var totalProducts = await _repo.GetTotalProducts();
            var totalOrders = await orderQuery.CountAsync();
            var totalRevenue = await orderQuery
                .Where(o => o.Status == "Delivered")
                .SumAsync(o => o.TotalAmount);

            var pendingOrders = await orderQuery.CountAsync(o => o.Status == "Pending");
            var lowStockProducts = await _repo.GetLowStockProducts();

            ws1.Cell(1, 1).Value = "Chỉ số";
            ws1.Cell(1, 2).Value = "Giá trị";
            ws1.Range("A1:B1").Style.Font.Bold = true;

            ws1.Cell(2, 1).Value = "Tổng doanh thu";
            ws1.Cell(2, 2).Value = totalRevenue;

            ws1.Cell(3, 1).Value = "Tổng đơn hàng";
            ws1.Cell(3, 2).Value = totalOrders;

            ws1.Cell(4, 1).Value = "Đơn chờ xử lý";
            ws1.Cell(4, 2).Value = pendingOrders;

            ws1.Cell(5, 1).Value = "Tổng sản phẩm";
            ws1.Cell(5, 2).Value = totalProducts;

            ws1.Cell(6, 1).Value = "Sản phẩm hết hàng";
            ws1.Cell(6, 2).Value = lowStockProducts;

            ws1.Cell(7, 1).Value = "Người dùng";
            ws1.Cell(7, 2).Value = totalUsers;

            ws1.Columns().AdjustToContents();

            var ws2 = workbook.Worksheets.Add("Revenue");

            var orders = await orderQuery
                .Where(o => o.Status == "Delivered")
                .ToListAsync();

            var revenueData = orders
                .GroupBy(o => o.OrderDate.ToString("yyyy-MM-dd"))
                .Select(g => new
                {
                    Date = g.Key,
                    Revenue = g.Sum(x => x.TotalAmount),
                    OrderCount = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToList();

            ws2.Cell(1, 1).Value = "Ngày";
            ws2.Cell(1, 2).Value = "Doanh thu";
            ws2.Cell(1, 3).Value = "Số đơn";
            ws2.Range("A1:C1").Style.Font.Bold = true;

            for (int i = 0; i < revenueData.Count; i++)
            {
                ws2.Cell(i + 2, 1).Value = revenueData[i].Date;
                ws2.Cell(i + 2, 2).Value = revenueData[i].Revenue;
                ws2.Cell(i + 2, 3).Value = revenueData[i].OrderCount;
            }

            ws2.Column(2).Style.NumberFormat.Format = "#,##0";
            ws2.Columns().AdjustToContents();

            var ws3 = workbook.Worksheets.Add("Order Status");

            var orderStatus = new
            {
                Pending = await orderQuery.CountAsync(o => o.Status == "Pending"),
                Processing = await orderQuery.CountAsync(o => o.Status == "Processing"),
                Shipped = await orderQuery.CountAsync(o => o.Status == "Shipped"),
                Delivered = await orderQuery.CountAsync(o => o.Status == "Delivered"),
                Cancelled = await orderQuery.CountAsync(o => o.Status == "Cancelled")
            };

            ws3.Cell(1, 1).Value = "Trạng thái";
            ws3.Cell(1, 2).Value = "Số lượng";
            ws3.Range("A1:B1").Style.Font.Bold = true;

            ws3.Cell(2, 1).Value = "Chờ xử lý";
            ws3.Cell(2, 2).Value = orderStatus.Pending;

            ws3.Cell(3, 1).Value = "Đang xử lý";
            ws3.Cell(3, 2).Value = orderStatus.Processing;

            ws3.Cell(4, 1).Value = "Đang vận chuyển";
            ws3.Cell(4, 2).Value = orderStatus.Shipped;

            ws3.Cell(5, 1).Value = "Đã giao";
            ws3.Cell(5, 2).Value = orderStatus.Delivered;

            ws3.Cell(6, 1).Value = "Đã hũy";
            ws3.Cell(6, 2).Value = orderStatus.Cancelled;

            ws3.Columns().AdjustToContents();

            var ws4 = workbook.Worksheets.Add("Top Products");

            var topProducts = await (
                from od in _repo.GetOrderDetailsQuery()
                join o in orderQuery on od.OrderId equals o.OrderId
                join p in _repo.GetProductsQuery() on od.ProductId equals p.ProductId
                where o.Status == "Delivered"
                select new { od, p }
            ).ToListAsync();

            var productData = topProducts
                .GroupBy(x => new { x.p.ProductId, x.p.ProductName })
                .Select(g => new
                {
                    g.Key.ProductId,
                    g.Key.ProductName,
                    TotalSold = g.Sum(x => x.od.Quantity),
                    Revenue = g.Sum(x => x.od.Quantity * x.od.UnitPrice)
                })
                .OrderByDescending(x => x.TotalSold)
                .Take(10)
                .ToList();

            ws4.Cell(1, 1).Value = "Mã SP";
            ws4.Cell(1, 2).Value = "Tên";
            ws4.Cell(1, 3).Value = "Đã bán";
            ws4.Cell(1, 4).Value = "Doanh thu";
            ws4.Range("A1:D1").Style.Font.Bold = true;

            for (int i = 0; i < productData.Count; i++)
            {
                ws4.Cell(i + 2, 1).Value = productData[i].ProductId;
                ws4.Cell(i + 2, 2).Value = productData[i].ProductName;
                ws4.Cell(i + 2, 3).Value = productData[i].TotalSold;
                ws4.Cell(i + 2, 4).Value = productData[i].Revenue;
            }

            ws4.Column(4).Style.NumberFormat.Format = "#,##0";
            ws4.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

    }
}
