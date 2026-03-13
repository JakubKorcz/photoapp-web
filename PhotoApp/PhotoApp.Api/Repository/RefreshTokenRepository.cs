using PhotoApp.Api.DbObjects;

namespace PhotoApp.Api.Repository
{
    public class RefreshTokenRepository(AppDbContext context)
    {
        private readonly AppDbContext context = context;
        public async Task<RefreshToken> CreateRefreshTokenAsync(string username, string token)
        {
            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = token,
                Username = username,
                Created = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };
            context.RefreshTokens.Add(refreshToken);
            await context.SaveChangesAsync();
            return refreshToken;
        }

        public async Task<RefreshToken?> GetRefreshTokenByTokenAsync(string token)
        {
            var rt = context.RefreshTokens.SingleOrDefault(rt => rt.Token == token);
            return rt;
        }

        public async Task<bool> IsRefreshTokenActiveAsync(string token) 
        { 
            var tokenFromDb = await GetRefreshTokenByTokenAsync(token);
            if (tokenFromDb == null) 
            { 
                return false;
            }

            if (!tokenFromDb.IsActive) 
            { 
                return false;
            }

            return true;
        }  

        public async Task<bool> SetAllTokensForUserAsRevokedAsync(string username)
        {
            try
            {
                var tokens = context.RefreshTokens.Where(rt => rt.Username == username && rt.IsActive).ToList();
                foreach (var token in tokens)
                {
                    token.IsRevoked = true;
                }
                context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
