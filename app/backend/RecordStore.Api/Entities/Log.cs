namespace RecordStore.Api.Entities;

public class Log
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public string ActionType { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Description { get; set; }
    
    public virtual AppUser User { get; set; } = null!;
}