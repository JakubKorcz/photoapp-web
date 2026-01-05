using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoApp.Common.ModelsShared
{
    public class MediaDto
    {
        public Guid Id { get; set; }
        public Guid ProjectDestination { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public bool IsLiked { get; set; }
    }
}
