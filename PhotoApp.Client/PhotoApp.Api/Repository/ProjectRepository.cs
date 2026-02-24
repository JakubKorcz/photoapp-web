using PhotoApp.Api.DbObjects;

namespace PhotoApp.Api.Repository
{
    public class ProjectRepository(AppDbContext context)
    {
        private readonly AppDbContext context = context;
        public async Task<Project> CreateProjectAsync(User user, string projectName)
        {
            var project = new Project
            {
                ProjectName = projectName,
                UserId = user.Id,
            };

            context.Projects.Add(project);
            await context.SaveChangesAsync();
            return project;
        }

        public async Task<List<Project>> GetAllProjectsForUserAsync(User user)
        {
            var list = context.Projects.Where(p => p.UserId == user.Id).ToList();
            return list;
        }

        public async Task<Project?> GetProjectByIdAsync(Guid id)
        {
            var project = context.Projects.FirstOrDefault(p => p.Id == id);
            return project;
        }
    }
}
