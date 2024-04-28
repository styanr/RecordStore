using RecordStore.Api.Dto.PurchaseOrders;
using RecordStore.Api.Extensions;
using RecordStore.Api.RequestHelpers.QueryParams;

namespace RecordStore.Api.Services.PurchaseOrders;

public interface IPurchaseOrderService
{
    Task<List<SupplierResponse>> GetSuppliersAsync();
    Task CreatePurchaseOrderAsync(PurchaseOrderCreateRequest purchaseOrderCreateRequest);
    
    Task<PagedResult<PurchaseOrderResponse>> GetPurchaseOrdersAsync(GetPurchaseOrderQueryParams queryParams);
    
    Task DeletePurchaseOrderAsync(int id);
}