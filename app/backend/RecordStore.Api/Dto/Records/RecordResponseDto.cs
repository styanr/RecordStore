namespace RecordStore.Api.Dto.Records;

public class RecordResponseDto
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateOnly? ReleaseDate { get; set; }

}