using System.Net.Http.Json;
using SkillSnap.Client.Models;

namespace SkillSnap.Client.Services;

public sealed class ProfileService(HttpClient http, UserSessionService session) : ApiServiceBase(http, session)
{
    public async Task<List<PortfolioDto>> GetProfilesAsync() =>
        await Http.GetFromJsonAsync<List<PortfolioDto>>("api/profiles") ?? [];

    public async Task<PortfolioDto?> GetMineAsync()
    {
        using var response = await Http.SendAsync(Authorized(HttpMethod.Get, "api/profiles/me"));
        await EnsureSuccessAsync(response);
        var profile = await response.Content.ReadFromJsonAsync<PortfolioDto>();
        Session.PortfolioId = profile?.Id;
        return profile;
    }

    public async Task UpdateMineAsync(ProfileFormModel model)
    {
        using var response = await Http.SendAsync(Authorized(HttpMethod.Put, "api/profiles/me", model));
        await EnsureSuccessAsync(response);
    }
}
