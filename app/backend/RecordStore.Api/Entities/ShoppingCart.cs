using System;
using System.Collections.Generic;

namespace RecordStore.Api.Entities;

public partial class ShoppingCart
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public virtual ICollection<ShoppingCartProduct> ShoppingCartProducts { get; set; } = new List<ShoppingCartProduct>();

    public virtual AppUser User { get; set; } = null!;
}
