using AutoMapper;
using System.Text;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace PhotoApp.Client.Connection
{
    public partial class ApiConnection : IDisposable
    {
        private readonly HttpClient _httpClient;
        private JsonSerializerOptions _options;
        private readonly IMapper _mapper;
        public ApiConnection(HttpClient httpClient, IConfiguration configuration, IMapper mapper)
        { 
            _httpClient = httpClient;
            var baseUrl = configuration["VITE_API_URL"] ?? "https://localhost:5001/api";
            _httpClient.BaseAddress = new Uri(baseUrl);

            _options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            _mapper = mapper;
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

        public async Task<TResponse?> SendPostRequest<TResponse, TRequest>(string url, TRequest data)
        {
            string jsonPayload = JsonSerializer.Serialize(data, _options);
            using var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            var jsonResponse = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TResponse>(jsonResponse, _options);
        }
    }
}
