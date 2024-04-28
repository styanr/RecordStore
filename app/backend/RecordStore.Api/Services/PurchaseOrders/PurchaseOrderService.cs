﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RecordStore.Api.Context;
using RecordStore.Api.Dto.PurchaseOrders;
using RecordStore.Api.Entities;
using RecordStore.Api.Extensions;
using RecordStore.Api.RequestHelpers.QueryParams;

namespace RecordStore.Api.Services.PurchaseOrders;

public class PurchaseOrderService : IPurchaseOrderService
{
    private readonly RecordStoreContext _context;
    private readonly IMapper _mapper;

    public PurchaseOrderService(RecordStoreContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<List<SupplierResponse>> GetSuppliersAsync()
    {
        var suppliers = await _context.Suppliers.ToListAsync();
        return _mapper.Map<List<SupplierResponse>>(suppliers);
    }

    public Task CreatePurchaseOrderAsync(PurchaseOrderCreateRequest purchaseOrderCreateRequest)
    {
        var purchaseOrder = _mapper.Map<PurchaseOrder>(purchaseOrderCreateRequest);
        
        _context.PurchaseOrders.Add(purchaseOrder);
        return _context.SaveChangesAsync();
    }

    public async Task<PagedResult<PurchaseOrderResponse>> GetPurchaseOrdersAsync(GetPurchaseOrderQueryParams queryParams)
    {
        var query = _context.PurchaseOrders.AsQueryable().ApplyIncludes().ApplyFiltersAndOrderBy(queryParams);
        
        var pagedResult = await query.GetPagedAsync(queryParams.Page, queryParams.PageSize);
        
        var purchaseOrderResponses = _mapper.Map<PagedResult<PurchaseOrderResponse>>(pagedResult);
        
        return purchaseOrderResponses;
    }
}