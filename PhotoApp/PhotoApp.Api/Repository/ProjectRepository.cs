using PhotoApp.Api.DbObjects;
using Microsoft.EntityFrameworkCore;
using PhotoApp.Common.ModelsShared;

namespace PhotoApp.Api.Repository
{
    public class ProjectRepository(AppDbContext context)
    {
        private readonly AppDbContext context = context;
        public async Task<Project?> CreateProjectAsync(Project project)
        {
            context.Projects.Add(project);
            await context.SaveChangesAsync();
            return project;
        }

        public async Task<List<Project>> GetAllProjectsForUserAsync(string username)
        {
            var list = context.Projects.Where(p => p.Username == username).ToList();
            return list;
        }

        public async Task<Project?> GetProjectByIdAsync(Guid id)
        {
            var project = context.Projects.FirstOrDefault(p => p.Id == id);
            return project;
        }

        public async Task<bool> ProjectExistsAsync(Guid projectId)
        {
            return await context.Projects
                .AnyAsync(p => p.Id == projectId);
        }
    }
}
