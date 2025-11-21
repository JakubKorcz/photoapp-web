using Microsoft.AspNetCore.Mvc;
using PhotoApp.Common.ModelsShared;

namespace PhotoApp.Api.Controllers
{
    [ApiController]
    [Route("account")]
    public class AccountController(IConfiguration configuration, AppDbContext context) : ControllerBase
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly AppDbContext _dbContext = context;

        [HttpGet("memory")]
        public IActionResult GetMemoryInfo()
        {
            return Ok(new MemoryInfoResponse() {
                TotalMemoryInBytes = 256000000,
                UsedMemoryInBytes = 128000000,
                FreeMemoryInBytes = 128000000
            });
        }
    }
}
