using PhotoApp.Client.Models;
using PhotoApp.Common.ModelsShared;

namespace PhotoApp.Client.Connection
{
    public partial class ApiConnection
    {
        public async Task<ProjectBaseInformationDto> CreateBaseProject(string username, ProjectFormModel model)
        {
            var url = $"/users/{username}/projects";
            var project = _mapper.Map<ProjectBaseInformationDto>(model);
            var response = await SendPostRequest<ProjectBaseInformationDto, ProjectBaseInformationDto>(url, project);
            return response!; //TODO handle null properly
        } 
        public async Task<IEnumerable<ProjectBaseInformationDto>> GetAllProjects(string username)
        {
            var url = $"/users/{username}/projects";
            var response = await SendGetRequestWithoutData<IEnumerable<ProjectBaseInformationDto>>(url);
            return response!; //TODO handle null properly
        }

        public async Task<ProjectDto> GetProject(string username, Guid projectId)
        {
            var url = $"/user/{username}/projects/{projectId}";
            var response = await SendGetRequestWithoutData<ProjectDto>(url);
            return response!; //TODO handle null properly
        }
    }
}
