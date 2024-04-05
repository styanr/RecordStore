using Microsoft.EntityFrameworkCore;
using RecordStore.Api.Context;
using RecordStore.Api.Entities;
using RecordStore.Api.Extensions;
using RecordStore.Api.RequestHelpers;

namespace RecordStore.Api.Services.Products;

public class ProductService : IProductService
{
    private readonly RecordStoreContext _context;

    public ProductService(RecordStoreContext context)
    {
        _context = context;
    }
    public async Task<List<Product>> GetAllAsync(GetProductQueryParams queryParams)
    {
        var query = _context.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(queryParams.Genre))
        {
            query = query.Where(p => p.Record.Genres.Any(g => g.Name == queryParams.Genre));
        }
        if (queryParams.MinPrice is not null)
        {
            query = query.Where(p => p.Price >= queryParams.MinPrice);
        }
        if (queryParams.MaxPrice is not null)
        {
            query = query.Where(p => p.Price <= queryParams.MaxPrice);
        }
        
        bool sortAsc = queryParams.OrderDirection == "asc";

        query = queryParams.OrderBy switch
        {
            "title" => query.OrderByBoolean(p => p.Record.Title, sortAsc),
            "price" => query.OrderByBoolean(p => p.Price, sortAsc),
            "releaseDate" => query.OrderByBoolean(p => p.Record.ReleaseDate, sortAsc),
            _ => query.OrderByBoolean(p => p.Record.Title, sortAsc)
        };

        query = query
            .Include(p => p.Record)
                .ThenInclude(r => r.Genres)
            .Include(p => p.Record)
                .ThenInclude(r => r.Artists)
            .Include(p => p.Format)
            .Include(p => p.TrackProducts)
                .ThenInclude(tp => tp.Track);
        PagedResult<Product> pagedResult = await query.GetPagedAsync(queryParams.Page, queryParams.PageSize);
        
        return pagedResult.Results.ToList();
    }

    public async Task<Product> GetByIdAsync(int id)
    {
        var product = await _context.Products
            .Include(p => p.Record)
                .ThenInclude(r => r.Genres)
            .Include(p => p.Record)
                .ThenInclude(r => r.Artists)
            .Include(p => p.Format)
            .Include(p => p.TrackProducts)
                .ThenInclude(tp => tp.Track)
            .FirstOrDefaultAsync(p => p.Id == id);
        
        if (product is null)
        {
            throw new KeyNotFoundException("Product not found");
        }
        
        return product;
    }

    public Task<Product> CreateAsync(Product entity)
    {
        throw new NotImplementedException();
    }

    public Task<Product> UpdateAsync(Product entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}