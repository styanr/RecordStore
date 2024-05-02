using System;
using System.Collections.Generic;

namespace RecordStore.Api.Entities;

// TODO: remove inventory logic, move to product entity
public partial class Inventory
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public string Location { get; set; } = null!;

    public int? RestockLevel { get; set; }

    public virtual Product Product { get; set; } = null!;
}
