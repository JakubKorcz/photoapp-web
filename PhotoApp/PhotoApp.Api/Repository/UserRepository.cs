using PhotoApp.Api.DbObjects;
using System.Reflection.Metadata.Ecma335;

namespace PhotoApp.Api.Repository
{
    public class UserRepository(AppDbContext context)
    {
        //CREATE
        public async Task<User> CreateUserAsync(string username)
        {
            var user = new User { Username = username };
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

        //UPDATE
        public async Task<User?> UpdateUserAsync()
        {
            return null;
        }

        public async Task<User?> UpdateUserEmailLoginCodeAsync(string username, string genCode, DateTime dateExpire)
        {
            var user = await GetUserByUsernameAsync(username);
            if (user == null) return null;
            user.EmailLoginCode = genCode;
            user.EmailLoginCodeExpiration = dateExpire;
            context.Users.Update(user);
            await context.SaveChangesAsync();
            return user;
        }

        //DELETE
        public async Task DeleteUserByUsernameAsync(string username) { 
            var user = await GetUserByUsernameAsync(username) ?? throw new KeyNotFoundException("User not found."); ;
            context.Users.Remove(user);
            await context.SaveChangesAsync();
        }
    }
}
