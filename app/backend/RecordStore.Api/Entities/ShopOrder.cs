using System;
using System.Collections.Generic;

namespace RecordStore.Api.Entities;

public partial class ShopOrder
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string City { get; set; } = null!;

    public string Street { get; set; } = null!;

    public string Building { get; set; } = null!;

    public string? Apartment { get; set; }

    public DateTime CreatedAt { get; set; }

    public string Region { get; set; } = null!;
    
    public OrderStatus Status { get; set; }
    public decimal Total => OrderLines.Sum(ol => ol.Price * ol.Quantity);

    public virtual ICollection<OrderLine> OrderLines { get; set; } = new List<OrderLine>();

    public virtual AppUser User { get; set; } = null!;
}
