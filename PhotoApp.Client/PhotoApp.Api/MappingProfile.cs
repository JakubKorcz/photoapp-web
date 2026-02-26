using AutoMapper;
using PhotoApp.Common.ModelsShared;
using PhotoApp.Api.DbObjects;

namespace PhotoApp.Api
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Project, ProjectBaseInformationDto>();
            //CreateMap<Project, ProjectDto>();
            CreateMap<Media, MediaDto>();
            CreateMap<MediaDto, Media>();

            //CreateMap<DbObjects.ProjectFolder, FolderDto>();
            //CreateMap<DbObjects.ProjectWebDesign, DesignSettingsDto>();
            //CreateMap<DbObjects.Media, MediaDto>();
        }
    }
}
