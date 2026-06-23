using Amazon.Runtime.Internal;
using Microsoft.AspNetCore.Identity;
using PhotoApp.Api.DbObjects;
using PhotoApp.Common.ModelsShared;
using System.Reflection.Metadata.Ecma335;

namespace PhotoApp.Api.Repository
{
    public class UserRepository(AppDbContext context)
    {
        private readonly AppDbContext context = context;
        //CREATE
        public async Task<User> CreateUserAsync(RegisterModelDto request)
        {
            var user = new User { Username = request.Username, Email = request.Email };
            var passwordHash = new PasswordHasher<User>().HashPassword(user, request.Password);
            user.PasswordHash = passwordHash;
            context.Users.Add(user);
            await context.SaveChangesAsync();
            return user;
        }

        //READ
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return context.Users.SingleOrDefault(u => u.Username == username);
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return context.Users.SingleOrDefault(u => u.Id == id);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return context.Users.SingleOrDefault(u => u.Email == email);
        }

        //UPDATE
        public async Task<User?> UpdateUserAsync()
        {
            return null;
        }

        public async Task<User> UpdateUserEmailLoginCodeAsync(string username, string genCode, DateTime dateExpire)
        {
            var user = await GetUserByUsernameAsync(username) ?? throw new Exception("Cannot ganarate number code for non existing user.");
            user.EmailLoginCode = genCode;
            user.EmailLoginCodeExpiration = dateExpire;
            context.Users.Update(user);
            await context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateUserPasswordAsync(string username, string newPassword)
        {
            var user = await GetUserByUsernameAsync(username) ?? throw new Exception("Cannot update password for non existing user.");
            var passwordHash = new PasswordHasher<User>().HashPassword(user, newPassword);
            user.PasswordHash = passwordHash;
            context.Users.Update(user);
            await context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> ActivateUserByUsername(string username)
        {
            var user = await GetUserByUsernameAsync(username) ?? throw new Exception("Cannot activate non existing user.");
            user.IsActive = true;
            context.Users.Update(user);
            await context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> RegisterFailedLoginCodeAttemptAsync(string username)
        {
            var user = await GetUserByUsernameAsync(username);
            if (user is null) return null;
            user.FailedLoginCodeAttempts++;
            if (user.FailedLoginCodeAttempts >= MaxFailedLoginCodeAttempts)
            {
                user.LoginCodeLockoutUntil = DateTime.UtcNow.AddMinutes(LoginCodeLockoutMinutes);
            }
            context.Users.Update(user);
            await context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> ResetLoginCodeAttemptsAsync(string username)
        {
            var user = await GetUserByUsernameAsync(username);
            if (user is null) return null;
            user.FailedLoginCodeAttempts = 0;
            user.LoginCodeLockoutUntil = null;
            context.Users.Update(user);
            await context.SaveChangesAsync();
            return user;
        }

        public const int MaxFailedLoginCodeAttempts = 15;
        public const int LoginCodeLockoutMinutes = 15;

        //DELETE
        public async Task DeleteUserByUsernameAsync(string username) { 
            var user = await GetUserByUsernameAsync(username) ?? throw new Exception("Cannot delete non existing user.");
            context.Users.Remove(user);
            await context.SaveChangesAsync();
        }
    }
}
