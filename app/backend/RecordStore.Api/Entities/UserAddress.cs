using System;
using System.Collections.Generic;

namespace RecordStore.Api.Entities;

public partial class UserAddress
{
    public int UserId { get; set; }

    public int AddressId { get; set; }

    public bool? IsDefault { get; set; }

    public virtual Address Address { get; set; } = null!;

    public virtual AppUser User { get; set; } = null!;
}
