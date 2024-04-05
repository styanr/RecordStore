using System;
using System.Collections.Generic;

namespace RecordStore.Api.Entities;

public partial class Track
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int DurationSeconds { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<TrackProduct> TrackProducts { get; set; } = new List<TrackProduct>();
}
