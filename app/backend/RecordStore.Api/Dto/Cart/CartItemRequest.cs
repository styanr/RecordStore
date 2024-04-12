using System.ComponentModel.DataAnnotations;

namespace RecordStore.Api.Dto.Cart;

public class CartItemRequest
{
    public int ProductId { get; set; }
    
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public int Quantity { get; set; }
}