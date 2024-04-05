using System;
using System.Collections.Generic;

namespace RecordStore.Api.Entities;

public partial class Region
{
    public int Id { get; set; }

    public string? RegionName { get; set; }

    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();
}
