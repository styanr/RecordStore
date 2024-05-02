using RecordStore.Api.Dto.Artists;
using RecordStore.Api.Dto.Formats;
using RecordStore.Api.Dto.Genres;
using RecordStore.Api.Dto.Tracks;

namespace RecordStore.Api.Dto.Products;

/// <summary>
/// Same as <see cref="ProductShortResponseDto"/> but also includes the track list.
/// </summary>
public class ProductResponseDto : ProductShortResponseDto
{
    public int RecordId { get; set; }
    public string Description { get; set; }
}