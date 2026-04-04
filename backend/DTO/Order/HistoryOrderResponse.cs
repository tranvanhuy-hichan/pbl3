namespace pbl3_web_api.DTO.Order
{
    public class HistoryOrderResponse
    {
        public int orderId { get; set; }
        public DateTime orderDate { get; set; }
        public decimal totalAmount { get; set; }
        public string status { get; set; } = null!;
        public string PaymentStatus { get; set; } = null!;
    }
}
