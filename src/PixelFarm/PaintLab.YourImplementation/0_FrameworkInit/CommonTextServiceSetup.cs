//MIT, 2017-present, WinterDev

using System.IO;
using System.Collections.Generic;
using Typography.FontCollections;

namespace YourImplementation
{

    public static class CommonTextServiceSetup
    {
        static InstalledTypefaceCollection s_intalledTypefaces;
        public static IInstalledTypefaceProvider FontLoader => s_intalledTypefaces;
        public static void SetInstalledTypefaceCollection(InstalledTypefaceCollection installedTypeface)
        {
            s_intalledTypefaces = installedTypeface;
        }
        public static void AddCustomFolder(string customFontFolder)
        {
            s_intalledTypefaces.LoadFontsFromFolder(customFontFolder);
        }
    }
}