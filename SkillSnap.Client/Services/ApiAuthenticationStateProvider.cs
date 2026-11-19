using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using SkillSnap.Client.Models;

namespace SkillSnap.Client.Services;

public sealed class ApiAuthenticationStateProvider(
    LocalStorageService storage,
    UserSessionService session) : AuthenticationStateProvider
{
    private const string StorageKey = "skillsnap.session";
    private static readonly ClaimsPrincipal Anonymous = new(new ClaimsIdentity());

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (session.IsAuthenticated) return BuildState(session.Current!);

        try
        {
            var saved = await storage.GetAsync<AuthResponse>(StorageKey);
            if (saved is not null && saved.ExpiresAtUtc > DateTime.UtcNow)
            {
                session.Set(saved);
                return BuildState(saved);
            }
            await storage.RemoveAsync(StorageKey);
        }
        catch (InvalidOperationException)
        {
            // JavaScript interop can be unavailable during an initial render.
        }

        session.Set(null);
        return new AuthenticationState(Anonymous);
    }

    public async Task SignInAsync(AuthResponse response)
    {
        session.Set(response);
        await storage.SetAsync(StorageKey, response);
        NotifyAuthenticationStateChanged(Task.FromResult(BuildState(response)));
    }

    public async Task SignOutAsync()
    {
        session.Set(null);
        await storage.RemoveAsync(StorageKey);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(Anonymous)));
    }

    private static AuthenticationState BuildState(AuthResponse response)
    {
        var identity = new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, response.UserId),
            new Claim(ClaimTypes.Name, response.DisplayName),
            new Claim(ClaimTypes.Role, response.Role)
        ], "jwt");
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }
}
