using System;
using System.Collections.Generic;

namespace RecordStore.Api.Entities;

public partial class ShoppingCartProduct
{
    public int ShoppingCartId { get; set; }

    public int ProductId { get; set; }

    public int? Quantity { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual ShoppingCart ShoppingCart { get; set; } = null!;
}
