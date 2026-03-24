using PhotoApp.Common.ModelsShared;
using PhotoApp.Front.Models;

namespace PhotoApp.Front.Connection;

public partial class ApiConnection
{
    public async Task<ApiResult<ServerAuthResponse>> Login(UserModel user)
    {
        var url = $"auth/login";
        var userDto = _mapper.Map<UserModelDto>(user);
        return await SendPostRequest<ServerAuthResponse, UserModelDto>(url, userDto);
    }

    public async Task<ApiResult<ServerAuthResponse>> LoginVerify(UserModel user, string code)
    {
        var url = $"auth/login/{code}";
        var userDto = _mapper.Map<UserModelDto>(user);
        return await SendPostRequest<ServerAuthResponse, UserModelDto>(url, userDto);
    }

    public async Task<ApiResult<ServerAuthResponse>> Register(UserModel user)
    {
        var url = $"auth/register";
        var userDto = _mapper.Map<UserModelDto>(user);
        return await SendPostRequest<ServerAuthResponse, UserModelDto>(url, userDto);
    }

    public async Task<ApiResult<ServerAuthResponse>> RegisterVerify(UserModel user, string code)
    {
        var url = $"auth/register/{code}";
        var userDto = _mapper.Map<UserModelDto>(user);
        return await SendPostRequest<ServerAuthResponse, UserModelDto>(url, userDto);
    }

    public async Task<ApiResult<ServerAuthResponse>> CheckActivity(UserModel user, string code)
    {
        var url = $"auth/register/activity";
        var userDto = _mapper.Map<UserModelDto>(user);
        return await SendPostRequest<ServerAuthResponse, UserModelDto>(url, userDto);
    }
}
