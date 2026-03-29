namespace pbl3_web_api.DTO.Product
{
    public class EditProductRequest
    {
        public string? productName { get; set; }

        public int? categoryId { get; set; }
        public int? brandId { get; set; }

        public string? description { get; set; }

        public decimal? price { get; set; }
        public int? stockQuantity { get; set; }

        public string? imageURL { get; set; }
        public string? specifications { get; set; }

        public bool? isActive { get; set; }
    }
}
