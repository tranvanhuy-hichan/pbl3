namespace pbl3_web_api.RequestDTO
{
    public class AddProductRequest
    {
        public string ProductName { get; set; } = null!;
        public string ProductCode { get; set; } = null!;

        public int CategoryId { get; set; }
        public int BrandId { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }
        public int StockQuantity { get; set; }

        public string? ImageURL { get; set; }
        public string? Specifications { get; set; }
    }
}
