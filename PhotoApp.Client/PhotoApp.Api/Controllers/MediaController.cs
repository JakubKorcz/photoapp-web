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
        public Task<IActionResult> GetMedia([FromBody] MediaDto media)
        {
            if (media == null)
            {
                throw new ArgumentNullException(nameof(media));
            }

            //var imageData = 
            ////var imageData = 
            //return File(media.ImageData, media.ContentType ?? "application/octet-stream", media.FileName);
        }
    }
}
