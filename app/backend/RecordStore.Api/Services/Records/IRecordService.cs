using RecordStore.Api.Entities;

namespace RecordStore.Api.Services.Records;

public interface IRecordService
{
    Task<IEnumerable<Record>> GetAllAsync();
    Task<Record> GetByIdAsync(int id);
    Task<Record> CreateAsync(Record entity);
    Task<Record> UpdateAsync(Record entity);
    Task<bool> DeleteAsync(int id);
}