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

            if (Glfw.Glfw3.glfwInit() == 0)
            {
                throw new NotSupportedException();
            }
            PixelFarm.Forms.GlfwPlatform glfwPlatform = new PixelFarm.Forms.GlfwPlatform();

            //----------
            //we use gles API,
            //so we need to hint before create a window
            //(if we hint after create a window,it will use default GL,
            // GL swapBuffer != GLES'sEGL swapBuffer())
            //see https://www.khronos.org/registry/EGL/sdk/docs/man/html/eglSwapBuffers.xhtml
            //----------
            Glfw.Glfw3.glfwWindowHint(Glfw.Glfw3.GLFW_CLIENT_API, Glfw.Glfw3.GLFW_OPENGL_ES_API);
            Glfw.Glfw3.glfwWindowHint(Glfw.Glfw3.GLFW_CONTEXT_CREATION_API, Glfw.Glfw3.GLFW_EGL_CONTEXT_API);
            Glfw.Glfw3.glfwWindowHint(Glfw.Glfw3.GLFW_CONTEXT_VERSION_MAJOR, 3);
            Glfw.Glfw3.glfwWindowHint(Glfw.Glfw3.GLFW_CONTEXT_VERSION_MINOR, 1);
            Glfw.Glfw3.glfwSwapInterval(1);
#if DEBUG
            string versionStr3 = Marshal.PtrToStringAnsi(Glfw.Glfw3.glfwGetVersionString());
#endif
            ////----------
            Glfw.Glfw3.glfwGetMonitorWorkarea(Glfw.Glfw3.glfwGetPrimaryMonitor(), out int xpos, out int ypos, out int width, out int height);
            Mini.RootDemoPath.Path = @"..\Data";

            MyApp3.s_formW = width;
            MyApp3.s_formH = height;



            MyApp3.Start();

            PixelFarm.Forms.GlfwAppLoop.Run(); //main app loop
        }
    }
}
