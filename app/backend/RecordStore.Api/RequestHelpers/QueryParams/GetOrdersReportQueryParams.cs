using System.Text.Json.Serialization;

namespace RecordStore.Api.RequestHelpers.QueryParams;

public class GetOrdersReportQueryParams
{
    public FileExportFormat Format { get; set; }
    public DateOnly? From { get; set; }
    public DateOnly? To { get; set; }
}