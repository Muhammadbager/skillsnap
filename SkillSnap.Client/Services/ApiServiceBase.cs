using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SkillSnap.Client.Services;

public abstract class ApiServiceBase(HttpClient http, UserSessionService session)
{
    protected HttpClient Http { get; } = http;
    protected UserSessionService Session { get; } = session;

    protected HttpRequestMessage Authorized(HttpMethod method, string path, object? body = null)
    {
        var request = new HttpRequestMessage(method, path);
        if (!string.IsNullOrWhiteSpace(Session.Token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Session.Token);
        }
        if (body is not null) request.Content = JsonContent.Create(body);
        return request;
    }

    public static async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode) return;
        var detail = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException(string.IsNullOrWhiteSpace(detail)
            ? $"Request failed with status {(int)response.StatusCode}."
            : detail, null, response.StatusCode);
    }
}
