using AutoMapper;
using PhotoApp.Common.ModelsShared;

namespace PhotoApp.Front.Client.Connection;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ProjectFormModel, ProjectBaseInformationDto>();
        CreateMap<UserModelDto, UserModelDto>();
    }
}
