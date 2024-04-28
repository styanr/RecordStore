using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RecordStore.Api.Context;
using RecordStore.Api.Dto.Products;
using RecordStore.Api.Dto.Users;
using RecordStore.Api.Entities;
using RecordStore.Api.Exceptions;
using RecordStore.Api.Extensions;
using RecordStore.Api.RequestHelpers;
using RecordStore.Api.RequestHelpers.QueryParams;
using RecordStore.Api.Services.Users;

namespace RecordStore.Api.Services.Products;

public class ProductService : IProductService
{
    private readonly RecordStoreContext _context;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public ProductService(RecordStoreContext context, IMapper mapper, IUserService userService)
    {
        _context = context;
        _mapper = mapper;
        _userService = userService;
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
        
        UserResponse currentUser;
        try
        {
            currentUser = await _userService.GetCurrentUserAsync();
        }
        catch (InvalidOperationException)
        {
            currentUser = null;
        }
        
        if (currentUser?.Role is "admin" or "employee")
        {
            // TODO: should extract that to a separate method
            productDto.Quantity = _context.Inventories
                .Where(i => i.ProductId == id)
                .Sum(i => i.Quantity);
        }
        
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

    public async Task<ProductFullResponseDto> UpdateAsync(int id, ProductUpdateRequest entity)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product is null)
        {
            throw new ProductNotFoundException();
        }

        _context.Products.Entry(product).CurrentValues.SetValues(entity);

        var format = await _context.Formats
            .FirstOrDefaultAsync(f => f.FormatName == entity.Format);

        if (format is null)
        {
            format = new Format
            {
                FormatName = entity.Format
            };
            _context.Formats.Add(format);
            
            await _context.SaveChangesAsync();
        }
        
        product.Format = format;
        
        await _context.SaveChangesAsync();
        
        var productFull = await _context.Products
            .AsQueryable()
            .ApplyIncludes()
            .FirstOrDefaultAsync(p => p.Id == id);
        
        var productDto = _mapper.Map<ProductFullResponseDto>(productFull);
        
        return productDto;
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