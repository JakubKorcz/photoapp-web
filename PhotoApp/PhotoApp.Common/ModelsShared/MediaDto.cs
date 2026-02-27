using PhotoApp.Common.EnumShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoApp.Common.ModelsShared
{
    public class MediaDto
    {
        public Guid? Id { get; init; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Extension { get; set; }
        public PhotoType Type { get; set; }
        public string ObjectKey { get; set; }
        public bool IsLiked { get; set; }
        public long SizeBytes { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
