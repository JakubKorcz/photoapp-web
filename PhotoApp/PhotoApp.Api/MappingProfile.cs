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
            //Ignoruj kopiowania Id, bo to jest generowane przez bazę danych i nie powinno być nadpisywane podczas tworzenia nowego projektu.
            CreateMap<ProjectBaseInformationDto, Project>()
                .ForMember(p => p.Id, opt => opt.Ignore());

            CreateMap<Media, MediaDto>();
            CreateMap<MediaDto, Media>();

        }
    }
}
