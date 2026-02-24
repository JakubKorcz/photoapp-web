using PhotoApp.Common.EnumShared;
using System.ComponentModel.DataAnnotations;

namespace PhotoApp.Api.DbObjects
{
    //Tabla pośrednicząca między projektem a web designem, ponieważ jeden projekt może mieć wiele web designów, a jeden web design może być przypisany do wielu projektów
    public class Project_ProjectWebDesign
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ProjectId { get; set; }
        public Project Project { get; set; } = null!;
        public Guid ProjectWebDesignId { get; set; }
        public ProjectWebDesign ProjectWebDesign { get; set; } = null!;
        public Device Device { get; set; }
    }
}
