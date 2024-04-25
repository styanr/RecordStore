using Microsoft.AspNetCore.Mvc;
using RecordStore.Api.Dto.Reviews;
using RecordStore.Api.RequestHelpers.QueryParams;
using RecordStore.Api.Services.Reviews;

namespace RecordStore.Api.Controllers;

[ApiController]
[Route("api/reviews")]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }
    
    [HttpGet]
    [Route("~/api/products/{productId}/reviews")]
    public async Task<IActionResult> GetReviews(int productId, [FromQuery] GetReviewQueryParams queryParams)
    {
        var reviews = await _reviewService.GetAllAsync(productId, queryParams);

        return Ok(reviews);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateReview([FromBody] CreateReviewRequest createReviewRequest)
    {
        await _reviewService.CreateAsync(createReviewRequest);

        return Ok();
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReview(int id)
    {
        await _reviewService.DeleteAsync(id);

        return Ok();
    }
}