using RecordStore.Api.Dto.PurchaseOrders;

namespace RecordStore.Api.Services.PurchaseOrders;

public interface IPurchaseOrderService
{
    Task<List<SupplierResponse>> GetSuppliersAsync();
    Task CreatePurchaseOrderAsync(PurchaseOrderCreateRequest purchaseOrderCreateRequest);
}