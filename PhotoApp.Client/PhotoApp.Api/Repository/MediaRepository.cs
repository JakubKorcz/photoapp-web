using PhotoApp.Api.DbObjects;

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
    }
}
