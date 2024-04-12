using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecordStore.Api.Dto.Cart;
using RecordStore.Api.Services.Carts;

namespace RecordStore.Api.Controllers;

[ApiController]
[Route("api/cart")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }
    
    [HttpGet]
    public async Task<ActionResult<CartResponse>> Get()
    {
        return await _cartService.GetCartAsync();
    }
    
    [HttpPost]
    public async Task<ActionResult> AddToCart([FromBody] CartItemRequest request)
    {
        await _cartService.AddToCartAsync(request);
        
        return Ok();
    }
    
    [HttpPut]
    public async Task<ActionResult> EditCart([FromBody] CartItemRequest request)
    {
        await _cartService.EditCartAsync(request);
        
        return Ok();
    }
    
    [HttpDelete("{productId}")]
    public async Task<ActionResult> RemoveFromCart(int productId)
    {
        await _cartService.RemoveFromCartAsync(productId);

        return NoContent();
    }
}