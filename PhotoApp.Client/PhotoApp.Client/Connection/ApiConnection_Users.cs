using PhotoApp.Common.Models;
using System;
using System.Text.Json;

namespace PhotoApp.Client.Connection
{
    public partial class ApiConnection
    {
        public async Task<ServerAuthResponse> Login(string username) {
            var url = $"https://localhost:7003/users/login/{username}";
            return await SendPostRequestWithoutData(url);
        }
        public async Task<ServerAuthResponse> LoginVerify(string username, string code) {
            var url = $"https://localhost:7003/users/login/{username}/{code}";
            return await SendPostRequestWithoutData(url);
        }
        public async Task<ServerAuthResponse> Register(string username) {
            var url = $"https://localhost:7003/users/register/{username}";
            return await SendPostRequestWithoutData(url);
        }
        public async Task<ServerAuthResponse> RegisterVerify(string username, string code) {
            var url = $"https://localhost:7003/users/register/{username}/{code}";
            return await SendPostRequestWithoutData(url);
        } 

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
            else { 
                return new ServerAuthResponse() { 
                    Message = "Odczytany obiekt jest pusty",
                    Success = false
                };
            }
        }
    }
}
