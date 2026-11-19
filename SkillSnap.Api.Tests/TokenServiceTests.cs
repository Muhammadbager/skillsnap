using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using SkillSnap.Api.Models;
using SkillSnap.Api.Services;

namespace SkillSnap.Api.Tests;

public sealed class TokenServiceTests
{
    [Fact]
    public void CreateToken_includes_identity_role_and_portfolio_claims()
    {
        var settings = new Dictionary<string, string?>
        {
            ["Jwt:Issuer"] = "tests",
            ["Jwt:Audience"] = "tests-client",
            ["Jwt:Key"] = "a-test-signing-key-that-is-at-least-32-characters"
        };
        var service = new TokenService(new ConfigurationBuilder().AddInMemoryCollection(settings).Build());
        var user = new ApplicationUser { Id = "user-42", Email = "dev@example.com" };
        var portfolio = new PortfolioUser { Id = 7, Name = "Ada" };

        var response = service.CreateToken(user, portfolio, "Admin");
        var token = new JwtSecurityTokenHandler().ReadJwtToken(response.Token);

        Assert.Equal("user-42", token.Subject);
        Assert.Contains(token.Claims, claim => claim.Type == ClaimTypes.Role && claim.Value == "Admin");
        Assert.Contains(token.Claims, claim => claim.Type == "portfolio_id" && claim.Value == "7");
        Assert.True(response.ExpiresAtUtc > DateTime.UtcNow);
    }
}
