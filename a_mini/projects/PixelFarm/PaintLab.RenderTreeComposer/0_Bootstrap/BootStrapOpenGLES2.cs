//MIT, 2017, WinterDev


namespace YourImplementation
{

#if GL_ENABLE
    public static class BootStrapOpenGLES2
    {
        static bool s_initInit;
        public static void SetupDefaultValues()
        {
            //init once
            if (s_initInit) return;
            //----
            //
            s_initInit = true;
            //
            //
#if DEBUG
            PixelFarm.Agg.ActualImage.InstallImageSaveToFileService((PixelFarm.Agg.ActualImage img, string filename) =>
            {

                using (System.Drawing.Bitmap newBmp = new System.Drawing.Bitmap(img.Width, img.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                {
                    PixelFarm.Agg.Imaging.BitmapHelper.CopyToGdiPlusBitmapSameSize(img, newBmp);
                    //save
                    newBmp.Save(filename);
                }
            });
#endif
             
            //
            OpenTK.Toolkit.Init();
            //use common font loader
            //user can create and use other font-loader
            CommonTextServiceSetup.SetupDefaultValues();
            PixelFarm.Drawing.GLES2.GLES2Platform.SetFontLoader(CommonTextServiceSetup.myFontLoader);
        }
    }
#endif

}