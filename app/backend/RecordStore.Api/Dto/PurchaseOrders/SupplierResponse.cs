namespace RecordStore.Api.Dto.PurchaseOrders;

public class SupplierResponse
{
    public int Id { get; set; }
    
    public string Name { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Address { get; set; } = null!;
}