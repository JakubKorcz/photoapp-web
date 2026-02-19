using PhotoApp.Common.ModelsShared;

namespace PhotoApp.Client.Models
{
    public class ProjectBaseInformationDto
    {
        public Guid Id { get; set; }
        public Guid Creator { get; set; }
        public string ProjectName { get; set; }
        public DateOnly CreatedAt { get; set; }
    }

    public class ProjectDto : ProjectBaseInformationDto
    {
        public List<MediaDto> Media { get; set; }
        public List<FolderDto> Folders { get; set; }
        public DesignSettingsDto MobileDesignSettings { get; set; } = new DesignSettingsDto();
        public DesignSettingsDto DesktopDesignSettings { get; set; }
        public ProjectSettingsDto ProjectSettings { get; set; } = new ProjectSettingsDto();

        //Pomocniczy konstruktor do wstępnej konfiguracji
        public ProjectDto()
        {
            Id = new Guid();
            Creator = new Guid();
            ProjectName = "Test Name";
            Media = new List<MediaDto>()
            {
                new MediaDto()
                {
                    Id = Guid.Empty,
                    ProjectDestination = new Guid(),
                    Url = "skunks.jpg",
                    Description = "SKUNKSIK",
                    IsLiked = false
                }
            };
            Folders = new List<FolderDto>() { 
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
            };
            DesktopDesignSettings = new DesignSettingsDto() { 
                CoverPhoto = Guid.Empty
            };
        }
    }
}
