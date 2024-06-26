﻿using RecordStore.Api.Dto.Orders;
using RecordStore.Api.Extensions;
using RecordStore.Api.RequestHelpers.QueryParams;

namespace RecordStore.Api.Services.Orders;

public interface IOrderService
{
    Task<PagedResult<OrderResponse>> GetAllForUserAsync(GetOrderQueryParams queryParams);
    Task<PagedResult<OrderResponse>> GetAllAsync(GetOrderQueryParams queryParams);
    Task<MemoryStream> GetOrdersReportAsync(GetOrdersReportQueryParams queryParams);
    List<string> GetOrderStatusesAsync();
    Task CreateAsync(CreateOrderRequest createOrderRequest);
    Task<OrderResponse> UpdateStatusAsync(int orderId, OrderStatusDto orderStatusDto);
    Task<OrderResponse> PayAsync(int orderId);
}