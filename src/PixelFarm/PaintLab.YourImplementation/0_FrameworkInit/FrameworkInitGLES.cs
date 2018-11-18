//MIT, 2017-present, WinterDev


namespace YourImplementation
{


#if GL_ENABLE
    public static class FrameworkInitGLES
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
            OpenTK.Platform.Factory.GetCustomPlatformFactory = () => OpenTK.Platform.Egl.EglAngle.NewFactory();
            OpenTK.Toolkit.Init(new OpenTK.ToolkitOptions
            {
                Backend = OpenTK.PlatformBackend.PreferNative,
            });
            OpenTK.Graphics.PlatformAddressPortal.GetAddressDelegate = OpenTK.Platform.Utilities.CreateGetAddress();
            //use common font loader
            //user can create and use other font-loader
            //CommonTextServiceSetup.SetupDefaultValues();
            PixelFarm.Drawing.GLES2.GLES2Platform.SetInstalledTypefaceProvider(CommonTextServiceSetup.FontLoader);
        }
    }
#endif

}