using System.ComponentModel.DataAnnotations;

namespace AppService.Models;

public class RequestUploadUrlDto
{
    [Required]
    public required string FileName { get; set; }

    public string Description { get; set; } = string.Empty;

    [Required]
    [AllowedValues("image/png", "image/jpeg", "image/jpg")]
    public required string ContentType { get; set; }

    [Required]
    [Range(1, 10 * 1024 * 1024, ErrorMessage = "File size must be between 1 byte and 10 MB")]
    public required long FileSizeLimit { get; set; }

}
