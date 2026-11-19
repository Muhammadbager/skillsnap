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
public sealed class ProjectsController(
    SkillSnapContext context,
    IMemoryCache cache,
    ILogger<ProjectsController> logger) : ControllerBase
{
    internal const string CacheKey = "projects:all";

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProjectDto>>> GetAll(CancellationToken cancellationToken)
    {
        if (cache.TryGetValue(CacheKey, out IReadOnlyList<ProjectDto>? cached) && cached is not null)
        {
            logger.LogInformation("Project cache hit");
            return Ok(cached);
        }

        logger.LogInformation("Project cache miss");
        var projects = await context.Projects.AsNoTracking()
            .OrderByDescending(project => project.Id)
            .Select(project => new ProjectDto(project.Id, project.Title, project.Description,
                project.ImageUrl, project.RepositoryUrl, project.PortfolioUserId))
            .ToListAsync(cancellationToken);

        cache.Set(CacheKey, projects, new MemoryCacheEntryOptions
        {
            SlidingExpiration = TimeSpan.FromMinutes(2),
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        });
        return Ok(projects);
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<ProjectDto>> Create(ProjectRequest request, CancellationToken cancellationToken)
    {
        var portfolioId = GetPortfolioId();
        if (portfolioId is null) return Unauthorized();

        var project = new Project
        {
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            ImageUrl = NullIfWhiteSpace(request.ImageUrl),
            RepositoryUrl = NullIfWhiteSpace(request.RepositoryUrl),
            PortfolioUserId = portfolioId.Value
        };
        context.Projects.Add(project);
        await context.SaveChangesAsync(cancellationToken);
        cache.Remove(CacheKey);

        return CreatedAtAction(nameof(GetAll), ToDto(project));
    }

    [Authorize]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProjectDto>> Update(int id, ProjectRequest request, CancellationToken cancellationToken)
    {
        var project = await context.Projects.FindAsync([id], cancellationToken);
        if (project is null) return NotFound();
        if (!CanModify(project.PortfolioUserId)) return Forbid();

        project.Title = request.Title.Trim();
        project.Description = request.Description.Trim();
        project.ImageUrl = NullIfWhiteSpace(request.ImageUrl);
        project.RepositoryUrl = NullIfWhiteSpace(request.RepositoryUrl);
        await context.SaveChangesAsync(cancellationToken);
        cache.Remove(CacheKey);
        return Ok(ToDto(project));
    }

    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var project = await context.Projects.FindAsync([id], cancellationToken);
        if (project is null) return NotFound();
        if (!CanModify(project.PortfolioUserId)) return Forbid();

        context.Projects.Remove(project);
        await context.SaveChangesAsync(cancellationToken);
        cache.Remove(CacheKey);
        return NoContent();
    }

    private int? GetPortfolioId() => int.TryParse(User.FindFirstValue("portfolio_id"), out var id) ? id : null;
    private bool CanModify(int ownerId) => User.IsInRole("Admin") || GetPortfolioId() == ownerId;
    private static string? NullIfWhiteSpace(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    private static ProjectDto ToDto(Project project) => new(project.Id, project.Title, project.Description,
        project.ImageUrl, project.RepositoryUrl, project.PortfolioUserId);
}
