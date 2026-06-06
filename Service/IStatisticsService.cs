namespace pbl3_web_api.Services
{
    public interface IStatisticsService
    {
        Task<object> GetDashboardOverview(DateTime? from, DateTime? to);
        Task<object> GetRevenue(DateTime? from, DateTime? to, string groupBy);
        Task<object> GetOrderStatus(DateTime? from, DateTime? to);
        Task<object> GetTopProducts(int top, DateTime? from, DateTime? to);
        Task<byte[]> ExportFullExcel(DateTime? from, DateTime? to);
    }
}
