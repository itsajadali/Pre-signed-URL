using System.ComponentModel.DataAnnotations;

namespace StorageService.Model;

public class FileMetadata
{
    [Required]
    public required string Id { get; set; }

    [Required]
    public required string FileName { get; set; }

    [Required]
    [AllowedValues("image/png", "image/jpeg", "image/jpg")]
    public required string ContentType { get; set; }

    [Required]
    [Range(1, 10 * 1024 * 1024, ErrorMessage = "File size must be between 1 byte and 10 MB")]
    public required long FileSizeLimit { get; set; }

    [Required]
    public required long ExpiresAt { get; set; }

    [Required]
    public required string Signature { get; set; }
}
