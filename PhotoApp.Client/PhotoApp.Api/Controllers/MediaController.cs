using Microsoft.AspNetCore.Mvc;
using PhotoApp.Client.Models;
using PhotoApp.Common.ModelsShared;

namespace PhotoApp.Api.Controllers
{
    [ApiController]
    [Route("media")]
    public class MediaController(IConfiguration configuration, AppDbContext context) : ControllerBase
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly AppDbContext _dbContext = context;

        [HttpGet("")]
        public Task<IActionResult> GetAllMediaForUser([FromBody] List<MediaDto> medias)
        {
            var media = await _dbContext.ProjectMedia
       .FirstOrDefaultAsync(m => m.ProjectId == projectId && m.Id == mediaId);

            if (media?.ImageData == null) return NotFound();

            return File(media.ImageData, media.ContentType ?? "application/octet-stream", media.FileName);
        }
    }
}
