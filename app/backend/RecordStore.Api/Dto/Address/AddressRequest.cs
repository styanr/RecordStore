namespace RecordStore.Api.Dto.Address;

public class AddressRequest
{
    public string City { get; set; } = null!;

    public string Street { get; set; } = null!;

    public string Building { get; set; } = null!;

    public string? Apartment { get; set; }

    public int? RegionId { get; set; }
}