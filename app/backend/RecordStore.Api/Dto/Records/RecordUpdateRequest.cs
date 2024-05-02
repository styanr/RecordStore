using System.ComponentModel.DataAnnotations;

namespace RecordStore.Api.Dto.Records;

public class RecordUpdateRequest
{
    [Required]
    public string Title { get; set; } = null!;
    
    public string? Description { get; set; }

    [Required]
    public DateOnly? ReleaseDate { get; set; }

    [Required]
    public List<int> ArtistIds { get; set; }
    
    [Required]
    public List<string> GenreNames { get; set; }
}