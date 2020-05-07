//Apache2, 2014-present, WinterDev

using System;
using PixelFarm.Drawing;
namespace LayoutFarm.UI.OpenGL
{

    public class MyTopWindowBridgeOpenGL : AbstractTopWindowBridge
    {

        bool _isInitGLControl;
        IGpuOpenGLSurfaceView _windowControl;
        OpenGLCanvasViewport _openGLViewport;
        RootGraphic _rootgfx;

        Cursor _defaultCur;
        Cursor _IBeamCur;
        Cursor _pointerCur;
        Cursor _arrowCur;

        public MyTopWindowBridgeOpenGL(RootGraphic root, ITopWindowEventRoot topWinEventRoot)
            : base(root, topWinEventRoot)
        {
            GlobalRootGraphic.CurrentRootGfx = root;

            _rootgfx = root;

            _defaultCur = UIPlatform.CreateCursor(new CursorRequest("system:Default"));
            _IBeamCur = UIPlatform.CreateCursor(new CursorRequest("system:IBeam"));
            _pointerCur = UIPlatform.CreateCursor(new CursorRequest("system:Pointer"));
            _arrowCur = UIPlatform.CreateCursor(new CursorRequest("system:Arrow"));
        }

        public override void PaintToOutputWindow(Rectangle invalidateArea)
        {
            if (_rootgfx != null)
            {
                if (invalidateArea.Width + invalidateArea.Height == 0)
                {
                    //entire window
                    //_rootgfx.InvalidateRectArea(new PixelFarm.Drawing.Rectangle(0, 0, _rootgfx.Width, _rootgfx.Height));
                }
                else
                {
                    RootGraphic.InvalidateRectArea(_rootgfx, invalidateArea);
                    _rootgfx.FlushAccumGraphics();
                    //PaintToOutputWindow();
                }
            }
        }
        public void SetCanvas(DrawBoard d)
        {
            _openGLViewport.SetDrawboard(d);
        }

        public override void BindWindowControl(IGpuOpenGLSurfaceView windowControl)
        {
            this.BindGLControl(windowControl);
        }

        /// <summary>
        /// bind to gl control
        /// </summary>
        /// <param name="myGLControl"></param>
        void BindGLControl(IGpuOpenGLSurfaceView myGLControl)
        {
            _windowControl = myGLControl;
            SetBaseCanvasViewport(_openGLViewport = new OpenGLCanvasViewport(this.RootGfx, _windowControl.GetSize()));
            RootGfx.SetPaintDelegates(
                null, //still do nothing
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
        protected override void ChangeCursor(Cursor cursor)
        {
            _windowControl.CurrentCursor = cursor;
        }
        protected override void ChangeCursor(MouseCursorStyle cursorStyle)
        {
            switch (cursorStyle)
            {
                case MouseCursorStyle.Pointer:
                    _windowControl.CurrentCursor = _pointerCur;
                    break;
                case MouseCursorStyle.IBeam:
                    _windowControl.CurrentCursor = _IBeamCur;
                    break;
                case MouseCursorStyle.Arrow:
                    _windowControl.CurrentCursor = _arrowCur;
                    break;
                default:
                    _windowControl.CurrentCursor = _defaultCur;
                    break;
            }
        }

#if DEBUG
        System.Diagnostics.Stopwatch _stopWatch = new System.Diagnostics.Stopwatch();
#endif
        public override void PaintToOutputWindow()
        {
            if (!_isInitGLControl)
            {
                return;
            }

#if DEBUG
            _stopWatch.Reset();
            _stopWatch.Start();
#endif
            _windowControl.MakeCurrent();
            _openGLViewport.PaintMe();
            _windowControl.SwapBuffers();
            //
#if DEBUG
            _stopWatch.Stop();
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
