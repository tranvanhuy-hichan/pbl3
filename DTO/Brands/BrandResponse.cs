namespace pbl3_web_api.DTO
{
    public class BrandResponse
    {
        public int BrandId { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
    }
}
