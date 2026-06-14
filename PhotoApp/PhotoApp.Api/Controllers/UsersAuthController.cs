using Amazon.Runtime.Internal;
using Amazon.S3.Model.Internal.MarshallTransformations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PhotoApp.Api.DbObjects;
using PhotoApp.Api.Repository;
using PhotoApp.Api.Service;
using PhotoApp.Api.Tools.Mailer;
using PhotoApp.Api.Tools.Tokens;
using PhotoApp.Common.EnumShared;
using PhotoApp.Common.ModelsShared;
using System.Security.Claims;

namespace PhotoApp.Api.Controllers
{
    [ApiController]
    [Route("auth")]
    public class UsersAuthController(UserService userService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<ServerAuthResponse?>> RegisterRequest([FromBody] RegisterModelDto request)
        {
            try
            {
                var user = await userService.RegisterUserAsync(request);
                if (user is null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Error during creating user.");
                }
                var accessToken = await userService.SendNewAccessTokenEmailAsync(user);
               
                return Ok(new ServerAuthResponse()
                {
                    AccessToken = accessToken,
                    Username = request.Username
                });                
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("register/activity")]
        [Authorize(Roles = nameof(SystemRole.Guest))]
        public async Task<ActionResult<ServerAuthResponse>> RegisterCheckUserActivity()
        { 
            try
            {
                var username = User.FindFirst(ClaimTypes.Name)?.Value;

                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized("Invalid token");
                }
                var isActive = await userService.CheckUserAccountActivityAsync(username);

                if (isActive)
                {
                    var accessToken = await userService.GenerateNewAccessToken(username);

                    if (string.IsNullOrEmpty(accessToken))
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, "Failed to generate access token.");
                    }

                    var refreshToken = await userService.GenerateNewRefreshToken(username);
                    if (refreshToken is null)
                    {
                        return StatusCode(StatusCodes.Status500InternalServerError, "Failed to generate refresh token.");
                    }
                    SetRefreshTokenCookie(refreshToken.Token, refreshToken.Expires);
                    SetUsernameCookie(username, refreshToken.Expires);

                    var response = new ServerAuthResponse()
                    {
                        Username = username,
                        AccessToken = accessToken
                    };

                    return Ok(response);
                }
                return Forbid();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPatch("register/activate")]
        [Authorize(Roles = nameof(SystemRole.Guest))]
        public async Task<IActionResult> RegisterActivateUser()
        {
            try
            {
                var username = User.FindFirst(ClaimTypes.Name)?.Value;
                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized("Invalid token");
                }
                var user = await userService.ActivateUserAsync(username) ?? throw new Exception("Error during user activation");
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("register/resend")]
        public async Task<IActionResult> ResendActivationEmail([FromBody] UserModelDto request)
        {
            try
            {
                var user = await userService.TryLoginAsync(request);
                if (user is null)
                {
                    return BadRequest("Login or password is incorrect.");
                }
                var accessToken = await userService.SendNewAccessTokenEmailAsync(user);

                return Ok(new ServerAuthResponse()
                {
                    AccessToken = accessToken,
                    Username = request.Username,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginRequest([FromBody] UserModelDto request)
        {
            try
            {
                var user = await userService.TryLoginAsync(request);
                if (user is null)
                {
                    return BadRequest("Login or password is incorrect.");
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }



        [HttpPost("login/{code}")]
        public async Task<ActionResult<ServerAuthResponse>> LoginVerify([FromBody] UserModelDto request, [FromRoute] string code)
        {
            try
            {
                var user = await userService.TryLoginAsync(request, false);
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
                    SetUsernameCookie(user.Username, refreshToken.Expires);

                    var response = new ServerAuthResponse()
                    {
                        Username = user.Username,
                        AccessToken = accessToken
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

        [HttpGet("refresh")]
        public async Task<ActionResult<ServerAuthResponse>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var username = Request.Cookies["username"];

            if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(username))
            {
                return Unauthorized("No refresh token provided.");
            }

            if (!await userService.ValidateRefreshTokenforUser(username, refreshToken))
            {
                Response.Cookies.Delete("refreshToken");
                Response.Cookies.Delete("username");
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
                AccessToken = newAccessToken
            });
        }

        [HttpDelete("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("refreshToken");
            Response.Cookies.Delete("username");
            return Ok();
        }

        private void SetRefreshTokenCookie(string token, DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = expires,
                Path = "/auth/refresh"
            };

            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        private void SetUsernameCookie(string username, DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = false,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = expires,
                Path = "/"
            };

            Response.Cookies.Append("username", username, cookieOptions);
        }
    }
}
