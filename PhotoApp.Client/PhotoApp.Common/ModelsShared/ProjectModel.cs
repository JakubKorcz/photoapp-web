namespace PhotoApp.Client.Models
{
    public enum FontWeight
    {
        Light = 300,
        Regular = 400,
        Medium = 500,
        Bold = 700,
        Black = 900
    }

    public enum Layout
    {
        Left,
        Center,
        Right
    }

    public class Photo
    {
        public Guid Id { get; set; }
        public Guid ProjectDestination { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public bool IsLiked { get; set; }
    }

    public class Folder
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class WebDesignSettings
    {
        public Guid CoverPhoto { get; set; }
        public string FontFamily { get; set; }
        public int FontSize { get; set; }
        public FontWeight FontWeight { get; set; }
        public Layout FontLayout { get; set; }
    }

    public class ProjectSettings
    {

    }
    public class ProjectModel
    {
        public Guid Id { get; set; }
        public Guid Creator { get; set; }
        public string ProjectName { get; set; }
        public List<Photo> Photos { get; set; }
        public List<Folder> Folders { get; set; }
        public WebDesignSettings MobileDesignSettings { get; set; } = new WebDesignSettings();
        public WebDesignSettings DesktopDesignSettings { get; set; }
        public ProjectSettings ProjectSettings { get; set; } = new ProjectSettings();

        //Pomocniczy konstruktor do wstępnej konfiguracji
        public ProjectModel()
        {
            Id = new Guid();
            Creator = new Guid();
            ProjectName = "Test Name";
            Photos = new List<Photo>()
            {
                new Photo()
                {
                    Id = Guid.Empty,
                    ProjectDestination = new Guid(),
                    Url = "skunks.jpg",
                    Description = "SKUNKSIK",
                    IsLiked = false
                }
            };
            Folders = null;
            DesktopDesignSettings = new WebDesignSettings() { 
                CoverPhoto = Guid.Empty
            };

        }
    }
}
