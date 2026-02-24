using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PhotoApp.Api.DbObjects;
using PhotoApp.Api.Repository;
using PhotoApp.Api.Service;
using PhotoApp.Client.Models;
using PhotoApp.Common.ModelsShared;

namespace PhotoApp.Api.Controllers
{
    [ApiController]
    [Route("project")]
    public class ProjectController(IConfiguration configuration, ProjectService projectService, IMapper mapper) : ControllerBase
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly ProjectService _projectService = projectService;
        private readonly IMapper _mapper = mapper;

        [HttpGet("{username}")]
        public ActionResult<IEnumerable<ProjectBaseInformationDto>> GetAllProjectsForUser([FromRoute] string username)
        {
            var projects = _projectService.GetAllProjectsByUsernameAsync(username);
            return Ok(_mapper.Map<List<ProjectBaseInformationDto>>(projects));
        }

        [HttpGet("{username}/{id}")]
        public ActionResult<ProjectDto> GetProjectConfiguration([FromRoute] string username, [FromRoute] Guid id)
        {
            var project = _projectService.GetProjectByIdAsync(id);
            return Ok(project);
        }
    }
}
