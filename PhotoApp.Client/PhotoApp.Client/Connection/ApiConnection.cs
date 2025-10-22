using PhotoApp.Common.Models;
using System.Text.Json;

namespace PhotoApp.Client.Connection
{
    public partial class ApiConnection : IDisposable
    {
        private readonly HttpClient _httpClient;
        public ApiConnection(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public void Dispose() { }

        private async Task<ServerAuthResponse> SendPostRequestWithoutData(string url)
        {
            var response = await _httpClient.PostAsync(url, null);
            var json = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var obj = JsonSerializer.Deserialize<ServerAuthResponse>(json, options);
            if (obj != null)
            {
                return obj;
            }
            else
            {
                return new ServerAuthResponse()
                {
                    Message = "Odczytany obiekt jest pusty",
                    Success = false
                };
            }
        }
    }
}
