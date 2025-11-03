using ASPDOTNETDEMO.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ASPDOTNETDEMO.Services
{
    public class JwtTokenService
    {
        private readonly IConfiguration _config;

        public JwtTokenService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(User user)
        {
            var jwtSettings = _config.GetSection("Jwt");

            var keyString = jwtSettings["Key"] 
                ?? throw new InvalidOperationException("JWT Key not found in configuration.");
            var issuer = jwtSettings["Issuer"] 
                ?? throw new InvalidOperationException("JWT Issuer not found in configuration.");
            var audience = jwtSettings["Audience"] 
                ?? throw new InvalidOperationException("JWT Audience not found in configuration.");
            var expireMinutesString = jwtSettings["ExpireMinutes"] ?? "60";

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role?.Name ?? "User")
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(expireMinutesString)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
