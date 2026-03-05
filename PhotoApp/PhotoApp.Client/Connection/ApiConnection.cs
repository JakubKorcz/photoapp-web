using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace PhotoApp.Client.Connection
{
    public partial class ApiConnection : IDisposable
    {
        private readonly HttpClient _httpClient;
        private JsonSerializerOptions _options;
        public ApiConnection(HttpClient httpClient, IConfiguration configuration)
        { 
            _httpClient = httpClient;
            var baseUrl = configuration["VITE_API_URL"] ?? "https://localhost:5001/api";
            _httpClient.BaseAddress = new Uri(baseUrl);
            _options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

        }
        public void Dispose() { }

        private async Task<T?> SendPostRequestWithoutData<T>(string url)
        {
            var response = await _httpClient.PostAsync(_httpClient.BaseAddress + url, null);
            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<T>(json, _options);
        }

        public async Task<T?> SendGetRequestWithoutData<T>(string url)
        {
            var response = await _httpClient.GetAsync(_httpClient.BaseAddress + url);
            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<T>(json, _options);
        }
    }
}
