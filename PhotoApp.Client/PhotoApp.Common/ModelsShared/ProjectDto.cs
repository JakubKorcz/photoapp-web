using PhotoApp.Common.ModelsShared;

namespace PhotoApp.Client.Models
{
    public class ProjectDto : ProjectBaseInformationDto
    {
        public FolderDto MainFolder { get; set; } = new FolderDto();
        public DesignSettingsDto MobileDesignSettings { get; set; } = new DesignSettingsDto();
        public DesignSettingsDto DesktopDesignSettings { get; set; }
        public ProjectSettingsDto ProjectSettings { get; set; } = new ProjectSettingsDto();

        //Pomocniczy konstruktor do wstępnej konfiguracji
        public ProjectDto()
        {
            Id = new Guid();
            Creator = new Guid();
            ProjectName = "Test Name";
            MainFolder = new FolderDto() { 
                Name = "home",
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
