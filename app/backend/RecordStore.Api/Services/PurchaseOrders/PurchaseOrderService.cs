using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RecordStore.Api.Context;
using RecordStore.Api.Dto.PurchaseOrders;
using RecordStore.Api.Entities;

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
}