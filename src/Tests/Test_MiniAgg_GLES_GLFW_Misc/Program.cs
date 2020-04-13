//MIT, 2016-present, WinterDev
using System;
using System.Runtime.InteropServices;
namespace TestGlfw
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {


            PixelFarm.Forms.GlfwPlatform.Init();
            Mini.RootDemoPath.Path = @"..\Data";

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            MyApp3.Start();


            sw.Stop();
            System.Diagnostics.Debug.WriteLine("load (ms):" + sw.ElapsedMilliseconds);

            PixelFarm.Forms.GlfwAppLoop.Run(); //main app loop
        }
    }
}
