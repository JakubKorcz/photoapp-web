using Microsoft.AspNetCore.Mvc;
using PhotoApp.Api.DbObjects;
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

            //Ręczna inicjalizacja projektów
            var projects = new List<ProjectBaseInformationDto>()
            {
               new ProjectBaseInformationDto(){Id = new Guid(), Creator = new Guid(), ProjectName = "Projekt", CreatedAt = new DateOnly(2025,10,10)},
               new ProjectBaseInformationDto(){Id = new Guid(), Creator = new Guid(), ProjectName = "Abecadło", CreatedAt = new DateOnly(2025,11,14)},
               new ProjectBaseInformationDto(){Id = new Guid(), Creator = new Guid(), ProjectName = "Kubabuba", CreatedAt = new DateOnly(2026,01,20)},
            };

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
