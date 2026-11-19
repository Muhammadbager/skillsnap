using System.ComponentModel.DataAnnotations;

namespace SkillSnap.Api.Contracts;

public sealed record RegisterRequest(
    [Required, StringLength(80)] string Name,
    [Required, EmailAddress] string Email,
    [Required, MinLength(8)] string Password);

public sealed record LoginRequest(
    [Required, EmailAddress] string Email,
    [Required] string Password);

public sealed record AuthResponse(string Token, string UserId, string DisplayName, string Role, DateTime ExpiresAtUtc);
