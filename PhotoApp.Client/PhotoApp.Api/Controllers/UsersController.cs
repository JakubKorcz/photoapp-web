using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using PhotoApp.Api.Objects;
using RegisterRequest = PhotoApp.Api.Objects.RegisterRequest;

namespace PhotoApp.Api.Controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;

        public UsersController(ILogger<UsersController> logger)
        {
            _logger = logger;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            try
            {
                using (var dbContext = new AppDbContext())
                {
                    var user = new User { Id = Guid.NewGuid(), Email = request.Username, Password = request.Password };
                    dbContext.Users.Add(user);              
                    dbContext.SaveChanges();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return BadRequest();
            }
        }


        [HttpGet]
        public IActionResult Test()
        {
            return Ok();
        }

    }
}
