using AutoMapper;
using PhotoApp.Common.ModelsShared;
using PhotoApp.Api.DbObjects;

namespace PhotoApp.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Project, ProjectBaseInformationDto>()
                .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.UserId)); 

            //CreateMap<DbObjects.ProjectFolder, FolderDto>();
            //CreateMap<DbObjects.ProjectWebDesign, DesignSettingsDto>();
            //CreateMap<DbObjects.Media, MediaDto>();
        }
    }
}
