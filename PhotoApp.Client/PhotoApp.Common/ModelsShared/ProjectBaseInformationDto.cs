using System;
using System.Collections.Generic;
using System.Text;

namespace PhotoApp.Common.ModelsShared
{
    public class ProjectBaseInformationDto
    {
        public Guid Id { get; set; }
        public Guid Creator { get; set; }
        public string ProjectName { get; set; }
        public DateTime CreatedAt { get; set; }

    }
}
