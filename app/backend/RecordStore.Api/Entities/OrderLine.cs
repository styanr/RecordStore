using System;
using System.Collections.Generic;

namespace RecordStore.Api.Entities;

public partial class OrderLine
{
    public int ShopOrderId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual ShopOrder ShopOrder { get; set; } = null!;
}
