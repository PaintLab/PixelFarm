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


            Glfw.Glfw3.glfwGetMonitorWorkarea(Glfw.Glfw3.glfwGetPrimaryMonitor(), out int xpos, out int ypos, out int width, out int height);

            Mini.RootDemoPath.Path = @"..\Data";
            PixelFarm.Forms.GlfwPlatform glfwPlatform = new PixelFarm.Forms.GlfwPlatform();
            MyApp3.s_formW = width;
            MyApp3.s_formH = height;

            MyApp3.Start();
        }
    }
}
