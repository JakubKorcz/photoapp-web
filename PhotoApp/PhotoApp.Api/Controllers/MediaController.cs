using Microsoft.AspNetCore.Mvc;
using PhotoApp.Api.Service;
using PhotoApp.Client.Models;
using PhotoApp.Common.ModelsShared;

namespace PhotoApp.Api.Controllers
{
    [ApiController]
    [Route("media")]
    public class MediaController(IConfiguration configuration, MediaService mediaService) : ControllerBase
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly MediaService _mediaService = mediaService;

        [HttpPost("")]
        public ActionResult<ProjectDto> GetProjectConfiguration([FromBody] MediaDto mediaDto)
        {
            _mediaService.CreateMediaAsync(mediaDto);
            return Ok();//TODO
        }

        //[HttpPost]
        //public ActionResult UpdateProject([FromBody] ProjectDto createProjectDto)
        //{
        //    //    var projectId = _projectService.CreateProjectAsync(createProjectDto);
        //    //    return Ok(projectId);
        //    //} //TODO
        //    return Ok();
        //}
    }
}