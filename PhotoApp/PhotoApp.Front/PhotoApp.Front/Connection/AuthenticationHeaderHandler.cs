using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using PhotoApp.Common.ModelsShared;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace PhotoApp.Front.Connection;

public class AuthenticationHeaderHandler : DelegatingHandler
{
    private readonly ProtectedSessionStorage _protectedStorage;
    private readonly NavigationManager _navigationManager;

    private static readonly SemaphoreSlim _refreshLock = new(1, 1);
    private static string? _cachedRefreshedToken;
    private static DateTime _cachedAt;
    private static readonly TimeSpan CachedTokenTtl = TimeSpan.FromSeconds(30);

    public AuthenticationHeaderHandler(ProtectedSessionStorage protectedStorage, NavigationManager navigationManager)
    {
        _protectedStorage = protectedStorage;
        _navigationManager = navigationManager;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await ReadTokenAsync();
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode != HttpStatusCode.Unauthorized)
        {
            return response;
        }

        if (IsRefreshRequest(request))
        {
            await ClearStorageAsync();
            RedirectToLogin();
            return response;
        }

        var refreshedToken = await TryRefreshAsync();
        if (string.IsNullOrEmpty(refreshedToken))
        {
            await ClearStorageAsync();
            RedirectToLogin();
            return response;
        }

        try
        {
            await _protectedStorage.SetAsync("authToken", refreshedToken);
        }
        catch
        {
        }

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", refreshedToken);
        return await base.SendAsync(request, cancellationToken);
    }

    private async Task<string?> ReadTokenAsync()
    {
        try
        {
            var result = await _protectedStorage.GetAsync<string>("authToken");
            return result.Success ? result.Value : null;
        }
        catch
        {
            return null;
        }
    }

    private static bool IsRefreshRequest(HttpRequestMessage request)
    {
        return request.RequestUri is not null
               && request.RequestUri.AbsolutePath.EndsWith("/auth/refresh", StringComparison.OrdinalIgnoreCase);
    }

    private async Task<string?> TryRefreshAsync()
    {
        await _refreshLock.WaitAsync();
        try
        {
            if (_cachedRefreshedToken is not null
                && (DateTime.UtcNow - _cachedAt) < CachedTokenTtl)
            {
                return _cachedRefreshedToken;
            }

            var refreshRequest = new HttpRequestMessage(HttpMethod.Get, "auth/refresh");
            var refreshResponse = await base.SendAsync(refreshRequest, CancellationToken.None);

            if (!refreshResponse.IsSuccessStatusCode)
            {
                return null;
            }

            var content = await refreshResponse.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(content))
            {
                return null;
            }

            var auth = JsonSerializer.Deserialize<ServerAuthResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (auth is null || string.IsNullOrEmpty(auth.AccessToken))
            {
                return null;
            }

            _cachedRefreshedToken = auth.AccessToken;
            _cachedAt = DateTime.UtcNow;
            return auth.AccessToken;
        }
        finally
        {
            _refreshLock.Release();
        }
    }

    private async Task ClearStorageAsync()
    {
        try { await _protectedStorage.DeleteAsync("authToken"); } catch { }
        try { await _protectedStorage.DeleteAsync("username"); } catch { }
    }

    private void RedirectToLogin()
    {
        try
        {
            _navigationManager.NavigateTo("/login", forceLoad: true);
        }
        catch
        {
        }
    }
}