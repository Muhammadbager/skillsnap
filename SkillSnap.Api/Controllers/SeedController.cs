using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillSnap.Api.Data;
using SkillSnap.Api.Models;

namespace SkillSnap.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class SeedController(
    SkillSnapContext context,
    UserManager<ApplicationUser> userManager,
    IWebHostEnvironment environment) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Seed(CancellationToken cancellationToken)
    {
        if (!environment.IsDevelopment()) return NotFound();
        if (await context.PortfolioUsers.AnyAsync(cancellationToken))
        {
            return Conflict(new { message = "Sample data already exists." });
        }

        const string email = "jordan@skillsnap.local";
        var user = new ApplicationUser { UserName = email, Email = email };
        var result = await userManager.CreateAsync(user, "DemoPass1!");
        if (!result.Succeeded) return BadRequest(result.Errors);
        await userManager.AddToRoleAsync(user, "User");

        var portfolio = new PortfolioUser
        {
            Name = "Jordan Developer",
            Bio = "Full-stack developer passionate about accessible products and learning new technology.",
            ProfileImageUrl = "https://api.dicebear.com/9.x/initials/svg?seed=Jordan%20Developer",
            ApplicationUserId = user.Id,
            Projects =
            [
                new Project { Title = "Task Tracker", Description = "A focused workflow for planning and completing weekly goals.", RepositoryUrl = "https://github.com/" },
                new Project { Title = "Weather Lens", Description = "A responsive forecast dashboard powered by a public weather API.", RepositoryUrl = "https://github.com/" }
            ],
            Skills =
            [
                new Skill { Name = "C#", Level = "Advanced" },
                new Skill { Name = "Blazor", Level = "Intermediate" },
                new Skill { Name = "EF Core", Level = "Intermediate" }
            ]
        };
        context.PortfolioUsers.Add(portfolio);
        await context.SaveChangesAsync(cancellationToken);
        return Ok(new { message = "Sample data inserted.", login = email, password = "DemoPass1!" });
    }
}
