using PhotoApp.Api.DbObjects;

namespace PhotoApp.Api.Repository
{
    public class UserRepository(AppDbContext context)
    {
        public async Task<User> CreateUserAsync(string username)
        {
            var user = new User { Username = username };
            context.Users.Add(user);
            await context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return context.Users.SingleOrDefault(u => u.Username == username);
        }
    }
}
