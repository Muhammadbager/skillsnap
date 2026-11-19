using System.ComponentModel.DataAnnotations;

namespace SkillSnap.Api.Contracts;

public sealed record ProjectDto(int Id, string Title, string Description, string? ImageUrl, string? RepositoryUrl, int PortfolioUserId);

public sealed class ProjectRequest
{
    [Required, StringLength(120)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(1200)]
    public string Description { get; set; } = string.Empty;

    [Url, StringLength(500)]
    public string? ImageUrl { get; set; }

    [Url, StringLength(500)]
    public string? RepositoryUrl { get; set; }
}

public sealed record SkillDto(int Id, string Name, string Level, int PortfolioUserId);

public sealed class SkillRequest
{
    [Required, StringLength(60)]
    public string Name { get; set; } = string.Empty;

    [Required, RegularExpression("^(Beginner|Intermediate|Advanced|Expert)$")]
    public string Level { get; set; } = "Beginner";
}

public sealed record PortfolioDto(int Id, string Name, string Bio, string? ProfileImageUrl);

public sealed class UpdatePortfolioRequest
{
    [Required, StringLength(80)]
    public string Name { get; set; } = string.Empty;

    [StringLength(600)]
    public string Bio { get; set; } = string.Empty;

    [Url, StringLength(500)]
    public string? ProfileImageUrl { get; set; }
}
