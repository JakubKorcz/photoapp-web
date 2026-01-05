using Microsoft.AspNetCore.Mvc;
using PhotoApp.Common.ModelsShared;

namespace PhotoApp.Api.Controllers
{
    [ApiController]
    [Route("projects")]
    public class ProjectController(IConfiguration configuration, AppDbContext context) : ControllerBase
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly AppDbContext _dbContext = context;

        //Metoda zwracająca wszystkie projekty dla danego użytkownika ()
        [HttpGet("")]
        public IActionResult GetAllProjectsForUser()
        {
            var projects = _dbContext.Projects.Select(p => p.Id).ToList();
            return Ok(projects);
        }

        [HttpGet("{id}")]
        public IActionResult GetProjectConfiguration([FromRoute] Guid id)
        {
            var configuration = _dbContext.Projects.FirstOrDefault(p => p.Id == id);
            return Ok(configuration);
        }
    }
}
