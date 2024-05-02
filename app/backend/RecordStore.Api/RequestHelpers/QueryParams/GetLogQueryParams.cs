namespace RecordStore.Api.RequestHelpers.QueryParams;

public class GetLogQueryParams
{
    public DateOnly? From { get; set; }
    public DateOnly? To { get; set; }
}