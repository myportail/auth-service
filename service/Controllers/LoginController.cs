using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace authService.Controllers
{
    [Route("api/login")]
    public class LoginController : Controller
    {
        private Services.IAuthService AuthService { get; }
        private Services.IPasswordHasher PasswordHasher { get; }
        private Services.IMongoDbService MongoDbService { get; }

        public LoginController(
            Services.IAuthService authService,
            Services.IPasswordHasher passwordHasher,
            Services.IMongoDbService mongoDbService)
        {
            AuthService = authService;
            PasswordHasher = passwordHasher;
            MongoDbService = mongoDbService;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] Model.Api.LoginCredentials credentials)
        {
            try
            {
                var hashedPwd = PasswordHasher.HashPassword(credentials.Password);
                var token = await AuthService.CreateToken(credentials);
                
                // await MongoDbService.Init();

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }
            catch (Exception ex)
            {
                return Unauthorized();
            }
        }
    }
}
