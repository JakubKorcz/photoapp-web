using AutoMapper;
using PhotoApp.Common.ModelsShared;
using PhotoApp.Front.Models;

namespace PhotoApp.Front.Connection
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
