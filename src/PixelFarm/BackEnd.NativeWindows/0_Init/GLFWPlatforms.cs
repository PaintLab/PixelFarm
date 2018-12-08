//MIT, 2016-present, WinterDev
using System;
using Pencil.Gaming;

namespace PixelFarm
{
    public static class GLFWPlatforms
    {

        public static bool Init()
        {

            if (!Glfw.Init())
            {
                return false;
            }


            //---------------------------------------------------
            //specific OpenGLES ***
            Glfw.WindowHint(WindowHint.GLFW_CLIENT_API, (int)OpenGLAPI.OpenGLESAPI);
            Glfw.WindowHint(WindowHint.GLFW_CONTEXT_CREATION_API, (int)OpenGLContextCreationAPI.GLFW_EGL_CONTEXT_API);
            Glfw.WindowHint(WindowHint.GLFW_CONTEXT_VERSION_MAJOR, 3);
            Glfw.WindowHint(WindowHint.GLFW_CONTEXT_VERSION_MINOR, 0);
            //---------------------------------------------------

            return true;
        }
        static GLFWContextForOpenTK s_glfwContextForOpenTK;
        static OpenTK.ContextHandle s_glContextHandler;
        static OpenTK.Graphics.GraphicsContext s_externalContext;
        public static void CreateGLESContext()
        {
            //make open gl es current context 
            GlfwWindowPtr currentContext = Glfw.GetCurrentContext();
            s_glContextHandler = new OpenTK.ContextHandle(currentContext.inner_ptr);

            s_glfwContextForOpenTK = new GLFWContextForOpenTK(s_glContextHandler);
            s_externalContext = OpenTK.Graphics.GraphicsContext.CreateExternalContext(s_glfwContextForOpenTK);

            //glfwContext = OpenTK.Graphics.GraphicsContext.CreateDummyContext(contextHandler);
            bool isCurrent = s_glfwContextForOpenTK.IsCurrent;
        }
        public static void MakeCurrentWindow(GlfwWinInfo windowInfo)
        {
            s_glfwContextForOpenTK.MakeCurrent(windowInfo);
        }
    }
    public class GlfwWinInfo : OpenTK.Platform.IWindowInfo
    {
        GlfwWindowPtr _glfwWindowPtr;
        IntPtr _glfwHandle;

        public GlfwWinInfo(GlfwWindowPtr glfwWindowPtr)
        {
            _glfwWindowPtr = glfwWindowPtr;
            _glfwHandle = glfwWindowPtr.inner_ptr;
        }
        public void Dispose()
        {
        }
        //
        internal GlfwWindowPtr GlfwWindowPtr => _glfwWindowPtr;
        //
        public IntPtr Handle => _glfwWindowPtr.inner_ptr;
        //
    }
}
