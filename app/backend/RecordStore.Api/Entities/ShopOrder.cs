using System;
using System.Collections.Generic;

namespace RecordStore.Api.Entities;

public partial class ShopOrder
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public decimal Total { get; set; }

    public string City { get; set; } = null!;

    public string Street { get; set; } = null!;

    public string Building { get; set; } = null!;

    public string? Apartment { get; set; }

    public int StatusId { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<OrderLine> OrderLines { get; set; } = new List<OrderLine>();

    public virtual OrderStatus Status { get; set; } = null!;

    public virtual AppUser User { get; set; } = null!;
}
