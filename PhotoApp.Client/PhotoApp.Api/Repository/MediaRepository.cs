using PhotoApp.Api.DbObjects;
using PhotoApp.Common.ModelsShared;

namespace PhotoApp.Api.Repository
{
    public class MediaRepository(AppDbContext context)
    {
        private readonly AppDbContext context = context;

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

        public async Task<Media> CreateMedia(MediaDto media)
        {
           

        }
    }
}
