using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RecordStore.Api.Context;
using RecordStore.Api.Dto.PurchaseOrders;
using RecordStore.Api.Entities;
using RecordStore.Api.Extensions;
using RecordStore.Api.RequestHelpers.QueryParams;
using RecordStore.Api.Services.Logs;

namespace RecordStore.Api.Services.PurchaseOrders;

public class PurchaseOrderService : IPurchaseOrderService
{
    private readonly RecordStoreContext _context;
    private readonly IMapper _mapper;
    private readonly ILogService _logService;

    public PurchaseOrderService(RecordStoreContext context, IMapper mapper, ILogService logService)
    {
        _context = context;
        _mapper = mapper;
        _logService = logService;
    }

    public async Task<List<SupplierResponse>> GetSuppliersAsync()
    {
        var suppliers = await _context.Suppliers.ToListAsync();
        var suppliersResponse = _mapper.Map<List<SupplierResponse>>(suppliers);

        await _logService.LogActionAsync("Get Suppliers", "Retrieved list of suppliers.");

        return suppliersResponse;
    }

    public async Task CreatePurchaseOrderAsync(PurchaseOrderCreateRequest purchaseOrderCreateRequest)
    {
        var purchaseOrder = _mapper.Map<PurchaseOrder>(purchaseOrderCreateRequest);

        _context.PurchaseOrders.Add(purchaseOrder);
        await _context.SaveChangesAsync();

        await _logService.LogActionAsync("Create Purchase Order", $"Purchase order created with ID: {purchaseOrder.Id}");
    }

    public async Task<PagedResult<PurchaseOrderResponse>> GetPurchaseOrdersAsync(GetPurchaseOrderQueryParams queryParams)
    {
        var query = _context.PurchaseOrders.AsQueryable().ApplyIncludes().ApplyFiltersAndOrderBy(queryParams);

        var pagedResult = await query.GetPagedAsync(queryParams.Page, queryParams.PageSize);

        var purchaseOrderResponses = _mapper.Map<PagedResult<PurchaseOrderResponse>>(pagedResult);

        await _logService.LogActionAsync("Get Purchase Orders", "Retrieved purchase orders.");

        return purchaseOrderResponses;
    }

    public async Task DeletePurchaseOrderAsync(int id)
    {
        var purchaseOrder = await _context.PurchaseOrders.FirstOrDefaultAsync(po => po.Id == id);

        if (purchaseOrder is null)
        {
            throw new InvalidOperationException("Purchase order not found");
        }

        _context.PurchaseOrders.Remove(purchaseOrder);

        await _context.SaveChangesAsync();

        await _logService.LogActionAsync("Delete Purchase Order", $"Purchase order deleted with ID: {id}");
    }
}