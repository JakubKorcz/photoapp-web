using PhotoApp.Api.DbObjects;
using PhotoApp.Common.ModelsShared;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace PhotoApp.Api.Repository
{
    public class MediaRepository(AppDbContext context)
    {
        private readonly AppDbContext context = context;

        public async Task<Media> CreateMediaAsync(Media media)
        {
            context.Medias.Add(media);
            return media;
        }

        public async Task<List<Media>> GetAllMediasFromFolderAsync(Guid folderId)
        {
            var medias = context.Medias.Where(m => m.ParentFolderId == folderId).ToList();
            return medias;
        }

        public async Task<List<Media>> GetAllMediasForProject(Guid projectId)
        {
            var medias = context.Medias.Where(m => m.ProjectId == projectId).ToList();
            return medias;
        }

        public async Task<Media?> GetMediaByIdAsync(Guid mediaId)
        {
            var media = context.Medias.SingleOrDefault(m => m.Id == mediaId);
            return media;
        }

        public async Task<Media?> UpdateDestinationFolderAsync(Guid mediaId, Guid destinationFolderId, Guid? destinationProjectId = null)
        {
            var mediaInDb = await GetMediaByIdAsync(mediaId)
                    ?? throw new KeyNotFoundException("Media not found");

            if (destinationProjectId != null)
            {
                mediaInDb.ProjectId = destinationProjectId;
            }
            
            mediaInDb.ParentFolderId = destinationFolderId;

            await context.SaveChangesAsync();
            return mediaInDb;
        }

        public async Task<bool> MediaExistsAsync(Guid mediaId)
        {
            return await context.Medias
                .AnyAsync(m => m.Id == mediaId);
        }
    }
}
