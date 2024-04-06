using RecordStore.Api.Dto.Artists;
using RecordStore.Api.Dto.Formats;
using RecordStore.Api.Dto.Genres;
using RecordStore.Api.Dto.Tracks;

namespace RecordStore.Api.Dto.Products;

/// <summary>
/// Same as <see cref="ProductResponseDto"/> but also includes the track list.
/// </summary>
public class ProductFullResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public decimal Price { get; set; }
    public DateOnly ReleaseDate { get; set; }
    
    public FormatResponseDto Format { get; set; }
    public List<GenreResposeDto> Genres { get; set; }
    public List<ArtistResponseDto> Artists { get; set; }
    public List<TrackResponseDTO> Tracklist { get; set; }
}