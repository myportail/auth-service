using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using authService.Exceptions;
using authService.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

namespace authService.Services
{
    public class AuthService : IAuthService
    {
        private Settings.Application AppSettings { get; }
        private Services.IPasswordHasher PasswordHasher { get; }
        
        private IMongoDbService MongoDbService { get; }

        public AuthService(
            IMongoDbService mongoDbService,
            Settings.Application appSettings,
            Services.IPasswordHasher passwordHasher )
        {
            AppSettings = appSettings;
            MongoDbService = mongoDbService;
            PasswordHasher = passwordHasher;
        }

        public async Task<JwtSecurityToken> CreateToken(Model.Api.LoginCredentials credentials)
        {
            try
            {
                List<Model.MongoDb.User> users = null;
                
                using (var usersQuery =
                    await MongoDbService.UsersCollection.FindAsync(x => x.Name.Equals(credentials.Username)))
                {
                    users = usersQuery.ToList();
                };

                if (!users.Any())
                    throw new Exception("unkown user");

                var user = users.First();

                var hashedPwd = PasswordHasher.HashPassword(credentials.Password);
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
