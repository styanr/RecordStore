using System;
using System.Collections.Generic;

namespace RecordStore.Api.Entities;

public partial class InventoryEvent
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int QuantityChange { get; set; }
    public EventType EventType { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Product Product { get; set; } = null!;
}
