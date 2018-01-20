//MIT, 2017-2018, WinterDev

using System.IO;
using Typography.TextServices;
namespace YourImplementation
{
    class MyIcuDataProvider : Typography.TextBreak.IIcuDataProvider
    {
        public string icuDir;

        public Stream GetDataStream(string strmUrl)
        {
            string fullname = icuDir + "/" + strmUrl;
            if (PixelFarm.Platforms.StorageService.Provider.DataExists(fullname))
            {
                return PixelFarm.Platforms.StorageService.Provider.ReadDataStream(fullname);
            }
            return null;
        }
    }



    static class CommonTextServiceSetup
    {
        static bool s_isInit;
        internal static MyIcuDataProvider s_icuDataProvider;
        internal static IFontLoader myFontLoader;


        internal static LocalFileStorageProvider s_localFileStorageProvider = new LocalFileStorageProvider();

        public static void SetupDefaultValues()
        {
            //--------
            if (s_isInit) return;
            //--------
            PixelFarm.Platforms.StorageService.RegisterProvider(s_localFileStorageProvider);
            myFontLoader = new OpenFontStore();
            //test Typography's custom text break, 
            //check if we have that data?

            //string typographyDir = @"../../PixelFarm/Typography/Typography.TextBreak/icu58/brkitr_src/dictionaries";
            string typographyDir = @"../../PixelFarm/Typography/Typography.TextBreak/icu60/brkitr_src/dictionaries";
            s_icuDataProvider = new MyIcuDataProvider(); 
            s_icuDataProvider.icuDir = typographyDir;

            Typography.TextBreak.CustomBreakerBuilder.Setup(s_icuDataProvider);
            s_isInit = true;
        }
    }


}