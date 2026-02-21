using Microsoft.AspNetCore.Mvc;

namespace PhotoApp.BucketImitator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MediaController : ControllerBase
    {
        
        [HttpGet(Name = "test")]
        public string GetTestString()
        {
            return "test";
        }
    }
}
