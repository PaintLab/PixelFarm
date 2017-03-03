//BSD, 2014-2017, WinterDev
using System;
using System.Windows.Forms;
using Mini;

namespace OpenTkEssTest
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            OpenTK.Toolkit.Init();
            DemoHelper.RegisterImageLoader(LoadImage);
            DemoHelper.RegisterFontProvider(new PixelFarm.Drawing.InstallFontsProviderWin32());
            Application.EnableVisualStyles();
            //----------------------------

            RootDemoPath.Path = @"..\Data";

            var formDev = new FormDev();
            Application.Run(formDev);
        }
        static PixelFarm.Agg.ActualImage LoadImage(string filename)
        {
            ImageTools.ExtendedImage extendedImg = new ImageTools.ExtendedImage();
            using (var fs = new System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                //TODO: review img loading, we should not use only its extension
                //
                string fileExt = System.IO.Path.GetExtension(filename).ToLower();
                switch (fileExt)
                {
                    case ".png":
                        {
                            var decoder = new ImageTools.IO.Png.PngDecoder();
                            extendedImg.Load(fs, decoder);
                        }
                        break;
                    case ".jpg":
                        {
                            var decoder = new ImageTools.IO.Jpeg.JpegDecoder();
                            extendedImg.Load(fs, decoder);
                        }
                        break;
                    default:
                        throw new System.NotSupportedException();

                }
                //var decoder = new ImageTools.IO.Png.PngDecoder();

            }
            //assume 32 bit 

            PixelFarm.Agg.ActualImage actualImg = PixelFarm.Agg.ActualImage.CreateFromBuffer(
                extendedImg.PixelWidth,
                extendedImg.PixelHeight,
                PixelFarm.Agg.PixelFormat.ARGB32,
                extendedImg.Pixels
                );
            //the imgtools load data as BigEndian
            actualImg.IsBigEndian = true;
            return actualImg;
        }
    }
}
