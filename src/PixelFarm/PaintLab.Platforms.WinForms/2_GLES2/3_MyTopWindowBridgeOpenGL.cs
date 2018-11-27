//Apache2, 2014-present, WinterDev
#if GL_ENABLE 
using System;
using System.Windows.Forms;
using PixelFarm.Drawing;


namespace LayoutFarm.UI.OpenGL
{
    class MyTopWindowBridgeOpenGL : TopWindowBridgeWinForm
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
            throw new NotImplementedException();
        }
        public void SetCanvas(DrawBoard canvas)
        {
            this._openGLViewport.SetCanvas(canvas);
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
            this._windowControl = myGLControl;
            SetBaseCanvasViewport(this._openGLViewport = new OpenGLCanvasViewport(this.RootGfx, this._windowControl.Size.ToSize()));
            RootGfx.SetPaintDelegates(
                (r) =>
                {

                }, //still do nothing
                this.PaintToOutputWindow);
            _openGLViewport.NotifyWindowControlBinding();

#if DEBUG
            this._openGLViewport.dbugOutputWindow = this;
#endif
            this.EvaluateScrollbar();
        }
        protected override void OnClosing()
        {
            //make current before clear GL resource
            this._windowControl.MakeCurrent();
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
                var bounds = Screen.PrimaryScreen.Bounds;
                _windowControl.InitSetup2d(bounds.X, bounds.Y, bounds.Width, bounds.Height);
                _isInitGLControl = true;
                //2.                 
                _windowControl.ClearSurface(OpenTK.Graphics.Color4.White);
                //3.
            }
        }


#if DEBUG
        System.Diagnostics.Stopwatch dbugStopWatch = new System.Diagnostics.Stopwatch();
#endif
        public override void PaintToOutputWindow()
        {
            if (!_isInitGLControl)
            {
                return;
            }

            //var innumber = dbugCount;
            //dbugCount++;
            //Console.WriteLine(">" + innumber);

#if DEBUG
            //dbugStopWatch.Reset();
            //dbugStopWatch.Start();
#endif
            _windowControl.MakeCurrent();
            this._openGLViewport.PaintMe();
            _windowControl.SwapBuffers();
#if DEBUG
            //dbugStopWatch.Stop();
            //long millisec_per_frame = dbugStopWatch.ElapsedMilliseconds;
            //int fps = (int)(1000.0f / millisec_per_frame);
            //System.Diagnostics.Debug.WriteLine(fps); 
#endif
            //Console.WriteLine("<" + innumber); 
        }
        public override void CopyOutputPixelBuffer(int x, int y, int w, int h, IntPtr outputBuffer)
        {
            throw new NotImplementedException();
        }
        protected override void ChangeCursorStyle(MouseCursorStyle cursorStyle)
        {

        }
    }
}
#endif