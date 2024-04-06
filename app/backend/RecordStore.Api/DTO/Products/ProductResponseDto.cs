using RecordStore.Api.DTO.Artists;
using RecordStore.Api.DTO.Formats;
using RecordStore.Api.DTO.Genres;

namespace RecordStore.Api.DTO.Products;

public class ProductResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public decimal Price { get; set; }
    public DateOnly ReleaseDate { get; set; }
    
    public FormatResponseDto Format { get; set; }
    public List<GenreResposeDto> Genres { get; set; }
    public List<ArtistResponseDto> Artists { get; set; }
}