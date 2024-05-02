using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RecordStore.Api.Dto.Artists;
using RecordStore.Api.Extensions;
using RecordStore.Api.RequestHelpers.QueryParams;
using RecordStore.Api.Services.Artists;

namespace RecordStore.Api.Controllers;

[ApiController]
[Route("api/artists")]
public class ArtistController : ControllerBase
{
    private readonly IArtistService _artistService;

    public ArtistController(IArtistService artistService)
    {
        _artistService = artistService;
    }
    
    [HttpGet]
    public async Task<ActionResult<PagedResult<ArtistResponseDto>>> Get([FromQuery] GetArtistQueryParams queryParams)
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
    
    [HttpPost]
    public async Task<ActionResult<ArtistResponseDto>> Create(ArtistCreateRequest entity)
    {
        var artist = await _artistService.CreateAsync(entity);
        return artist;
    }
    
    [HttpPut("{id}")]
    public async Task<ActionResult<ArtistResponseDto>> Update(int id, ArtistUpdateRequest entity)
    {
        var artist = await _artistService.UpdateAsync(id, entity);
        return artist;
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _artistService.DeleteAsync(id);
        return NoContent();
    }
}