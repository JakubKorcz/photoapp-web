using PhotoApp.Client.Models;
using PhotoApp.Common.ModelsShared;

namespace PhotoApp.Client.Connection
{
    public partial class ApiConnection
    {
        public async Task<IEnumerable<ProjectBaseInformationDto>> GetAllProjects(Guid userId)
        {
            var url = $"https://localhost:7003/project/{userId}";
            var response = await SendGetRequestWithoutData<IEnumerable<ProjectBaseInformationDto>>(url);
            return response!; //TODO handle null properly
        }

        public async Task<ProjectDto> GetProject(Guid userId, Guid projectId)
        {
            var url = $"https://localhost:7003/project/{userId}/{projectId}";
            var response = await SendGetRequestWithoutData<ProjectDto>(url);
            return response!; //TODO handle null properly
        }
    }
}
