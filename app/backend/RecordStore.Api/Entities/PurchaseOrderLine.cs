using System;
using System.Collections.Generic;

namespace RecordStore.Api.Entities;

public partial class PurchaseOrderLine
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public int PurchaseOrderId { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual PurchaseOrder PurchaseOrder { get; set; } = null!;
}
