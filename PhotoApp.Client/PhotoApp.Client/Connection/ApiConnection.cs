using System.Text.Json;

namespace PhotoApp.Client.Connection
{
    public partial class ApiConnection : IDisposable
    {
        private readonly HttpClient _httpClient;
        private JsonSerializerOptions _options;
        public ApiConnection(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

        }
        public void Dispose() { }

        private async Task<T?> SendPostRequestWithoutData<T>(string url)
        {
            var response = await _httpClient.PostAsync(url, null);
            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<T>(json, _options);
        }

        public async Task<T?> SendGetRequestWithoutData<T>(string url)
        {
            var response = await _httpClient.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<T>(json, _options);
        }
    }
}
