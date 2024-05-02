namespace RecordStore.Api.Dto.Logs;

public class LogResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string ActionType { get; set; } = null!;
    public DateTime Timestamp { get; set; }
    public string? Description { get; set; }
}