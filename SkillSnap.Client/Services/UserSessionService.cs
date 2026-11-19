using SkillSnap.Client.Models;

namespace SkillSnap.Client.Services;

public sealed class UserSessionService
{
    public AuthResponse? Current { get; private set; }
    public bool IsAuthenticated => Current is { ExpiresAtUtc: var expires } && expires > DateTime.UtcNow;
    public string? Token => IsAuthenticated ? Current?.Token : null;
    public int? PortfolioId { get; set; }

    public void Set(AuthResponse? session)
    {
        Current = session;
        if (session is null) PortfolioId = null;
    }
}
