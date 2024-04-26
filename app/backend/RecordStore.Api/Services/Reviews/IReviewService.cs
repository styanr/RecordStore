using RecordStore.Api.Dto.Reviews;
using RecordStore.Api.RequestHelpers.QueryParams;

namespace RecordStore.Api.Services.Reviews;

public interface IReviewService
{
    public Task CreateAsync(int id, CreateReviewRequest createReviewRequest);
    public Task DeleteAsync(int id);
    public Task<List<ReviewResponse>> GetAllAsync(int id, GetReviewQueryParams queryParams);
}