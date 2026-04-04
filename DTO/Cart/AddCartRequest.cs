namespace pbl3_web_api.DTO
{
    public class AddCartRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        // Optional fallback if auth is not yet set up
        public int? UserId { get; set; }
    }
}