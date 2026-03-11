namespace pbl3_web_api.RequestDTO
{
    public class AddProductRequest
    {
        public string productName { get; set; } = null!;
        public string productCode { get; set; } = null!;

        public int categoryId { get; set; }
        public int brandId { get; set; }

        public string? description { get; set; }

        public decimal price { get; set; }
        public int stockQuantity { get; set; }

        public string? imageURL { get; set; }
        public string? specifications { get; set; }
    }
}
