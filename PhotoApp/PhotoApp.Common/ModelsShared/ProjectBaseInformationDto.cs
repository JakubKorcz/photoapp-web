using PhotoApp.Common.EnumShared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PhotoApp.Common.ModelsShared
{
    public class ProjectBaseInformationDto
    {
        public Guid? Id { get; init; }
        public string Username { get; set; } 
        public string ProjectName { get; set; }
        public DateTime? CreatedAt { get; set; }

        public DateOnly? PhotoShootDate {get; set;}
        public bool IsPernament { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public Language Language { get; set; }
        public string? Password { get; set; }

    }
}
