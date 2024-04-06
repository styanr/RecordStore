namespace RecordStore.Api.Dto.Tracks;

public class TrackResponseDTO
{
    public string Title { get; set; }
    public int DurationSeconds { get; set; }
    public string? TrackOrder { get; set; }
}