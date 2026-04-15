using LMS.Application.Common.Interfaces;
using LMS.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;


namespace LMS.Infrastructure.Services.Auth
{
    public sealed class JwtService(IConfiguration config) : IJwtService
    {
        public string GenerateAccessToken(User user)
        {
            var secret = config["Jwt:Secret"]!;
            var issuer = config["Jwt:Issuer"]!;
            var audience = config["Jwt:Audience"]!;
            var expiry = int.Parse(config["Jwt:ExpiryMinutes"] ?? "60");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
{
    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // مهم جدًا
    new Claim(ClaimTypes.Role, user.Role.ToString()),
    new Claim(JwtRegisteredClaimNames.Email, user.Email),
    new Claim(JwtRegisteredClaimNames.Name, user.FullName),
    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
};

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiry),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            // 64 cryptographically random bytes → base64 string
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }
    }
}
