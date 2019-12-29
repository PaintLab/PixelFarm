//MIT, 2016-present, WinterDev
using System;
using Pencil.Gaming;
using Glfw;

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
            Glfw.WindowHint(WindowHint.GLFW_CONTEXT_VERSION_MAJOR, 2);
            Glfw.WindowHint(WindowHint.GLFW_CONTEXT_VERSION_MINOR, 0);
            //---------------------------------------------------

            Glfw.SwapInterval(1);
            //------------


            OpenTK.Platform.Factory.GetCustomPlatformFactory = () => OpenTK.Platform.Egl.EglAngle.NewFactory();
            OpenTK.Toolkit.Init(new OpenTK.ToolkitOptions
            {
                Backend = OpenTK.PlatformBackend.PreferNative,
            });
            OpenTK.Graphics.PlatformAddressPortal.GetAddressDelegate = OpenTK.Platform.Utilities.CreateGetAddress();

            return true;
        }
      
        internal static void CreateGLESContext(PixelFarm.Forms.GlFwForm glFwForm)
        {
            //make open gl es current context 
            //GlfwWindowPtr currentContext = Glfw.GetCurrentContext(); 
            var s_glContextHandler = new OpenTK.ContextHandle(glFwForm.GlfwWindowPtr.inner_ptr);
            glFwForm.GlfwContextForOpenTK = new GLFWContextForOpenTK(s_glContextHandler);
            glFwForm.OpenTKGraphicContext = OpenTK.Graphics.GraphicsContext.CreateExternalContext(glFwForm.GlfwContextForOpenTK);

            //glfwContext = OpenTK.Graphics.GraphicsContext.CreateDummyContext(contextHandler);
#if DEBUG
            bool isCurrent = glFwForm.GlfwContextForOpenTK.IsCurrent;
#endif
            
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
