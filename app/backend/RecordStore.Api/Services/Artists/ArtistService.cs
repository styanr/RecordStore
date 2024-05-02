using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RecordStore.Api.Context;
using RecordStore.Api.Dto.Artists;
using RecordStore.Api.Dto.Logs;
using RecordStore.Api.Entities;
using RecordStore.Api.Exceptions;
using RecordStore.Api.Extensions;
using RecordStore.Api.RequestHelpers.QueryParams;
using RecordStore.Api.Services.Logs;

namespace RecordStore.Api.Services.Artists;

public class ArtistService : IArtistService
{
    private readonly RecordStoreContext _context;
    private readonly IMapper _mapper;
    private readonly ILogService _logService;

    public ArtistService(RecordStoreContext context, IMapper mapper, ILogService logService)
    {
        _context = context;
        _mapper = mapper;
        _logService = logService;
    }

    public async Task<PagedResult<ArtistResponseDto>> GetAllAsync(GetArtistQueryParams queryParams)
    {
        var query = _context.Artists
            .AsQueryable()
            .ApplyFiltersAndOrderBy(queryParams);

        PagedResult<Artist> pagedResult = await query.GetPagedAsync(queryParams.Page, queryParams.PageSize);

        var artists = _mapper.Map<PagedResult<ArtistResponseDto>>(pagedResult);

        // Log the retrieval of artists
        await _logService.LogActionAsync("Get All Artists", "All artists retrieved.");

        return artists;
    }

    public async Task<ArtistFullResponseDto> GetByIdAsync(int id)
    {
        var artist = await _context.Artists.FindAsync(id);

        if (artist is null)
        {
            throw new ArtistNotFoundException();
        }

        var artistDto = _mapper.Map<ArtistFullResponseDto>(artist);

        // Log the retrieval of an artist by ID
        await _logService.LogActionAsync("Get Artist", $"Artist retrieved with ID: {id}");

        return artistDto;
    }

    public async Task<ArtistResponseDto> CreateAsync(ArtistCreateRequest entity)
    {
        var artist = _mapper.Map<Artist>(entity);

        _context.Artists.Add(artist);
        await _context.SaveChangesAsync();

        var artistDto = _mapper.Map<ArtistResponseDto>(artist);

        // Log the creation of an artist
        await _logService.LogActionAsync("Create Artist", $"Artist created with ID: {artist.Id}");

        return artistDto;
    }

    public async Task<ArtistResponseDto> UpdateAsync(int id, ArtistUpdateRequest entity)
    {
        var artist = await _context.Artists.FindAsync(id);

        if (artist is null)
        {
            throw new ArtistNotFoundException();
        }

        _context.Artists.Entry(artist).CurrentValues.SetValues(entity);
        await _context.SaveChangesAsync();

        var artistDto = _mapper.Map<ArtistResponseDto>(artist);

        // Log the update of an artist
        await _logService.LogActionAsync("Update Artist", $"Artist updated with ID: {id}");

        return artistDto;
    }

    public async Task DeleteAsync(int id)
    {
        var artist = await _context.Artists.FindAsync(id);

        if (artist is null)
        {
            throw new ArtistNotFoundException();
        }

        _context.Artists.Remove(artist);
        await _context.SaveChangesAsync();

        // Log the deletion of an artist
        await _logService.LogActionAsync("Delete Artist", $"Artist deleted with ID: {id}");
    }
}