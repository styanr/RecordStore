namespace RecordStore.Api.Dto.Logs;

public class LogCreateRequest
{
    public string ActionType { get; set; } = null!;
    public string Description { get; set; } = null!;
}