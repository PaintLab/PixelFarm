//MIT,  2016, WinterDev 
using System;
using System.Collections.Generic;

namespace PixelFarm.Drawing.Fonts
{
    public interface IInstalledFontProvider
    {
        IEnumerable<string> GetInstalledFontIter();
    }
     
}