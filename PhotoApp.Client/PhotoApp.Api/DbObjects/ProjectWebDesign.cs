using PhotoApp.Client.Models;
using PhotoApp.Common.EnumShared;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace PhotoApp.Api.DbObjects
{
    public enum Device
    {
        Mobile = 0,
        Desktop = 1
    }
    public class ProjectWebDesign
    {
        [Key]
        public Guid Id { get; set; }
        public Device Device { get; set; }
        public Media CoverPhoto { get; set; }
        public string FontFamily { get; set; }
        public int FontSize { get; set; }
        public FontWeight FontWeight { get; set; }
        public Layout FontLayout { get; set; }
        //FK
        public Project Project { get; set; }
    }
}
