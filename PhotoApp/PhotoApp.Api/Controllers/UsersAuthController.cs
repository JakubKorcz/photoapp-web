using Amazon.S3.Model.Internal.MarshallTransformations;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PhotoApp.Api.DbObjects;
using PhotoApp.Api.Repository;
using PhotoApp.Api.Service;
using PhotoApp.Api.Tools.Mailer;
using PhotoApp.Api.Tools.Tokens;
using PhotoApp.Common.ModelsShared;

namespace PhotoApp.Api.Controllers
{
    [ApiController]
    [Route("auth")]
    public class UsersAuthController(IConfiguration _configuration, UserService userService) : ControllerBase
    {
        [HttpPost("register")]
        public IActionResult RegisterRequest([FromBody] UserModel request)
        {
            try
            {
                var generatedCode = new CodeGenerator().Generate();
                var mailer = new Mailer(_configuration); 
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
        public async Task<IActionResult> RegisterVerify([FromRoute] string username, [FromRoute] string code)
        {
            //TODO NALEZY JESZCZE DOKLADNIE PRZEMYSLEC
            try
            {
                if (!Mailer.IsValidEmail(username))
                {
                    throw new Exception("This value is not a proper email!");
                }

                if (!string.IsNullOrEmpty(username))
                {
                    return Ok(await _userRepository.CreateUserAsync(username));
                }

                return BadRequest("Username cannot be empty");
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return BadRequest();
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult> LoginRequest([FromBody] UserModel request)
        {
            var user = await userService.TryLoginAsync(request);
            if (user is null)
            {
                return BadRequest("Login or password is incorrect.");
            }
            return Ok();
        }

        [HttpPost("login/{code}")]
        public async Task<ActionResult<ServerAuthResponse>> LoginVerify([FromBody] UserModel request, [FromRoute] string code)
        {
            try
            {
                var user = await userService.TryLoginAsync(request);
                if (user is null)
                {
                    return BadRequest("Login or password is incorrect.");
                }

                if (user.HasValidLoginCode(code))
                {
                    var accessToken = await userService.GenerateNewAccessToken(user.Username);

                    if (string.IsNullOrEmpty(accessToken))
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, "Failed to generate access token.");
                    }

                    var refreshToken = await userService.GenerateNewRefreshToken(user.Username);
                    if (refreshToken is null)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, "Failed to generate refresh token.");
                    }
                    SetRefreshTokenCookie(refreshToken.Token, refreshToken.Expires);

                    var response = new ServerAuthResponse()
                    {
                        Username = user.Username,
                        AccessToken = accessToken,
                        Success = true  
                    };

                    return Ok(response);
                }
                return Unauthorized(); 
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost("refresh-token/{username}")]
        public async Task<ActionResult<ServerAuthResponse>> RefreshToken([FromRoute] string username)
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized("No refresh token provided.");
            }

            if (!await userService.ValidateRefreshTokenforUser(username, refreshToken))
            {
                Response.Cookies.Delete("refreshToken");
                return Unauthorized("Invalid refresh token.");
            }

            var newAccessToken = await userService.GenerateNewAccessToken(username);
            if (newAccessToken is null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to generate access token.");
            }

            return Ok(new ServerAuthResponse
            {
                Username = username,
                AccessToken = newAccessToken,
                Success = true
            });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("refreshToken");
            return Ok();
        }

        private void SetRefreshTokenCookie(string token, DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = expires
            };

            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }
    }
}
