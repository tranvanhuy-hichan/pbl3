namespace pbl3_web_api.DTO.Order
{
    public class AdminOrdersResponse
    {
        public int orderId { get; set; }

        public int userId { get; set; }
        public string userName { get; set; } = null!;

        public DateTime orderDate { get; set; }

        public decimal totalAmount { get; set; }

        public string status { get; set; } = null!;

    }
}
