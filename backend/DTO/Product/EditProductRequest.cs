using Microsoft.AspNetCore.Mvc;

namespace pbl3_web_api.DTO.Product
{
    public class EditProductRequest
    {
        public string? ProductName { get; set; }
        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }
        public decimal? Price { get; set; }
        public int? StockQuantity { get; set; }
        public string? Description { get; set; }
        public string? Specifications { get; set; }
        public bool? IsActive { get; set; }
    }

}
