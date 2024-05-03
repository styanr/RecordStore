using System;
using System.Collections.Generic;

namespace RecordStore.Api.Entities;

public partial class Review
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int ProductId { get; set; }

    public int Rating { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual AppUser User { get; set; } = null!;
}
