using PhotoApp.Common.ModelsShared;

namespace PhotoApp.Client.Connection
{
    public partial class ApiConnection
    {
        public async Task<ServerAuthResponse> Login(UserModel user)
        {
            var url = $"/auth/login";
            var response = await SendPostRequest<ServerAuthResponse, UserModel>(url, user);
            return response!;
        }
        public async Task<ServerAuthResponse> LoginVerify(UserModel user, string code)
        {
            var url = $"/auth/login/{code}";
            var response = await SendPostRequest<ServerAuthResponse, UserModel>(url, user);
            return response!; 
        }
        public async Task<ServerAuthResponse> Register(UserModel user)
        {
            var url = $"/auth/register";
            var response = await SendPostRequestWithoutData<ServerAuthResponse>(url);
            return response!; //TODO handle null properly
        }
        public async Task<ServerAuthResponse> RegisterVerify(UserModel user, string code)
        {
            var url = $"/auth/register/{code}";
            var response = await SendPostRequestWithoutData<ServerAuthResponse>(url);
            return response!; //TODO handle null properly
        }
    }
}
