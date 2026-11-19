using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkillSnap.Api.Models;

public sealed class Project
{
    [Key]
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(1200)]
    public string Description { get; set; } = string.Empty;

    [Url, StringLength(500)]
    public string? ImageUrl { get; set; }

    [Url, StringLength(500)]
    public string? RepositoryUrl { get; set; }

    [ForeignKey(nameof(PortfolioUser))]
    public int PortfolioUserId { get; set; }

    public PortfolioUser? PortfolioUser { get; set; }
}
