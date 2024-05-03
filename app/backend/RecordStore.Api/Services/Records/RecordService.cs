using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
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
        foreach (int id in artistIds)
        {
            var artist = await _context.Artists.FirstOrDefaultAsync(a => a.Id == id);

            if (artist is null)
            {
                throw new ArtistNotFoundException();
            }

            record.Artists.Add(artist);
        }
    }

    private async Task GetOrCreateGenres(Record record, List<string> genreNames)
    {
        foreach (string name in genreNames)
        {
            var genre = await _context.Genres.FirstOrDefaultAsync(g => g.Name == name);

            if (genre is null)
            {
                genre = new Genre { Name = name };
                await _context.Genres.AddAsync(genre);
            }

            record.Genres.Add(genre);
        }
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

    public async Task DeleteAsync(int id)
    {
        var record = _context.Records.FirstOrDefault(r => r.Id == id);
        
        if (record is null)
        {
            throw new RecordNotFoundException();
        }

        try
        {
            _context.Records.Remove(record);
            await _context.SaveChangesAsync();
        } 
        catch (DbUpdateException e)
        {
            var postgresException = e.InnerException as PostgresException;
            if (postgresException.SqlState == "23502")
            {
                throw new InvalidOperationException("Record is in use and cannot be deleted");
            }

            throw;
        }
    }
}