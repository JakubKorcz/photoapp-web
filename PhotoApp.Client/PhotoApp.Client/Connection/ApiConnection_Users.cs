using PhotoApp.Common.Models;
using System;
using System.Text.Json;

namespace PhotoApp.Client.Connection
{
    public partial class ApiConnection
    {
        public async Task<ServerAuthResponse> Login(string username) {
            var url = $"http://localhost:5231/users/login/{username}";
            return await SendPostRequestWithoutData(url);
        }
        public async Task<ServerAuthResponse> LoginVerify(string username, string code) {
            var url = $"http://localhost:5231/users/login/{username}/{code}";
            return await SendPostRequestWithoutData(url);
        }
        public async Task<ServerAuthResponse> Register(string username) {
            var url = $"http://localhost:5231/users/register/{username}";
            return await SendPostRequestWithoutData(url);
        }
        public async Task<ServerAuthResponse> RegisterVerify(string username, string code) {
            var url = $"http://localhost:5231/users/register/{username}/{code}";
            return await SendPostRequestWithoutData(url);
        } 

        private async Task<ServerAuthResponse> SendPostRequestWithoutData(string url)
        {
            using var httpClient = new HttpClient();
            var response = await httpClient.PostAsync(url, null);
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ServerAuthResponse>(json)!;
        }
    }
}
