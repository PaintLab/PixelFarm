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

            
            string icu_datadir = YourImplementation.RelativePathBuilder.SearchBackAndBuildFolderPath(System.IO.Directory.GetCurrentDirectory(), "PixelFarm", @"..\Typography\Typography.TextBreak\icu62\brkitr");
            if (!System.IO.Directory.Exists(icu_datadir))
            {
                throw new System.NotSupportedException("dic");
            }
            var dicProvider = new Typography.TextBreak.IcuSimpleTextFileDictionaryProvider() { DataDir = icu_datadir };
            Typography.TextBreak.CustomBreakerBuilder.Setup(dicProvider);



            PixelFarm.CpuBlit.MemBitmapExtensions.DefaultMemBitmapIO = new PixelFarm.Drawing.WinGdi.GdiBitmapIO();
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