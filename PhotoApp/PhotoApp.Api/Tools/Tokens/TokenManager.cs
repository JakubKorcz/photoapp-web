using Microsoft.IdentityModel.Tokens;
using PhotoApp.Api.DbObjects;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PhotoApp.Api.Tools.Tokens
{
    public class TokenManager(IConfiguration configuration)
    {
        public string GenerateJWTAccessToken(User user)
        {
            var claims = new List<Claim>() 
            { 
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
            };

            var jwtKey = configuration.GetValue<string>("AppSettings:Token")
             ?? throw new InvalidOperationException("JWT Key is missing from configuration!");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>(jwtKey)!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("AppSettings:Issuer"),
                audience: configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
