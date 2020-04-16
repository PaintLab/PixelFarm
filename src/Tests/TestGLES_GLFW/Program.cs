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


            Mini.RootDemoPath.Path = @"..\Data";
            GLFWProgram2.Start();
        }
    }
}
