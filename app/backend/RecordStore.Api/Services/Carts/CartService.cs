﻿using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using RecordStore.Api.Context;
using RecordStore.Api.Dto.Cart;
using RecordStore.Api.Entities;
using RecordStore.Api.Exceptions;
using RecordStore.Api.Services.Products;
using RecordStore.Api.Services.Users;

namespace RecordStore.Api.Services.Carts;
public class CartService : ICartService
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly RecordStoreContext _context;
    private readonly IMapper _mapper;
    private readonly IProductService _productService;
    private readonly IUserService _userService;

    public CartService(
        IHttpContextAccessor contextAccessor, 
        RecordStoreContext context, 
        IMapper mapper, 
        IProductService productService,
        IUserService userService
        )
    {
        _contextAccessor = contextAccessor;
        _context = context;
        _mapper = mapper;
        _productService = productService;
        _userService = userService;
    }
    
    public async Task<CartResponse> GetCartAsync()
    {
        var userId = await GetUserId();

        var cart = await _context.ShoppingCarts
            .Include(sc => sc.ShoppingCartProducts)
                .ThenInclude(scp => scp.Product)
                    .ThenInclude(p => p.Record)
                        .ThenInclude(r => r.Artists)
            .Include(sc => sc.ShoppingCartProducts)
                .ThenInclude(scp => scp.Product)
                    .ThenInclude(p => p.Format)
            .Include(sc => sc.ShoppingCartProducts)
                .ThenInclude(scp => scp.Product)
                    .ThenInclude(p => p.Reviews)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart is null)
        {
            throw new CartNotFoundException();
        }

        return _mapper.Map<CartResponse>(cart);
    }
    
    public async Task AddToCartAsync(CartItemRequest request)
    {
        var userId = await GetUserId();

        var cart = await _context.ShoppingCarts
            .Include(sc => sc.ShoppingCartProducts)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart is null)
        {
            cart = new ShoppingCart
            {
                UserId = userId
            };

            await _context.ShoppingCarts.AddAsync(cart);
        }

        var product = await _productService.GetByIdAsync(request.ProductId);

        if (product is null)
        {
            throw new ProductNotFoundException();
        }

        var shoppingCartProduct = cart.ShoppingCartProducts.FirstOrDefault(scp => scp.ProductId == request.ProductId);

        if (shoppingCartProduct is null)
        {
            shoppingCartProduct = new ShoppingCartProduct
            {
                ProductId = request.ProductId,
                Quantity = request.Quantity
            };

            cart.ShoppingCartProducts.Add(shoppingCartProduct);
        }
        else
        {
            shoppingCartProduct.Quantity += request.Quantity;
        }

        await _context.SaveChangesAsync();
    }
    
    public async Task EditCartAsync(CartItemRequest request)
    {
        var userId = await GetUserId();

        var cart = await _context.ShoppingCarts
            .Include(sc => sc.ShoppingCartProducts)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart is null)
        {
            throw new CartNotFoundException();
        }

        var shoppingCartProduct = cart.ShoppingCartProducts.FirstOrDefault(scp => scp.ProductId == request.ProductId);

        if (shoppingCartProduct is null)
        {
            throw new ProductNotFoundException();
        }

        shoppingCartProduct.Quantity = request.Quantity;

        await _context.SaveChangesAsync();
    }
    
    public async Task RemoveFromCartAsync(int productId)
    {
        var userId = await GetUserId();

        var cart = await _context.ShoppingCarts
            .Include(sc => sc.ShoppingCartProducts)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart is null)
        {
            throw new CartNotFoundException();
        }

        var shoppingCartProduct = cart.ShoppingCartProducts.FirstOrDefault(scp => scp.ProductId == productId);

        if (shoppingCartProduct is null)
        {
            throw new ProductNotFoundException();
        }

        cart.ShoppingCartProducts.Remove(shoppingCartProduct);

        await _context.SaveChangesAsync();
    }
    
    private async Task<int> GetUserId()
    {
        var user = await _userService.GetCurrentUserAsync();

        if (user is null)
        {
            throw new UserNotFoundException();
        }
        
        if (user.Role is not "user")
        {
            throw new UnauthorizedAccessException();
        }
        
        return user.Id;
    }
}