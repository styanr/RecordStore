using Microsoft.AspNetCore.Mvc;
using RecordStore.Api.Dto.Genres;
using RecordStore.Api.Services.Genres;

namespace RecordStore.Api.Controllers;

[ApiController]
[Route("api/genres")]
public class GenreController
{
    private readonly IGenreService _formatService;

    public GenreController(IGenreService formatService)
    {
        _formatService = formatService;
    }
    [HttpGet]
    public async Task<ActionResult<List<GenreResponseDto>>> GetGenres(string name)
    {
        return await _formatService.GetGenresAsync(name);
    }
}