namespace RecordStore.Api.Dto.PurchaseOrders;

public class PurchaseOrderCreateRequest
{
    public decimal Total { get; set; }
    public int SupplierId { get; set; }
    public List<PurchaseOrderLineCreateRequest> PurchaseOrderLines { get; set; }
}