using System.ComponentModel.DataAnnotations;

namespace RecordStore.Api.Dto.Reviews;

public class CreateReviewRequest
{
    public int ProductId { get; set; }
    
    [Range(1, 5)]
    public int Rating { get; set; }

    public string? Description { get; set; }
}