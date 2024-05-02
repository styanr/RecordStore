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
    public async Task<PagedResult<ProductShortResponseDto>> GetAllAsync(GetProductQueryParams queryParams)
    {
        var query = _context.Products
            .AsQueryable()
            .ApplyFiltersAndOrderBy(queryParams)
            .ApplyIncludes();
        
        PagedResult<Product> pagedResult = await query.GetPagedAsync(queryParams.Page, queryParams.PageSize);
        
        PagedResult<ProductShortResponseDto> pagedResultDto = _mapper.Map<PagedResult<ProductShortResponseDto>>(pagedResult);
        
        return pagedResultDto;
    }

    public async Task<ProductResponseDto> GetByIdAsync(int id)
    {
        UserResponse? currentUser = await GetCurrentUserAsync();
        
        var query = _context.Products
            .AsQueryable()
            .ApplyIncludes();
        
        if (currentUser?.Role is "admin" or "employee")
        {
            query = query.Include(p => p.Inventories);
        }
         
        var product = await query.FirstOrDefaultAsync(p => p.Id == id);
        
        if (product is null)
        {
            throw new ProductNotFoundException();
        }
        
        var productDto = MapProductDto(product, currentUser);
        
        return productDto;
    }
    
    private ProductResponseDto MapProductDto(Product product, UserResponse? currentUser)
    {
        // Determine the appropriate DTO based on user role
        return currentUser?.Role is "admin" or "employee"
            ? _mapper.Map<ProductFullResponseDto>(product)
            : _mapper.Map<ProductResponseDto>(product);
    }
    
    private async Task<UserResponse?> GetCurrentUserAsync()
    {
        try
        {
            return await _userService.GetCurrentUserAsync();
        }
        catch (Exception e) when (e is InvalidOperationException or UserNotFoundException)
        {
            return null;
        }
    }

    public async Task<List<ProductShortResponseDto>> GetByRecordIdAsync(int recordId, GetRecordProductQueryParams queryParams)
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
        
        return _mapper.Map<List<ProductShortResponseDto>>(products);
    }

    public async Task<ProductFullResponseDto> CreateAsync(ProductCreateRequest entity)
    {
        // that should be done in the record service
        var record = await _context.Records
            .FirstOrDefaultAsync(r => r.Id == entity.RecordId);

        if (record is null)
        {
            throw new RecordNotFoundException();
        }
        
        var format = await GetOrCreateFormatAsync(entity.FormatName);

        var product = _mapper.Map<Product>(entity);
        
        product.Format = format;
        
        var inventory = new Inventory
        {
            Product = product,
            Quantity = entity.Quantity,
            Location = entity.Location,
            RestockLevel = entity.RestockLevel
        };
        _context.Products.Add(product);
        
        _context.Inventories.Add(inventory);
        
        await _context.SaveChangesAsync();
        
        // TODO: use a private method to get product
        var productFull = await _context.Products
            .AsQueryable()
            .ApplyIncludes()
            .Include(p => p.Inventories)
            .FirstOrDefaultAsync(p => p.Id == product.Id);
        
        return _mapper.Map<ProductFullResponseDto>(productFull);
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

        await UpdateProductInventory(id, entity);
        
        var format = await GetOrCreateFormatAsync(entity.FormatName);
        
        product.Format = format;
        
        await _context.SaveChangesAsync();
        
        var productFull = await _context.Products
            .AsQueryable()
            .ApplyIncludes()
            .Include(p => p.Inventories)
            .FirstOrDefaultAsync(p => p.Id == id);
        
        return _mapper.Map<ProductFullResponseDto>(productFull);
}
    // this will be removed in the future
    private async Task UpdateProductInventory(int productId, ProductUpdateRequest entity)
    {
        var productInventory = await _context.Inventories
            .FirstOrDefaultAsync(i => i.ProductId == productId);
        
        if (productInventory is null)
        {
            throw new EntityNotFoundException("Inventory");
        }
        
        _context.Inventories.Entry(productInventory).CurrentValues.SetValues(entity);
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
    
    private async Task<Format> GetOrCreateFormatAsync(string formatName)
    {
        var format = await _context.Formats.FirstOrDefaultAsync(f => f.FormatName == formatName);

        if (format is null)
        {
            format = new Format
            {
                FormatName = formatName
            };
            _context.Formats.Add(format);
        }

        return format;
    }
}