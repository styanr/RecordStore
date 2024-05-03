using System;
using System.Collections.Generic;

namespace RecordStore.Api.Entities;

public partial class Product
{
    public int Id { get; set; }
    
    public string? ImageUrl { get; set; }

    public int RecordId { get; set; }

    public int FormatId { get; set; }

    public int LabelId { get; set; }
    public string? Description { get; set; }

    public decimal Price { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Format Format { get; set; } = null!;
    
    public virtual Label Label { get; set; } = null!;

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

    public virtual ICollection<OrderLine> OrderLines { get; set; } = new List<OrderLine>();

    public virtual ICollection<PurchaseOrderLine> PurchaseOrderLines { get; set; } = new List<PurchaseOrderLine>();

    public virtual Record Record { get; set; } = null!;

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<ShoppingCartProduct> ShoppingCartProducts { get; set; } = new List<ShoppingCartProduct>();
}
