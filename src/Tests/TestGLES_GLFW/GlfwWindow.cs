//MIT, 2016-present, WinterDev
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

using Glfw;
namespace PixelFarm.Forms
{
    public class GlfwWindowEventListener
    {

        readonly Glfw3.GLFWmousebuttonfun _mouseFunc;
        internal readonly IntPtr _mouseFuncPtr;
        //
        readonly Glfw3.GLFWkeyfun _keyFunc;
        internal readonly IntPtr _keyFuncPtr;
        //
        readonly Glfw3.GLFWcursorposfun _cursorFunc;
        internal readonly IntPtr _cursorFuncPtr;
        //
        readonly Glfw3.GLFWcharfun _keyCharFunc;
        internal readonly IntPtr _keyCharFuncPtr;
        //
        readonly Glfw3.GLFWscrollfun _scrollFunc;
        internal readonly IntPtr _scrollFuncPtr;

        readonly Glfw3.GLFWmonitorfun _monitorFunc;
        internal readonly IntPtr _monitorFuncPtr;

        readonly Glfw3.GLFWwindowmaximizefun _windowmaximizeFunc;
        internal readonly IntPtr _windowmaximizeFuncPtr;

        readonly Glfw3.GLFWwindowclosefun _windowcloseFunc;
        internal readonly IntPtr _windowcloseFuncPtr;

        readonly Glfw3.GLFWwindowcontentscalefun _windowcontentscaleFunc;
        internal readonly IntPtr _windowcontentscaleFuncPtr;

        readonly Glfw3.GLFWwindowfocusfun _windowfocusFunc;
        internal readonly IntPtr _windowfocusFuncPtr;

        readonly Glfw3.GLFWwindowiconifyfun _windowiconifyFunc;
        internal readonly IntPtr _windowiconifyFuncPtr;

        readonly Glfw3.GLFWwindowposfun _windowposFunc;
        internal readonly IntPtr _windowposFuncPtr;

        readonly Glfw3.GLFWwindowrefreshfun _windowRefreshFunc;
        internal readonly IntPtr _windowRefreshFuncPtr;

        readonly Glfw3.GLFWwindowsizefun _windowsizeFunc;
        internal readonly IntPtr _windowsizeFuncPtr;

        readonly Glfw3.GLFWcharmodsfun _charmodsFunc;
        internal readonly IntPtr _charmodsFuncPtr;

        readonly Glfw3.GLFWcursorenterfun _cursorenterFunc;
        internal readonly IntPtr _cursorenterFuncPtr;

        readonly Glfw3.GLFWdropfun _dropFunc;
        internal readonly IntPtr _dropFuncPtr;

        readonly Glfw3.GLFWerrorfun _errorFunc;
        internal readonly IntPtr _errorFuncPtr;

        readonly Glfw3.GLFWframebuffersizefun _framebuffersizeFunc;
        internal readonly IntPtr _framebuffersizeFuncPtr;

        readonly Glfw3.GLFWglproc _glprocFunc;
        internal readonly IntPtr _glprocFuncPtr;

        readonly Glfw3.GLFWjoystickfun _joystickFunc;
        internal readonly IntPtr _joystickFuncPtr;

        readonly Glfw3.GLFWvkproc _vkprocFunc;
        internal readonly IntPtr _vkprocFuncPtr;

        public GlfwWindowEventListener()
        {
            _mouseFunc = MouseEvent;
            _mouseFuncPtr = Marshal.GetFunctionPointerForDelegate(_mouseFunc);
            //
            _keyFunc = KeyEvent;
            _keyFuncPtr = Marshal.GetFunctionPointerForDelegate(_keyFunc);

            //(mousemove)
            _cursorFunc = CursorEvent;
            _cursorFuncPtr = Marshal.GetFunctionPointerForDelegate(_cursorFunc);
            //
            //translated keyboard event
            _keyCharFunc = KeyChar;
            _keyCharFuncPtr = Marshal.GetFunctionPointerForDelegate(_keyCharFunc);
            //
            _scrollFunc = Scroll;
            _scrollFuncPtr = Marshal.GetFunctionPointerForDelegate(_scrollFunc);

            _monitorFunc = MonitorEvent;
            _monitorFuncPtr = Marshal.GetFunctionPointerForDelegate(_monitorFunc);

            _windowmaximizeFunc = Maximize;
            _windowmaximizeFuncPtr = Marshal.GetFunctionPointerForDelegate(_windowmaximizeFunc);

            _windowcloseFunc = WindowClose;
            _windowcloseFuncPtr = Marshal.GetFunctionPointerForDelegate(_windowcloseFunc);

            _windowcontentscaleFunc = WindowContentScale;
            _windowcontentscaleFuncPtr = Marshal.GetFunctionPointerForDelegate(_windowcontentscaleFunc);

            _windowfocusFunc = WindowFocus;
            _windowfocusFuncPtr = Marshal.GetFunctionPointerForDelegate(_windowfocusFunc);

            _windowiconifyFunc = WindowIconify;
            _windowiconifyFuncPtr = Marshal.GetFunctionPointerForDelegate(_windowiconifyFunc);

            _windowposFunc = WindowPos;
            _windowposFuncPtr = Marshal.GetFunctionPointerForDelegate(_windowposFunc);

            _windowRefreshFunc = WindowRefresh;
            _windowRefreshFuncPtr = Marshal.GetFunctionPointerForDelegate(_windowRefreshFunc);

            _windowsizeFunc = WindowSize;
            _windowsizeFuncPtr = Marshal.GetFunctionPointerForDelegate(_windowsizeFunc);

            _charmodsFunc = CharMods;
            _charmodsFuncPtr = Marshal.GetFunctionPointerForDelegate(_charmodsFunc);

            _cursorenterFunc = CursorEnter;
            _cursorenterFuncPtr = Marshal.GetFunctionPointerForDelegate(_cursorenterFunc);

            ////_dropFunc = Drop;
            //_dropFuncPtr = Marshal.GetFunctionPointerForDelegate(_dropFunc);

            _errorFunc = Error;
            _errorFuncPtr = Marshal.GetFunctionPointerForDelegate(_errorFunc);

            _framebuffersizeFunc = FrameBufferSize;
            _framebuffersizeFuncPtr = Marshal.GetFunctionPointerForDelegate(_framebuffersizeFunc);

            _glprocFunc = GLProc;
            _glprocFuncPtr = Marshal.GetFunctionPointerForDelegate(_glprocFunc);

            _joystickFunc = JoyStick;
            _joystickFuncPtr = Marshal.GetFunctionPointerForDelegate(_joystickFunc);

            _vkprocFunc = VKProc;
            _vkprocFuncPtr = Marshal.GetFunctionPointerForDelegate(_vkprocFunc);
        }

        public virtual void MouseEvent(IntPtr winPtr, int button, int action, int modifier)
        {
            //System.Diagnostics.Debug.WriteLine("mouse:" + button.ToString());
        }
        public virtual void KeyEvent(IntPtr/*GLFWwindow*/ windowPtr, int key, int scancode, int action, int mods/*x476*/)
        {
            System.Diagnostics.Debug.WriteLine("key:" + key);
        }
        public virtual void CursorEvent(IntPtr/*GLFWwindow*/ windowPtr, double xpos, double ypos)
        {
            //(mouse move);
            //System.Diagnostics.Debug.WriteLine("cursor:" + xpos + "," + ypos);
        }
        public virtual void KeyChar(IntPtr windowPtr, uint codepoint)
        {
            //TODO: translate
            //System.Diagnostics.Debug.WriteLine("keychar:" + ((char)codepoint).ToString());
        }
        public virtual void Scroll(IntPtr/*GLFWwindow*/ window, double xoffset, double yoffset)
        {
            System.Diagnostics.Debug.WriteLine("scroll:" + xoffset + "," + yoffset);
        }
        public virtual void Maximize(IntPtr/*GLFWwindow*/ window, int iconified)
        {
            System.Diagnostics.Debug.WriteLine("maximize:" + iconified);
        }
        public virtual void MonitorEvent(IntPtr/*GLFWmonitor*/ monitor, int event0)
        {
            System.Diagnostics.Debug.WriteLine("monitor:" + event0);
        }
        public virtual void WindowClose(IntPtr/*GLFWwindow*/ window)
        {
            System.Diagnostics.Debug.WriteLine("windowclose");
        }
        public virtual void WindowContentScale(IntPtr/*GLFWwindow*/ window, float xscale, float yscale)
        {
            System.Diagnostics.Debug.WriteLine("windowcontentscale: " + xscale + "," + yscale);
        }
        public virtual void WindowFocus(IntPtr/*GLFWwindow*/ window, int focused)
        {
            System.Diagnostics.Debug.WriteLine("windowfocus: " + focused);
        }
        public virtual void WindowIconify(IntPtr/*GLFWwindow*/ window, int iconified)
        {
            System.Diagnostics.Debug.WriteLine("windowiconify: " + iconified);
        }
        public virtual void WindowPos(IntPtr/*GLFWwindow*/ window, int xpos, int ypos)
        {
            //System.Diagnostics.Debug.WriteLine("windowpos: " + xpos + "," + ypos);
        }
        public virtual void WindowRefresh(IntPtr/*GLFWwindow*/ window)
        {
            System.Diagnostics.Debug.WriteLine("windowrefresh");
        }
        public virtual void WindowSize(IntPtr/*GLFWwindow*/ window, int width, int height)
        {
            System.Diagnostics.Debug.WriteLine("windowsize: " + width + "," + height);
        }
        public virtual void CharMods(IntPtr/*GLFWwindow*/ window, uint codepoint, int mods)
        {
            System.Diagnostics.Debug.WriteLine("charmods: codepoint>" + codepoint + ", mods>" + mods);
        }
        public virtual void CursorEnter(IntPtr/*GLFWwindow*/ window, int entered)
        {
            System.Diagnostics.Debug.WriteLine("cursorenter: " + entered);
        }
        public virtual void Drop(IntPtr/*GLFWwindow*/ window, int count, string paths)
        {
            //TODO:

            System.Diagnostics.Debug.WriteLine("drop: count>" + count);
        }

        private void Error(int error, string description)
        {
            System.Diagnostics.Debug.WriteLine("error: code>" + error + ", description>" + description);
        }

        private void FrameBufferSize(IntPtr window, int width, int height)
        {
            System.Diagnostics.Debug.WriteLine("framebuffersize: w>" + width + ", h>" + height);
        }

        private void GLProc()
        {
            System.Diagnostics.Debug.WriteLine("glproc");
        }

        private void JoyStick(int jid, int event0)
        {
            System.Diagnostics.Debug.WriteLine("joystick: " + jid + ", event: " + event0);
        }

        private void VKProc()
        {
            System.Diagnostics.Debug.WriteLine("vkporc");
        }
    }

    public static class GlfwAppLoop
    {
        static List<GlFwForm> s_forms = new List<GlFwForm>();
        static GlFwForm s_mainMsgWin = null;
        internal static void RegisterGlfwForm(GlFwForm form)
        {
            form.IsRegistered = true;
            s_forms.Add(form);

            if (s_mainMsgWin == null)
            {
                s_mainMsgWin = form;
            }

        }
        public static void Run()
        {
            if (s_mainMsgWin == null) return;

            while (!s_mainMsgWin.ShouldClose())
            {
                /* Render here */
                /* Swap front and back buffers */

                s_mainMsgWin.SwapBuffer();

                /* Poll for and process events */
                Glfw3.glfwPollEvents();
            }
            Glfw3.glfwTerminate();
        }
    }

    public partial class GlFwForm
    {
        IntPtr _glfwWindow;
        IntPtr _nativeWindowPtr;//platform specific window Hwnd

        int _width;
        int _height;
        string _title;
        float _opacity = 1;//100% (opaque)

        GlfwWindowEventListener _winEventListener;

        internal bool IsRegistered;
        internal bool ShouldClose()
        {
            return Glfw3.glfwWindowShouldClose(_glfwWindow) != 0;
        }
        public GlFwForm(int width, int height, string title, GlfwWindowEventListener eventListener)
        {
            _width = width;
            _height = height;
            _title = title;
            _winEventListener = eventListener;
            //
            SetGlfwWindowHandler(Glfw3.glfwCreateWindow(_width, _height, title, IntPtr.Zero, IntPtr.Zero));
            GlfwAppLoop.RegisterGlfwForm(this);
        }
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                byte[] titleBuffer = System.Text.Encoding.UTF8.GetBytes(value);
                unsafe
                {
                    fixed (byte* h = &titleBuffer[0])
                    {
                        Glfw3.glfwSetWindowTitle(_glfwWindow, h);
                    }
                }
            }
        }
        public void SetClipboardText(string str)
        {
            Glfw3.glfwSetClipboardString(_glfwWindow, str);
        }
        public void Maximize()
        {
            Glfw3.glfwMaximizeWindow(_glfwWindow);//Maximize windows
        }
        public void Minimize()
        {
            Glfw3.glfwIconifyWindow(_glfwWindow);
        }
        public void Show()
        {
            Glfw3.glfwShowWindow(_glfwWindow);//show window      
        }
        public void Hide()
        {
            Glfw3.glfwHideWindow(_glfwWindow);//show window      
        }
        public float Opacity => _opacity;
        public void SetOpacity(float opacity)
        {
            _opacity = opacity;
            Glfw3.glfwSetWindowOpacity(_glfwWindow, opacity);
        }
        public void MakeCurrent()
        {
            Glfw3.glfwMakeContextCurrent(_glfwWindow);
        }
        public void SwapBuffer()
        {
            Glfw3.glfwSwapBuffers(_glfwWindow);
        }
        public int Width => _width;
        public int Height => _height;
        public void SetSize(int width, int height)
        {
            _width = width;
            _height = height;

            Glfw3.glfwSetWindowSize(_glfwWindow, _width, _height);
        }

        [System.Runtime.InteropServices.DllImport("glfw3")]
        public static extern IntPtr glfwGetWin32Window(IntPtr glfwWindow);


        void SetGlfwWindowHandler(IntPtr glfwWindow)
        {
            _glfwWindow = glfwWindow;
            _nativeWindowPtr = glfwGetWin32Window(glfwWindow);

            //event handlers...
            Glfw3.glfwSetMouseButtonCallback(_glfwWindow, _winEventListener._mouseFuncPtr);
            Glfw3.glfwSetKeyCallback(_glfwWindow, _winEventListener._keyFuncPtr);
            Glfw3.glfwSetCursorPosCallback(_glfwWindow, _winEventListener._cursorFuncPtr);
            Glfw3.glfwSetKeyCallback(_glfwWindow, _winEventListener._keyCharFuncPtr);
            Glfw3.glfwSetScrollCallback(_glfwWindow, _winEventListener._scrollFuncPtr);
            //

            Glfw3.glfwSetCharModsCallback(_glfwWindow, _winEventListener._charmodsFuncPtr);
            Glfw3.glfwSetCursorEnterCallback(_glfwWindow, _winEventListener._cursorenterFuncPtr);
            Glfw3.glfwSetDropCallback(_glfwWindow, _winEventListener._dropFuncPtr);
            Glfw3.glfwSetErrorCallback(_winEventListener._errorFuncPtr);
            Glfw3.glfwSetFramebufferSizeCallback(_glfwWindow, _winEventListener._framebuffersizeFuncPtr);
            Glfw3.glfwSetJoystickCallback(_winEventListener._joystickFuncPtr);
            Glfw3.glfwSetMonitorCallback(_winEventListener._monitorFuncPtr);
            Glfw3.glfwSetWindowCloseCallback(_glfwWindow, _winEventListener._windowcloseFuncPtr);
            Glfw3.glfwSetWindowContentScaleCallback(_glfwWindow, _winEventListener._windowcontentscaleFuncPtr);
            Glfw3.glfwSetWindowFocusCallback(_glfwWindow, _winEventListener._windowfocusFuncPtr);
            Glfw3.glfwSetWindowIconifyCallback(_glfwWindow, _winEventListener._windowiconifyFuncPtr);
            Glfw3.glfwSetWindowMaximizeCallback(_glfwWindow, _winEventListener._windowmaximizeFuncPtr);
            Glfw3.glfwSetWindowPosCallback(_glfwWindow, _winEventListener._windowposFuncPtr);
            Glfw3.glfwSetWindowRefreshCallback(_glfwWindow, _winEventListener._windowRefreshFuncPtr);
            Glfw3.glfwSetWindowSizeCallback(_glfwWindow, _winEventListener._windowsizeFuncPtr);
        }

    }
}