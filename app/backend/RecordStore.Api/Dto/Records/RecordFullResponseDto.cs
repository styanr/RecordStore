using RecordStore.Api.Dto.Artists;
using RecordStore.Api.Dto.Genres;


namespace RecordStore.Api.Dto.Records;
public class RecordFullResponseDto : RecordResponseDto
{
    public IEnumerable<ArtistResponseDto> Artists { get; set; } = null!;

    public IEnumerable<GenreResponseDto> Genres { get; set; } = null!;
}