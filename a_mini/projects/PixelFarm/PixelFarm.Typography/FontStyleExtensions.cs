//MIT, 2016-2017, WinterDev 
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Typography.OpenFont;
using PixelFarm.Drawing;
namespace Typography.FontManagement
{
    public static class FontStyleExtensions
    {
        public static InstalledFontStyle ConvToInstalledFontStyle(this FontStyle style)
        {
            InstalledFontStyle installedStyle = InstalledFontStyle.Regular;//regular
            switch (style)
            {
                default: break;
                case FontStyle.Bold:
                    installedStyle = InstalledFontStyle.Bold;
                    break;
                case FontStyle.Italic:
                    installedStyle = InstalledFontStyle.Italic;
                    break;
                case FontStyle.Bold | FontStyle.Italic:
                    installedStyle = InstalledFontStyle.Italic;
                    break;
            }
            return installedStyle;
        }
    }

}