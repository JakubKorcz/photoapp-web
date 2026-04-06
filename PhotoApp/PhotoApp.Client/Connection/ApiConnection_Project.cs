using PhotoApp.Client.Models;
using PhotoApp.Common.ModelsShared;

namespace PhotoApp.Client.Connection;

public partial class ApiConnection
{
    public async Task<ApiResult<ProjectBaseInformationDto>> CreateBaseProject(string username, ProjectFormModel model)
    {
        var url = $"project/users/{username}/projects";
        var project = _mapper.Map<ProjectBaseInformationDto>(model);
        return await SendPostRequest<ProjectBaseInformationDto, ProjectBaseInformationDto>(url, project);
    }

    public async Task<ApiResult<IEnumerable<ProjectBaseInformationDto>>> GetAllProjects(string username)
    {
        var url = $"project/user/{username}/projects";
        return await SendGetRequestWithoutData<IEnumerable<ProjectBaseInformationDto>>(url);
    }

    public async Task<ApiResult<ProjectDto>> GetProject(string username, Guid projectId)
    {
        var url = $"project/users/{username}/projects/{projectId}";
        return await SendGetRequestWithoutData<ProjectDto>(url);
    }
}
