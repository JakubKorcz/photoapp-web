using PhotoApp.Api.DbObjects;
using PhotoApp.Common.ModelsShared;

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
            var media = context.Medias.FirstOrDefault(m => m.Id == mediaId);
            return media;
        }
    }
}
