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



            ////----------
            Glfw.Glfw3.glfwGetMonitorWorkarea(Glfw.Glfw3.glfwGetPrimaryMonitor(), out int xpos, out int ypos, out int width, out int height);
            Mini.RootDemoPath.Path = @"..\Data";

            MyApp3.s_formW = width;
            MyApp3.s_formH = height;


            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            MyApp3.Start();


            sw.Stop();
            System.Diagnostics.Debug.WriteLine("load (ms):" + sw.ElapsedMilliseconds);

            PixelFarm.Forms.GlfwAppLoop.Run(); //main app loop
        }
    }
}
