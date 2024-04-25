namespace RecordStore.Api.Dto.Orders;

public class CreateOrderRequest
{
    public string City { get; set; } = null!;

    public string Street { get; set; } = null!;

    public string Building { get; set; } = null!;

    public string? Apartment { get; set; }
    
    public string? Region { get; set; }
}