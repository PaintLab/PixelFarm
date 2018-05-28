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
            //temp fix
            if (File.Exists(fullname))
            {
                return new FileStream(fullname, FileMode.Open);
            }

            if (PixelFarm.Platforms.StorageService.Provider.DataExists(fullname))
            {
                return PixelFarm.Platforms.StorageService.Provider.ReadDataStream(fullname);
            }
            return null;
        }
    }



    public static class CommonTextServiceSetup
    {
        static bool s_isInit;
        internal static MyIcuDataProvider s_icuDataProvider;
        internal static IFontLoader myFontLoader;


        static LocalFileStorageProvider s_localFileStorageProvider = new LocalFileStorageProvider();
        static FileDBStorageProvider s_filedb = new FileDBStorageProvider("textservicedb");

        public static void SetupDefaultValues()
        {
            //--------
            if (s_isInit) return;
            //--------
            PixelFarm.Platforms.StorageService.RegisterProvider(s_filedb);
            myFontLoader = new OpenFontStore();
            //test Typography's custom text break, 
            //check if we have that data? 

            //string typographyDir = @"/icu/brkitr_src/dictionaries";
            //***
            string typographyDir = @"d:/test/icu60/brkitr_src/dictionaries";
            s_icuDataProvider = new MyIcuDataProvider();
            if (System.IO.Directory.Exists(typographyDir))
            {
                s_icuDataProvider.icuDir = typographyDir;
            }
            Typography.TextBreak.CustomBreakerBuilder.Setup(s_icuDataProvider);
            s_isInit = true;
        }
    }


}