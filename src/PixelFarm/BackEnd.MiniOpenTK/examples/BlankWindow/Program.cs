using System;
using OpenTK;

namespace BlankWindow
{
    sealed class Program : GameWindow
    {
        [STAThread]
        static void Main()
        {

            OpenTK.Platform.Factory.GetCustomPlatformFactory = () => OpenTK.Platform.Egl.EglAngle.NewFactory();

            Toolkit.Init(new ToolkitOptions
            {
                Backend = PlatformBackend.PreferNative,

            });

            OpenTK.Graphics.PlatformAddressPortal.GetAddressDelegate = OpenTK.Platform.Utilities.CreateGetAddress();



            var program = new Program();
            program.Run();
        }
    }
}
