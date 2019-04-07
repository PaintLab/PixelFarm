using System;
using System.Diagnostics;

using Foundation;
using GLKit;
using OpenGLES;
using OpenTK.Graphics.ES20;
using CustomApp01;
using CoreGraphics;
using UIKit;

namespace TestApp01.iOS
{
    [Register("GameViewController")]
    public class GameViewController : GLKViewController, IGLKViewDelegate
    {

        EAGLContext context { get; set; }
        //[Export("initWithCoder:")]
        public GameViewController()
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Code to start the Xamarin Test Cloud Agent
#if ENABLE_TEST_CLOUD
            //Xamarin.Calabash.Start();
#endif

            context = new EAGLContext(EAGLRenderingAPI.OpenGLES2);

            if (context == null)
            {
                Debug.WriteLine("Failed to create ES context");
            }

            //UIImage img = UIImage.FromBundle("test_img.png");
            //PixelFarm.CpuBlit.MemBitmap memBmp1 = CreateMemBitmap(img.CGImage);

            //SharedBmp._memBmp = memBmp1;

            var view = (GLKView)View;
            view.Context = context;
            view.DrawableDepthFormat = GLKViewDrawableDepthFormat.Format24;
            _view_width = (int)view.Frame.Width;
            _view_height = (int)view.Frame.Height;
            SetupGL();
        }
        PixelFarm.CpuBlit.MemBitmap CreateMemBitmap(CGImage cgImage)
        {
            int w = (int)cgImage.Width;
            int h = (int)cgImage.Height;

            PixelFarm.CpuBlit.MemBitmap memBmp = new PixelFarm.CpuBlit.MemBitmap(w, h);
            var ptr1 = PixelFarm.CpuBlit.MemBitmap.GetBufferPtr(memBmp);
            unsafe
            {
                PixelFarm.CpuBlit.NativeMemMx.MemCopy((byte*)ptr1.Ptr, (byte*)cgImage.Handle, w * h);
            }
            return memBmp;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            TearDownGL();

            if (EAGLContext.CurrentContext == context)
                EAGLContext.SetCurrentContext(null);
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();

            if (IsViewLoaded && View.Window == null)
            {
                View = null;

                TearDownGL();

                if (EAGLContext.CurrentContext == context)
                {
                    EAGLContext.SetCurrentContext(null);
                }
            }

            // Dispose of any resources that can be recreated.
        }

        public override bool PrefersStatusBarHidden()
        {
            return true;
        }


        CustomApp _customApp;
        int _max;
        int _view_width;
        int _view_height;
        void SetupGL()
        {

            EAGLContext.SetCurrentContext(context);
            _customApp = new CustomApp();
            _max = Math.Max(_view_width * 2, _view_height * 2);
            _customApp.Setup(_view_width * 2, _view_height * 2);
        }
        public override void Update()
        {
            GL.Viewport(0, 0, _max, _max);
            _customApp.RenderFrame();

        }
        //----------------
        void TearDownGL()
        {

        }

    }
}