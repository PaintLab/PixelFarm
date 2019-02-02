//MIT, 2016-present, WinterDev
using System;
using OpenTK.Graphics.ES20;
using Pencil.Gaming;
using PixelFarm;
using PixelFarm.Forms;

namespace TestGlfw
{
    class GLFWProgram5
    {
        public static void Start()
        {
            if (!GLFWPlatforms.Init())
            {
                Console.WriteLine("can't init");
                return;
            }


            GlFwForm form1 = new GlFwForm(
                800,
                600,
                "PixelFarm on GLfw and OpenGLES2");

            //------------------------------------
            //***



            //------------------------------------
            form1.Activate();

            //----------------
            //this not need if we use glfwcontext for opentk
            // new OpenTK.Graphics.ES20.GL().LoadEntryPoints();
            //----------------

            //var demo = new OpenTkEssTest.T52_HelloTriangle2();
            //var demo = new OpenTkEssTest.T107_SampleDrawImage();
            //var demo = new OpenTkEssTest.T107_SampleDrawImage();
            var demoContext = new Mini.GLDemoContext(800, 600);
            demoContext.LoadDemo(new OpenTkEssTest.T108_LionFill());


            //var demo = new OpenTkEssTest.T107_SampleDrawImage();
            //demo.Width = 800;
            //demo.Height = 600;
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
            GL.Viewport(0, 0, max, max);
            //--------------------------------------------------------------------------------
            form1.SetDrawFrameDelegate(e =>
            {
                demoContext.Render();
                //demo.Render();
            });

            //---------------------------------
            GlFwForm f2 = new GlFwForm(800, 600, "Form2");
            //new form is the current form
            f2.Activate();

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.ClearColor(1, 1, 1, 1);
            //GL.Viewport(0, 0, 800, 600);
            GL.Viewport(0, 0, max, max);

            //---------------------------------
            f2.SetDrawFrameDelegate(e =>
            {
                //simple draw
                GL.ClearColor(0, 1, 1, 1);
            });


            while (!GlfwApp.ShouldClose())
            {
                //---------------
                //render phase and swap
                GlfwApp.UpdateWindowsFrame();
                /* Poll for and process events */
                Glfw.PollEvents();
            }
            demoContext.Close();
            Glfw.Terminate();
        }
    }
}