namespace pbl3_web_api.DTO.Product
{
    public class ProductDetailResponse
    {
        public int productId { get; set; }
        public string productName { get; set; } = null!;
        public string productCode { get; set; } = null!;
        public string? description { get; set; }
        public decimal price { get; set; }
        public int stockQuantity { get; set; }
        public string? imageURL { get; set; }
        public string? specifications { get; set; }

        public CategoryProductResponse? category { get; set; }
        public BrandProductResponse? brand { get; set; }
    }
    public class CategoryProductResponse
    {
        public int id { get; set; }
        public string name { get; set; } = null!;
    }

    public class BrandProductResponse
    {
        public int id { get; set; }
        public string name { get; set; } = null!;
    }
}

