//
// The Open Toolkit Library License
//
// Copyright (c) 2006 - 2009 the Open Toolkit library, except where noted.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using OpenTK.Platform;
using OpenTK.Graphics;
using OpenTK;

namespace LayoutFarm.UI
{

    public static class GLESInit
    {
        static OpenTK.Graphics.GraphicsMode s_gfxmode;
        static bool s_initOpenTK;
        public static void InitGLES()
        {
            if (s_initOpenTK) return;

            OpenTK.Platform.Factory.GetCustomPlatformFactory = () => OpenTK.Platform.Egl.EglAngle.NewFactory();
            OpenTK.Toolkit.Init(new OpenTK.ToolkitOptions {
                Backend = OpenTK.PlatformBackend.PreferNative,
            });
            OpenTK.Graphics.PlatformAddressPortal.GetAddressDelegate = OpenTK.Platform.Utilities.CreateGetAddress();

            (new OpenTK.Graphics.ES20.GL()).LoadAll();
            (new OpenTK.Graphics.ES30.GL()).LoadAll();
            s_initOpenTK = true;
        }

        public static OpenTK.Graphics.GraphicsMode GetDefaultGraphicsMode()
        {
            if (!s_initOpenTK)
            {
                InitGLES();
            }
            if (s_gfxmode == null)
            {
                s_gfxmode = new OpenTK.Graphics.GraphicsMode(
                    DisplayDevice.Default.BitsPerPixel,//default 32 bits color
                    16,//depth buffer => 16
                    8, //stencil buffer => 8 (set this if you want to use stencil buffer toos)
                    0, //number of sample of FSAA (not always work)
                    0, //accum buffer
                    2, // n buffer, 2=> double buffer
                    false);//sterio 
            }
            return s_gfxmode;
        }

        public static int GLES_Major = 2;
        public static int GLES_Minor = 1;
    }



    public class MyWin32WindowWrapper : IGpuOpenGLSurfaceView
    {
        IGraphicsContext _context;
        IGLControl _implementation;
        AbstractTopWindowBridge _topWinBridge;

        int _major;
        int _minor;
        GraphicsContextFlags _flags;
        IntPtr _nativeHwnd;
        bool _isCpuSurface;
        int _width = 800;
        int _height = 600;
        int _left = 0;
        int _top = 0;

        public MyWin32WindowWrapper(IntPtr nativeHwnd, bool isCpuSurface)
        {
            SetNativeHwnd(nativeHwnd, isCpuSurface);
        }

        void IGpuOpenGLSurfaceView.Dispose()
        {
        }
        void IGpuOpenGLSurfaceView.Refresh()
        {
        }
        void IGpuOpenGLSurfaceView.Invalidate()
        {
        }
        int IGpuOpenGLSurfaceView.Width => _width;
        int IGpuOpenGLSurfaceView.Height => _width;

        Cursor _cursor;
        public Cursor CurrentCursor
        {
            get => _cursor;
            set
            {
                //TODO: review here
                _cursor = value;
#if DEBUG
                System.Diagnostics.Debugger.Break();
#endif
            }
        }
        public PixelFarm.Drawing.Size GetSize() => new PixelFarm.Drawing.Size(_width, _height);
        public void SetNativeHwnd(IntPtr nativeHwnd, bool isCpuSurface)
        {
            if (_isCpuSurface = isCpuSurface)
            {
                _nativeHwnd = nativeHwnd;
            }
            else
            {
                SetNativeHwnd(nativeHwnd,
                         GLESInit.GLES_Major,
                         GLESInit.GLES_Minor,
                         OpenTK.Graphics.GraphicsContextFlags.Embedded |
                         OpenTK.Graphics.GraphicsContextFlags.Angle |
                         OpenTK.Graphics.GraphicsContextFlags.AngleD3D11 |
                         OpenTK.Graphics.GraphicsContextFlags.AngleD3D9);
            }

        }
        public void SetNativeHwnd(IntPtr nativeHwnd, int major, int minor, GraphicsContextFlags flags)
        {
            //handle is created
            _nativeHwnd = nativeHwnd;
            _major = major;
            _minor = minor;
            _flags = flags;

            Win32.MyWin32.RECT rect = new Win32.MyWin32.RECT();
            Win32.MyWin32.GetWindowRect(_nativeHwnd, ref rect);
            _width = rect.right - rect.left;
            _height = rect.bottom - rect.top;

            _implementation = new GLControlFactory().CreateGLControl(GLESInit.GetDefaultGraphicsMode(), nativeHwnd);
            _context = _implementation.CreateContext(_major, _minor, _flags);
            //------
            //complex render tree here...  
            MakeCurrent();
        }


        public void SetTopWinBridge(AbstractTopWindowBridge topWinBridge)
        {
            _topWinBridge = topWinBridge;
            topWinBridge.BindWindowControl(this);
        }
        public void SetBounds(int left, int top, int width, int height)
        {
            //TODO 

            _left = left;
            _top = top;
            _width = width;
            _height = height;

            if (_nativeHwnd != IntPtr.Zero)
            {
                Win32.MyWin32.MoveWindow(_nativeHwnd, _left, _top, width, height, true);
            }
        }
        public void SetSize(int width, int height)
        {
            _width = width;
            _height = height;

            if (_nativeHwnd != IntPtr.Zero)
            {
                Win32.MyWin32.MoveWindow(_nativeHwnd, _left, _top, width, height, true);
            }
        }

        protected virtual void OnPaint(UIPaintEventArgs e)
        {
            _topWinBridge.PaintToOutputWindow(
                new PixelFarm.Drawing.Rectangle(
                    e.Left,
                    e.Top,
                    e.Right - e.Left,
                    e.Bottom - e.Top));
        }
        protected virtual void OnMouseDown(PrimaryMouseEventArgs e)
        {
            _topWinBridge.HandleMouseDown(e);
        }
        protected virtual void OnMouseMove(PrimaryMouseEventArgs e)
        {
            _topWinBridge.HandleMouseMove(e);
        }
        protected virtual void OnMouseUp(PrimaryMouseEventArgs e)
        {
            _topWinBridge.HandleMouseUp(e);
        }

        protected virtual void OnWheel(PrimaryMouseEventArgs e)
        {
            _topWinBridge.HandleMouseWheel(e);
        }
        protected virtual void OnKeyDown(UIKeyEventArgs e)
        {
            _topWinBridge.HandleKeyDown(e);
        }
        protected virtual void OnKeyPress(UIKeyEventArgs e)
        {
            _topWinBridge.HandleKeyPress(e, e.KeyChar);
        }
        protected virtual void OnKeyUp(UIKeyEventArgs e)
        {
            _topWinBridge.HandleKeyUp(e);
        }

        //------------
        internal static void InvokeMouseDown(MyWin32WindowWrapper control, PrimaryMouseEventArgs e)
        {
            control.OnMouseDown(e);
        }
        internal static void InvokeMouseUp(MyWin32WindowWrapper control, PrimaryMouseEventArgs e)
        {
            control.OnMouseUp(e);
        }
        internal static void InvokeMouseMove(MyWin32WindowWrapper control, PrimaryMouseEventArgs e)
        {
            control.OnMouseMove(e);
        }
        internal static void InvokeWheel(MyWin32WindowWrapper control, PrimaryMouseEventArgs e)
        {
            control.OnWheel(e);
        }
        //------------
        internal static void InvokeOnPaint(MyWin32WindowWrapper control, UIPaintEventArgs e)
        {
            control.OnPaint(e);
        }

        //------------
        internal static void InvokeOnDialogKey(MyWin32WindowWrapper control, UIKeyEventArgs e)
        {
            control.OnKeyDown(e);
        }
        internal static void InvokeOnKeyDown(MyWin32WindowWrapper control, UIKeyEventArgs e)
        {
            control.OnKeyDown(e);
        }
        internal static void InvokeOnKeyUp(MyWin32WindowWrapper control, UIKeyEventArgs e)
        {
            control.OnKeyUp(e);
        }
        internal static void InvokeOnKeyPress(MyWin32WindowWrapper control, UIKeyEventArgs e)
        {
            control.OnKeyPress(e);
        }
        public void MakeCurrent()
        {
            _context.MakeCurrent(_implementation.WindowInfo);
        }
        public void SwapBuffers()
        {
            _context.SwapBuffers();
        }

        /// <summary>
        /// Gets the <see cref="OpenTK.Platform.IWindowInfo"/> for this instance.
        /// </summary>
        public IWindowInfo WindowInfo => _implementation.WindowInfo;

        public IntPtr NativeWindowHwnd => _nativeHwnd;


        public IntPtr GetEglDisplay()
        {
            if (((IGraphicsContextInternal)_context).Implementation is OpenTK.Platform.Egl.IEglContext eglContext)
            {
                return eglContext.MyWindowInfo.Display;
            }
            return IntPtr.Zero;
        }
        public IntPtr GetEglSurface()
        {
            if (((IGraphicsContextInternal)_context).Implementation is OpenTK.Platform.Egl.IEglContext eglContext)
            {
                return eglContext.MyWindowInfo.Surface;
            }
            return IntPtr.Zero;
        }
    }


    public class GLESContext
    {
        IGraphicsContext _context;
        IGLControl _implementation;
        int _major;
        int _minor;
        GraphicsContextFlags _flags;
        IntPtr _nativeHwnd;

        public GLESContext(IntPtr nativeHwnd)
        {
            SetNativeHwnd(nativeHwnd,
                         GLESInit.GLES_Major,
                         GLESInit.GLES_Minor,
                         OpenTK.Graphics.GraphicsContextFlags.Embedded |
                         OpenTK.Graphics.GraphicsContextFlags.Angle |
                         OpenTK.Graphics.GraphicsContextFlags.AngleD3D11 |
                         OpenTK.Graphics.GraphicsContextFlags.AngleD3D9);
        }

        void SetNativeHwnd(IntPtr nativeHwnd, int major, int minor, GraphicsContextFlags flags)
        {
            //handle is created
            _nativeHwnd = nativeHwnd;
            _major = major;
            _minor = minor;
            _flags = flags;

            _implementation = new GLControlFactory().CreateGLControl(GLESInit.GetDefaultGraphicsMode(), nativeHwnd);
            _context = _implementation.CreateContext(_major, _minor, _flags);
            //------
            //complex render tree here...  
            MakeCurrent();
        }

        public void MakeCurrent()
        {
            _context.MakeCurrent(_implementation.WindowInfo);
        }
        public void SwapBuffers()
        {
            _context.SwapBuffers();
        }

        /// <summary>
        /// Gets the <see cref="OpenTK.Platform.IWindowInfo"/> for this instance.
        /// </summary>
        public IWindowInfo WindowInfo => _implementation.WindowInfo;

        public IntPtr NativeWindowHwnd => _nativeHwnd;


        public IntPtr GetEglDisplay()
        {
            if (((IGraphicsContextInternal)_context).Implementation is OpenTK.Platform.Egl.IEglContext eglContext)
            {
                return eglContext.MyWindowInfo.Display;
            }
            return IntPtr.Zero;
        }
        public IntPtr GetEglSurface()
        {
            if (((IGraphicsContextInternal)_context).Implementation is OpenTK.Platform.Egl.IEglContext eglContext)
            {
                return eglContext.MyWindowInfo.Surface;
            }
            return IntPtr.Zero;
        }
    }


    public class Win32EventBridge
    {

        readonly PrimaryMouseEventArgs _mouseArgs = new PrimaryMouseEventArgs();
        readonly UIKeyEventArgs _keyEventArgs = new UIKeyEventArgs();
        readonly UIPaintEventArgs _paintEventArgs = new UIPaintEventArgs();

        MyWin32WindowWrapper _myWindow;

        public void SetMainWindowControl(MyWin32WindowWrapper control)
        {
            _myWindow = control;
        }
        public void InvokeProcessDialogKey(uint virtualKey)
        {
            _keyEventArgs.UIEventName = UIEventName.ProcessDialogKey;
            _keyEventArgs.SetEventInfo(virtualKey, s_shiftDown = ShiftKeyDown(), s_altDown = AltKeyDown(), s_controlDown = ControlKeyDown());
            MyWin32WindowWrapper.InvokeOnDialogKey(_myWindow, _keyEventArgs);
        }
        public void InvokeOnPaint(int left, int top, int width, int height)
        {
            _paintEventArgs.Left = left;
            _paintEventArgs.Top = top;
            _paintEventArgs.Right = left + width;
            _paintEventArgs.Bottom = top + height;
            MyWin32WindowWrapper.InvokeOnPaint(_myWindow, _paintEventArgs);
        }
        public void InvokeOnMouseDown(int x, int y, UIMouseButtons buttons)
        {
            s_mouseDown = true;
            _mouseArgs.SetMouseDownEventInfo(x, y, buttons, 1);
            MyWin32WindowWrapper.InvokeMouseDown(_myWindow, _mouseArgs);
        }
        public void InvokeOnMouseMove(int x, int y)
        {
            _mouseArgs.SetMouseMoveEventInfo(x, y);
            MyWin32WindowWrapper.InvokeMouseMove(_myWindow, _mouseArgs);
        }
        public void InvokeOnMouseUp(int x, int y, UIMouseButtons buttons)
        {
            s_mouseDown = false;
            _mouseArgs.SetMouseUpEventInfo(x, y, buttons);
            MyWin32WindowWrapper.InvokeMouseUp(_myWindow, _mouseArgs);
        }

        public bool CustomPanelMsgHandler(IntPtr hwnd, uint msg,
              IntPtr wparams,
              IntPtr lparams)
        {
            if (_myWindow == null) { return false; }

            //----
            //translate msg and its parameter to event
            //use event args pool
            switch (msg)
            {
                default:
                    {

                    }
                    break;
                case Win32.MyWin32.WM_PAINT:
                    {
                        //wParam,lparam => not used   
                        Win32.MyWin32.RECT r = new Win32.MyWin32.RECT();
                        Win32.MyWin32.GetUpdateRect(hwnd, ref r, false);
                        InvokeOnPaint(r.left, r.top, r.right - r.left, r.bottom - r.top);
                        break;
                    }
                case Win32.MyWin32.WM_SIZE:
                    {
                        //sent to a window after its size has changed
                        int mouse_pos = lparams.ToInt32();
                        int new_width = (mouse_pos & 0xffff);
                        int new_height = ((mouse_pos >> 16) & 0xffff);
                    }
                    break;
                case Win32.MyWin32.WM_ACTIVATEAPP:
                    {

                    }
                    break;
                case Win32.MyWin32.WM_ACTIVATE:
                    {

                    }
                    break;
                case Win32.MyWin32.WM_SHOWWINDOW:
                    {

                    }
                    break;
                case Win32.MyWin32.WM_LBUTTONDOWN:
                    {
                        //1. event name
                        //2. modifier
                        //3. essential parameter

                        int mouse_pos = lparams.ToInt32();
                        s_mouseDown = true;
                        _mouseArgs.SetMouseDownEventInfo((mouse_pos & 0xffff), ((mouse_pos >> 16) & 0xffff), UIMouseButtons.Left, 1);
                        MyWin32WindowWrapper.InvokeMouseDown(_myWindow, _mouseArgs);
                    }
                    break;
                case Win32.MyWin32.WM_LBUTTONUP:
                    {
                        //mouse up
                        int mouse_pos = lparams.ToInt32();
                      
                        s_mouseDown = false;
                        _mouseArgs.SetMouseUpEventInfo((mouse_pos & 0xffff), ((mouse_pos >> 16) & 0xffff), UIMouseButtons.Left);
                        MyWin32WindowWrapper.InvokeMouseUp(_myWindow, _mouseArgs);
                    }
                    break;
                case Win32.MyWin32.WM_RBUTTONDOWN:
                    {
                        int mouse_pos = lparams.ToInt32();
                       
                        s_mouseDown = true;
                        _mouseArgs.SetMouseDownEventInfo((mouse_pos & 0xffff), ((mouse_pos >> 16) & 0xffff), UIMouseButtons.Right, 1);
                        MyWin32WindowWrapper.InvokeMouseDown(_myWindow, _mouseArgs);

                    }
                    break;
                case Win32.MyWin32.WM_RBUTTONUP:
                    {
                        int mouse_pos = lparams.ToInt32();
                        
                        s_mouseDown = false;
                        _mouseArgs.SetMouseUpEventInfo((mouse_pos & 0xffff), ((mouse_pos >> 16) & 0xffff), UIMouseButtons.Right);
                        MyWin32WindowWrapper.InvokeMouseUp(_myWindow, _mouseArgs);
                    }
                    break;
                case Win32.MyWin32.WM_MBUTTONDOWN:
                    {
                        int mouse_pos = lparams.ToInt32();
                       
                        s_mouseDown = true;
                        _mouseArgs.SetMouseDownEventInfo((mouse_pos & 0xffff), ((mouse_pos >> 16) & 0xffff), UIMouseButtons.Middle, 1);
                        MyWin32WindowWrapper.InvokeMouseDown(_myWindow, _mouseArgs);
                        return true;
                    }
                case Win32.MyWin32.WM_MBUTTONUP:
                    {
                        int mouse_pos = lparams.ToInt32();
                     
                        s_mouseDown = false;
                        _mouseArgs.SetMouseUpEventInfo((mouse_pos & 0xffff), ((mouse_pos >> 16) & 0xffff), UIMouseButtons.Middle);
                        MyWin32WindowWrapper.InvokeMouseUp(_myWindow, _mouseArgs);
                    }
                    break;
                case Win32.MyWin32.WM_MOUSEMOVE:
                    {
                        int mouse_pos = lparams.ToInt32(); 

                        //button depend on prev mouse down button? 
                        _mouseArgs.SetMouseMoveEventInfo((mouse_pos & 0xffff), ((mouse_pos >> 16) & 0xffff));
                        MyWin32WindowWrapper.InvokeMouseMove(_myWindow, _mouseArgs);

                    }
                    break;
                case Win32.MyWin32.WM_MOUSEHWHEEL:
                    {
                        //if we derived directly from System.Windows.Control
                        int mouse_pos = lparams.ToInt32();
                   
                        int delta = ((int)wparams.ToInt64() >> 16);

                        //button depend on prev mouse down button?                         
                        _mouseArgs.SetMouseWheelEventInfo((mouse_pos & 0xffff), ((mouse_pos >> 16) & 0xffff), delta);
                        MyWin32WindowWrapper.InvokeWheel(_myWindow, _mouseArgs);
                    }
                    break;
                case Win32.MyWin32.WM_MOUSEWHEEL:
                    {
                        //invoke mouse wheel 
                        int mouse_pos = lparams.ToInt32();
                        

                        int delta = ((int)wparams.ToInt64() >> 16);

                        //button depend on prev mouse down button?                         
                        _mouseArgs.SetMouseWheelEventInfo((mouse_pos & 0xffff), ((mouse_pos >> 16) & 0xffff), delta);
                        MyWin32WindowWrapper.InvokeWheel(_myWindow, _mouseArgs);

                    }
                    break;
                //case Win32.MyWin32.WM_MOUSELEAVE:
                //    {
                //        //Posted to a window when the cursor leaves the client area of the window specified 
                //        //in a prior call to TrackMouseEvent.  
                //    }
                //    break;
                //---------------------
                case Win32.MyWin32.WM_SETFOCUS:
                    break;
                case Win32.MyWin32.WM_KILLFOCUS:
                    break;
                case Win32.MyWin32.WM_KEYDOWN:
                    {
                        //wparams=> The virtual-key code of the nonsystem key. See Virtual-Key Codes. 
                        uint virtualKey = (uint)wparams.ToInt32();

                        _keyEventArgs.UIEventName = UIEventName.KeyDown;
                        _keyEventArgs.SetEventInfo(virtualKey, s_shiftDown = ShiftKeyDown(), s_altDown = AltKeyDown(), s_controlDown = ControlKeyDown());

                        MyWin32WindowWrapper.InvokeOnKeyDown(_myWindow, _keyEventArgs);
                        break;
                    }
                case Win32.MyWin32.WM_CHAR:
                    {
                        uint codepoint = (uint)wparams.ToInt32();
                        char c = (char)codepoint;
                        _keyEventArgs.UIEventName = UIEventName.KeyPress;
                        _keyEventArgs.SetEventInfo(codepoint, s_shiftDown, s_altDown, s_controlDown);
                        MyWin32WindowWrapper.InvokeOnKeyPress(_myWindow, _keyEventArgs);
                        break;
                    }
                case Win32.MyWin32.WM_KEYUP:
                    {
                        uint virtualKey = (uint)wparams.ToInt32();
                        _keyEventArgs.UIEventName = UIEventName.KeyUp;
                        _keyEventArgs.SetEventInfo(virtualKey, s_shiftDown, s_altDown, s_controlDown);
                        MyWin32WindowWrapper.InvokeOnKeyUp(_myWindow, _keyEventArgs);

                        s_shiftDown = s_altDown = s_controlDown = false;//reset
                        break;
                    }
                //------------------------
                case Win32.MyWin32.WM_SETCURSOR:
                    {
                        //set cursor 
                        int hitArea = (int)(wparams.ToInt64() & 0xffff);
                    }
                    break;

            }
            return false;
        }
        static bool s_shiftDown;
        static bool s_altDown;
        static bool s_controlDown;
        static bool s_mouseDown;

        static bool IsLeftKey() => Win32.MyWin32.GetKeyState(Win32.MyWin32.VK_LEFT) == 1;
        static bool ShiftKeyDown() => Win32.MyWin32.GetKeyState(Win32.MyWin32.VK_SHIFT) == 1;
        static bool AltKeyDown() => Win32.MyWin32.GetKeyState(Win32.MyWin32.VK_MENU) == 1;
        static bool ControlKeyDown() => Win32.MyWin32.GetKeyState(Win32.MyWin32.VK_SHIFT) == 1;
    }


}
