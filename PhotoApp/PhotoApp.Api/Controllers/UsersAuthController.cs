using Amazon.Runtime.Internal;
using Amazon.S3.Model.Internal.MarshallTransformations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
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
    public class UsersAuthController(UserService userService, ILogger<UsersAuthController> logger) : ControllerBase
    {
        [HttpPost("register")]
        [EnableRateLimiting("auth-strict")]
        public async Task<ActionResult<ServerAuthResponse?>> RegisterRequest([FromBody] RegisterModelDto request)
        {
            try
            {
                var user = await userService.RegisterUserAsync(request);
                if (user is null)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, "Error during creating user.");
                }

                await userService.SendNewNumberCodeEmailAsync(user);

                return Ok(new ServerAuthResponse()
                {
                    AccessToken = null,
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
        [EnableRateLimiting("auth-resend")]
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
        [EnableRateLimiting("auth-strict")]
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
        [EnableRateLimiting("auth-code")]
        public async Task<ActionResult<ServerAuthResponse>> LoginVerify([FromBody] UserModelDto request, [FromRoute] string code)
        {
            try
            {
                var user = await userService.TryLoginAsync(request, false);
                if (user is null)
                {
                    return BadRequest("Login or password is incorrect.");
                }

                if (user.IsLoginCodeLockedOut)
                {
                    var remaining = user.LoginCodeLockoutUntil!.Value - DateTime.UtcNow;
                    var minutes = Math.Max(1, (int)Math.Ceiling(remaining.TotalMinutes));
                    return StatusCode(StatusCodes.Status429TooManyRequests, $"Przekroczono limit prób. Spróbuj ponownie za {minutes} min.");
                }

                if (!user.HasValidLoginCode(code))
                {
                    await userService.RegisterFailedLoginCodeAttemptAsync(user.Username);
                    return Unauthorized("Nieprawidłowy kod.");
                }

                await userService.ResetLoginCodeAttemptsAsync(user.Username);

                if (!user.IsActive)
                {
                    await userService.ActivateUserAsync(user.Username);
                }

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
                    AccessToken = accessToken
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "LoginVerify failed for username={Username}", request?.Username);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("refresh")]
        [EnableRateLimiting("refresh")]
        public async Task<ActionResult<ServerAuthResponse>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized("No refresh token provided.");
            }

            var (newAccessToken, newRefreshToken, username) = await userService.RotateRefreshTokenAsync(refreshToken);

            if (newAccessToken is null || newRefreshToken is null || username is null)
            {
                Response.Cookies.Delete("refreshToken");
                return Unauthorized("Invalid refresh token.");
            }

            SetRefreshTokenCookie(newRefreshToken.Token, newRefreshToken.Expires);

            return Ok(new ServerAuthResponse
            {
                Username = username,
                AccessToken = newAccessToken
            });
        }

        [HttpDelete("logout")]
        public async Task<IActionResult> Logout()
        {
            if (Request.Cookies.TryGetValue("refreshToken", out var refreshToken) && !string.IsNullOrEmpty(refreshToken))
            {
                await userService.RevokeRefreshTokenAsync(refreshToken);
            }
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
                Expires = expires,
                Path = "/auth/refresh"
            };

            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }
    }
}
