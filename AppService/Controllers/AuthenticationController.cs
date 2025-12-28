using AppService.Models;
using AppService.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AppService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(UserManager<IdentityUser> userManager,
                            ITokenService tokenService,
                            ILogger<AuthController> logger) : ControllerBase
{
    [HttpPost("authenticate")]
    public async Task<ActionResult<string>> Authenticate(AuthenticationDto request)
    {
        logger.LogInformation("Authentication attempt for UserName: {UserName}", request.UserName);

        var user = await userManager.FindByNameAsync(request.UserName);

        if (user == null || !await userManager.CheckPasswordAsync(user, request.Password))
        {
            logger.LogWarning("Authentication failed for UserName: {UserName}", request.UserName);
            return Unauthorized();
        }

        var roles = await userManager.GetRolesAsync(user);
        var token = tokenService.GenerateAccessToken(user, roles);


        logger.LogInformation("Authentication successful for UserName: {UserName}", request.UserName);

        return Ok(new { token });
    }
}

