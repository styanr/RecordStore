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
    
    public async Task<IEnumerable<RecordResponseDto>> GetAllAsync(GetRecordQueryParams queryParams)
    {
        PagedResult<Record> pagedResult = await _context.Records
            .ApplyFiltersAndOrderBy(queryParams)
            .GetPagedAsync(1, 15);

        IList<Record> results = pagedResult.Results;
        
        return _mapper.Map<List<RecordResponseDto>>(results);
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

    public Task<Record> CreateAsync(Record entity)
    {
        throw new NotImplementedException();
    }

    public Task<Record> UpdateAsync(Record entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}