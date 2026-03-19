using System;
using System.ComponentModel.DataAnnotations;

namespace PBL3.Models 
{
    public class Product
    {
        [Key]
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public int? CategoryID { get; set; } // Dấu ? vì SQL cho phép null
        public string? Description { get; set; }
        public decimal Price { get; set; }

        // Đây chính là cột Tồn kho 
        public int StockQuantity { get; set; }

        public string? ImageURL { get; set; }
        public string? Brand { get; set; }
        public string? Specifications { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}