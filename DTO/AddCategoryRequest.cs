namespace pbl3_web_api.DTO
{
    public class AddCategoryRequest
    {
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
