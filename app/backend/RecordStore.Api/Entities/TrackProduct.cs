using System;
using System.Collections.Generic;

namespace RecordStore.Api.Entities;

public partial class TrackProduct
{
    public int TrackId { get; set; }

    public int ProductId { get; set; }

    public string? TrackOrder { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Track Track { get; set; } = null!;
}
