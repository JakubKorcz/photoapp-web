using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoApp.Common.ModelsShared
{
    public class FolderDto
    {
        public Guid? Id { get; init; }
        public string Name { get; set; } = string.Empty;
        public List<FolderDto> Folders { get; set; } = [];
        public List<MediaDto>? Medias { get; set; } = [];
    }
}
