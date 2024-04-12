using Microsoft.AspNetCore.Mvc;
using RecordStore.Api.Dto.Orders;
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
    public async Task<ActionResult<List<OrderResponse>>> Get([FromQuery] GetOrderQueryParams queryParams)
    {
        var orders = await _orderService.GetAllAsync(queryParams);
        return orders;
    }
    
    [HttpPost]
    public async Task<ActionResult> Post([FromBody] CreateOrderRequest createOrderRequest)
    {
        await _orderService.CreateAsync(createOrderRequest);
        return Created();
    }
}