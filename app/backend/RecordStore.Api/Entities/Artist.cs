using System;
using System.Collections.Generic;

namespace RecordStore.Api.Entities;

public partial class Artist
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<Genre> Genres { get; set; } = new List<Genre>();

    public virtual ICollection<Record> Records { get; set; } = new List<Record>();
}
