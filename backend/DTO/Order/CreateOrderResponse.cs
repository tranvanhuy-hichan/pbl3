namespace pbl3_web_api.DTO.Order
{
    public class CreateOrderResponse
    {
        public string message { get; set; } = null!;
        public int orderId { get; set; }
        public decimal totalAmount { get; set; }
    }

}
