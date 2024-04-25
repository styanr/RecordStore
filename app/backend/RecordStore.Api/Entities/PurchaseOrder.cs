using System;
using System.Collections.Generic;

namespace RecordStore.Api.Entities;

public partial class PurchaseOrder
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public decimal Total { get; set; }

    public int SupplierId { get; set; }

    public virtual ICollection<PurchaseOrderLine> PurchaseOrderLines { get; set; } = new List<PurchaseOrderLine>();

    public virtual Supplier Supplier { get; set; } = null!;
}
