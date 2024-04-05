using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace RecordStore.Api.RequestHelpers;

public class GetProductQueryParams
{
    public string? Genre { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? OrderBy { get; set; }
    public string? OrderDirection { get; set; }
    
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 15;
}
