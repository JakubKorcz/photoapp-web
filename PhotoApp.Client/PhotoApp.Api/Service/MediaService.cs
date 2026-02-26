using PhotoApp.Api.DbObjects;
using PhotoApp.Api.Repository;

namespace PhotoApp.Api.Service
{
    public class MediaService(MediaRepository mediaRepository, ProjectRepository projectRepository, ProjectFolderRepository folderRepository)
    {
        public async Task<Media> ConnectWithProject(Guid mediaId, Guid projectId, Guid parentFolderId)
        {
            var mediaInDb = await context.Medias.FindAsync(mediaId)
                    ?? throw new KeyNotFoundException("Media not found");

            _ = await context.Projects.FindAsync(projectId)
                    ?? throw new KeyNotFoundException("Project not found");

            _ = await context.Folders.FindAsync(parentFolderId)
                    ?? throw new KeyNotFoundException("Folder not found");

            mediaInDb.ProjectId = projectId;
            mediaInDb.ParentFolderId = parentFolderId;

            await context.SaveChangesAsync();
            return mediaInDb;
        }

        public async Task<Media> ChangeDestinationFolderAsync(Guid mediaId, Guid destinationFolderId)
        {
            var mediaInDb = await context.Medias.FindAsync(mediaId)
                    ?? throw new KeyNotFoundException("Media not found");

            var folder = await context.Folders.FindAsync(destinationFolderId)
                   ?? throw new KeyNotFoundException("Folder not found");



            mediaInDb.ParentFolderId = destinationFolderId;

            await context.SaveChangesAsync();
            return mediaInDb;
        }
    }
}
