//BSD, 2014-present, WinterDev 

using System.Collections.Generic;
using Typography.FontManagement;
namespace PixelFarm.Drawing.Skia
{
    public static class SkiaGraphicsPlatform
    {
        static IInstalledTypefaceProvider s_installedTypefaceProvider;
        static Dictionary<InstalledTypeface, SkiaSharp.SKTypeface> skTypeFaces = new Dictionary<InstalledTypeface, SkiaSharp.SKTypeface>();


        public static void SetInstalledTypefaceProvider(IInstalledTypefaceProvider provider)
        {
            s_installedTypefaceProvider = provider;
        }
        internal static SkiaSharp.SKTypeface GetInstalledFont(string typefaceName)
        {

            InstalledTypeface installedFont = s_installedTypefaceProvider.GetInstalledTypeface(typefaceName, Typography.FontManagement.TypefaceStyle.Regular);
            if (installedFont == null)
            {
                return null;
            }
            else
            {
                SkiaSharp.SKTypeface loadedTypeFace;
                if (!skTypeFaces.TryGetValue(installedFont, out loadedTypeFace))
                {
                    loadedTypeFace = SkiaSharp.SKTypeface.FromFile(installedFont.FontPath);
                    skTypeFaces.Add(installedFont, loadedTypeFace);
                }
                return loadedTypeFace;
            }
        }
    }



}