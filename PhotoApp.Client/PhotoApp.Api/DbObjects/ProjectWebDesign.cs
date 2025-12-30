using PhotoApp.Client.Models;
using System.Drawing;

namespace PhotoApp.Api.DbObjects
{
    public enum Device
    {
        Mobile,
        Desktop
    }
    public class ProjectWebDesign
    {
        public string Id { get; set; }
        public Device Device { get; set; }
        public Photo CoverPhoto { get; set; }
        public string FontFamily { get; set; }
        public int FontSize { get; set; }
        public FontWeight FontWeight { get; set; }
        public Layout FontLayout { get; set; }
    }
}
