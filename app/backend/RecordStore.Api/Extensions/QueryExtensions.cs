using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RecordStore.Api.Dto.Products;
using RecordStore.Api.Entities;
using RecordStore.Api.RequestHelpers;
using RecordStore.Api.RequestHelpers.QueryParams;

namespace RecordStore.Api.Extensions;

public abstract class PagedResultBase
{
    public int CurrentPage { get; set; }
    public int PageCount { get; set; }
    public int PageSize { get; set; }
    public int RowCount { get; set; }

    public int FirstRowOnPage => (CurrentPage - 1) * PageSize + 1;

    public int LastRowOnPage => Math.Min(CurrentPage * PageSize, RowCount);
}

public class PagedResult<T> : PagedResultBase where T : class
{
    public PagedResult()
    {
        Results = new List<T>();
    }

    public IList<T> Results { get; set; }
}

public static class QueryExtensions
{
    public static async Task<PagedResult<T>> GetPagedAsync<T>(this IQueryable<T> query,
        int page, int pageSize) where T : class
    {
        var result = new PagedResult<T>();
        result.CurrentPage = page;
        result.PageSize = pageSize;
        result.RowCount = query.Count();


        var pageCount = (double)result.RowCount / pageSize;
        result.PageCount = (int)Math.Ceiling(pageCount);

        var skip = (page - 1) * pageSize;
        result.Results = await query.Skip(skip).Take(pageSize).ToListAsync();

        return result;
    }

    public static IQueryable<T> OrderByBoolean<T, TKey>(this IQueryable<T> source, Expression<Func<T, TKey>> selector,
        bool ascending)
    {
        if (ascending)
            return source.OrderBy(selector);
        return source.OrderByDescending(selector);
    }

    /// <summary>
    ///     Applies necessary includes to the <c>Product</c> select query
    /// </summary>
    public static IQueryable<Product> ApplyIncludes(this IQueryable<Product> query)
    {

        return query
            .Include(p => p.Record)
            .ThenInclude(r => r.Genres)
            .Include(p => p.Record)
            .ThenInclude(r => r.Artists)
            .Include(p => p.Format)
            .Include(p => p.Reviews);
    }

    public static IQueryable<Product> ApplyFiltersAndOrderBy(this IQueryable<Product> query, GetProductQueryParams queryParams)
    {
        if (!string.IsNullOrWhiteSpace(queryParams.Title))
        {
            string normalizedTitle = queryParams.Title.ToLower().Trim();
            query = query.Where(p => p.Record.Title.ToLower().Contains(normalizedTitle));
        }
        
        if (!string.IsNullOrWhiteSpace(queryParams.Artist))
        {
            string normalizedArtist = queryParams.Artist.ToLower().Trim();
            query = query.Where(p => p.Record.Artists.Any(a => a.Name.ToLower().Contains(normalizedArtist)));
        }
        
        if (!string.IsNullOrWhiteSpace(queryParams.Genre))
        {
            query = query.Where(p => p.Record.Genres.Any(g => g.Name == queryParams.Genre));
        }

        if (!string.IsNullOrWhiteSpace(queryParams.Format))
        {
            query = query.Where(p => p.Format.FormatName == queryParams.Format);
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
            "rating" => query.OrderByBoolean(p => p.Reviews.Average(r => r.Rating), sortAsc),
            "reviewCount" => query.OrderByBoolean(p => p.Reviews.Count, sortAsc),
            _ => query.OrderByBoolean(p => p.Record.Title, sortAsc)
        };
        
        return query;
    }
    
    public static IQueryable<Artist> ApplyFiltersAndOrderBy(this IQueryable<Artist> query, GetArtistQueryParams queryParams)
    {
        if (!string.IsNullOrWhiteSpace(queryParams.Name))
        {
            string normalizedName = queryParams.Name.ToLower().Trim();
            query = query.Where(a => a.Name.ToLower().Contains(normalizedName));
        }
        
        bool sortAsc = queryParams.OrderDirection == "asc";

        query = queryParams.OrderBy switch
        {
            "name" => query.OrderByBoolean(a => a.Name, sortAsc),
            _ => query.OrderByBoolean(a => a.Name, sortAsc)
        };
        
        return query;
    }
    
    public static IQueryable<Record> ApplyFiltersAndOrderBy(this IQueryable<Record> query, GetRecordQueryParams queryParams)
    {
        if (!string.IsNullOrWhiteSpace(queryParams.Title))
        {
            string normalizedTitle = queryParams.Title.ToLower().Trim();
            query = query.Where(r => r.Title.ToLower().Contains(normalizedTitle));
        }
        
        if (queryParams.MinYear is not null)
        {
            query = query.Where(r => r.ReleaseDate >= new DateOnly(queryParams.MinYear.Value, 1, 1));
        }
        if (queryParams.MaxYear is not null)
        {
            query = query.Where(r => r.ReleaseDate <= new DateOnly(queryParams.MaxYear.Value, 12, 31));
        }
        
        if (queryParams.HasProducts)
        {
            query = query
                .Include(r => r.Products)
                .Where(r => r.Products.Count > 0);
        }
        
        bool sortAsc = queryParams.OrderDirection == "asc";

        query = queryParams.OrderBy switch
        {
            "title" => query.OrderByBoolean(r => r.Title, sortAsc),
            "releaseDate" => query.OrderByBoolean(r => r.ReleaseDate, sortAsc),
            _ => query.OrderByBoolean(r => r.Title, sortAsc)
        };
        
        return query;
    }
    
    public static IQueryable<ShopOrder> ApplyIncludes(this IQueryable<ShopOrder> query)
    {
        return query
            .Include(o => o.OrderLines)
                .ThenInclude(ol => ol.Product)
                .ThenInclude(p => p.Record)
            .Include(o => o.OrderLines)
                .ThenInclude(ol => ol.Product)
                .ThenInclude(p => p.Format)
            .Include(o => o.OrderLines)
                .ThenInclude(ol => ol.Product)
                .ThenInclude(p => p.Reviews);
    }
    
    public static IQueryable<ShopOrder> ApplyFiltersAndOrderBy(this IQueryable<ShopOrder> query, GetOrderQueryParams queryParams)
    {
        bool sortAsc = queryParams.OrderDirection == "asc";
        
        query = queryParams.OrderBy switch
        {
            "orderDate" => query.OrderByBoolean(o => o.CreatedAt, sortAsc),
            "totalPrice" => query.OrderByBoolean(o => o.OrderLines.Sum(ol => ol.Product.Price * ol.Quantity), sortAsc),
            _ => query.OrderByBoolean(o => o.CreatedAt, sortAsc)
        };
        
        return query;
    }

    public static IQueryable<PurchaseOrder> ApplyIncludes(this IQueryable<PurchaseOrder> query)
    {
        return query.Include(o => o.Supplier)
            .Include(o => o.PurchaseOrderLines);
    }
    
    public static IQueryable<PurchaseOrder> ApplyFiltersAndOrderBy(this IQueryable<PurchaseOrder> query, GetPurchaseOrderQueryParams queryParams)
    {
        bool sortAsc = queryParams.OrderDirection == "asc";
        
        query = queryParams.OrderBy switch
        {
            "orderDate" => query.OrderByBoolean(o => o.CreatedAt, sortAsc),
            "totalPrice" => query.OrderByBoolean(o => o.Total, sortAsc),
            _ => query.OrderByBoolean(o => o.CreatedAt, sortAsc)
        };
        
        return query;
    }
    
    public static IQueryable<AppUser> ApplyIncludes(this IQueryable<AppUser> query)
    {
        return query.Include(u => u.Role);
    }
    
    public static IQueryable<AppUser> ApplyFiltersAndOrderBy(this IQueryable<AppUser> query, GetUserQueryParams queryParams)
    {
        if (!string.IsNullOrWhiteSpace(queryParams.Email))
        {
            string normalizedEmail = queryParams.Email.ToLower().Trim();
            query = query.Where(u => u.Email.ToLower().Contains(normalizedEmail));
        }
        
        if (!string.IsNullOrWhiteSpace(queryParams.RoleName))
        {
            query = query.Where(u => u.Role.RoleName == queryParams.RoleName);
        }
        
        if (!string.IsNullOrWhiteSpace(queryParams.Name))
        {
            string normalizedName = queryParams.Name.ToLower().Trim();
            query = query.Where(u => u.FirstName.ToLower().Contains(normalizedName) || u.LastName.ToLower().Contains(normalizedName));
        }
        
        bool sortAsc = queryParams.OrderDirection == "asc";

        query = queryParams.OrderBy switch
        {
            "id" => query.OrderByBoolean(u => u.Id, sortAsc),
            "email" => query.OrderByBoolean(u => u.Email, sortAsc),
            "name" => query.OrderByBoolean(u => u.FirstName, sortAsc),
            "role" => query.OrderByBoolean(u => u.Role.RoleName, sortAsc),
            "createdAt" => query.OrderByBoolean(u => u.CreatedAt, sortAsc),
            _ => query.OrderByBoolean(u => u.Email, sortAsc)
        };
        
        return query;
    }
    
    
}