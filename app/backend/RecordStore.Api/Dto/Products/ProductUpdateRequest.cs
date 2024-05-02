using System.ComponentModel.DataAnnotations;

namespace RecordStore.Api.Dto.Products;

public class ProductUpdateRequest
{
    [Required]
    public string FormatName { get; set; }
    
    [Required]
    public string ImageUrl { get; set; } = null!;
    
    [Required]
    public string Description { get; set; }

    [Required]
    public decimal Price { get; set; }
    
    [Required]
    public int Quantity { get; set; }
    
    [Required]
    public string Location { get; set; } = null!;
    
    [Required]
    public int RestockLevel { get; set; }
}