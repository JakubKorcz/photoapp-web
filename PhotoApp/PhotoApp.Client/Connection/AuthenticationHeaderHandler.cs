using System.Net.Http.Headers;

namespace PhotoApp.Client.Connection;

public class AuthenticationHeaderHandler : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return base.SendAsync(request, cancellationToken);
    }
}
