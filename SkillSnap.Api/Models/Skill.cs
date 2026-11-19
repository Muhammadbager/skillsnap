using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SkillSnap.Api.Models;

public sealed class Skill
{
    [Key]
    public int Id { get; set; }

    [Required, StringLength(60)]
    public string Name { get; set; } = string.Empty;

    [Required, RegularExpression("^(Beginner|Intermediate|Advanced|Expert)$")]
    public string Level { get; set; } = "Beginner";

    [ForeignKey(nameof(PortfolioUser))]
    public int PortfolioUserId { get; set; }

    public PortfolioUser? PortfolioUser { get; set; }
}
