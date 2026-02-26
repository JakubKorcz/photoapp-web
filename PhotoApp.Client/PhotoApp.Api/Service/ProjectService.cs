using AutoMapper;
using PhotoApp.Api.DbObjects;
using PhotoApp.Api.Repository;
using PhotoApp.Common.ModelsShared;

namespace PhotoApp.Api.Service
{
    public class ProjectService(IMapper mapper, UserRepository userRepository, ProjectRepository projectRepository, ProjectFolderRepository projectFolderRepository, MediaRepository mediaRepository)
    {
        private readonly IMapper _mapper = mapper;
        //CREATE
        public async Task<Project> CreateProjectWithUsernameAsync(string username, string projectName)
        {
            var user = await userRepository.GetUserByUsernameAsync(username) ?? throw new Exception($"Cannot create project for non-existent user '{username}'.");
            var project = await projectRepository.CreateProjectAsync(user, projectName);

            //TODO Tworzenie katalogów, tworzenie stylów i podpięcie
            return project;
        }

        //READ
        public async Task<List<ProjectBaseInformationDto>> GetAllProjectsByUsernameAsync(string username)
        {
            var user = await userRepository.GetUserByUsernameAsync(username) ?? throw new Exception($"Cannot create project for non-existent user '{username}'.");
            var projects = await projectRepository.GetAllProjectsForUserAsync(user);
            return _mapper.Map<List<ProjectBaseInformationDto>>(projects);
        }

        public async Task<Project?> GetProjectByIdAsync(Guid id)
        {
            var project = await projectRepository.GetProjectByIdAsync(id) ?? throw new Exception($"Cannot find project with id '{id}'.");
            project.MainFolder = await projectFolderRepository.GetFolderByIdAsync(project.MainFolderId) ?? throw new Exception($"Cannot find main folder with id '{project.MainFolderId}' for project '{project.Id}'.");
            project.MainFolder.Medias = await mediaRepository.GetAllMediasFromFolderAsync(project.MainFolderId);
            foreach (var folder in project.MainFolder.Folders)
            {
                folder.Medias = await mediaRepository.GetAllMediasFromFolderAsync(folder.Id);
            }
            //TODO download design
            return project;
        }

        //UPDATE
        public async Task<Project> UpdateProjectAsync(Project project)
        {
            return await projectRepository.UpdateProjectAsync(project);
        }
        //DELETE
    }
}
