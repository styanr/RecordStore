namespace RecordStore.Api.RequestHelpers.QueryParams;

public enum FileExportFormat
{
    Json,
    Xml,
    Csv
}

public static class FileExportFormatExtensions
{
    public static string ToFileExtension(this FileExportFormat format)
    {
        return format switch
        {
            FileExportFormat.Json => "json",
            FileExportFormat.Xml => "xml",
            FileExportFormat.Csv => "csv",
            _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
        };
    }
}