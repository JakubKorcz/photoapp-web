using AutoMapper;
using PhotoApp.Client.Models;
using PhotoApp.Common.ModelsShared;

namespace PhotoApp.Client.Connection
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ProjectFormModel, ProjectBaseInformationDto>();
            CreateMap<UserModel, UserModelDto>();
        }
    }
}
