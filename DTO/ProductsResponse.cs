namespace pbl3_web_api.RequestDTO
{
    public class ProductsResponse
    {
        public int productId { get; set; }
        public string productName { get; set; }
        public string productCode { get; set; }
        public decimal price { get; set; }
        public int stockQuantity { get; set; }
        public string imageURL { get; set; }
        public string categoryName { get; set; }
        public string brandName { get; set; }
        public bool isActive { get; set; }
    }

}
