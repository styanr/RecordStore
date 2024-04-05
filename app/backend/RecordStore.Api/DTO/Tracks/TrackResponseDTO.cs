namespace RecordStore.Api.DTO.Tracks;

public class TrackResponseDTO
{
    public string Title { get; set; }
    public int DurationSeconds { get; set; }
    public string? Order { get; set; }
}