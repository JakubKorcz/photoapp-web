using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Net.Http.Headers;

namespace PhotoApp.Front.Connection;

public class AuthenticationHeaderHandler : DelegatingHandler
{
    private readonly ProtectedSessionStorage _protectedStorage;

    public AuthenticationHeaderHandler(ProtectedSessionStorage protectedStorage)
    {
        _protectedStorage = protectedStorage;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _protectedStorage.GetAsync<string>("authToken");

            if (result.Success && !string.IsNullOrEmpty(result.Value))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.Value);
            }
        }
        catch (Exception)
        {
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
