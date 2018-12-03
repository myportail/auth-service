

using System.IdentityModel.Tokens.Jwt;

namespace authService.Services
{
    public interface IAuthService
    {
        JwtSecurityToken CreateToken(Model.Api.LoginCredentials credentials);
    }
}
