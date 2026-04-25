namespace pbl3_web_api.DTO.Review
{
    // DTO/Review/ReviewRequest.cs
    public class ReviewRequest
    {
        public int ProductId { get; set; }
        public byte Rating { get; set; }
        public string? Comment { get; set; }
    }

    // DTO/Review/ReviewResponse.cs
    public class ReviewResponse
    {
        public int ReviewId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public int ProductId { get; set; }
        public byte Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
