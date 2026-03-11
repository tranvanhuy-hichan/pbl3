namespace pbl3_web_api.RequestDTO
{
    public class ProductDetailResponse
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string ProductCode { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string? ImageURL { get; set; }
        public string? Specifications { get; set; }

        public CategoryProductResponse? Category { get; set; }
        public BrandProductResponse? Brand { get; set; }
    }
    public class CategoryProductResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }

    public class BrandProductResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
}

