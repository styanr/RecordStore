using Microsoft.AspNetCore.Mvc;
using RecordStore.Api.Dto.PurchaseOrders;
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