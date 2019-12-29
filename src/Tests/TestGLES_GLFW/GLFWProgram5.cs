////MIT, 2016-present, WinterDev
//using System;
//using OpenTK.Graphics.ES20;
//using Pencil.Gaming;
//using PixelFarm;
//using PixelFarm.Forms;
//namespace TestGlfw
//{
//    class GLFWProgram5
//    {
//        public static void Start()
//        {
//            if (!GLFWPlatforms.Init())
//            {
//                Console.WriteLine("can't init");
//                return;
//            }


//            GlFwForm form1 = new GlFwForm(
//                800,
//                600,
//                "PixelFarm on GLfw and OpenGLES2");

//            //----------------
//            //this not need if we use glfwcontext for opentk
//            // new OpenTK.Graphics.ES20.GL().LoadEntryPoints();
//            //----------------

//            //var demo = new OpenTkEssTest.T52_HelloTriangle2();
//            //var demo = new OpenTkEssTest.T107_SampleDrawImage();
//            //var demo = new OpenTkEssTest.T107_SampleDrawImage();
//            var demoContext = new Mini.GLDemoContext(800, 600);
//            demoContext.LoadDemo(new OpenTkEssTest.T108_LionFill());
//            ////var demo = new OpenTkEssTest.T107_SampleDrawImage();

//            form1.SetDrawFrameDelegate(e =>
//            {
//                demoContext.Render();
//                //demo.Render();
//            });

//            //---------------------------------
//            GlFwForm f2 = new GlFwForm(800, 600, "Form2");
//            f2.SetDrawFrameDelegate(e =>
//            {
//                //simple draw
//                GL.ClearColor(0, 1, 1, 1);
//            });

//            GlfwApp.RunMainLoop();

//            demoContext.Close();
//        }
//    }
//}