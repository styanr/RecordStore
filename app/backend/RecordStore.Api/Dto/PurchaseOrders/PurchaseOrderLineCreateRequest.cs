namespace RecordStore.Api.Dto.PurchaseOrders;

public class PurchaseOrderLineCreateRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}