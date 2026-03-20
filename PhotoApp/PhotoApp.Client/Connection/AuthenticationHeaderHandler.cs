using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;


namespace PhotoApp.Client.Connection
{
    public class AuthenticationHeaderHandler : DelegatingHandler
    {
        //private readonly ProtectedLocalStorage _protectedStorage;
        //public AuthenticationHeaderHandler(ProtectedLocalStorage protectedStorage)
        //{
        //    _protectedStorage = protectedStorage;
        //}

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                // Wyciągamy token z localStorage
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
}
