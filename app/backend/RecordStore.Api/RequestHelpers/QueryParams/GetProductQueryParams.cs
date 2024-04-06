namespace RecordStore.Api.RequestHelpers.QueryParams;

public class GetProductQueryParams : GetQueryParams
{
    public string? Title { get; set; }
    public string? Artist { get; set; }
    public string? Genre { get; set; }
    public string? Format { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
}
