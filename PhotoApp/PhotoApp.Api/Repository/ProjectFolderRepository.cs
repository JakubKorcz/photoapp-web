using PhotoApp.Api.DbObjects;
using System.ComponentModel;
using System.Data.Entity;

namespace PhotoApp.Api.Repository
{
    public class ProjectFolderRepository(AppDbContext context)
    {
        private readonly AppDbContext context = context;

        //CREATE
        public async Task<ProjectFolder> CreateMainFolderAsync(Guid projectId)
        {
            var mainFolder = new ProjectFolder { IsHeadFolder = true, ProjectId = projectId };

            if (await context.Folders.AnyAsync(f => f.ProjectId == projectId && f.IsHeadFolder))
            {
                throw new InvalidOperationException("Project already has a main folder.");
            }

            context.Add(mainFolder);
            await context.SaveChangesAsync();
            return mainFolder;
        }

        public async Task<ProjectFolder> CreateSubFolderAsync(Guid ProjectId, Guid parentFolderId, string name)
        {
            var subFolder = new ProjectFolder { ProjectId = ProjectId, ParentFolderId = parentFolderId, Name = name };
            context.Add(subFolder);
            await context.SaveChangesAsync();
            return subFolder;
        }

        //READ
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

        //UPDATE
        public async Task<ProjectFolder> UpdateFolderAsync(Guid oldFolderId, ProjectFolder newProjectFolder)
        {
            var folderInDb = await context.Folders.FindAsync(oldFolderId)
                    ?? throw new KeyNotFoundException("Folder not found");

            folderInDb.Name = newProjectFolder.Name;
            await context.SaveChangesAsync();
            return folderInDb;
        }

        //DELETE
        public async Task DeleteFolderAsync(Guid idToDelete)
        {
            var folder = await context.Folders
                .Include(f => f.Medias)
                .Include(f => f.Folders)
                .FirstOrDefaultAsync(f => f.Id == idToDelete);

            if (folder == null)
            {
                throw new KeyNotFoundException("Folder not found.");
            }

            if (folder.IsHeadFolder)
            {
                throw new InvalidOperationException("Cannot delete the project's main folder.");
            }

            if (folder.Folders.Count != 0)
            {
                throw new InvalidOperationException("Cannot delete folder with existing subfolders.");
            }

            if (folder.Medias.Count != 0)
            {
                throw new InvalidOperationException("Cannot delete folder with existing medias.");
            }

            context.Folders.Remove(folder);
            await context.SaveChangesAsync();
        }

        //EXISTS
        public async Task<bool> FolderExistsInProjectAsync(Guid folderId, Guid projectId)
        {
            return await context.Folders.AsNoTracking().AnyAsync(f => f.Id == folderId && f.ProjectId == projectId);
        }
    }
}
