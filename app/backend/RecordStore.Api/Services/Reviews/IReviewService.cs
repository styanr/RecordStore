using RecordStore.Api.Dto.Reviews;
using RecordStore.Api.RequestHelpers.QueryParams;

namespace RecordStore.Api.Services.Reviews;

public interface IReviewService
{
    public Task CreateAsync(CreateReviewRequest createReviewRequest);
    public Task DeleteAsync(int id);
    public Task<List<ReviewResponse>> GetAllAsync(int id, GetReviewQueryParams queryParams);
}