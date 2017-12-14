//MIT, 2017, WinterDev

using System.IO;
using Typography.TextServices;

namespace YourImplementation
{
    public static class BootStrapWinGdi
    {
        static MyIcuDataProvider s_icuDataProvider;
        static IFontLoader myFontLoader;
        public static IFontLoader GetFontLoader()
        {
#if DEBUG
            if (myFontLoader == null)
            {
            }
#endif
            return myFontLoader;
        }
        public static void SetupDefaultValues()
        {
            myFontLoader = new OpenFontStore();
            //test Typography's custom text break, 
            //check if we have that data?

            //string typographyDir = @"../../PixelFarm/Typography/Typography.TextBreak/icu58/brkitr_src/dictionaries";
            string typographyDir = @"../../PixelFarm/Typography/Typography.TextBreak/icu60/brkitr_src/dictionaries";
            s_icuDataProvider = new MyIcuDataProvider();
            if (System.IO.Directory.Exists(typographyDir))
            {
                s_icuDataProvider.icuDir = typographyDir;
            }
            Typography.TextBreak.CustomBreakerBuilder.Setup(s_icuDataProvider);

            PixelFarm.Drawing.WinGdi.WinGdiPlusPlatform.SetFontLoader(YourImplementation.BootStrapWinGdi.myFontLoader);
        }

        class MyIcuDataProvider : Typography.TextBreak.IIcuDataProvider
        {
            public string icuDir;

            public Stream GetDataStream(string strmUrl)
            {
                string fullname = icuDir + "/" + strmUrl;
                if (File.Exists(fullname))
                {
                    return new FileStream(fullname, FileMode.Open);
                }
                return null;
            }
        }
    }
}