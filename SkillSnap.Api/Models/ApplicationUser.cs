using Microsoft.AspNetCore.Identity;

namespace SkillSnap.Api.Models;

public sealed class ApplicationUser : IdentityUser
{
    public PortfolioUser? Portfolio { get; set; }
}
