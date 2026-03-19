using System.ComponentModel.DataAnnotations;

namespace PBL3.Models
{
    public class OrderDetail
    {
        [Key]
        public int OrderDetailID { get; set; }
        public int OrderID { get; set; }
        public int ProductID { get; set; }

        // Số lượng mua
        public int Quantity { get; set; }

        // Giá lúc mua
        public decimal UnitPrice { get; set; }
    }
}