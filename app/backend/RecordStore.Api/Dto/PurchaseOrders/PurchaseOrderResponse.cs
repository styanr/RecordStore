using RecordStore.Api.Entities;

namespace RecordStore.Api.Dto.PurchaseOrders;

public class PurchaseOrderResponse
{
    public int Id { get; set; }

    public decimal Total { get; set; }

    public List<PurchaseOrderLineResponse> PurchaseOrderLines { get; set; }

    public SupplierResponse Supplier { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
}