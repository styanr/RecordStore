namespace RecordStore.Api.Dto.Orders;

public class OrderResponse
{
    public int Id { get; set; }

    public decimal Total { get; set; }

    public string City { get; set; } = null!;

    public string Street { get; set; } = null!;

    public string Building { get; set; } = null!;

    public string? Apartment { get; set; }

    public DateTime CreatedAt { get; set; }

    public ICollection<OrderLineResponse> OrderLines { get; set; } = new List<OrderLineResponse>();

    public string Status { get; set; } = null!;
}