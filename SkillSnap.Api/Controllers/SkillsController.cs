using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SkillSnap.Api.Contracts;
using SkillSnap.Api.Data;
using SkillSnap.Api.Models;

namespace SkillSnap.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class SkillsController(SkillSnapContext context, IMemoryCache cache) : ControllerBase
{
    internal const string CacheKey = "skills:all";

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<SkillDto>>> GetAll(CancellationToken cancellationToken)
    {
        var skills = await cache.GetOrCreateAsync(CacheKey, async entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromMinutes(2);
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
            return await context.Skills.AsNoTracking()
                .OrderBy(skill => skill.Name)
                .Select(skill => new SkillDto(skill.Id, skill.Name, skill.Level, skill.PortfolioUserId))
                .ToListAsync(cancellationToken);
        });
        return Ok(skills ?? []);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<SkillDto>> Create(SkillRequest request, CancellationToken cancellationToken)
    {
        var portfolioId = GetPortfolioId();
        if (portfolioId is null) return Unauthorized();

        var skill = new Skill { Name = request.Name.Trim(), Level = request.Level, PortfolioUserId = portfolioId.Value };
        context.Skills.Add(skill);
        await context.SaveChangesAsync(cancellationToken);
        cache.Remove(CacheKey);
        return CreatedAtAction(nameof(GetAll), ToDto(skill));
    }

    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<SkillDto>> Update(int id, SkillRequest request, CancellationToken cancellationToken)
    {
        var skill = await context.Skills.FindAsync([id], cancellationToken);
        if (skill is null) return NotFound();
        if (!CanModify(skill.PortfolioUserId)) return Forbid();

        skill.Name = request.Name.Trim();
        skill.Level = request.Level;
        await context.SaveChangesAsync(cancellationToken);
        cache.Remove(CacheKey);
        return Ok(ToDto(skill));
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var skill = await context.Skills.FindAsync([id], cancellationToken);
        if (skill is null) return NotFound();
        if (!CanModify(skill.PortfolioUserId)) return Forbid();

        context.Skills.Remove(skill);
        await context.SaveChangesAsync(cancellationToken);
        cache.Remove(CacheKey);
        return NoContent();
    }

    private int? GetPortfolioId() => int.TryParse(User.FindFirstValue("portfolio_id"), out var id) ? id : null;
    private bool CanModify(int ownerId) => User.IsInRole("Admin") || GetPortfolioId() == ownerId;
    private static SkillDto ToDto(Skill skill) => new(skill.Id, skill.Name, skill.Level, skill.PortfolioUserId);
}
