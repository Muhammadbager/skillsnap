using System.ComponentModel.DataAnnotations;

namespace SkillSnap.Api.Models;

public sealed class PortfolioUser
{
    [Key]
    public int Id { get; set; }

    [Required, StringLength(80)]
    public string Name { get; set; } = string.Empty;

    [StringLength(600)]
    public string Bio { get; set; } = string.Empty;

    [Url, StringLength(500)]
    public string? ProfileImageUrl { get; set; }

    [Required]
    public string ApplicationUserId { get; set; } = string.Empty;

    public ApplicationUser? ApplicationUser { get; set; }
    public List<Project> Projects { get; set; } = [];
    public List<Skill> Skills { get; set; } = [];
}
