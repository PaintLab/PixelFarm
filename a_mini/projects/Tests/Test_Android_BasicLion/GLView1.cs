using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES20;
using OpenTK.Platform;
using OpenTK.Platform.Android;
using Android.Views;
using Android.Content;
using Android.Util;

namespace Test_Android_BasicLion
{
    class GLView1 : AndroidGameView
    {
        public GLView1(Context context) : base(context)
        {
        }
        int view_width;
        int view_height;
        // This gets called when the drawing surface is ready
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Android.Graphics.Point sc_size = new Android.Graphics.Point();
            Display.GetSize(sc_size);

            this.view_width = sc_size.X;
            this.view_height = sc_size.Y;
            SetupGL();

            // Run the render loop
            Run();
        }

        Mini.GLDemoContext demoContext;
        void SetupGL()
        {
            MakeCurrent();

            int ww_w = view_width;
            int ww_h = view_height;
            int max = Math.Max(ww_w, ww_h);
            demoContext = new Mini.GLDemoContext(max, max);
            demoContext.LoadDemo(new OpenTkEssTest.T108_LionFill());
            //--------------------------------------------------------------------------------
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.ClearColor(1, 1, 1, 1);
            //setup viewport size 

            //square viewport
            GL.Viewport(0, 0, max, max);

        }
        // This method is called everytime the context needs
        // to be recreated. Use it to set any egl-specific settings
        // prior to context creation
        //
        // In this particular case, we demonstrate how to set
        // the graphics mode and fallback in case the device doesn't
        // support the defaults
        protected override void CreateFrameBuffer()
        {
            //essential, from https://github.com/xamarin/monodroid-samples/blob/master/GLTriangle20-1.0/PaintingView.cs
            ContextRenderingApi = GLVersion.ES2;

            // the default GraphicsMode that is set consists of (16, 16, 0, 0, 2, false)
            try
            {
                Log.Verbose("GLTriangle", "Loading with default settings");

                // if you don't call this, the context won't be created
                base.CreateFrameBuffer();
                return;
            }
            catch (Exception ex)
            {
                Log.Verbose("GLTriangle", "{0}", ex);
            }

            // this is a graphics setting that sets everything to the lowest mode possible so
            // the device returns a reliable graphics setting.
            try
            {
                Log.Verbose("GLTriangle", "Loading with custom Android settings (low mode)");
                GraphicsMode = new AndroidGraphicsMode(0, 0, 0, 0, 0, false);

                // if you don't call this, the context won't be created
                base.CreateFrameBuffer();
                return;
            }
            catch (Exception ex)
            {
                Log.Verbose("GLTriangle", "{0}", ex);
            }
            throw new Exception("Can't load egl, aborting");
        }

        // This gets called on each frame render
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            //set clear color to white
            GL.ClearColor(1f, 1, 1, 1);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            demoContext.Render();

            SwapBuffers();
        }


    }
}
