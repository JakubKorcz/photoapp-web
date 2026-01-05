using PhotoApp.Client.Models;
using PhotoApp.Common.EnumShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoApp.Common.ModelsShared
{
    public class DesignSettingsDto
    {
        public Guid CoverPhoto { get; set; }
        public string FontFamily { get; set; }
        public int FontSize { get; set; }
        public FontWeight FontWeight { get; set; }
        public Layout FontLayout { get; set; }
    }
}
