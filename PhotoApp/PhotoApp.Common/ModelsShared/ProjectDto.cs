using PhotoApp.Common.ModelsShared;

namespace PhotoApp.Client.Models
{
    public class ProjectDto  
    {
        public ProjectBaseInformationDto ProjectBaseInformationDto {  get; set; }
        public FolderDto? MainFolder { get; set; }
        public DesignSettingsDto MobileDesignSettings { get; set; } = new DesignSettingsDto();
        public DesignSettingsDto DesktopDesignSettings { get; set; }
        public ProjectSettingsDto ProjectSettings { get; set; } = new ProjectSettingsDto();

        //Pomocniczy konstruktor do wstępnej konfiguracji
        public ProjectDto()
        {
            MainFolder = new FolderDto() { 
                Folders = new List<FolderDto>()
                {
                    new FolderDto()
                    {
                        Id = Guid.Empty,
                        Name = "Żaba"
                    },
                    new FolderDto()
                    {
                        Id = Guid.Empty,
                        Name = "Królik"
                    },
                    new FolderDto()
                    {
                        Id = Guid.Empty,
                        Name = "Koń"
                    }
                }
            };
            DesktopDesignSettings = new DesignSettingsDto() { 
                CoverPhoto = Guid.Empty
            };
        }
    }
}
