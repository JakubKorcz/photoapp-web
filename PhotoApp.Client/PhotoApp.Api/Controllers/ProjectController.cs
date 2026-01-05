using Microsoft.AspNetCore.Mvc;
using PhotoApp.Client.Models;
using PhotoApp.Common.ModelsShared;

namespace PhotoApp.Api.Controllers
{
    [ApiController]
    [Route("project")]
    public class ProjectController(IConfiguration configuration, AppDbContext context) : ControllerBase
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly AppDbContext _dbContext = context;

        [HttpGet("{userId}")]
        public ActionResult<IEnumerable<ProjectBaseInformationDto>> GetAllProjectsForUser([FromRoute] Guid userId)
        {
            var projects = _dbContext.Projects
                .Where(p => p.Creator == userId)
                .Select(p => new ProjectBaseInformationDto
                {
                    Id = p.Id,
                    Creator = p.Creator,
                    ProjectName = p.ProjectName
                })
                .ToList();

            return Ok(projects);
        }

        [HttpGet("{userId}/{id}")]
        public ActionResult<ProjectDto> GetProjectConfiguration([FromRoute] Guid userId, [FromRoute] Guid id)
        {
            var configuration = _dbContext.Projects
                .FirstOrDefault(p => p.Id == id && p.Creator == userId);
            return Ok(configuration);
        }
    }
}
