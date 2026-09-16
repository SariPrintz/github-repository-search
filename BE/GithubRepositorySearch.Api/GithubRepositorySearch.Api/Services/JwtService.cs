using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace GithubRepositorySearch.Api.Services
{
    public class JwtService
    {
        private readonly string _key;
        private readonly string? _issuer;
        private readonly string? _audience;

        public JwtService(IConfiguration configuration)
        {
            if (configuration is null) throw new ArgumentNullException(nameof(configuration));

            _key = configuration["Jwt:Key"] ?? string.Empty;
            _issuer = configuration["Jwt:Issuer"];
            _audience = configuration["Jwt:Audience"];

            if (string.IsNullOrWhiteSpace(_key))
            {
                throw new InvalidOperationException("JWT configuration value 'Jwt:Key' is missing or empty.");
            }
        }

        public string GenerateToken(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("username must be provided", nameof(username));

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username)
            };

            var keyBytes = Encoding.UTF8.GetBytes(_key);
            var securityKey = new SymmetricSecurityKey(keyBytes);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var now = DateTime.UtcNow;
            var expires = now.AddHours(1);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                notBefore: now,
                expires: expires,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
