using RecordStore.Api.Dto.Records;
using RecordStore.Api.Entities;
using RecordStore.Api.RequestHelpers.QueryParams;

namespace RecordStore.Api.Services.Records;

public interface IRecordService
{
    Task<IEnumerable<RecordResponseDto>> GetAllAsync(GetRecordQueryParams queryParams);
    Task<RecordFullResponseDto> GetByIdAsync(int id);
    Task<IEnumerable<RecordResponseDto>> GetByArtistIdAsync(int artistId, GetRecordQueryParams queryParams);
    Task<Record> CreateAsync(Record entity);
    Task<Record> UpdateAsync(Record entity);
    Task<bool> DeleteAsync(int id);
}