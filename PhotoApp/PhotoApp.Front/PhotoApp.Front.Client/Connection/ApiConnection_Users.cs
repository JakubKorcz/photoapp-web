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

    public async Task<ApiResult<ServerAuthResponse>> RegisterVerify(UserModelDto user, string code)
    {
        var url = $"auth/register/{code}";
        var userDto = _mapper.Map<UserModelDto>(user);
        return await SendPostRequest<ServerAuthResponse, UserModelDto>(url, userDto);
    }

    public async Task<ApiResult<ServerAuthResponse>> CheckActivity(UserModelDto user, string code)
    {
        var url = $"auth/register/activity";
        var userDto = _mapper.Map<UserModelDto>(user);
        return await SendPostRequest<ServerAuthResponse, UserModelDto>(url, userDto);
    }
}
