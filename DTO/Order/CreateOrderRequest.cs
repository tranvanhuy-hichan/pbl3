namespace pbl3_web_api.DTO.Order
{
    public class CreateOrderRequest
    {
        public string shippingAddress { get; set; } = null!;
        public string paymentMethod { get; set; } = null!;
        public string? notes { get; set; }
        public decimal? shippingFee { get; set; }
    }
}
