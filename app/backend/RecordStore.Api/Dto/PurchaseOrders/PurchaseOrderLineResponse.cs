namespace RecordStore.Api.Dto.PurchaseOrders;

public class PurchaseOrderLineResponse
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }
}