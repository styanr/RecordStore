using RecordStore.Api.Dto.Artists;
using RecordStore.Api.Dto.Formats;
using RecordStore.Api.Dto.Genres;

namespace RecordStore.Api.Dto.Products;

public class ProductShortResponseDto
{
    public int Id { get; set; }
    public string ImageUrl { get; set; }
    public string Title { get; set; }
    public decimal Price { get; set; }
    public float? AverageRating { get; set; }
    public int TotalRatings { get; set; }
    public DateOnly ReleaseDate { get; set; }
    
    public FormatResponseDto Format { get; set; }
    public List<GenreResponseDto> Genres { get; set; }
    public List<ArtistResponseDto> Artists { get; set; }
}