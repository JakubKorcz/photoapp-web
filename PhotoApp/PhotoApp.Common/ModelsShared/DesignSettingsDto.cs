using PhotoApp.Common.EnumShared;

namespace PhotoApp.Common.ModelsShared
{
    public class DesignSettingsDto
    {
        public Guid? Id { get; init; }
        public Guid CoverPhoto { get; set; }
        public string FontFamily { get; set; }
        public int FontSize { get; set; }
        public FontWeight FontWeight { get; set; }
        public Layout FontLayout { get; set; }
    }
}
