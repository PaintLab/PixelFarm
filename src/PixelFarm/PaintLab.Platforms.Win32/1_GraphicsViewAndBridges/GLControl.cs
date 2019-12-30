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
            OpenTK.Toolkit.Init(new OpenTK.ToolkitOptions
            {
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



    public class MyNativeWindow : IGpuOpenGLSurfaceView
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

        public MyNativeWindow()
        {

        }
        public MyNativeWindow(IntPtr nativeHwnd, bool isCpuSurface)
        {
            SetNativeHwnd(nativeHwnd, isCpuSurface);
        }
        public void Dispose()
        {

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
        internal IntPtr NativeHwnd => _nativeHwnd;

        public void SetTopWinBridge(AbstractTopWindowBridge topWinBridge)
        {
            _topWinBridge = topWinBridge;
            topWinBridge.BindWindowControl(this);
        }
        public void SetBounds(int left, int top, int width, int height)
        {
            //TODO
            _width = width;
            _height = height;
        }
        public void SetSize(int w, int h)
        {
            _width = w;
            _height = h;
        }
        public int Width => _width;
        public int Height => _height;
        public void Invalidate()
        {
            //redraw window
        }

        public void Refresh()
        {
            //invalidate 
            //and update windows
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
        protected virtual void OnMouseDown(UIMouseEventArgs e)
        {
            _topWinBridge.HandleMouseDown(e);
        }
        protected virtual void OnMouseMove(UIMouseEventArgs e)
        {
            _topWinBridge.HandleMouseMove(e);
        }
        protected virtual void OnMouseUp(UIMouseEventArgs e)
        {
            _topWinBridge.HandleMouseUp(e);
        }
        protected virtual void OnWheel(UIMouseEventArgs e)
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
        internal static void InvokeMouseDown(MyNativeWindow control, UIMouseEventArgs e)
        {
            control.OnMouseDown(e);
        }
        internal static void InvokeMouseUp(MyNativeWindow control, UIMouseEventArgs e)
        {
            control.OnMouseUp(e);
        }
        internal static void InvokeMouseMove(MyNativeWindow control, UIMouseEventArgs e)
        {
            control.OnMouseMove(e);
        }
        internal static void InvokeWheel(MyNativeWindow control, UIMouseEventArgs e)
        {
            control.OnWheel(e);
        }
        internal static void InvokeOnPaint(MyNativeWindow control, UIPaintEventArgs e)
        {
            control.OnPaint(e);
        }

        //------------
        internal static void InvokeOnDialogKey(MyNativeWindow control, UIKeyEventArgs e)
        {
            control.OnKeyDown(e);
        }
        internal static void InvokeOnKeyDown(MyNativeWindow control, UIKeyEventArgs e)
        {
            control.OnKeyDown(e);
        }
        internal static void InvokeOnKeyUp(MyNativeWindow control, UIKeyEventArgs e)
        {
            control.OnKeyUp(e);
        }
        internal static void InvokeOnKeyPress(MyNativeWindow control, UIKeyEventArgs e)
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



    public class Win32EventBridge
    {
        UIMouseEventArgs _mouseEventArgs = new UIMouseEventArgs();
        UIKeyEventArgs _keyEventArgs = new UIKeyEventArgs();
        UIPaintEventArgs _paintEventArgs = new UIPaintEventArgs();

        MyNativeWindow _myWindow;

        public void SetMainWindowControl(MyNativeWindow control)
        {
            _myWindow = control;
        }
        public void SendProcessDialogKey(uint virtualKey)
        {
            _keyEventArgs.UIEventName = UIEventName.ProcessDialogKey;
            _keyEventArgs.SetEventInfo(virtualKey, s_shiftDown = ShiftKeyDown(), s_altDown = AltKeyDown(), s_controlDown = ControlKeyDown());
            MyNativeWindow.InvokeOnDialogKey(_myWindow, _keyEventArgs);
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
                case Win32.MyWin32.WM_LBUTTONDOWN:
                    {
                        //1. event name
                        //2. modifier
                        //3. essential parameter

                        int mouse_pos = lparams.ToInt32();
                        int x = (mouse_pos & 0xffff);
                        int y = ((mouse_pos >> 16) & 0xffff);
                        s_mouseDown = true;
                        _mouseEventArgs.UIEventName = UIEventName.MouseDown;
                        _mouseEventArgs.SetEventInfo(x, y, UIMouseButtons.Left, 1, 0);
                        MyNativeWindow.InvokeMouseDown(_myWindow, _mouseEventArgs);

                        return true;
                    }
                case Win32.MyWin32.WM_LBUTTONUP:
                    {
                        //mouse up
                        int mouse_pos = lparams.ToInt32();
                        int x = (mouse_pos & 0xffff);
                        int y = ((mouse_pos >> 16) & 0xffff);
                        s_mouseDown = false;
                        _mouseEventArgs.UIEventName = UIEventName.MouseUp;
                        _mouseEventArgs.SetEventInfo(x, y, UIMouseButtons.Left, 1, 0);

                        MyNativeWindow.InvokeMouseUp(_myWindow, _mouseEventArgs);
                    }
                    break;
                case Win32.MyWin32.WM_RBUTTONDOWN:
                    {
                        int mouse_pos = lparams.ToInt32();
                        int x = (mouse_pos & 0xffff);
                        int y = ((mouse_pos >> 16) & 0xffff);
                        s_mouseDown = true;
                        _mouseEventArgs.UIEventName = UIEventName.MouseDown;
                        _mouseEventArgs.SetEventInfo(x, y, UIMouseButtons.Right, 1, 0);
                        MyNativeWindow.InvokeMouseDown(_myWindow, _mouseEventArgs);
                    }
                    break;
                case Win32.MyWin32.WM_RBUTTONUP:
                    {
                        int mouse_pos = lparams.ToInt32();
                        int x = (mouse_pos & 0xffff);
                        int y = ((mouse_pos >> 16) & 0xffff);
                        s_mouseDown = false;
                        _mouseEventArgs.UIEventName = UIEventName.MouseUp;
                        _mouseEventArgs.SetEventInfo(x, y, UIMouseButtons.Right, 1, 0);
                        MyNativeWindow.InvokeMouseUp(_myWindow, _mouseEventArgs);
                    }
                    break;
                case Win32.MyWin32.WM_MBUTTONDOWN:
                    {
                        int mouse_pos = lparams.ToInt32();
                        int x = (mouse_pos & 0xffff);
                        int y = ((mouse_pos >> 16) & 0xffff);
                        s_mouseDown = true;
                        _mouseEventArgs.UIEventName = UIEventName.MouseDown;
                        _mouseEventArgs.SetEventInfo(x, y, UIMouseButtons.Middle, 1, 0);
                        MyNativeWindow.InvokeMouseDown(_myWindow, _mouseEventArgs);
                    }
                    break;
                case Win32.MyWin32.WM_MBUTTONUP:
                    {
                        int mouse_pos = lparams.ToInt32();
                        int x = (mouse_pos & 0xffff);
                        int y = ((mouse_pos >> 16) & 0xffff);
                        s_mouseDown = false;
                        _mouseEventArgs.UIEventName = UIEventName.MouseUp;
                        _mouseEventArgs.SetEventInfo(x, y, UIMouseButtons.Middle, 1, 0);
                        MyNativeWindow.InvokeMouseUp(_myWindow, _mouseEventArgs);
                    }
                    break;
                case Win32.MyWin32.WM_MOUSEMOVE:
                    {
                        int mouse_pos = lparams.ToInt32();
                        int x = (mouse_pos & 0xffff);
                        int y = ((mouse_pos >> 16) & 0xffff);


                        //button depend on prev mouse down button?
                        _mouseEventArgs.UIEventName = UIEventName.MouseMove;
                        _mouseEventArgs.SetEventInfo(x, y, UIMouseButtons.None, 1, 0);
                        MyNativeWindow.InvokeMouseMove(_myWindow, _mouseEventArgs);
                    }
                    break;
                case Win32.MyWin32.WM_MOUSEHWHEEL:
                    {
                        int mouse_pos = lparams.ToInt32();
                        int x = (mouse_pos & 0xffff);
                        int y = ((mouse_pos >> 16) & 0xffff);
                        int delta = ((int)wparams.ToInt64() >> 16);

                        //button depend on prev mouse down button?
                        _mouseEventArgs.UIEventName = UIEventName.Wheel;
                        _mouseEventArgs.SetEventInfo(x, y, UIMouseButtons.None, 0, delta);
                        MyNativeWindow.InvokeWheel(_myWindow, _mouseEventArgs);
                    }
                    break;
                case Win32.MyWin32.WM_MOUSEWHEEL:
                    {
                        //invoke mouse wheel 
                        int mouse_pos = lparams.ToInt32();
                        int x = (mouse_pos & 0xffff);
                        int y = ((mouse_pos >> 16) & 0xffff);

                        int delta = ((int)wparams.ToInt64() >> 16);

                        //button depend on prev mouse down button?
                        _mouseEventArgs.UIEventName = UIEventName.Wheel;
                        _mouseEventArgs.SetEventInfo(x, y, UIMouseButtons.None, 0, delta);

                        MyNativeWindow.InvokeWheel(_myWindow, _mouseEventArgs);
                    }
                    break;
                case Win32.MyWin32.WM_KEYDOWN:
                    {
                        //wparams=> The virtual-key code of the nonsystem key. See Virtual-Key Codes. 
                        uint virtualKey = (uint)wparams.ToInt32();

                        _keyEventArgs.UIEventName = UIEventName.KeyDown;
                        _keyEventArgs.SetEventInfo(virtualKey, s_shiftDown = ShiftKeyDown(), s_altDown = AltKeyDown(), s_controlDown = ControlKeyDown());

                        MyNativeWindow.InvokeOnKeyDown(_myWindow, _keyEventArgs);
                    }
                    break;
                case Win32.MyWin32.WM_CHAR:
                    {
                        uint codepoint = (uint)wparams.ToInt32();
                        char c = (char)codepoint;
                        _keyEventArgs.UIEventName = UIEventName.KeyPress;
                        _keyEventArgs.SetEventInfo(codepoint, s_shiftDown, s_altDown, s_controlDown);
                        MyNativeWindow.InvokeOnKeyPress(_myWindow, _keyEventArgs);
                    }
                    break;

                case Win32.MyWin32.WM_KEYUP:
                    {
                        uint virtualKey = (uint)wparams.ToInt32();
                        _keyEventArgs.UIEventName = UIEventName.KeyUp;
                        _keyEventArgs.SetEventInfo(virtualKey, s_shiftDown, s_altDown, s_controlDown);
                        MyNativeWindow.InvokeOnKeyUp(_myWindow, _keyEventArgs);

                        s_shiftDown = s_altDown = s_controlDown = false;//reset
                    }
                    break;
                //------------------------
                case Win32.MyWin32.WM_PAINT:
                    {
                        //wParam,lparam => not used  
                        Win32.MyWin32.RECT r = new Win32.MyWin32.RECT();
                        Win32.MyWin32.GetUpdateRect(hwnd, ref r, false);
                        _paintEventArgs.Left = r.left;
                        _paintEventArgs.Top = r.top;
                        _paintEventArgs.Right = r.right;
                        _paintEventArgs.Bottom = r.bottom;

                        MyNativeWindow.InvokeOnPaint(_myWindow, _paintEventArgs);
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
