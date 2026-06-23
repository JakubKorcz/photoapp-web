using PhotoApp.Common.ModelsShared;

namespace PhotoApp.Front.Client.Connection;

public partial class ApiConnection
{
    public async Task<ApiResult<ServerAuthResponse>> Login(UserModelDto user)
    {
        var url = $"auth/login";
        var userDto = _mapper.Map<UserModelDto>(user);
        return await SendPostRequest<ServerAuthResponse, UserModelDto>(url, userDto);
    }

    public async Task<ApiResult<ServerAuthResponse>> LoginVerify(UserModelDto user, string code)
    {
        var url = $"auth/login/{code}";
        var userDto = _mapper.Map<UserModelDto>(user);
        return await SendPostRequest<ServerAuthResponse, UserModelDto>(url, userDto);
    }

    public async Task<ApiResult<ServerAuthResponse>> Register(UserModelDto user)
    {
        var url = $"auth/register";
        var userDto = _mapper.Map<UserModelDto>(user);
        return await SendPostRequest<ServerAuthResponse, UserModelDto>(url, userDto);
    }

    public async Task<ApiResult<object>> Logout()
    {
        var url = $"auth/logout";
        return await SendRequest<object, object>(HttpMethod.Delete, url, null);
    }
}
