using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillSnap.Api.Data;

namespace SkillSnap.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
public sealed class AdminController(SkillSnapContext context) : ControllerBase
{
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(CancellationToken cancellationToken) => Ok(new
    {
        users = await context.Users.CountAsync(cancellationToken),
        portfolios = await context.PortfolioUsers.CountAsync(cancellationToken),
        projects = await context.Projects.CountAsync(cancellationToken),
        skills = await context.Skills.CountAsync(cancellationToken)
    });
}
