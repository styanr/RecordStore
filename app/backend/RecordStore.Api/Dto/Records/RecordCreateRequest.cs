using System.ComponentModel.DataAnnotations;

namespace RecordStore.Api.Dto.Records;

public class RecordCreateRequest
{
    [Required]
    public string Title { get; set; } = null!;

    [Required]
    public string? Description { get; set; }

    [Required]
    public DateOnly? ReleaseDate { get; set; }

    [Required]
    public List<int> ArtistIds { get; set; }
    
    [Required]
    public List<string> GenreNames { get; set; }
}