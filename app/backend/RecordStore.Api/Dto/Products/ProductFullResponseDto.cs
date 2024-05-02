namespace RecordStore.Api.Dto.Products;

public class ProductFullResponseDto : ProductResponseDto
{
    public string Location { get; set; }
    public int? Quantity { get; set; }
    public int? RestockLevel { get; set; }
}