using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SkillSnap.Api.Data;
using SkillSnap.Api.Models;

namespace SkillSnap.Api.Tests;

public sealed class ModelTests
{
    [Fact]
    public void Skill_rejects_an_unknown_level()
    {
        var skill = new Skill { Name = "C#", Level = "Wizard" };
        var results = new List<ValidationResult>();

        var valid = Validator.TryValidateObject(skill, new ValidationContext(skill), results, true);

        Assert.False(valid);
        Assert.Contains(results, result => result.MemberNames.Contains(nameof(Skill.Level)));
    }

    [Fact]
    public void Context_configures_required_one_to_many_relationships()
    {
        var options = new DbContextOptionsBuilder<SkillSnapContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        using var context = new SkillSnapContext(options);

        var projectForeignKey = context.Model.FindEntityType(typeof(Project))!
            .GetForeignKeys().Single(key => key.PrincipalEntityType.ClrType == typeof(PortfolioUser));
        var skillForeignKey = context.Model.FindEntityType(typeof(Skill))!
            .GetForeignKeys().Single(key => key.PrincipalEntityType.ClrType == typeof(PortfolioUser));

        Assert.True(projectForeignKey.IsRequired);
        Assert.Equal(DeleteBehavior.Cascade, projectForeignKey.DeleteBehavior);
        Assert.True(skillForeignKey.IsRequired);
        Assert.Equal(DeleteBehavior.Cascade, skillForeignKey.DeleteBehavior);
    }
}
