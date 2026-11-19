using System.Net.Http.Json;
using SkillSnap.Client.Models;

namespace SkillSnap.Client.Services;

public sealed class AuthService(
    HttpClient http,
    ApiAuthenticationStateProvider authenticationStateProvider)
{
    public async Task RegisterAsync(RegisterModel model)
    {
        var response = await http.PostAsJsonAsync("api/auth/register", model);
        await ApiServiceBase.EnsureSuccessAsync(response);
        await authenticationStateProvider.SignInAsync((await response.Content.ReadFromJsonAsync<AuthResponse>())!);
    }

    public async Task LoginAsync(LoginModel model)
    {
        var response = await http.PostAsJsonAsync("api/auth/login", model);
        await ApiServiceBase.EnsureSuccessAsync(response);
        await authenticationStateProvider.SignInAsync((await response.Content.ReadFromJsonAsync<AuthResponse>())!);
    }

    public Task LogoutAsync() => authenticationStateProvider.SignOutAsync();
}
