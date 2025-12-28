namespace AppService.Models;

using System.ComponentModel.DataAnnotations;

public class ProductCreationDto
{
    [Required(ErrorMessage = "Product title is mandatory")]
    [StringLength(100, MinimumLength = 3)]
    public required string Title { get; set; }

    [Required]
    public required string Description { get; set; }

    [Required]
    [Range(0.01, 100000, ErrorMessage = "Price must be greater than zero")]
    public required decimal Price { get; set; }

    [Required]
    public required string CategoryId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public required int StockQuantity { get; set; }

    [Required(ErrorMessage = "You must provide the Uploaded Image ID")]
    public required string ImageId { get; set; }
}