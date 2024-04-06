using RecordStore.Api.Dto.Artists;
using RecordStore.Api.Entities;
using RecordStore.Api.RequestHelpers.QueryParams;

namespace RecordStore.Api.Services.Artists;

public interface IArtistService
{
    Task<List<ArtistResponseDto>> GetAllAsync(GetArtistQueryParams queryParams);
    Task<ArtistFullResponseDto> GetByIdAsync(int id);
    Task<ArtistResponseDto> CreateAsync(Artist entity);
    Task<ArtistResponseDto> UpdateAsync(Artist entity);
    Task<bool> DeleteAsync(int id);
}