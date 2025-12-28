using System.ComponentModel.DataAnnotations;

namespace AppService.Models;

public class AuthenticationDto
{
    [Required]
    public required string UserName { get; set; }

    [Required]
    public required string Password { get; set; }
}
