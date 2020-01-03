using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;

namespace authService.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        Settings.Application AppSettings { get; }

        public PasswordHasher(
            Settings.Application appSettings )
        {
            AppSettings = appSettings;
        }
        
        public string HashPassword(string password)
        {
            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.ASCII.GetBytes(AppSettings.TokenGeneration.SecurityKey),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

            return hashed;
        }
    }
}
