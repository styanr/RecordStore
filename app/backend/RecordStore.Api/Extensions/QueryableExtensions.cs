using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace RecordStore.Api.Extensions;

public abstract class PagedResultBase
{
    public int CurrentPage { get; set; } 
    public int PageCount { get; set; } 
    public int PageSize { get; set; } 
    public int RowCount { get; set; }
 
    public int FirstRowOnPage
    {
 
        get { return (CurrentPage - 1) * PageSize + 1; }
    }
 
    public int LastRowOnPage
    {
        get { return Math.Min(CurrentPage * PageSize, RowCount); }
    }
}
 
public class PagedResult<T> : PagedResultBase where T : class
{
    public IList<T> Results { get; set; }
 
    public PagedResult()
    {
        Results = new List<T>();
    }
}

public static class QueryableExtensions
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
    
    public static IQueryable<T> OrderByBoolean<T, TKey>(this IQueryable<T> source, Expression<Func<T, TKey>> selector, bool ascending)
    {
        if (ascending)
        {
            return source.OrderBy(selector);
        }
        else
        {
            return source.OrderByDescending(selector);
        }
    }
}