using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;

namespace authService.Controllers
{
    [Route("api/login")]
    public class LoginController : Controller
    {
        private Services.IAuthService AuthService { get; }
        private Services.IPasswordHasher PasswordHasher { get; }

        public LoginController(
            Services.IAuthService authService,
            Services.IPasswordHasher passwordHasher )
        {
            AuthService = authService;
            PasswordHasher = passwordHasher;
        }

        public IActionResult Login([FromBody] Model.Api.LoginCredentials credentials)
        {
            try
            {
                var hashedPwd = PasswordHasher.HashPassword(credentials.Password);
                var token = AuthService.CreateToken(credentials);

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
