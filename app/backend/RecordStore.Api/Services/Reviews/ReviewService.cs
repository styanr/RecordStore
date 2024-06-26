﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RecordStore.Api.Context;
using RecordStore.Api.Dto.Reviews;
using RecordStore.Api.Entities;
using RecordStore.Api.Exceptions;
using RecordStore.Api.RequestHelpers.QueryParams;
using RecordStore.Api.Services.Logs;
using RecordStore.Api.Services.Users;

namespace RecordStore.Api.Services.Reviews;

public class ReviewService : IReviewService
{
    private readonly RecordStoreContext _context;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;
    private readonly ILogService _logService;

    public ReviewService(RecordStoreContext context, IMapper mapper, IUserService userService, ILogService logService)
    {
        _context = context;
        _mapper = mapper;
        _userService = userService;
        _logService = logService;
    }

    public async Task CreateAsync(int id, CreateReviewRequest createReviewRequest)
    {
        var user = await _userService.GetCurrentUserAsync();

        var userOrderedProduct = _context.ShopOrders.Any(order =>
            order.Status == OrderStatus.Delivered &&
            order.UserId == user.Id &&
            order.OrderLines.Any(orderLine => orderLine.ProductId == id));

        if (!userOrderedProduct) throw new UnauthorizedAccessException("User hasn't ordered this product");

        var userReviewedProduct = _context.Reviews.Any(review =>
            review.UserId == user.Id &&
            review.ProductId == id);

        if (userReviewedProduct) throw new InvalidOperationException("User has already reviewed this product");

        var review = _mapper.Map<Review>(createReviewRequest);

        review.ProductId = id;
        review.UserId = user.Id;

        _context.Reviews.Add(review);

        await _context.SaveChangesAsync();

        await _logService.LogActionAsync("Create Review", $"Review created for product with ID: {id}");
    }

    public async Task DeleteAsync(int id)
    {
        var review = _context.Reviews.Find(id);

        if (review is null) throw new EntityNotFoundException("Review not found");

        var user = await _userService.GetCurrentUserAsync();

        if (review.UserId != user.Id) throw new UnauthorizedException("You can't delete someone else's review");

        _context.Reviews.Remove(review);

        await _context.SaveChangesAsync();

        await _logService.LogActionAsync("Delete Review", $"Review deleted with ID: {id}");
    }

    public async Task<List<ReviewResponse>> GetAllAsync(int id, GetReviewQueryParams queryParams)
    {
        var reviews = await _context.Reviews
            .Where(review => review.ProductId == id)
            .Include(r => r.User)
            .OrderByDescending(review => review.CreatedAt)
            .ToListAsync();

        var reviewResponses = _mapper.Map<List<ReviewResponse>>(reviews);

        await _logService.LogActionAsync("Get Reviews", $"Reviews retrieved for product with ID: {id}");

        return reviewResponses;
    }
}