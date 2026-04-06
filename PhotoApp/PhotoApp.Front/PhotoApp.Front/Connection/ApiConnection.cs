using AutoMapper;
using System.Net;
using System.Text;
using System.Text.Json;

namespace PhotoApp.Front.Connection;

public partial class ApiConnection : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _options;
    private readonly IMapper _mapper;

    public ApiConnection(HttpClient httpClient, IConfiguration configuration, IMapper mapper)
    { 
        _httpClient = httpClient;
        var baseUrl = configuration["VITE_API_URL"] ?? "https://localhost:5001";
        if (!baseUrl.EndsWith("/api"))
        {
            baseUrl = $"{baseUrl.TrimEnd('/')}/api";
        }
        _httpClient.BaseAddress = new Uri(baseUrl);

        _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        _mapper = mapper;
    }

    public void Dispose() { }

    public async Task<ApiResult<TResponse>> SendPostRequest<TResponse, TRequest>(string url, TRequest data)
    {
        return await SendRequest<TResponse, TRequest>(HttpMethod.Post, url, data);
    }

    public async Task<ApiResult<TResponse>> SendPostRequestWithoutData<TResponse>(string url)
    {
        return await SendRequest<TResponse, object>(HttpMethod.Post, url, null);
    }

    public async Task<ApiResult<TResponse>> SendGetRequestWithoutData<TResponse>(string url)
    {
        return await SendRequest<TResponse, object>(HttpMethod.Get, url, null);
    }

    public async Task<ApiResult<TResponse>> SendRequest<TResponse, TRequest>(HttpMethod method, string url, TRequest? data)
    {
        try
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
                var errorType = MapStatusCodeToErrorType(response.StatusCode);
                var errorMessage = ParseErrorMessage(responseContent, response.StatusCode);
                return ApiResult<TResponse>.Failure(errorMessage, errorType);
            }

            if (string.IsNullOrWhiteSpace(responseContent))
            {
                return ApiResult<TResponse>.Success(default!);
            }

            var result = JsonSerializer.Deserialize<TResponse>(responseContent, _options);
            return ApiResult<TResponse>.Success(result!);
        }
        catch (HttpRequestException ex)
        {
            return ApiResult<TResponse>.Failure($"Błąd połączenia: {ex.Message}", ApiErrorType.NetworkError);
        }
        catch (TaskCanceledException)
        {
            return ApiResult<TResponse>.Failure("Upłynął limit czasu połączenia.", ApiErrorType.NetworkError);
        }
        catch (Exception ex)
        {
            return ApiResult<TResponse>.Failure($"Nieoczekiwany błąd: {ex.Message}", ApiErrorType.Unknown);
        }
    }

    private static ApiErrorType MapStatusCodeToErrorType(HttpStatusCode statusCode)
    {
        return statusCode switch
        {
            HttpStatusCode.BadRequest => ApiErrorType.BadRequest,
            HttpStatusCode.Unauthorized => ApiErrorType.Unauthorized,
            HttpStatusCode.Forbidden => ApiErrorType.Forbidden,
            HttpStatusCode.NotFound => ApiErrorType.NotFound,
            HttpStatusCode.InternalServerError => ApiErrorType.ServerError,
            _ => ApiErrorType.Unknown
        };
    }

    private static string ParseErrorMessage(string responseContent, HttpStatusCode statusCode)
    {
        if (!string.IsNullOrWhiteSpace(responseContent))
        {
            try
            {
                var json = JsonDocument.Parse(responseContent);
                if (json.RootElement.TryGetProperty("message", out var message))
                {
                    return message.GetString() ?? GetDefaultMessage(statusCode);
                }
                if (json.RootElement.TryGetProperty("error", out var error))
                {
                    return error.GetString() ?? GetDefaultMessage(statusCode);
                }
                return responseContent;
            }
            catch
            {
                return responseContent;
            }
        }
        return GetDefaultMessage(statusCode);
    }

    private static string GetDefaultMessage(HttpStatusCode statusCode)
    {
        return statusCode switch
        {
            HttpStatusCode.BadRequest => "Nieprawidłowe dane.",
            HttpStatusCode.Unauthorized => "Twoja sesja wygasła. Zaloguj się ponownie.",
            HttpStatusCode.Forbidden => "Nie masz uprawnień do tej akcji.",
            HttpStatusCode.NotFound => "Nie znaleziono zasobu.",
            HttpStatusCode.InternalServerError => "Serwer napotkał problem. Spróbuj później.",
            _ => "Wystąpił nieoczekiwany błąd."
        };
    }
}
