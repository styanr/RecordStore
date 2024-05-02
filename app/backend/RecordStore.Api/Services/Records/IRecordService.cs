using RecordStore.Api.Dto.Records;
using RecordStore.Api.Extensions;
using RecordStore.Api.RequestHelpers.QueryParams;

namespace RecordStore.Api.Services.Records;

public interface IRecordService
{
    Task<PagedResult<RecordResponseDto>> GetAllAsync(GetRecordQueryParams queryParams);
    Task<RecordFullResponseDto> GetByIdAsync(int id);
    Task<IEnumerable<RecordResponseDto>> GetByArtistIdAsync(int artistId, GetRecordQueryParams queryParams);
    Task<RecordFullResponseDto> CreateAsync(RecordCreateRequest request);
    Task<RecordFullResponseDto> UpdateAsync(int id, RecordUpdateRequest request);
    Task DeleteAsync(int id);
}