using System.ComponentModel.DataAnnotations;

namespace RecordStore.Api.Dto.Products;

public class ProductCreateRequest
{
    [Required]
    public int RecordId { get; set; }
    
    public string? ImageUrl { get; set; }

    [Required]
    public string FormatName { get; set; } = null!;
    
    [Required]
    public string LabelName { get; set; } = null!;
    
    public string? Description { get; set; }
    
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }
    
    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "Quantity must be greater or equal to 0")]
    public int Quantity { get; set; }

    [Required]
    public string Location { get; set; } = null!;

    [Required]
    public int? RestockLevel { get; set; }
}