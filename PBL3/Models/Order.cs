using System;
using System.ComponentModel.DataAnnotations;

namespace PBL3.Models
{
    public class Order
    {
        [Key]
        public int OrderID { get; set; }
        public int UserID { get; set; }
        public DateTime OrderDate { get; set; }

        // Đây là cột Tổng tiền để tính Doanh thu
        public decimal TotalAmount { get; set; }

        // Đây là cột Trạng thái đơn hàng (Pending, Delivered...)
        public string Status { get; set; }

        public string ShippingAddress { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public string? Notes { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}