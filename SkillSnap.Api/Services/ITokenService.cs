using SkillSnap.Api.Contracts;
using SkillSnap.Api.Models;

namespace SkillSnap.Api.Services;

public interface ITokenService
{
    AuthResponse CreateToken(ApplicationUser user, PortfolioUser portfolio, string role);
}
