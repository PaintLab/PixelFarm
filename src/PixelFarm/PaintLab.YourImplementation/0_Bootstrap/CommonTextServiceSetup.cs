//MIT, 2017-present, WinterDev

using System.IO;
using Typography.TextServices;
namespace YourImplementation
{
    class MyIcuDataProvider
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
        static MyIcuDataProvider s_icuDataProvider;
        static IFontLoader myFontLoader;


        static LocalFileStorageProvider s_localFileStorageProvider = new LocalFileStorageProvider();
        static FileDBStorageProvider s_filedb;

        public static IFontLoader FontLoader
        {
            get
            {
                return myFontLoader;
            }
        }
        public static void SetupDefaultValues()
        {
            //--------
            //This is optional if you don't use Typography Text Service.            
            //--------
           
            if (s_isInit)
                return;
            //--------

            //--------
            //1. Storage provider
            // choose local file or filedb 
            // if we choose filedb => then this will create/open a 'disk' file for read/write data
            s_filedb = new FileDBStorageProvider("textservicedb");
            // then register to the storage service
            PixelFarm.Platforms.StorageService.RegisterProvider(s_filedb);

            //--------
            //2. Typography's Text Service settings...
            //this set some essentail values for Typography Text Serice
            //
            //2.1 Font Loader: how to load a font
            myFontLoader = new OpenFontStore();
            //2.2 Icu Text Break info
            //test Typography's custom text break,
            //check if we have that data?             
            string typographyDir = @"d:/test/icu60/brkitr_src/dictionaries";
            s_icuDataProvider = new MyIcuDataProvider();
            if (System.IO.Directory.Exists(typographyDir))
            {
                s_icuDataProvider.icuDir = typographyDir;
            }
            Typography.TextBreak.CustomBreakerBuilder.Setup(typographyDir);
            s_isInit = true;
        }
    }


}