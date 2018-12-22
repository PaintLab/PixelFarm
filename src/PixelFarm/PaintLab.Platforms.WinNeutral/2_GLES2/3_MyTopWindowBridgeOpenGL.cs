//Apache2, 2014-present, WinterDev


using PixelFarm.Drawing;
using PixelFarm.Forms;
using LayoutFarm.UI.WinNeutral;
using PixelFarm.Drawing.GLES2;
using PixelFarm.DrawingGL;

namespace LayoutFarm.UI.OpenGL
{

    class MyTopWindowBridgeOpenGL : TopWindowBridgeWinNeutral
    {

        bool _isInitGLControl;
        OpenGLCanvasViewport _openGLViewport;
        UISurfaceViewportControl _windowControl;
        GLPainterContext _glsx;

        public MyTopWindowBridgeOpenGL(RootGraphic root, ITopWindowEventRoot topWinEventRoot)
            : base(root, topWinEventRoot)
        {

        }
        public void SetupCanvas(DrawBoard canvas)
        {

            _openGLViewport.SetCanvas(canvas);
        }
        void HandleGLPaint(object sender, System.EventArgs e)
        {
            _glsx.SmoothMode = SmoothMode.Smooth;
            _glsx.StrokeColor = PixelFarm.Drawing.Color.Black;
            _glsx.ClearColorBuffer();
            //example
            //canvasPainter.FillColor = PixelFarm.Drawing.Color.Black;
            //canvasPainter.FillRectLBWH(20, 20, 150, 150);
            ////load bmp image 
            ////------------------------------------------------------------------------- 
            //if (exampleBase != null)
            //{
            //    exampleBase.Draw(canvasPainter);
            //}
            //miniGLControl.SwapBuffers();
        }
        public override void BindWindowControl(Control windowControl)
        {
            _windowControl = (UISurfaceViewportControl)windowControl;
            SetBaseCanvasViewport(_openGLViewport = new OpenGLCanvasViewport(
                this.RootGfx,
                new Size(windowControl.Width, windowControl.Height)));

            RootGfx.SetPaintDelegates(
                (r) =>
                {

                }, //still do nothing
                this.PaintToOutputWindow);

            ////--------------------
            ////---------------------------------------
            //myGLControl.SetGLPaintHandler(HandleGLPaint);
            //hh1 = miniGLControl.Handle;
            //miniGLControl.MakeCurrent();
            //int max = Math.Max(this.Width, this.Height);
            //canvas2d = PixelFarm.Drawing.GLES2.GLES2Platform.CreateCanvasGL2d(max, max);
            //canvasPainter = new GLCanvasPainter(canvas2d, max, max);
            ////create text printer for opengl 
            ////----------------------
            ////1. win gdi based
            ////var printer = new WinGdiFontPrinter(canvas2d, w, h);
            ////canvasPainter.TextPrinter = printer;
            ////----------------------
            ////2. raw vxs
            ////var printer = new PixelFarm.Drawing.Fonts.VxsTextPrinter(canvasPainter);
            ////canvasPainter.TextPrinter = printer;
            ////----------------------
            ////3. agg texture based font texture
            ////var printer = new AggFontPrinter(canvasPainter, w, h);
            ////canvasPainter.TextPrinter = printer;
            ////----------------------
            ////4. texture atlas based font texture

            ////------------
            ////resolve request font


            //var printer = new GLBmpGlyphTextPrinter(canvasPainter, YourImplementation.BootStrapWinGdi.myFontLoader);
            //canvasPainter.TextPrinter = printer;



            ////--------------------
            //openGLViewport.NotifyWindowControlBinding();
#if DEBUG
            _openGLViewport.dbugOutputWindow = this;
#endif
            this.EvaluateScrollbar();
        }

        protected override void OnClosing()
        {
            //make current before clear GL resource
            _windowControl.MakeCurrent();
        }
        internal override void OnHostControlLoaded()
        {

            if (!_isInitGLControl)
            {
                //init gl after this control is loaded
                //set myGLControl detail
                //1.
                //TODO: review here again 
                _windowControl.InitSetup2d(new Rectangle(0, 0, 800, 600));// Screen.PrimaryScreen.Bounds);
                _isInitGLControl = true;
                //2.
                //windowControl.ClearColor = new OpenTK.Graphics.Color4(1f, 1f, 1f, 1f);
                //3.

            }
        }
        //public override void PaintToCanvas(Canvas canvas)
        //{
        //    throw new NotImplementedException();
        //}

        public override void PaintToOutputWindow()
        {
            //if (!isInitGLControl)
            //{
            //    return;
            //}
            //var innumber = dbugCount;
            //dbugCount++;
            //Console.WriteLine(">" + innumber);
            //TODO: review here

            _windowControl.MakeCurrent();
            _openGLViewport.PaintMe();
            _windowControl.SwapBuffers();

            //Console.WriteLine("<" + innumber); 
        }
        protected override void ChangeCursorStyle(MouseCursorStyle cursorStyle)
        {

        }

        public override void InvalidateRootArea(Rectangle r)
        {

        }
    }
}
