using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillSnap.Api.Contracts;
using SkillSnap.Api.Data;

namespace SkillSnap.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ProfilesController(SkillSnapContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PortfolioDto>>> GetAll(CancellationToken cancellationToken) => Ok(
        await context.PortfolioUsers.AsNoTracking()
            .OrderBy(profile => profile.Name)
            .Select(profile => new PortfolioDto(profile.Id, profile.Name, profile.Bio, profile.ProfileImageUrl))
            .ToListAsync(cancellationToken));

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<PortfolioDto>> GetMine(CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var profile = await context.PortfolioUsers.AsNoTracking()
            .Where(item => item.ApplicationUserId == userId)
            .Select(item => new PortfolioDto(item.Id, item.Name, item.Bio, item.ProfileImageUrl))
            .SingleOrDefaultAsync(cancellationToken);
        return profile is null ? NotFound() : Ok(profile);
    }

    [Authorize]
    [HttpPut("me")]
    public async Task<ActionResult<PortfolioDto>> UpdateMine(UpdatePortfolioRequest request, CancellationToken cancellationToken)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var profile = await context.PortfolioUsers.SingleOrDefaultAsync(item => item.ApplicationUserId == userId, cancellationToken);
        if (profile is null) return NotFound();

        profile.Name = request.Name.Trim();
        profile.Bio = request.Bio.Trim();
        profile.ProfileImageUrl = string.IsNullOrWhiteSpace(request.ProfileImageUrl) ? null : request.ProfileImageUrl.Trim();
        await context.SaveChangesAsync(cancellationToken);
        return Ok(new PortfolioDto(profile.Id, profile.Name, profile.Bio, profile.ProfileImageUrl));
    }
}
