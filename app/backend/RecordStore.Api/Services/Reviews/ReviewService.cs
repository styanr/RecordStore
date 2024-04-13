using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RecordStore.Api.Context;
using RecordStore.Api.Dto.Reviews;
using RecordStore.Api.Entities;
using RecordStore.Api.Exceptions;
using RecordStore.Api.RequestHelpers.QueryParams;
using RecordStore.Api.Services.Users;

namespace RecordStore.Api.Services.Reviews;

public class ReviewService : IReviewService
{
    private readonly RecordStoreContext _context;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public ReviewService(RecordStoreContext context, IMapper mapper, IUserService userService)
    {
        _context = context;
        _mapper = mapper;
        _userService = userService;
    }

    public async Task CreateAsync(CreateReviewRequest createReviewRequest)
    {
        var user = await _userService.GetCurrentUserAsync();

        var userOrderedProduct = _context.ShopOrders.Any(order =>
            order.UserId == user.Id &&
            order.OrderLines.Any(orderLine => orderLine.ProductId == createReviewRequest.ProductId));

        if (!userOrderedProduct) throw new UnauthorizedException("User can't review a product they haven't ordered");
        
        var userReviewedProduct = _context.Reviews.Any(review =>
            review.UserId == user.Id &&
            review.ProductId == createReviewRequest.ProductId);
        
        if (userReviewedProduct) throw new InvalidOperationException("User has already reviewed this product"); 
        
        var review = _mapper.Map<Review>(createReviewRequest);

        review.UserId = user.Id;

        _context.Reviews.Add(review);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var review = _context.Reviews.Find(id);

        if (review is null) throw new EntityNotFoundException("Review not found");

        var user = await _userService.GetCurrentUserAsync();

        if (review.UserId != user.Id) throw new UnauthorizedException("You can't delete someone else's review");

        _context.Reviews.Remove(review);

        await _context.SaveChangesAsync();
    }

    public async Task<List<ReviewResponse>> GetAllAsync(GetReviewQueryParams queryParams)
    {
        var reviews = await _context.Reviews
            .Where(review => review.ProductId == queryParams.ProductId)
            .Include(r => r.User)
            .ToListAsync();

        return _mapper.Map<List<ReviewResponse>>(reviews);
    }
}