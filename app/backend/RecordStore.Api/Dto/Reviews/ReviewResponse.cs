namespace RecordStore.Api.Dto.Reviews;

public class ReviewResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserFullName { get; set; }
    
    public int Rating { get; set; }

    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}