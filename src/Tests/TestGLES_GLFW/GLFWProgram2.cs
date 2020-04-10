//MIT, 2016-present, WinterDev
using System;
using OpenTK.Graphics.ES20;
using PixelFarm;
using PixelFarm.Forms;
using System.Runtime.InteropServices;

using Glfw;
namespace TestGlfw
{
    class GLFWProgram2
    {

        public static void Start()
        {

            GlfwWindowEventListener winEventListener = new GlfwWindowEventListener();
            var form = new GlFwForm(800, 600, "hello!", winEventListener);
            form.MakeCurrent();

            //----------
            //(test) use gles2.1
            Glfw.Glfw3.glfwWindowHint(Glfw.Glfw3.GLFW_CLIENT_API, Glfw.Glfw3.GLFW_OPENGL_ES_API);
            Glfw.Glfw3.glfwWindowHint(Glfw.Glfw3.GLFW_CONTEXT_CREATION_API, Glfw.Glfw3.GLFW_EGL_CONTEXT_API);
            Glfw.Glfw3.glfwWindowHint(Glfw.Glfw3.GLFW_CONTEXT_VERSION_MAJOR, 2);
            Glfw.Glfw3.glfwWindowHint(Glfw.Glfw3.GLFW_CONTEXT_VERSION_MINOR, 1);
            Glfw.Glfw3.glfwSwapInterval(1);

            //----------


            string versionStr3 = Marshal.PtrToStringAnsi(Glfw3.glfwGetVersionString());


            OpenTK.Platform.Factory.GetCustomPlatformFactory = () => OpenTK.Platform.Egl.EglAngle.NewFactory();
            OpenTK.Toolkit.Init(new OpenTK.ToolkitOptions {
                Backend = OpenTK.PlatformBackend.PreferNative,
            });
            OpenTK.Graphics.PlatformAddressPortal.GetAddressDelegate = OpenTK.Platform.Utilities.CreateGetAddress();

            //-----------
            IntPtr currentContext = Glfw3.glfwGetCurrentContext();
            var contextHandler = new OpenTK.ContextHandle(currentContext);

            var glfwContext = new GLFWContextForOpenTK(contextHandler);
            var context = OpenTK.Graphics.GraphicsContext.CreateExternalContext(glfwContext);


            bool isCurrent = context.IsCurrent;
            var demoContext = new Mini.GLDemoContext(800, 600);
            demoContext.LoadDemo(new OpenTkEssTest.T108_LionFill());

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.ClearColor(1, 1, 1, 1);
            //--------------------------------------------------------------------------------
            //setup viewport size
            //set up canvas
            int ww_w = 800;
            int ww_h = 600;
            int max = Math.Max(ww_w, ww_h);
            GL.Viewport(0, 0, max, max);

            //---------
            form.RenderDel = () =>
            {
                demoContext.Render();                 
                form.SwapBuffers();
            };
            //---------

            GlfwAppLoop.Run();
        }
    }
}