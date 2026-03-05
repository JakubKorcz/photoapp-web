using PhotoApp.Common.ModelsShared;

namespace PhotoApp.Client.Connection
{
    public partial class ApiConnection
    {
        public async Task<ServerAuthResponse> Login(string username)
        {
            var url = $"/users/login/{username}";
            var response = await SendPostRequestWithoutData<ServerAuthResponse>(url);
            return response!; //TODO handle null properly
        }
        public async Task<ServerAuthResponse> LoginVerify(string username, string code)
        {
            var url = $"/users/login/{username}/{code}";
            var response = await SendPostRequestWithoutData<ServerAuthResponse>(url);
            return response!; //TODO handle null properly
        }
        public async Task<ServerAuthResponse> Register(string username)
        {
            var url = $"/users/register/{username}";
            var response = await SendPostRequestWithoutData<ServerAuthResponse>(url);
            return response!; //TODO handle null properly
        }
        public async Task<ServerAuthResponse> RegisterVerify(string username, string code)
        {
            var url = $"/users/register/{username}/{code}";
            var response = await SendPostRequestWithoutData<ServerAuthResponse>(url);
            return response!; //TODO handle null properly
        }
    }
}
