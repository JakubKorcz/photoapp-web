using AutoMapper;
using PhotoApp.Api.Repository;

namespace PhotoApp.Api.Service
{
    public class TokenService(IMapper mapper, RefreshTokenRepository tokenRepository)
    {
        private readonly IMapper _mapper = mapper;
    }
}
