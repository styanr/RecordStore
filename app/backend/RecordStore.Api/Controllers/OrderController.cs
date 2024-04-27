﻿using Microsoft.AspNetCore.Authorization;
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
}