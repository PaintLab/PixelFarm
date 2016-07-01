using System;
using OpenTK.Graphics.ES20;
using Pencil.Gaming;
using PixelFarm;

namespace TestGlfw
{
    class GLFWProgram2
    {

        public static void Start()
        {

            if (!GLPlatforms.Init())
            {
                Console.WriteLine("can't init");
            }

            GlfwMonitorPtr monitor = new GlfwMonitorPtr();
            GlfwWindowPtr winPtr = new GlfwWindowPtr();
            GlfwWindowPtr glWindow = Glfw.CreateWindow(800, 600, "Test Glfw", monitor, winPtr);

            /* Make the window's context current */
            Glfw.MakeContextCurrent(glWindow);
            Glfw.SwapInterval(1);

            GlfwWindowPtr currentContext = Glfw.GetCurrentContext();

            var contextHandler = new OpenTK.ContextHandle(currentContext.inner_ptr);
            var context = OpenTK.Graphics.GraphicsContext.CreateDummyContext(contextHandler);
            bool isCurrent = context.IsCurrent;
            PixelFarm.GlfwWinInfo winInfo = new PixelFarm.GlfwWinInfo(glWindow.inner_ptr);
            context.MakeCurrent(winInfo);
            //-------------------------------------- 
            //bind open gl funcs here..
            new OpenTK.Graphics.ES20.GL().LoadEntryPoints();
            //-------------------------------------- 
            OpenTkEssTest.T52_HelloTriangle2 t52 = new OpenTkEssTest.T52_HelloTriangle2();
            t52.Width = 800;
            t52.Height = 600;

            //--------------------------------------------------------------------------------    
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.ClearColor(1, 1, 1, 1);
            //--------------------------------------------------------------------------------
            //setup viewport size
            //set up canvas
            int ww_w = 800;
            int ww_h = 600;
            int max = Math.Max(ww_w, ww_h);
            GL.Viewport(0, 0, 800, 600);

            t52.InitGLProgram();

            while (!Glfw.WindowShouldClose(glWindow))
            {
                t52.Render();
                /* Render here */
                /* Swap front and back buffers */
                Glfw.SwapBuffers(glWindow);
                /* Poll for and process events */
                Glfw.PollEvents();
            }
            t52.Close();
            Glfw.Terminate();
        }
    }
}