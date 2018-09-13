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
            //PaintLab.Svg.SvgParser parser = new PaintLab.Svg.SvgParser();
            //string svgContent = System.IO.File.ReadAllText("Samples/arrow2.svg");
            //parser.ParseDocument(new LayoutFarm.WebLexer.TextSnapshot(svgContent));




            RootDemoPath.Path = @"..\Data";
            YourImplementation.TestBedStartup.Setup();

#if GL_ENABLE
            YourImplementation.BootStrapOpenGLES2.SetupDefaultValues();
#endif
            //you can use your font loader
            YourImplementation.BootStrapWinGdi.SetupDefaultValues();
            //default text breaker, this bridge between              
#if DEBUG
            //PixelFarm.Agg.ActualImage.InstallImageSaveToFileService((IntPtr imgBuffer, int stride, int width, int height, string filename) =>
            //{

            //    using (System.Drawing.Bitmap newBmp = new System.Drawing.Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            //    {
            //        PixelFarm.Agg.Imaging.BitmapHelper.CopyToGdiPlusBitmapSameSize(imgBuffer, newBmp);
            //        //save
            //        newBmp.Save(filename);
            //    }
            //});
#endif

            //Typography's TextServices
            //you can implement   Typography.TextBreak.DictionaryProvider  by your own
            //this set some essentail values for Typography Text Serice
            // 
            //2.2 Icu Text Break info
            //test Typography's custom text break,
            //check if we have that data?            
            //------------------------------------------- 
            //string typographyDir = @"brkitr_src/dictionaries";
            string icu_datadir = @"D:\projects\Typography\Typography.TextBreak\icu62\brkitr";

            if (!System.IO.Directory.Exists(icu_datadir))
            {
                throw new System.NotSupportedException("dic");
            }
            var dicProvider = new IcuSimpleTextFileDictionaryProvider() { DataDir = icu_datadir };
            Typography.TextBreak.CustomBreakerBuilder.Setup(dicProvider);

            //---------------------------------------------------
            //register image loader
            Mini.DemoHelper.RegisterImageLoader(LoadImage);
            //----------------------------

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormDev());
        }
        static PixelFarm.CpuBlit.ActualBitmap LoadImage(string filename)
        {


            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(filename);


            var bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                                       System.Drawing.Imaging.ImageLockMode.ReadOnly,
                                       System.Drawing.Imaging.PixelFormat.Format32bppArgb //lock and read as 32-argb
                                       );

            int[] imgBuffer = new int[bmpData.Width * bmp.Height];
            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, imgBuffer, 0, imgBuffer.Length);
            bmp.UnlockBits(bmpData);

            //PixelFarm.Agg.PixelFormat selectedFormat = PixelFarm.Agg.PixelFormat.ARGB32;
            //switch (bmp.PixelFormat)
            //{
            //    default:
            //        throw new NotSupportedException();
            //    //case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
            //    //    {
            //    //        bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
            //    //             System.Drawing.Imaging.ImageLockMode.ReadOnly,
            //    //             System.Drawing.Imaging.PixelFormat.Format32bppArgb //lock and read as 32-argb 
            //    //             );
            //    //        selectedFormat = PixelFarm.Agg.PixelFormat.ARGB32; //lock and read as 32-argb
            //    //        imgBuffer = new byte[bmpData.Stride * bmp.Height];
            //    //        System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, imgBuffer, 0, imgBuffer.Length);
            //    //        bmp.UnlockBits(bmpData);
            //    //    }
            //    //    break;
            //    case System.Drawing.Imaging.PixelFormat.Format32bppRgb:
            //    case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
            //        {
            //            selectedFormat = PixelFarm.Agg.PixelFormat.ARGB32;
            //            bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
            //                System.Drawing.Imaging.ImageLockMode.ReadOnly,
            //                bmp.PixelFormat //lock and read as 32-argb
            //                );

            //            imgBuffer = new int[bmpData.Width * bmp.Height];
            //            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, imgBuffer, 0, imgBuffer.Length);
            //            bmp.UnlockBits(bmpData);
            //        }
            //        break;
            //    case System.Drawing.Imaging.PixelFormat.Format8bppIndexed:
            //        //grey scale
            //        //selectedFormat = PixelFarm.Agg.PixelFormat.GrayScale8;
            //        throw new NotSupportedException();
            //}

            PixelFarm.CpuBlit.ActualBitmap actualImg = PixelFarm.CpuBlit.ActualBitmap.CreateFromBuffer(
                bmp.Width,
                bmp.Height,
                imgBuffer
                );
            //gdi+ load as little endian             
            actualImg.IsBigEndian = false;
            bmp.Dispose();
            return actualImg;
        }



        class IcuSimpleTextFileDictionaryProvider : Typography.TextBreak.DictionaryProvider
        {
            //read from original ICU's dictionary
            //.. 
            public string DataDir
            {
                get;
                set;
            }
            public override IEnumerable<string> GetSortedUniqueWordList(string dicName)
            {
                //user can provide their own data 
                //....

                switch (dicName)
                {
                    default:
                        return null;
                    case "thai":
                        return GetTextListIterFromTextFile(DataDir + "/dictionaries/thaidict.txt");
                    case "lao":
                        return GetTextListIterFromTextFile(DataDir + "/dictionaries/laodict.txt");
                }

            }
            static IEnumerable<string> GetTextListIterFromTextFile(string filename)
            {
                //read from original ICU's dictionary
                //..

                using (FileStream fs = new FileStream(filename, FileMode.Open))
                using (StreamReader reader = new StreamReader(fs))
                {
                    string line = reader.ReadLine();
                    while (line != null)
                    {
                        line = line.Trim();
                        if (line.Length > 0 && (line[0] != '#')) //not a comment
                        {
                            yield return line.Trim();
                        }
                        line = reader.ReadLine();//next line
                    }
                }
            }
        }


    }
}
