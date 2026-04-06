using AutoMapper;
using PhotoApp.Common.ModelsShared;

namespace PhotoApp.Front.Connection
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ProjectFormModel, ProjectBaseInformationDto>();
        }
    }
}
