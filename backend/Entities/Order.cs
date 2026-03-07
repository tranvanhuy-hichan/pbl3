using System;
using System.Collections.Generic;

namespace pbl3_web_api.Entities;

public partial class Order
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public DateTime OrderDate { get; set; }

    public decimal TotalAmount { get; set; }

    public string Status { get; set; } = null!;

    public string ShippingAddress { get; set; } = null!;

    public string? PaymentMethod { get; set; }

    public string PaymentStatus { get; set; } = null!;

    public string? Notes { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual User User { get; set; } = null!;
}
