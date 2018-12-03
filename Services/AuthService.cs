using System;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using authService.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace authService.Services
{
    public class AuthService : IAuthService
    {
        private Contexts.UsersDbContext UsersContext { get; }
        private Settings.Application AppSettings { get; }
        private Services.IPasswordHasher PasswordHasher { get; }

        public AuthService(
            Contexts.UsersDbContext usersContext,
            Settings.Application appSettings,
            Services.IPasswordHasher passwordHasher )
        {
            UsersContext = usersContext;
            AppSettings = appSettings;
            PasswordHasher = passwordHasher;
        }

        public JwtSecurityToken CreateToken(Model.Api.LoginCredentials credentials)
        {
            try
            {
                var users = UsersContext.Users.Where(x => x.Name.Equals(credentials.Username));

                if (!users.Any())
                    throw new Exception("unkown user");

                var hashedPwd = PasswordHasher.HashPassword(credentials.Password);
                var user = users.First();
                if (!user.Password.Equals(hashedPwd))
                    throw new Exception("invalid credentials");
                
                var claims = new[]
                {
                    new Claim("id", user.Id),
                    new Claim("username", user.Name)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppSettings.TokenGeneration.SecurityKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: AppSettings.TokenGeneration.Issuer,
                    audience: AppSettings.TokenGeneration.Audience,
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: creds);

                return token;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                throw;
            }
        }
    }
}
