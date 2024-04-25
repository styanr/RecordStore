using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RecordStore.Api.Context;
using RecordStore.Api.Dto.Products;
using RecordStore.Api.Entities;
using RecordStore.Api.Exceptions;
using RecordStore.Api.Extensions;
using RecordStore.Api.RequestHelpers;
using RecordStore.Api.RequestHelpers.QueryParams;

namespace RecordStore.Api.Services.Products;

public class ProductService : IProductService
{
    private readonly RecordStoreContext _context;
    private readonly IMapper _mapper;

    public ProductService(RecordStoreContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<PagedResult<ProductResponseDto>> GetAllAsync(GetProductQueryParams queryParams)
    {
        var query = _context.Products
            .AsQueryable()
            .ApplyFiltersAndOrderBy(queryParams)
            .ApplyIncludes();
        
        PagedResult<Product> pagedResult = await query.GetPagedAsync(queryParams.Page, queryParams.PageSize);
        
        PagedResult<ProductResponseDto> pagedResultDto = _mapper.Map<PagedResult<ProductResponseDto>>(pagedResult);
        
        return pagedResultDto;
    }

    public async Task<ProductFullResponseDto> GetByIdAsync(int id)
    {
        var product = await _context.Products
            .AsQueryable()
            .ApplyIncludes()
            .FirstOrDefaultAsync(p => p.Id == id);
        
        if (product is null)
        {
            throw new ProductNotFoundException();
        }
        
        var productDto = _mapper.Map<ProductFullResponseDto>(product);
        
        return productDto;
    }

    public async Task<List<ProductResponseDto>> GetByRecordIdAsync(int recordId, GetRecordProductQueryParams queryParams)
    {
        var query = _context.Products
            .AsQueryable()
            .Where(p => p.RecordId == recordId)
            .ApplyIncludes();

        var sortAsc = queryParams.OrderDirection == "asc";

        query = queryParams.OrderBy switch
        {
            "price" => query.OrderByBoolean(p => p.Price, sortAsc),
            _ => query.OrderByBoolean(p => p.Id, sortAsc)
        };
        
        var products = await query.ToListAsync();
        
        return _mapper.Map<List<ProductResponseDto>>(products);
    }

    public Task<ProductFullResponseDto> CreateAsync(Product entity)
    {
        throw new NotImplementedException();
    }

    public Task<ProductFullResponseDto> UpdateAsync(Product entity)
    {
        throw new NotImplementedException();
    }

    public async Task<PriceMinMaxResponse> GetPriceMinMaxAsync()
    {
        return new PriceMinMaxResponse
        {
            MinPrice = await _context.Products.MinAsync(p => p.Price),
            MaxPrice = await _context.Products.MaxAsync(p => p.Price)
        };
    }

    public Task<bool> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}