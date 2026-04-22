using pbl3_web_api.Entities;

namespace pbl3_web_api.DTO.Order
{
    public class AdminOrderDetailResponse
    {
        public int orderId { get; set; }

        public int userId { get; set; }
        public string userName { get; set; } = null!;

        public string phoneNumber { get; set; } = null!;

        public DateTime orderDate { get; set; }

        public decimal totalAmount { get; set; }

        public string status { get; set; } = null!;

        public string shippingAddress { get; set; } = null!;

        public string? paymentMethod { get; set; }

        public string paymentStatus { get; set; } = null!;

        public string? notes { get; set; }

        public DateTime updatedAt { get; set; }

        public virtual ICollection<OrderDetailItem> orderDetails { get; set; } = new List<OrderDetailItem>();
    }
}
