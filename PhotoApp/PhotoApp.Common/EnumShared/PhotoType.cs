using System;
using System.Collections.Generic;
using System.Text;

namespace PhotoApp.Common.EnumShared
{
    // Nazwa enumu: PhotoType to bezpośrednia nazwa bucketu w object storage
    public enum PhotoType
    {
        Original, // Oryginalny format zdjęcia
        Preview, // Pomniejszony format zdjęcia do wyświetlania w internecie
        Thumbnail // Zdjęcie bardzo małe 
    }
}
