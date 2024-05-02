using RecordStore.Api.Dto.Artists;
using RecordStore.Api.Entities;
using RecordStore.Api.Extensions;
using RecordStore.Api.RequestHelpers.QueryParams;

namespace RecordStore.Api.Services.Artists;

public interface IArtistService
{
    Task<PagedResult<ArtistResponseDto>> GetAllAsync(GetArtistQueryParams queryParams);
    Task<ArtistFullResponseDto> GetByIdAsync(int id);
    Task<ArtistResponseDto> CreateAsync(ArtistCreateRequest entity);
    Task<ArtistResponseDto> UpdateAsync(int id, ArtistUpdateRequest entity);
    Task DeleteAsync(int id);
}