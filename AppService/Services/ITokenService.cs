using Microsoft.AspNetCore.Identity;

namespace AppService.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(IdentityUser user , IList<string> roles);
    }
}
