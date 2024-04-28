using Microsoft.AspNetCore.Mvc;
using RecordStore.Api.Dto.PurchaseOrders;
using RecordStore.Api.Extensions;
using RecordStore.Api.RequestHelpers.QueryParams;
using RecordStore.Api.Services.PurchaseOrders;

namespace RecordStore.Api.Controllers;

[ApiController]
[Route("api/purchase-orders")]
public class PurchaseOrderController : ControllerBase
{
    private readonly IPurchaseOrderService _purchaseOrderService;

    public PurchaseOrderController(IPurchaseOrderService purchaseOrderService)
    {
        _purchaseOrderService = purchaseOrderService;
    }
    
    [HttpGet]
    public async Task<ActionResult<PagedResult<PurchaseOrderResponse>>> GetPurchaseOrdersAsync([FromQuery] GetPurchaseOrderQueryParams queryParams)
    {
        var purchaseOrders = await _purchaseOrderService.GetPurchaseOrdersAsync(queryParams);
        return purchaseOrders;
    }
    
    [HttpPost]
    public async Task<ActionResult> CreatePurchaseOrderAsync([FromBody] PurchaseOrderCreateRequest purchaseOrderCreateRequest)
    {
        await _purchaseOrderService.CreatePurchaseOrderAsync(purchaseOrderCreateRequest);
        return Ok();
    }
    
    [HttpGet("suppliers")]
    public async Task<ActionResult<List<SupplierResponse>>> GetSuppliersAsync()
    {
        var suppliers = await _purchaseOrderService.GetSuppliersAsync();
        return suppliers;
    }
}