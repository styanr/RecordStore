namespace RecordStore.Api.RequestHelpers.QueryParams;

public class GetRecordQueryParams : GetQueryParams
{
    public bool HasProducts { get; set; } = true;
    public string? Title { get; set; }
    public int? MinYear { get; set; }
    public int? MaxYear { get; set; }
}