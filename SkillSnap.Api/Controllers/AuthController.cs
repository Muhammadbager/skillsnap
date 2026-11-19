using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillSnap.Api.Contracts;
using SkillSnap.Api.Data;
using SkillSnap.Api.Models;
using SkillSnap.Api.Services;

namespace SkillSnap.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    SkillSnapContext context,
    ITokenService tokenService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        if (await userManager.FindByEmailAsync(email) is not null)
        {
            return Conflict(new { message = "An account with this email already exists." });
        }

        var user = new ApplicationUser { UserName = email, Email = email };
        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return ValidationProblem(new ValidationProblemDetails(
                result.Errors.GroupBy(error => error.Code)
                    .ToDictionary(group => group.Key, group => group.Select(error => error.Description).ToArray())));
        }

        await userManager.AddToRoleAsync(user, "User");
        var portfolio = new PortfolioUser
        {
            Name = request.Name.Trim(),
            Bio = "Tell the world what you build and what you are learning.",
            ApplicationUserId = user.Id
        };
        context.PortfolioUsers.Add(portfolio);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(Login), tokenService.CreateToken(user, portfolio, "User"));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email.Trim());
        if (user is null || !(await signInManager.CheckPasswordSignInAsync(user, request.Password, true)).Succeeded)
        {
            return Unauthorized(new { message = "Email or password is incorrect." });
        }

        var portfolio = await context.PortfolioUsers.AsNoTracking()
            .SingleAsync(profile => profile.ApplicationUserId == user.Id);
        var roles = await userManager.GetRolesAsync(user);
        return Ok(tokenService.CreateToken(user, portfolio, roles.FirstOrDefault() ?? "User"));
    }
}
