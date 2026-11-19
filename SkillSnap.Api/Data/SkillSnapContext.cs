using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SkillSnap.Api.Models;

namespace SkillSnap.Api.Data;

public sealed class SkillSnapContext(DbContextOptions<SkillSnapContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<PortfolioUser> PortfolioUsers => Set<PortfolioUser>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Skill> Skills => Set<Skill>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<PortfolioUser>()
            .HasIndex(user => user.ApplicationUserId)
            .IsUnique();

        builder.Entity<PortfolioUser>()
            .HasMany(user => user.Projects)
            .WithOne(project => project.PortfolioUser)
            .HasForeignKey(project => project.PortfolioUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<PortfolioUser>()
            .HasMany(user => user.Skills)
            .WithOne(skill => skill.PortfolioUser)
            .HasForeignKey(skill => skill.PortfolioUserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
