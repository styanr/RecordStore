using System;
using System.Collections.Generic;

namespace RecordStore.Api.Entities;

public partial class Format
{
    public int Id { get; set; }

    public string FormatName { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
