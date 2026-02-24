using PhotoApp.Api.DbObjects;

namespace PhotoApp.Api.Repository
{
    public class ProjectFolderRepository(AppDbContext context)
    {
        private readonly AppDbContext context = context;

        public async Task<ProjectFolder?> GetFolderByIdAsync(Guid folderId)
        {
            var folder = context.Folders.FirstOrDefault(x => x.Id == folderId);
            folder?.Folders = await GetSubFoldersForFolderAsync(folderId);
            return folder;
        }

        public async Task<List<ProjectFolder>> GetSubFoldersForFolderAsync(Guid parentId)
        {
            var folders = context.Folders.Where(x => x.ParentFolderId == parentId).ToList();
            return folders;
        }
    }
}
