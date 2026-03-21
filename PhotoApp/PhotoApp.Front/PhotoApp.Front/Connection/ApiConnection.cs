using AutoMapper;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace PhotoApp.Front.Connection
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

        public async Task<TResponse?> SendRequest<TResponse, TRequest>(HttpMethod method, string url, TRequest? data)
        {
            var request = new HttpRequestMessage(method, url);

            if (data != null && method != HttpMethod.Get)
            {
                var json = JsonSerializer.Serialize(data, _options);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            var response = await _httpClient.SendAsync(request);

            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                HandleErrors(response.StatusCode, responseContent);
            }

            return string.IsNullOrWhiteSpace(responseContent)
                   ? default
                   : JsonSerializer.Deserialize<TResponse>(responseContent, _options);
        }

        private void HandleErrors(HttpStatusCode statusCode, string responseContent)
        {
            switch (statusCode)
            {
                case HttpStatusCode.BadRequest: 
                    throw new Exception(responseContent);

                case HttpStatusCode.Unauthorized: 
                    throw new Exception("Twoja sesja wygasła. Zaloguj się ponownie.");

                case HttpStatusCode.Forbidden: 
                    throw new Exception("Nie masz uprawnień do tej akcji.");

                case HttpStatusCode.NotFound: 
                    throw new Exception("Nie znaleziono zasobu.");

                case HttpStatusCode.InternalServerError:
                    throw new Exception("Serwer napotkał problem. Spróbuj później.");

                default:
                    throw new Exception($"Wystąpił nieoczekiwany błąd (Status: {statusCode})");
            }
        }
    }
}
