using RecordStore.Api.Dto.Products;

namespace RecordStore.Api.Dto.Cart;

public class CartItemResponse
{
    public ProductShortResponseDto Product { get; set; }
    public int Quantity { get; set; }
}