using Microsoft.AspNetCore.Mvc;
using RecordStore.Api.Dto.Artists;
using RecordStore.Api.RequestHelpers.QueryParams;
using RecordStore.Api.Services.Artists;

namespace RecordStore.Api.Controllers;

[ApiController]
[Route("api/artists")]
public class ArtistController
{
    private readonly IArtistService _artistService;

    public ArtistController(IArtistService artistService)
    {
        _artistService = artistService;
    }
    
    [HttpGet]
    public async Task<ActionResult<List<ArtistResponseDto>>> Get([FromQuery] GetArtistQueryParams queryParams)
    {
        var artists = await _artistService.GetAllAsync(queryParams);
        return artists;
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<ArtistFullResponseDto>> GetById(int id)
    {
        var artist = await _artistService.GetByIdAsync(id);
        return artist;
    }
}