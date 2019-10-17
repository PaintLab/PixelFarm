//Apache2, 2014-present, WinterDev

using System;
using System.Windows.Forms;

using PixelFarm.Drawing;
using LayoutFarm.UI.InputBridge;
namespace LayoutFarm.UI.OpenGL
{

    public class MyTopWindowBridgeOpenGL : TopWindowBridgeWinForm
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
            SetBaseCanvasViewport(_openGLViewport = new OpenGLCanvasViewport(this.RootGfx, _windowControl.Size.ToSize()));
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
        public override void OnHostControlLoaded()
        {
            //TODO: review this
            if (!_isInitGLControl)
            {
                //init gl after this control is loaded
                //set myGLControl detail
                //1.
                var bounds = Screen.PrimaryScreen.Bounds;

                _isInitGLControl = true;
                //2.    

                _windowControl.MakeCurrent();
                OpenTK.Graphics.ES20.GL.ClearColor(OpenTK.Graphics.Color4.White);

                //3.
            }
        }
        protected override void ChangeCursor(ImageBinder imgbinder)
        {
            //use custom cursor 
            //if not support then just ignore
            return;
        }
        protected override void ChangeCursor(MouseCursorStyle cursorStyle)
        {
            switch (cursorStyle)
            {
                case MouseCursorStyle.Pointer:
                    {
                        _windowControl.Cursor = Cursors.Hand;
                    }
                    break;
                case MouseCursorStyle.IBeam:
                    {
                        _windowControl.Cursor = Cursors.IBeam;
                    }
                    break;
                default:
                    {
                        _windowControl.Cursor = Cursors.Default;
                    }
                    break;
            }
        }

        //#if DEBUG
        System.Diagnostics.Stopwatch _stopWatch = new System.Diagnostics.Stopwatch();
        //#endif
        public override void PaintToOutputWindow()
        {
            if (!_isInitGLControl)
            {
                return;
            }

            //#if DEBUG
            _stopWatch.Reset();
            _stopWatch.Start();
            //#endif
            _windowControl.MakeCurrent();
            _openGLViewport.PaintMe();
            _windowControl.SwapBuffers();
            //
            _stopWatch.Stop();
#if DEBUG
            long millisec_per_frame = _stopWatch.ElapsedMilliseconds;
            int fps = (int)(1000.0f / millisec_per_frame);
            System.Diagnostics.Debug.WriteLine("fps:" + fps);
#endif

        }
        public override void CopyOutputPixelBuffer(int x, int y, int w, int h, IntPtr outputBuffer)
        {
            throw new NotImplementedException();
        }

    }
}
