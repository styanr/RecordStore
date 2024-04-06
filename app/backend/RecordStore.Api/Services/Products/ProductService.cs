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
    public async Task<List<ProductResponseDto>> GetAllAsync(GetProductQueryParams queryParams)
    {
        var query = _context.Products
            .AsQueryable()
            .ApplyFiltersAndOrderBy(queryParams)
            .ApplyIncludes();
        
        PagedResult<Product> pagedResult = await query.GetPagedAsync(queryParams.Page, queryParams.PageSize);
        
        var productDtos = _mapper.Map<List<ProductResponseDto>>(pagedResult.Results);
        
        return productDtos;
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

    public Task<ProductFullResponseDto> CreateAsync(Product entity)
    {
        throw new NotImplementedException();
    }

    public Task<ProductFullResponseDto> UpdateAsync(Product entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}