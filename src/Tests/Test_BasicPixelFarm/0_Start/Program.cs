//Apache2, 2014-present, WinterDev

using System;
namespace TestGraphicPackage2
{
    static class Program
    {

        [STAThread]
        static void Main(string[] args)
        {
            
            PixelFarm.Platforms.StorageService.RegisterProvider(new YourImplementation.LocalFileStorageProvider(""));

            //2.2 Icu Text Break info
            //test Typography's custom text break,
            //check if we have that data?            
            //------------------------------------------- 
            //string typographyDir = @"brkitr_src/dictionaries";
             

            string icu_datadir = YourImplementation.RelativePathBuilder.SearchBackAndBuildFolderPath(System.IO.Directory.GetCurrentDirectory(), "PixelFarm", @"..\Typography\Typography.TextBreak\icu62\brkitr");
            if (!System.IO.Directory.Exists(icu_datadir))
            {
                throw new System.NotSupportedException("dic");
            }
            var dicProvider = new Typography.TextBreak.IcuSimpleTextFileDictionaryProvider() { DataDir = icu_datadir };
            Typography.TextBreak.CustomBreakerBuilder.Setup(dicProvider);
            YourImplementation.TestBedStartup.Setup();


            //-------------------------------------------
            YourImplementation.TestBedStartup.RunDemoList(typeof(Program));
        }
    }
}
