using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecordStore.Api.Dto.Orders;
using RecordStore.Api.Entities;
using RecordStore.Api.Extensions;
using RecordStore.Api.RequestHelpers.QueryParams;
using RecordStore.Api.Services.Orders;

namespace RecordStore.Api.Controllers;

[ApiController]
[Route("api/orders")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }
    [HttpGet]
    public async Task<ActionResult<PagedResult<OrderResponse>>> Get([FromQuery] GetOrderQueryParams queryParams)
    {
        var orders = await _orderService.GetAllForUserAsync(queryParams);
        
        return orders;
    }
    
    [HttpGet]
    [Route("~/api/admin/orders")]
    [Authorize(Roles = ("admin, employee"))]
    public async Task<ActionResult<PagedResult<OrderResponse>>> GetAdminOrders([FromQuery] GetOrderQueryParams queryParams)
    {
        var orders = await _orderService.GetAllAsync(queryParams);
        
        return orders;
    }
    
    [HttpGet]
    [Route("statuses")]
    public ActionResult<List<string>> GetOrderStatuses()
    {
        var orderStatuses = _orderService.GetOrderStatusesAsync();
        
        return orderStatuses;
    }
    
    [HttpPost]
    public async Task<ActionResult> Post([FromBody] CreateOrderRequest createOrderRequest)
    {
        await _orderService.CreateAsync(createOrderRequest);
        return Created();
    }
    
    [HttpPatch]
    [Route("~/api/admin/orders/{orderId}/status")]
    [Authorize(Roles = ("admin, employee"))]
    public async Task<ActionResult<OrderResponse>> UpdateStatus(int orderId, [FromBody] OrderStatusDto orderStatusDto)
    {
        var orderResponse = await _orderService.UpdateStatusAsync(orderId, orderStatusDto);
        return orderResponse;
    }
    
    [HttpPatch]
    [Route("{orderId}/pay")]
    [Authorize(Roles = "user")]
    public async Task<ActionResult<OrderResponse>> Pay(int orderId)
    {
        var orderResponse = await _orderService.PayAsync(orderId);
        return orderResponse;
    }
    
    [HttpGet]
    [Route("~/api/admin/orders/report")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetOrdersReport([FromQuery] GetOrdersReportQueryParams queryParams)
    {
        var report = await _orderService.GetOrdersReportAsync(queryParams);
        
        var result = new FileContentResult(report.ToArray(), "application/octet-stream")
        {
            FileDownloadName = $"orders-report.{queryParams.Format.ToFileExtension()}"
        };
        
        return result;
    }
}