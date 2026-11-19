using System.Net.Http.Json;
using SkillSnap.Client.Models;

namespace SkillSnap.Client.Services;

public sealed class SkillService(HttpClient http, UserSessionService session) : ApiServiceBase(http, session)
{
    public async Task<List<SkillDto>> GetSkillsAsync() =>
        await Http.GetFromJsonAsync<List<SkillDto>>("api/skills") ?? [];

    public async Task AddSkillAsync(SkillFormModel model)
    {
        using var response = await Http.SendAsync(Authorized(HttpMethod.Post, "api/skills", model));
        await EnsureSuccessAsync(response);
    }

    public async Task DeleteSkillAsync(int id)
    {
        using var response = await Http.SendAsync(Authorized(HttpMethod.Delete, $"api/skills/{id}"));
        await EnsureSuccessAsync(response);
    }
}
