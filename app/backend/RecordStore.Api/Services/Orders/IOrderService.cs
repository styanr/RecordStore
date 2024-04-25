using RecordStore.Api.Dto.Orders;
using RecordStore.Api.Extensions;
using RecordStore.Api.RequestHelpers.QueryParams;

namespace RecordStore.Api.Services.Orders;

public interface IOrderService
{
    Task<PagedResult<OrderResponse>> GetAllAsync(GetOrderQueryParams queryParams);
    Task CreateAsync(CreateOrderRequest createOrderRequest);
}