//MIT, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Mini
{
    static class Program
    {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
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

            //register image loader   
            //default text breaker, this bridge between     
            //PixelFarm.CpuBlit.MemBitmapExtensions.DefaultMemBitmapIO = new PixelFarm.Drawing.WinGdi.GdiBitmapIO();
            YourImplementation.LocalFileStorageProvider.s_globalBaseDir = Directory.GetCurrentDirectory();
            PixelFarm.CpuBlit.MemBitmapExtensions.DefaultMemBitmapIO = new YourImplementation.ImgCodecMemBitmapIO();


            //PixelFarm.CpuBlit.MemBitmap.LoadBitmap("test_img.png");

            YourImplementation.TestBedStartup.Setup();

            //---------------------------------------------------
            //register image loader 
            //---------------------------- 
            //app specfic
            RootDemoPath.Path = @"..\Data";//*** 
            //----------------------------
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormDev());
        }
    }
}