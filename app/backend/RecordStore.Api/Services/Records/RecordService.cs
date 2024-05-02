using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RecordStore.Api.Context;
using RecordStore.Api.Dto.Records;
using RecordStore.Api.Entities;
using RecordStore.Api.Exceptions;
using RecordStore.Api.Extensions;
using RecordStore.Api.RequestHelpers.QueryParams;

namespace RecordStore.Api.Services.Records;

public class RecordService : IRecordService
{
    private readonly RecordStoreContext _context;
    private readonly IMapper _mapper;

    public RecordService(RecordStoreContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    
    public async Task<PagedResult<RecordResponseDto>> GetAllAsync(GetRecordQueryParams queryParams)
    {
        PagedResult<Record> pagedResult = await _context.Records
            .ApplyFiltersAndOrderBy(queryParams)
            .GetPagedAsync(queryParams.Page, queryParams.PageSize);

        IList<Record> results = pagedResult.Results;
        
        return _mapper.Map<PagedResult<RecordResponseDto>>(pagedResult);
    }

    public async Task<RecordFullResponseDto> GetByIdAsync(int id)
    {
        var record = await _context.Records
            .Include(r => r.Genres)
            .Include(r => r.Artists)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (record is null)
        {
            throw new RecordNotFoundException();
        }
        
        return _mapper.Map<RecordFullResponseDto>(record);
    }

    public async Task<IEnumerable<RecordResponseDto>> GetByArtistIdAsync(int artistId, GetRecordQueryParams queryParams)
    {
        var pagedResult = await _context.Records
            .Where(r => r.Artists.Any(a => a.Id == artistId))
            .Include(r => r.Artists)
            .ApplyFiltersAndOrderBy(queryParams)
            .GetPagedAsync(queryParams.Page, queryParams.PageSize);
        
        return _mapper.Map<List<RecordResponseDto>>(pagedResult.Results);
    }

    public async Task<RecordFullResponseDto> CreateAsync(RecordCreateRequest request)
    {
        var record = _mapper.Map<Record>(request);
        
        await GetOrCreateGenres(record, request.GenreNames);
        await GetArtists(record, request.ArtistIds);
        
        await _context.Records.AddAsync(record);
        await _context.SaveChangesAsync();
        
        return _mapper.Map<RecordFullResponseDto>(record);
    }

    private async Task GetArtists(Record record, List<int> artistIds)
    {
        artistIds.ForEach(id =>
        {
            var artist = _context.Artists.FirstOrDefault(a => a.Id == id);
            
            if (artist is null)
            {
                throw new ArtistNotFoundException();
            }
            
            record.Artists.Add(artist);
        });
    }
    private async Task GetOrCreateGenres(Record record, List<string> genreNames)
    {
        genreNames.ForEach(async (name) =>
        {
            var genre = _context.Genres.FirstOrDefault(g => g.Name == name);
            if (genre is null)
            {
                genre = new Genre { Name = name };
                await _context.Genres.AddAsync(genre);
            }
            
            record.Genres.Add(genre);
        });
    }

    public async Task<RecordFullResponseDto> UpdateAsync(int id, RecordUpdateRequest request)
    {
        var record = _context.Records
            .Include(r => r.Genres)
            .Include(r => r.Artists)
            .FirstOrDefault(r => r.Id == id);
        
        if (record is null)
        {
            throw new RecordNotFoundException();
        }
        
        _context.Records.Entry(record).CurrentValues.SetValues(request);
        
        record.Genres.Clear();
        await GetOrCreateGenres(record, request.GenreNames);
        
        record.Artists.Clear();
        await GetArtists(record, request.ArtistIds);
        
        await _context.SaveChangesAsync();
        
        return _mapper.Map<RecordFullResponseDto>(record);
    }

    public Task DeleteAsync(int id)
    {
        var record = _context.Records.FirstOrDefault(r => r.Id == id);
        
        if (record is null)
        {
            throw new RecordNotFoundException();
        }
        
        _context.Records.Remove(record);
        
        return _context.SaveChangesAsync();
    }
}