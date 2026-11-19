using System.Net.Http.Json;
using SkillSnap.Client.Models;

namespace SkillSnap.Client.Services;

public sealed class ProjectService(HttpClient http, UserSessionService session) : ApiServiceBase(http, session)
{
    public async Task<List<ProjectDto>> GetProjectsAsync() =>
        await Http.GetFromJsonAsync<List<ProjectDto>>("api/projects") ?? [];

    public async Task AddProjectAsync(ProjectFormModel model)
    {
        using var response = await Http.SendAsync(Authorized(HttpMethod.Post, "api/projects", model));
        await EnsureSuccessAsync(response);
    }

    public async Task UpdateProjectAsync(int id, ProjectFormModel model)
    {
        using var response = await Http.SendAsync(Authorized(HttpMethod.Put, $"api/projects/{id}", model));
        await EnsureSuccessAsync(response);
    }

    public async Task DeleteProjectAsync(int id)
    {
        using var response = await Http.SendAsync(Authorized(HttpMethod.Delete, $"api/projects/{id}"));
        await EnsureSuccessAsync(response);
    }
}
