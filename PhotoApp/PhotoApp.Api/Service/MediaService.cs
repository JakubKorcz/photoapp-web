using PhotoApp.Api.DbObjects;
using PhotoApp.Api.Repository;

namespace PhotoApp.Api.Service
{
    public class MediaService(MediaRepository mediaRepository, ProjectRepository projectRepository, ProjectFolderRepository folderRepository)
    {
        public async Task<Media?> ConnectWithProject(Guid mediaId, Guid projectId, Guid parentFolderId)
        {
            if (!await projectRepository.ProjectExistsAsync(projectId))
                throw new KeyNotFoundException("Project not found");

            if (!await folderRepository.FolderExistsInProjectAsync(parentFolderId, projectId))
                throw new KeyNotFoundException("Folder not found in project");

            var media = await mediaRepository.UpdateDestinationFolderAsync(mediaId, parentFolderId, projectId);
            return media;
        }

        public async Task<Media?> ChangeDestinationFolderAsync(Guid mediaId, Guid destinationFolderId)
        {
            var mediaInDb = await mediaRepository.GetMediaByIdAsync(mediaId)
                              ?? throw new KeyNotFoundException("Media not found");

            if (mediaInDb.ProjectId == null)
                throw new InvalidOperationException("Media is not connected to a project");

            if (!await folderRepository.FolderExistsInProjectAsync(destinationFolderId, mediaInDb.ProjectId!.Value))
                throw new KeyNotFoundException("Folder not found in project");

            var media = await mediaRepository.UpdateDestinationFolderAsync(mediaId, destinationFolderId);
            return media;
        }
    }
}
