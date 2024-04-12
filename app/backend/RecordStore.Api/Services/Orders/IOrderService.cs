using RecordStore.Api.Dto.Orders;
using RecordStore.Api.RequestHelpers.QueryParams;

namespace RecordStore.Api.Services.Orders;

public interface IOrderService
{
    Task<List<OrderResponse>> GetAllAsync(GetOrderQueryParams queryParams);
    Task CreateAsync(CreateOrderRequest createOrderRequest);
}