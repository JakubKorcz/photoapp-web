using PhotoApp.Common.EnumShared;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace PhotoApp.Api.DbObjects
{
    public class ProjectWebDesign
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Media? CoverPhoto { get; set; }
        public string? FontFamily { get; set; }
        public int? FontSize { get; set; }
        public FontWeight? FontWeight { get; set; }
        public Layout? FontLayout { get; set; }
        public List<Project_ProjectWebDesign> WebDesignAssignments { get; set; } = new();
    }
}
