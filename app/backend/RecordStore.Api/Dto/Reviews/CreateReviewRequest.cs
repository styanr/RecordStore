namespace RecordStore.Api.Dto.Reviews;

public class CreateReviewRequest
{
    public int ProductId { get; set; }

    public int Rating { get; set; }

    public string? Description { get; set; }
}