using Amazon.Runtime.Internal;
using Microsoft.AspNetCore.Identity;
using PhotoApp.Api.DbObjects;
using PhotoApp.Api.Repository;
using PhotoApp.Api.Tools.Mailer;
using PhotoApp.Api.Tools.Tokens;
using PhotoApp.Common.EnumShared;
using PhotoApp.Common.ModelsShared;
using System.ComponentModel;
using System.Reactive.Disposables;

namespace PhotoApp.Api.Service
{
    public class UserService(UserRepository _userRepository, RefreshTokenRepository _refreshTokenRepository, IConfiguration _configuration)
    {
        public async Task<User?> TryLoginAsync(UserModelDto request, bool isNewEmailCodeGenerating = true)
        {
            var user = await _userRepository.GetUserByUsernameAsync(request.Username);
            if (user is null)
            {
                return null;
            }

            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
            {
                return null;
            }

            return user;
        }

        public async Task<User?> RegisterUserAsync(RegisterModelDto request)
        {
            if (!Mailer.IsValidEmail(request.Email))
            {
                throw new Exception("This value is not a proper email!");
            }

            if (await _userRepository.GetUserByUsernameAsync(request.Username) is not null)
            {
                throw new Exception("Username is already taken");
            }

            if (await _userRepository.GetUserByEmailAsync(request.Email) is not null)
            {
                throw new Exception("Email is already taken");
            }

            return await _userRepository.CreateUserAsync(request);
        }

        public async Task<User?> CheckEmailCodeAsync(UserModelDto request, string code)
        {
            var user = await _userRepository.GetUserByUsernameAsync(request.Username);
            if (user is null)
            {
                return null;
            }

            if (user.HasValidLoginCode(code))
            {
                return user;
            }

            return null;
        }

        public async Task<bool> ValidateRefreshTokenforUser(string username, string refreshToken)
        {
            var tokenFromDb = await _refreshTokenRepository.GetRefreshTokenByTokenAsync(refreshToken);
            if (tokenFromDb is null || tokenFromDb.Username != username || !tokenFromDb.IsActive)
            {
                return false;
            }
            return true;
        }

        public async Task<User?> GetUserByTokenAsync(string token)
        {
            var refreshToken = await _refreshTokenRepository.GetRefreshTokenByTokenAsync(token);
            if (refreshToken is null || !refreshToken.IsActive)
            {
                return null;
            }
            return await _userRepository.GetUserByUsernameAsync(refreshToken.Username);
        }

        public async Task<RefreshToken?> GenerateNewRefreshToken(string username)
        {
            var user = await _userRepository.GetUserByUsernameAsync(username);
            if (user is null)
            {
                return null;
            }
            var tm = new TokenManager(_configuration);
            var refreshToken = tm.GenerateRefreshToken();
            var result = await _refreshTokenRepository.SetAllTokensForUserAsRevokedAsync(username);
            if (!result)
            {
                return null;
            }
            return await _refreshTokenRepository.CreateRefreshTokenAsync(username, refreshToken);
        }

        public async Task<string?> GenerateNewAccessToken(string username, SystemRole role = SystemRole.Member)
        {
            var user = await _userRepository.GetUserByUsernameAsync(username);
            if (user is null)
            {
                throw new Exception("Cannot generate access token for non existing user");
            }
            var tm = new TokenManager(_configuration);
            var accessToken = tm.GenerateJWTAccessToken(user: user, role: role);
            return accessToken;
        }

        public async Task<User?> ActivateUserAsync(string username)
        {
            return await _userRepository.ActivateUserByUsername(username);
        }

        public async Task<bool> CheckUserAccountActivityAsync(string username)
        {
            var user = await _userRepository.GetUserByUsernameAsync(username);
            if (user is null) throw new Exception("Cannot check account activity for non existing user");
            return user.IsActive;
        }

        public async Task<User> SendNewNumberCodeEmailAsync(User user)
        {
            var generatedCode = new CodeGenerator().Generate();
            var mailer = new Mailer(_configuration);
            mailer.SendLoginMail(user.Email, user.Username, generatedCode);

            if (user is not null)
            {
                return await _userRepository.UpdateUserEmailLoginCodeAsync(user.Username, generatedCode, DateTime.UtcNow.AddMinutes(10));
            }

            throw new Exception("Cannot send email with new number code");
        }

        public async Task<string> SendNewAccessTokenEmailAsync(User user)
        {
            var accessToken = await GenerateNewAccessToken(user.Username, SystemRole.Guest);
            var mailer = new Mailer(_configuration);
            var url = accessToken; // TODO Url z krwi i kości
            mailer.SendLoginMail(user.Email, user.Username, url);
            return accessToken;
        }
    }
}
