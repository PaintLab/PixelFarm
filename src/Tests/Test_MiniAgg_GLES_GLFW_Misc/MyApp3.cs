﻿//MIT, 2016-present, WinterDev
using System;
using PixelFarm.Forms;
using PixelFarm.Drawing;
using PixelFarm.CpuBlit;

using LayoutFarm;
using LayoutFarm.UI;
using PaintLab.Svg;

using LayoutFarm.CustomWidgets;
using PixelFarm.DrawingGL;
using System.IO;

namespace TestGlfw
{


#if DEBUG
    class dbugMyBoxUI : UIElement
    {
        RenderElement _renderElem;
        public dbugMyBoxUI()
        {
        }

        public void SetRenderElement(RenderElement renderE)
        {
            _renderElem = renderE;
        }

        public override RenderElement CurrentPrimaryRenderElement => _renderElem;

        public override RenderElement GetPrimaryRenderElement() => _renderElem;

        public override void InvalidateGraphics() => _renderElem.InvalidateGraphics();
        protected override void OnMouseMove(UIMouseMoveEventArgs e)
        {
            if (_isMouseDown)
            {
                //dragging

                _renderElem.SetLocation(_renderElem.X + e.XDiff, _renderElem.Y + e.YDiff);
            }

            base.OnMouseMove(e);
        }
        bool _isMouseDown;
        protected override void OnMouseDown(UIMouseDownEventArgs e)
        {
            _isMouseDown = true;
            base.OnMouseDown(e);
        }
        protected override void OnMouseUp(UIMouseUpEventArgs e)
        {
            _isMouseDown = false;
            base.OnMouseUp(e);
        }

    }
    class dbugMySprite : RenderElement
    {
        VgVisualElement _renderVx;
        public dbugMySprite(int w, int h) : base(w, h)
        {
            _renderVx = VgVisualDocHelper.CreateVgVisualDocFromFile(@"lion.svg").VgRootElem;
        }
        protected override void RenderClientContent(DrawBoard d, UpdateArea updateArea)
        {

            using (Tools.More.BorrowVgPaintArgs(d.GetPainter(), out var paintArgs))
            {
                _renderVx.Paint(paintArgs);
            }
            //d.FillRectangle(Color.Blue, 0, 0, 50, 50);
        }

    }
#endif
    static class MyApp3
    {


        static MyRootGraphic s_myRootGfx;
        static GraphicsViewRoot s_viewroot;
        static void Init(GlFwForm form)
        {
            //PART1:
            //1. storage io
            PixelFarm.Platforms.StorageService.RegisterProvider(new YourImplementation.LocalFileStorageProvider(""));

            //2. img-io implementation
            PixelFarm.CpuBlit.MemBitmapExt.DefaultMemBitmapIO = new YourImplementation.ImgCodecMemBitmapIO(); // new PixelFarm.Drawing.WinGdi.GdiBitmapIO();
            //PixelFarm.CpuBlit.MemBitmapExtensions.DefaultMemBitmapIO = new PixelFarm.Drawing.WinGdi.GdiBitmapIO();

            //------------------------------------------------------------------------
            // 
            //if we don't set this, it will error on read-write image
            //you can implement this with other lib that can read-write images

            var pars = new PixelFarm.Platforms.ImageIOSetupParameters();
            pars.SaveToPng = (IntPtr imgBuffer, int stride, int width, int height, string filename) =>
            {
                MemBitmap memBmp = new MemBitmap(width, height, imgBuffer);
                using (FileStream fs = new FileStream(filename, FileMode.Create))
                {
                    PixelFarm.CpuBlit.MemBitmapExt.DefaultMemBitmapIO.SaveImage(memBmp, fs,
                         MemBitmapIO.OutputImageFormat.Png,
                         null);
                }

                //---save with GDI+---
                //using (System.Drawing.Bitmap newBmp = new System.Drawing.Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                //{
                //    PixelFarm.CpuBlit.BitmapHelper.CopyToGdiPlusBitmapSameSize(imgBuffer, newBmp);
                //    //save
                //    newBmp.Save(filename);
                //}
            };
            pars.ReadFromMemStream = (System.IO.MemoryStream ms, string kind) =>
            {

                return PixelFarm.CpuBlit.MemBitmapExt.DefaultMemBitmapIO.LoadImage(ms);
                //read/guest img format                 

                //--- load img with GDI+---
                ////read                   
                //using (System.Drawing.Bitmap gdiBmp = new System.Drawing.Bitmap(ms))
                //{
                //    PixelFarm.CpuBlit.MemBitmap memBmp = new PixelFarm.CpuBlit.MemBitmap(gdiBmp.Width, gdiBmp.Height);
                //    //#if DEBUG
                //    //                        memBmp._dbugNote = "img;
                //    //#endif

                //    PixelFarm.CpuBlit.BitmapHelper.CopyFromGdiPlusBitmapSameSizeTo32BitsBuffer(gdiBmp, memBmp);
                //    return memBmp;
                //}

            };
            PixelFarm.Platforms.ImageIOPortal.Setup(pars);
            //------------------------------------------------------------------------


            //3. setup text-breaker
            string icu_datadir = "brkitr"; //see brkitr folder, we link data from Typography project and copy to output if newer
            if (!System.IO.Directory.Exists(icu_datadir))
            {
                throw new System.NotSupportedException("dic");
            }
            Typography.TextBreak.CustomBreakerBuilder.Setup(new Typography.TextBreak.IcuSimpleTextFileDictionaryProvider() { DataDir = icu_datadir });

            //---------------------------------------------------------------------------
            //4. Typography TextService           
            Typography.Text.OpenFontTextService textService = new Typography.Text.OpenFontTextService();
            textService.LoadFontsFromFolder("Fonts");
            Typography.Text.TextServiceClient serviceClient = textService.CreateNewServiceClient();
            GlobalTextService.TextService = serviceClient;
            //---------------------------------------------------------------------------

            //PART2: root graphics
            Size primScreenSize = UIPlatform.CurrentPlatform.GetPrimaryMonitorSize();
            s_myRootGfx = new MyRootGraphic(primScreenSize.Width, primScreenSize.Height);
            s_viewroot = new GraphicsViewRoot(primScreenSize.Width, primScreenSize.Height);
            MyGlfwTopWindowBridge bridge1 = new MyGlfwTopWindowBridge(s_myRootGfx, s_myRootGfx.TopWinEventPortal);
            ((MyGlfwTopWindowBridge.GlfwEventBridge)(form.WindowEventListener)).SetWindowBridge(bridge1);


            var glfwWindowWrapper = new GlfwWindowWrapper(form);
            bridge1.BindWindowControl(glfwWindowWrapper);

            s_viewroot.InitRootGraphics(s_myRootGfx,
                  s_myRootGfx.TopWinEventPortal,
                  InnerViewportKind.GLES,
                  glfwWindowWrapper,
                  bridge1);



            //------------------------------------------------------------------------
            //optional:
            if (s_viewroot.GetGLPainter() is GLPainter glPainter)
            {
                glPainter.SmoothingMode = SmoothingMode.AntiAlias;
            }



        }
#if DEBUG
        public static void dbugStart_Basic()
        {
            //demonstrate basic setup
            var bridge = new MyGlfwTopWindowBridge.GlfwEventBridge();
            Size primScreenSize = UIPlatform.CurrentPlatform.GetPrimaryMonitorSize();
            var form = new GlFwForm(primScreenSize.Width, primScreenSize.Height, "GLES_GLFW", bridge);
            Init(form);
            //------ 

            //this is an app detail
            Box bgBox = new Box(primScreenSize.Width, primScreenSize.Height);
            bgBox.BackColor = Color.White;
            s_myRootGfx.AddChild(bgBox.GetPrimaryRenderElement());

            //----------------------
            dbugMySprite sprite = new dbugMySprite(200, 300);
            dbugMyBoxUI boxUI = new dbugMyBoxUI();
            boxUI.SetRenderElement(sprite);
            sprite.SetController(boxUI);

            bgBox.Add(boxUI);
            //---------  
        }
#endif
        public static void Start()
        {
            var bridge = new MyGlfwTopWindowBridge.GlfwEventBridge();

            Size primScreenSize = UIPlatform.CurrentPlatform.GetPrimaryMonitorSize();
            var form = new GlFwForm(primScreenSize.Width, primScreenSize.Height, "GLES_GLFW", bridge);
            //
            Init(form);
            //------

            AppHost appHost = new AppHost();
            AppHostConfig config = new AppHostConfig();
            config.RootGfx = s_myRootGfx;
            config.ScreenW = primScreenSize.Width;
            config.ScreenH = primScreenSize.Height;
            appHost.Setup(config);
            //------
            Box bgBox = new Box(primScreenSize.Width, primScreenSize.Height);
            bgBox.BackColor = Color.White;
            s_myRootGfx.AddChild(bgBox.GetPrimaryRenderElement());
            //------ 


            //appHost.StartApp(new Demo_BoxEvents3());
            appHost.StartApp(new Demo_ScrollView());
            //appHost.StartApp(new Demo_MultipleLabels());
            //appHost.StartApp(new Demo_MultipleLabels2());
            //---------  

        }
    }
}