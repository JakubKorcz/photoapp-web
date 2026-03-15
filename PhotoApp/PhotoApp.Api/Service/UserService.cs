using Amazon.Runtime.Internal;
using Microsoft.AspNetCore.Identity;
using PhotoApp.Api.DbObjects;
using PhotoApp.Api.Repository;
using PhotoApp.Api.Tools.Mailer;
using PhotoApp.Api.Tools.Tokens;
using PhotoApp.Common.ModelsShared;
using System.ComponentModel;
using System.Reactive.Disposables;

namespace PhotoApp.Api.Service
{
    public class UserService(UserRepository _userRepository, RefreshTokenRepository _refreshTokenRepository, IConfiguration _configuration)
    {
        public async Task<User?> TryLoginAsync(UserModel request)
        {
            var user = await _userRepository.GetUserByUsernameAsync(request.Email);
            if (user is null)
            {
                return null;
            }

            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
            {
                return null;
            }

            var generatedCode = new CodeGenerator().Generate();
            var mailer = new Mailer(_configuration);
            mailer.SendLoginMail(request.Email, "Daryna", generatedCode);

            if (user is not null)
            {
                return await _userRepository.UpdateUserEmailLoginCodeAsync(user.Username, generatedCode, DateTime.UtcNow.AddMinutes(10));
            }

            return null;
        }

        public async Task<User?> RegisterUserAsync(UserModel request)
        {
            if (!Mailer.IsValidEmail(request.Email))
            {
                throw new Exception("This value is not a proper email!");
            }

            var existingUser = await _userRepository.GetUserByUsernameAsync(request.Email);
            if (existingUser is not null)
            {
                return null;
            }

            var user = await _userRepository.CreateUserAsync(request.Email, request.Password);
            var generatedCode = new CodeGenerator().Generate();
            var mailer = new Mailer(_configuration);
            mailer.SendLoginMail(request.Email, "Daryna", generatedCode);

            if (user is not null)
            {
                return await _userRepository.UpdateUserEmailLoginCodeAsync(user.Username, generatedCode, DateTime.UtcNow.AddMinutes(10));
            }
            return null;
        }

        public async Task<User?> CheckEmailCodeAsync(UserModel request, string code)
        {
            var user = await _userRepository.GetUserByUsernameAsync(request.Email);
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

        public async Task<string?> GenerateNewAccessToken(string username)
        {
            var user = await _userRepository.GetUserByUsernameAsync(username);
            if (user is null)
            {
                return null;
            }
            var tm = new TokenManager(_configuration);
            var accessToken = tm.GenerateJWTAccessToken(user: user);
            return accessToken;
        }

        public async Task<User?> ActivateUserAsync(string username)
        {
            return await _userRepository.ActivateUserByUsername(username);
        }
    }
}
