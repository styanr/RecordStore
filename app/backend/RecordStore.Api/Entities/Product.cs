using System;
using System.Collections.Generic;

namespace RecordStore.Api.Entities;

public partial class Product
{
    public int Id { get; set; }

    public int RecordId { get; set; }

    public int FormatId { get; set; }

    public string? Description { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public bool Inactive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Discount> Discounts { get; set; } = new List<Discount>();

    public virtual Format Format { get; set; } = null!;

    public virtual ICollection<OrderLine> OrderLines { get; set; } = new List<OrderLine>();

    public virtual Record Record { get; set; } = null!;

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<ShoppingCartProduct> ShoppingCartProducts { get; set; } = new List<ShoppingCartProduct>();

    public virtual ICollection<TrackProduct> TrackProducts { get; set; } = new List<TrackProduct>();
}
