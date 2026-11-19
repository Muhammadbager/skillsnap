using System.ComponentModel.DataAnnotations;

namespace SkillSnap.Client.Models;

public sealed record AuthResponse(string Token, string UserId, string DisplayName, string Role, DateTime ExpiresAtUtc);
public sealed record ProjectDto(int Id, string Title, string Description, string? ImageUrl, string? RepositoryUrl, int PortfolioUserId);
public sealed record SkillDto(int Id, string Name, string Level, int PortfolioUserId);
public sealed record PortfolioDto(int Id, string Name, string Bio, string? ProfileImageUrl);

public sealed class LoginModel
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
}

public sealed class RegisterModel
{
    [Required, StringLength(80)]
    public string Name { get; set; } = string.Empty;
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required, MinLength(8)]
    public string Password { get; set; } = string.Empty;
}

public sealed class ProjectFormModel
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

public sealed class SkillFormModel
{
    [Required, StringLength(60)]
    public string Name { get; set; } = string.Empty;
    [Required]
    public string Level { get; set; } = "Intermediate";
}

public sealed class ProfileFormModel
{
    [Required, StringLength(80)]
    public string Name { get; set; } = string.Empty;
    [StringLength(600)]
    public string Bio { get; set; } = string.Empty;
    [Url, StringLength(500)]
    public string? ProfileImageUrl { get; set; }
}
