using RecordStore.Api.Dto.Genres;

namespace RecordStore.Api.Services.Genres;

public interface IGenreService
{
    public Task<List<GenreResponseDto>> GetGenresAsync(string name);
}