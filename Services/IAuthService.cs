

using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace authService.Services
{
    public interface IAuthService
    {
        Task<JwtSecurityToken> CreateToken(Model.Api.LoginCredentials credentials);
    }
}
