using Microsoft.AspNetCore.Mvc;
using PhotoApp.Api.Service;
using PhotoApp.Client.Models;
using PhotoApp.Common.ModelsShared;

namespace PhotoApp.Api.Controllers
{
    [ApiController]
    [Route("media")]
    public class MedaiController(IConfiguration configuration, MediaService mediaService) : ControllerBase
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly MediaService _mediaService = mediaService;

        [HttpGet("{mediaId}/url")]
        public ActionResult<IEnumerable<string>> GetMediaBucketUrl([FromRoute] Guid mediaId)
        {
            return Ok("Dupa");
            //return Ok(projects);
        }

        [HttpPost("")]
        public ActionResult<ProjectDto> GetProjectConfiguration([FromBody] MediaDto mediaDto)
        {
            _mediaService.CreateMediaAsync(mediaDto);
            return Ok(project);
        }

        [HttpPost]
        public ActionResult UpdateProject([FromBody] ProjectDto createProjectDto)
        {
            var projectId = _projectService.CreateProjectAsync(createProjectDto);
            return Ok(projectId);
        }
    }
}