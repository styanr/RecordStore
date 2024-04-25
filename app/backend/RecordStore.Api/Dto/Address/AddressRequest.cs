using System.ComponentModel.DataAnnotations;

namespace RecordStore.Api.Dto.Address;

public class AddressRequest
{
    [Required(AllowEmptyStrings = false)]
    public string City { get; set; } = null!;

    [Required(AllowEmptyStrings = false)]
    public string Street { get; set; } = null!;

    [Required(AllowEmptyStrings = false)]
    public string Building { get; set; } = null!;

    public string? Apartment { get; set; }

    [Required]
    public string? Region { get; set; }
}