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
    public class ProjectController(IConfiguration configuration, ProjectService projectService) : ControllerBase
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly ProjectService _projectService = projectService;

        [HttpGet("user/{username}/projects")]
        public ActionResult<IEnumerable<ProjectBaseInformationDto>> GetAllProjectsForUser([FromRoute] string username)
        {
            var projects = _projectService.GetAllProjectsByUsernameAsync(username);
            return Ok(projects);
        }

        [HttpGet("users/{username}/projects/{id}")]
        public ActionResult<ProjectDto> GetProjectConfiguration([FromRoute] string username, [FromRoute] Guid id)
        {
            var project = _projectService.GetProjectByIdAsync(id);
            return Ok(project);
        }

        //[HttpPost("users/{username}/projects")]
        //public ActionResult<ProjectBaseInformationDto> CreateBaseProject([FromBody] ProjectBaseInformationDto projectBase)
        //{
        //    var project = _projectService.CreateProjectWithUserIdAsync
        //}

        [HttpPut("users/{username}/projects")]
        public ActionResult UpdateProject([FromBody] ProjectDto createProjectDto)
        {
            //var projectId = _projectService.CreateProjectAsync(createProjectDto);
            //return Ok(projectId);
            return Ok(); //TODO
        }
    }
}
