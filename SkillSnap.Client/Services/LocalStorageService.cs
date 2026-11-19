using System.Text.Json;
using Microsoft.JSInterop;

namespace SkillSnap.Client.Services;

public sealed class LocalStorageService(IJSRuntime js)
{
    public async ValueTask SetAsync<T>(string key, T value) =>
        await js.InvokeVoidAsync("localStorage.setItem", key, JsonSerializer.Serialize(value));

    public async ValueTask<T?> GetAsync<T>(string key)
    {
        var json = await js.InvokeAsync<string?>("localStorage.getItem", key);
        return string.IsNullOrWhiteSpace(json) ? default : JsonSerializer.Deserialize<T>(json);
    }

    public async ValueTask RemoveAsync(string key) => await js.InvokeVoidAsync("localStorage.removeItem", key);
}
