using Microsoft.AspNetCore.Mvc;
using PhotoApp.Client.Models;
using PhotoApp.Common.ModelsShared;

namespace PhotoApp.Api.Controllers
{
    [ApiController]
    [Route("media")]
    public class MediaController(IConfiguration configuration, AppDbContext context) : ControllerBase
    {
        //private readonly IConfiguration _configuration = configuration;
        //private readonly AppDbContext _dbContext = context;
        //private readonly MediaConv _mediaController = new Medai;

        //[HttpGet("{url}")]
        //public Task<ActionResult> GetMedia([FromRoute] string url)
        //{
        //    //if (media == null)
        //    //{
        //    //    throw new ArgumentNullException(nameof(media));
        //    //}

        //    ////var imageData = 
        //    //////var imageData = 
        //    //return File(media.ImageData, "application/octet-stream", media.Description);
        //    return Ok()
        //}

        //[HttpPost("upload")]
        //public Task<ActionResult> UploadMedia(IFormFile file)
        //{
        //    if (file == null || file.Length == 0)
        //        return BadRequest("No file uploaded");

        //}
    }
}
