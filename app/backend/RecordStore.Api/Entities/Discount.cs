using System;
using System.Collections.Generic;

namespace RecordStore.Api.Entities;

public partial class Discount
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public int DiscountPercent { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual Product Product { get; set; } = null!;
}
