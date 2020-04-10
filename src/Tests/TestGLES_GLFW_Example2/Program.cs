//MIT, 2016-present, WinterDev
using System;
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

            if (Glfw.Glfw3.glfwInit() == 0)
            {
                throw new NotSupportedException();
            }

            Mini.RootDemoPath.Path = @"..\Data"; 
            PixelFarm.Forms.GlfwPlatform glfwPlatform = new PixelFarm.Forms.GlfwPlatform(); 
            MyApp3.Start();             
        }
    }
}
