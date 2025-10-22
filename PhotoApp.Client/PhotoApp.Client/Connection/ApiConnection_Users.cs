using PhotoApp.Common.Models;

namespace PhotoApp.Client.Connection
{
    public partial class ApiConnection
    {
        public async Task<ServerAuthResponse> Login(string username)
        {
            var url = $"https://localhost:7003/users/login/{username}";
            return await SendPostRequestWithoutData(url);
        }
        public async Task<ServerAuthResponse> LoginVerify(string username, string code)
        {
            var url = $"https://localhost:7003/users/login/{username}/{code}";
            return await SendPostRequestWithoutData(url);
        }
        public async Task<ServerAuthResponse> Register(string username)
        {
            var url = $"https://localhost:7003/users/register/{username}";
            return await SendPostRequestWithoutData(url);
        }
        public async Task<ServerAuthResponse> RegisterVerify(string username, string code)
        {
            var url = $"https://localhost:7003/users/register/{username}/{code}";
            return await SendPostRequestWithoutData(url);
        }
    }
}
