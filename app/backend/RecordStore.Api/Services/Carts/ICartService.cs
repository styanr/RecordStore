using RecordStore.Api.Dto.Cart;

namespace RecordStore.Api.Services.Carts;

public interface ICartService
{
    Task<CartResponse> GetCartAsync();
    Task AddToCartAsync(CartItemRequest request);
    Task EditCartAsync(CartItemRequest request);
    Task RemoveFromCartAsync(int productId);
}