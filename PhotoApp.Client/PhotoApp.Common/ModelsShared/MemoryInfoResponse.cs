using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoApp.Common.ModelsShared
{
    public class MemoryInfoResponse
    {
        public long TotalMemoryInBytes { get; set; }
        public long UsedMemoryInBytes { get; set; }
        public long FreeMemoryInBytes { get; set; }
    }
}
