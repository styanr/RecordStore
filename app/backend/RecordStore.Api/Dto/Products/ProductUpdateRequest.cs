namespace RecordStore.Api.Dto.Products;

public class ProductUpdateRequest
{
    public string Format { get; set; }

    public string? Description { get; set; }

    public decimal Price { get; set; }
}