using Microsoft.AspNetCore.Mvc;
using PhotoApp.Api.DbObjects;
using PhotoApp.Api.Mailer;
using PhotoApp.Common.Models;

namespace PhotoApp.Api.Controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController(IConfiguration configuration, AppDbContext context) : ControllerBase
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly AppDbContext _dbContext = context;

        [HttpPost("register/{username}")]
        public IActionResult RegisterRequest([FromRoute] string username)
        {
            try
            {
                var generatedCode = new CodeGenerator().Generate();
                var mailer = new Mailer.Mailer(_configuration); 
                mailer.SendRegisterMail(username, "Daryna", generatedCode);
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return BadRequest();
            }
        }

        [HttpPost("register/{username}/{code}")]
        public IActionResult RegisterVerify([FromRoute] string username, [FromRoute] string code)
        {
            //TODO NALEZY JESZCZE DOKLADNIE PRZEMYSLEC
            try
            {
                if (!Mailer.Mailer.IsValidEmail(username))
                {
                    throw new Exception("This value is not a proper email!");
                }
                var user = new User { Id = Guid.NewGuid(), Username = username };
                _dbContext.Users.Add(user);              
                _dbContext.SaveChanges();
               
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return BadRequest();
            }
        }

        [HttpPost("login/{username}")]
        public ActionResult<ServerAuthResponse> LoginRequest([FromRoute] string username)
        {
            try
            {
                var generatedCode = new CodeGenerator().Generate();
                var mailer = new Mailer.Mailer(_configuration);
                mailer.SendLoginMail(username, "Daryna", generatedCode);
                var user = _dbContext.Users.SingleOrDefault(u => u.Username == username);
                if (user is not null)
                {
                    user.LoginCode = generatedCode;
                    user.CodeExpiration = DateTime.UtcNow.AddMinutes(10);
                    _dbContext.SaveChanges();
                }
                var response = new ServerAuthResponse
                {
                    Token = Guid.NewGuid().ToString(),
                    Message = "Login code sent",
                    Success = true
                };

                return Ok(response);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return BadRequest();
            }
        }

        [HttpPost("login/{username}/{code}")]
        public ActionResult<ServerAuthResponse> LoginVerify([FromRoute] string username, [FromRoute]string code)
        {
            try
            {
                var user = _dbContext.Users.SingleOrDefault(u => u.Username == username);
                if (user is null)
                {
                    return BadRequest();
                }
                if (user.LoginCode == int.Parse(code) && user.CodeExpiration >= DateTime.UtcNow)
                {
                    var response = new ServerAuthResponse()
                    {
                        Message = "Login Succesful",
                        Token = Guid.NewGuid().ToString(),
                        Success = true
                    };
                    return Ok(response);
                }
                return Unauthorized(); 
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
