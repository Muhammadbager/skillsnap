using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SkillSnap.Api.Contracts;
using SkillSnap.Api.Models;

namespace SkillSnap.Api.Services;

public sealed class TokenService(IConfiguration configuration) : ITokenService
{
    public AuthResponse CreateToken(ApplicationUser user, PortfolioUser portfolio, string role)
    {
        var expires = DateTime.UtcNow.AddHours(2);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, portfolio.Name),
            new(ClaimTypes.Role, role),
            new("portfolio_id", portfolio.Id.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is missing.")));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: credentials);

        return new AuthResponse(
            new JwtSecurityTokenHandler().WriteToken(token),
            user.Id,
            portfolio.Name,
            role,
            expires);
    }
}
