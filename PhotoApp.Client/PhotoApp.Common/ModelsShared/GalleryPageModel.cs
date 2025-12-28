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
        
    }
    public class Photo
    {
        public Guid Id { get; set; }
        public Guid Destination { get; set; }
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
        public Photo CoverPhoto { get; set; }
        public string FontFamily { get; set; }
        public int FontSize { get; set; }
        public FontWeight FontWeight { get; set; }
        public Layout FontLayout { get; set; }
    }

    public class ProjectSettings
    {

    }
    public class GalleryPageModel
    {
        public Guid Id { get; set; }
        public string ProjectName { get; set; }
        public List<Photo> Photos { get; set; }
        public List<Folder> Folders { get; set; }
        public WebDesignSettings DesignSettings { get; set; }
        public ProjectSettings ProjectSettings { get; set; }
    }
}
