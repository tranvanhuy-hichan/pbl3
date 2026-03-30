namespace pbl3_web_api.DTO.Order
{
    public class OrderDetailItem
    {
        public string? imageURL { get; set; } 
        public int productId { get; set; }
        public string productName { get; set; } = null!;

        public int quantity { get; set; }

        public decimal unitPrice { get; set; }

        public decimal subtotal { get; set; }
    }
}
