namespace pbl3_web_api.DTO.Product
{
    public class ProductsResponse
    {
        public int productId { get; set; }
        public string productName { get; set; } = null!;
        public string productCode { get; set; } = null!;
        public decimal price { get; set; }
        public int stockQuantity { get; set; }
        public string? imageURL { get; set; }
        public string? specifications { get; set; }
        public string categoryName { get; set; } = null!;
        public string brandName { get; set; } = null!;
        public bool isActive { get; set; }
    }

}
