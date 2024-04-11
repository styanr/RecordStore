using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RecordStore.Api.Context;
using RecordStore.Api.Dto.Artists;
using RecordStore.Api.Entities;
using RecordStore.Api.Exceptions;
using RecordStore.Api.Extensions;
using RecordStore.Api.RequestHelpers.QueryParams;

namespace RecordStore.Api.Services.Artists;

public class ArtistService : IArtistService
{
    private readonly RecordStoreContext _context;
    private readonly IMapper _mapper;

    public ArtistService(RecordStoreContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public async Task<List<ArtistResponseDto>> GetAllAsync(GetArtistQueryParams queryParams)
    {
        var query = _context.Artists
            .AsQueryable()
            .ApplyFiltersAndOrderBy(queryParams);
        
        PagedResult<Artist> pagedResult = await query.GetPagedAsync(queryParams.Page, queryParams.PageSize);

        var artists = _mapper.Map<List<ArtistResponseDto>>(pagedResult.Results);
        
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
        
        return artistDto;
    }

    public Task<ArtistResponseDto> CreateAsync(Artist entity)
    {
        throw new NotImplementedException();
    }

    public Task<ArtistResponseDto> UpdateAsync(Artist entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}