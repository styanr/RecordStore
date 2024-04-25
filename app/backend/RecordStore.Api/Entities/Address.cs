using System;
using System.Collections.Generic;

namespace RecordStore.Api.Entities;

public partial class Address
{
    public int Id { get; set; }

    public string City { get; set; } = null!;

    public string Street { get; set; } = null!;

    public string Building { get; set; } = null!;

    public string? Apartment { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public int UserId { get; set; }

    public string? Region { get; set; }

    public virtual AppUser User { get; set; } = null!;
}
