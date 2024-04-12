using RecordStore.Api.Dto.Products;

namespace RecordStore.Api.Dto.Orders;

public class OrderLineResponse
{
    public ProductResponseDto Product { get; set; }
    
    public int Quantity { get; set; }

    public decimal Price { get; set; }
}