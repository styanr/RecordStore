using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
using RecordStore.Api.Context;
using RecordStore.Api.Dto.Orders;
using RecordStore.Api.Entities;
using RecordStore.Api.Exceptions;
using RecordStore.Api.Extensions;
using RecordStore.Api.RequestHelpers.QueryParams;
using RecordStore.Api.Services.Users;
using ServiceStack;
using ServiceStack.Text;

namespace RecordStore.Api.Services.Orders;

public class OrderService : IOrderService
{
    private readonly RecordStoreContext _context;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public OrderService(RecordStoreContext context, IMapper mapper, IUserService userService)
    {
        _context = context;
        _mapper = mapper;
        _userService = userService;
    }

    public async Task<PagedResult<OrderResponse>> GetAllForUserAsync(GetOrderQueryParams queryParams)
    {
        var user = await _userService.GetCurrentUserAsync();
        var userId = user.Id;
        
        var query = _context.ShopOrders
            .ApplyIncludes()
            .ApplyFiltersAndOrderBy(queryParams)
            .Where(o => o.UserId == userId);

        var pagedResult = await query.GetPagedAsync(queryParams.Page, queryParams.PageSize);

        var orders = _mapper.Map<PagedResult<OrderResponse>>(pagedResult);

        return orders;
    }

    public async Task<PagedResult<OrderResponse>> GetAllAsync(GetOrderQueryParams queryParams)
    {
        var query = _context.ShopOrders
            .ApplyIncludes()
            .ApplyFiltersAndOrderBy(queryParams);

        var pagedResult = await query.GetPagedAsync(queryParams.Page, queryParams.PageSize);
        
        var orders = _mapper.Map<PagedResult<OrderResponse>>(pagedResult);
        
        return orders; 
    }

    public async Task<MemoryStream> GetOrdersReportAsync(GetOrdersReportQueryParams queryParams)
    {
        var from = queryParams.From ?? DateOnly.MinValue;
        var to = queryParams.To ?? DateOnly.MaxValue;
        
        var fromDateTime = new DateTime(from.Year, from.Month, from.Day, 0, 0, 0, DateTimeKind.Utc);
        var toDateTime = new DateTime(to.Year, to.Month, to.Day, 23, 59, 59, DateTimeKind.Utc);

        var orders = await _context.ShopOrders
            .Where(o => o.CreatedAt >= fromDateTime && o.CreatedAt <= toDateTime)
            .ApplyIncludes()
            .ToListAsync();

        
        var ordersResponse = _mapper.Map<List<OrderResponse>>(orders);
        
        var data = queryParams.Format switch
        {
            FileExportFormat.Csv => ordersResponse.ToCsv(),
            FileExportFormat.Json => ordersResponse.ToJson(),
            FileExportFormat.Xml => ordersResponse.ToXml(),
            _ => throw new ArgumentOutOfRangeException()
        };
        
        var stream = new MemoryStream(Encoding.Unicode.GetBytes(data));
        
        return stream;
    }

    public List<string> GetOrderStatusesAsync()
    {
        return Enum.GetValues(typeof(OrderStatus))
            .Cast<OrderStatus>()
            .Select(s => s.ToString())
            .ToList();
    }

    public async Task CreateAsync(CreateOrderRequest createOrderRequest)
    {
        var user = await _userService.GetCurrentUserAsync();
        var userId = user.Id;
        var sql = "call create_order (@UserId, @City, @Street, @Building, @Apartment, @Region)";
        
        var rowsAffected = await _context.Database.ExecuteSqlRawAsync(sql,
        [
            new NpgsqlParameter
            {
              ParameterName  = "UserId",
              NpgsqlDbType = NpgsqlDbType.Integer,
              Value = userId
            },
            new NpgsqlParameter
            {
                ParameterName = "City",
                NpgsqlDbType = NpgsqlDbType.Varchar,
                Value = createOrderRequest.City,
            },
            new NpgsqlParameter
            {
                ParameterName = "Street",
                NpgsqlDbType = NpgsqlDbType.Varchar,
                Value = createOrderRequest.Street,
            },
            new NpgsqlParameter
            {
                ParameterName = "Building",
                NpgsqlDbType = NpgsqlDbType.Varchar,
                Value = createOrderRequest.Building,
            },
            new NpgsqlParameter
            {
                ParameterName = "Apartment",
                NpgsqlDbType = NpgsqlDbType.Varchar,
                Value = createOrderRequest.Apartment ?? (object)DBNull.Value,
            },
            new NpgsqlParameter
            {
                ParameterName = "Region",
                NpgsqlDbType = NpgsqlDbType.Varchar,
                Value = createOrderRequest.Region ?? (object)DBNull.Value,
            }
        ]);

        if (rowsAffected == 0) throw new Exception("Order was not created");
    }
    
    public async Task<OrderResponse> UpdateStatusAsync(int orderId, OrderStatusDto status)
    {
        var statusExists = Enum.TryParse<OrderStatus>(status.Name, out _);
        
        if (!statusExists) throw new EntityNotFoundException("Status not found");
        
        var order = await _context.ShopOrders.ApplyIncludes().FirstOrDefaultAsync(o => o.Id == orderId);
        
        if (order == null) throw new EntityNotFoundException("Order not found");
        
        var orderStatus = Enum.Parse<OrderStatus>(status.Name);
        
        order.Status = orderStatus;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException e)
        {
            var postgresException = e.InnerException as PostgresException;
            if (postgresException.SqlState == "23514")
            {
                throw new InvalidOperationException("Product is out of stock");
            }
        }
        
        return _mapper.Map<OrderResponse>(order);
    }
}