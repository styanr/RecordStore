using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using RecordStore.Api.Context;
using RecordStore.Api.Dto.Cart;
using RecordStore.Api.Entities;
using RecordStore.Api.Exceptions;
using RecordStore.Api.Services.Logs;
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
    private readonly ILogService _logService;

    public CartService(
        IHttpContextAccessor contextAccessor,
        RecordStoreContext context,
        IMapper mapper,
        IProductService productService,
        IUserService userService,
        ILogService logService
    )
    {
        _contextAccessor = contextAccessor;
        _context = context;
        _mapper = mapper;
        _productService = productService;
        _userService = userService;
        _logService = logService;
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

        await _logService.LogActionAsync("Get Cart", $"Cart retrieved for user with ID: {userId}");

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

        await _logService.LogActionAsync("Add to Cart", $"Product with ID: {request.ProductId} added to cart for user with ID: {userId}");
        
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

        await _logService.LogActionAsync("Edit Cart", $"Product with ID: {request.ProductId} quantity updated to {request.Quantity} in cart for user with ID: {userId}");
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

        await _logService.LogActionAsync("Remove from Cart", $"Product with ID: {productId} removed from cart for user with ID: {userId}");
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