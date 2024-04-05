using Microsoft.EntityFrameworkCore;
using RecordStore.Api.Context;
using RecordStore.Api.Entities;
using RecordStore.Api.Extensions;

namespace RecordStore.Api.Services.Records;

public class RecordService : IRecordService
{
    private readonly RecordStoreContext _context;
    
    public RecordService(RecordStoreContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Record>> GetAllAsync()
    {
        PagedResult<Record> pagedResult = await _context.Records
            .OrderBy(r => r.UpdatedAt)
            .GetPagedAsync(1, 15);

        IList<Record> results = pagedResult.Results;
        
        return results;
    }

    public Task<Record> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
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