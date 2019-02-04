//Apache2, 2014-present, WinterDev

using PixelFarm.Drawing;
using PixelFarm.Forms;
using LayoutFarm.UI.InputBridge;
using LayoutFarm.UI.WinNeutral; 

namespace LayoutFarm.UI.OpenGL
{

    class MyTopWindowBridgeOpenGL : TopWindowBridgeWinNeutral
    {

        bool _isInitGLControl;
        GpuOpenGLSurfaceView _windowControl;
        OpenGLCanvasViewport _openGLViewport;

        public MyTopWindowBridgeOpenGL(RootGraphic root, ITopWindowEventRoot topWinEventRoot)
            : base(root, topWinEventRoot)
        {

        }
        public override void PaintToOutputWindow(Rectangle invalidateArea)
        {
            PaintToOutputWindow();
        }
        public void SetCanvas(DrawBoard canvas)
        {
            _openGLViewport.SetCanvas(canvas);
        }
        public override void InvalidateRootArea(Rectangle r)
        {

        }
        public override void BindWindowControl(Control windowControl)
        {
            this.BindGLControl((GpuOpenGLSurfaceView)windowControl);
        }
        /// <summary>
        /// bind to gl control
        /// </summary>
        /// <param name="myGLControl"></param>
        void BindGLControl(GpuOpenGLSurfaceView myGLControl)
        {
            _windowControl = myGLControl;
            SetBaseCanvasViewport(_openGLViewport = new OpenGLCanvasViewport(this.RootGfx, new Size(_windowControl.Width, _windowControl.Height)));
            RootGfx.SetPaintDelegates(
                (r) =>
                {

                }, //still do nothing
                this.PaintToOutputWindow);
            _openGLViewport.NotifyWindowControlBinding();

#if DEBUG
            _openGLViewport.dbugOutputWindow = this;
#endif
            this.EvaluateScrollbar();
        }
        protected override void OnClosing()
        {
            //make current before clear GL resource
            _windowControl.MakeCurrent();
            if (_openGLViewport != null)
            {
                _openGLViewport.Close();
            }
            if (_windowControl != null)
            {
                _windowControl.Dispose();
            }
        }
        

        internal override void OnHostControlLoaded()
        {

            if (!_isInitGLControl)
            {
                //init gl after this control is loaded
                //set myGLControl detail
                //1.
                //TODO: review here again 
                _windowControl.InitSetup2d(0, 0, 800, 600);// Screen.PrimaryScreen.Bounds);
                _isInitGLControl = true;
                //2.
                //windowControl.ClearColor = new OpenTK.Graphics.Color4(1f, 1f, 1f, 1f);
                //3.

            }
        }
        protected override void ChangeCursor(MouseCursorStyle cursorStyle)
        {

        }
        protected override void ChangeCursor(ImageBinder imgbinder)
        {
            

        }
        public override void PaintToOutputWindow()
        {

            if (!_isInitGLControl)
            {
                return;
            }
            _windowControl.MakeCurrent();
            _openGLViewport.PaintMe();
            _windowControl.SwapBuffers();

            //Console.WriteLine("<" + innumber); 
        }


    }
}
