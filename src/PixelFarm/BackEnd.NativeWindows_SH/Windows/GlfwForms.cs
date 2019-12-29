////MIT, 2016-present, WinterDev
//using System;
//using System.Collections.Generic;
//using Pencil.Gaming;
//using OpenTK.Graphics.ES20;

//namespace PixelFarm.Forms
//{
//    public static class GlfwApp
//    {
//        static Dictionary<GlfwWindowPtr, GlFwForm> s_existingForms = new Dictionary<GlfwWindowPtr, GlFwForm>();
//        static List<GlFwForm> exitingFormList = new List<GlFwForm>();
//        static GlfwWindowCloseFun s_windowCloseCb;
//        static GlfwWindowFocusFun s_windowFocusCb;
//        static GlfwWindowIconifyFun s_windowIconifyCb;
//        static GlfwWindowPosFun s_windowPosCb;
//        static GlfwWindowRefreshFun s_windowRefreshCb;
//        static GlfwWindowSizeFun s_windowSizeCb;
//        static GlfwCursorPosFun s_windowCursorPosCb;
//        static GlfwCursorEnterFun s_windowCursorEnterCb;
//        static GlfwMouseButtonFun s_windowMouseButtonCb;
//        static GlfwScrollFun s_scrollCb;
//        static GlfwKeyFun s_windowKeyCb; //key up, key down
//        static GlfwCharFun s_windowCharCb; //key press

//        static IntPtr s_latestGlWindowPtr;
//        static GlFwForm s_latestForm;

//        static GlfwApp()
//        {
//            s_windowCloseCb = (GlfwWindowPtr wnd) =>
//            {


//                if (GetGlfwForm(wnd, out GlFwForm found))
//                {
//                    //user can cancel window close here here
//                    bool userCancel = false;
//                    GlFwForm.InvokeOnClosing(found, ref userCancel);
//                    if (userCancel)
//                    {
//                        return;
//                    }
//                    //--------------------------------------
//                    s_latestForm = null;
//                    s_latestGlWindowPtr = IntPtr.Zero;
//                    //user let this window close ***
//                    Glfw.SetWindowShouldClose(wnd, true);
//                    Glfw.DestroyWindow(wnd); //destroy this
//                    s_existingForms.Remove(wnd);
//                    exitingFormList.Remove(found);
//                    //--------------------------------------
//                }
//            };
//            s_windowFocusCb = (GlfwWindowPtr wnd, bool focus) =>
//            {

//                if (GetGlfwForm(wnd, out GlFwForm found))
//                {
//                    GlFwForm.SetFocusState(found, focus);
//                }
//            };
//            s_windowIconifyCb = (GlfwWindowPtr wnd, bool iconify) =>
//            {

//                if (GetGlfwForm(wnd, out GlFwForm found))
//                {
//                    GlFwForm.SetIconifyState(found, iconify);
//                }
//            };

//            s_windowPosCb = (GlfwWindowPtr wnd, int x, int y) =>
//            {

//                if (GetGlfwForm(wnd, out GlFwForm found))
//                {
//                    GlFwForm.InvokeOnWindowMove(found, x, y);
//                }
//            };
//            s_windowRefreshCb = (GlfwWindowPtr wnd) =>
//            {

//                if (GetGlfwForm(wnd, out GlFwForm found))
//                {
//                    GlFwForm.InvokeOnRefresh(found);
//                }
//            };

//            s_windowSizeCb = (GlfwWindowPtr wnd, int width, int height) =>
//            {

//                if (GetGlfwForm(wnd, out GlFwForm found))
//                {
//                    GlFwForm.InvokeOnSizeChanged(found, width, height);
//                }
//            };
//            s_windowCursorPosCb = (GlfwWindowPtr wnd, double x, double y) =>
//            {

//                if (GetGlfwForm(wnd, out GlFwForm found))
//                {
//                    found._latestMouseX = x;
//                    found._latestMouseY = y;

//                    GlFwForm.InvokeCursorPos(found, x, y);
//                }
//            };
//            s_windowCursorEnterCb = (GlfwWindowPtr wnd, bool enter) =>
//            {

//                if (GetGlfwForm(wnd, out GlFwForm found))
//                {
//                    GlFwForm.SetCursorEnterState(found, enter);
//                }
//            };
//            s_windowMouseButtonCb = (GlfwWindowPtr wnd, MouseButton btn, KeyActionKind keyAction) =>
//            {

//                if (GetGlfwForm(wnd, out GlFwForm found))
//                {

//                    int x = (int)found._latestMouseX;
//                    int y = (int)found._latestMouseY;

//                    GlFwForm.InvokeMouseButton(found, btn, keyAction, x, y);
//                }
//            };
//            s_scrollCb = (GlfwWindowPtr wnd, double xoffset, double yoffset) =>
//            {

//                if (GetGlfwForm(wnd, out GlFwForm found))
//                {
//                    int x = (int)found._latestMouseX;
//                    int y = (int)found._latestMouseY;
//                    GlFwForm.InvokeOnScroll(found, x, y, (int)xoffset, (int)yoffset);
//                }
//            };
//            s_windowKeyCb = (GlfwWindowPtr wnd, Key key, int scanCode, KeyActionKind action, KeyModifiers mods) =>
//            {

//                if (GetGlfwForm(wnd, out GlFwForm found))
//                {
//                    GlFwForm.InvokeKey(found, key, scanCode, action, mods);
//                }
//            };
//            s_windowCharCb = (GlfwWindowPtr wnd, char ch) =>
//            {

//                if (GetGlfwForm(wnd, out GlFwForm found))
//                {
//                    GlFwForm.InvokeKeyPress(found, ch);
//                }
//            };
//        }
//        static bool GetGlfwForm(GlfwWindowPtr wnd, out GlFwForm found)
//        {
//            if (wnd.inner_ptr == s_latestGlWindowPtr)
//            {
//                found = s_latestForm;
//                return true;
//            }
//            else
//            {

//                if (s_existingForms.TryGetValue(wnd, out found))
//                {
//                    s_latestGlWindowPtr = wnd.inner_ptr;
//                    s_latestForm = found;
//                    return true;
//                }
//                //reset
//                s_latestGlWindowPtr = IntPtr.Zero;
//                s_latestForm = null;
//                return false;
//            }
//        }

//        internal static void InitGlFwForm(GlFwForm f, int initW, int initH, string title = "")
//        {

//            GlfwWindowPtr glWindowPtr = Glfw.CreateWindow(initW, initH,
//                title,
//                new GlfwMonitorPtr(),//default monitor
//                new GlfwWindowPtr()); //default top window 

//            Glfw.MakeContextCurrent(glWindowPtr);//***

//            f.SetupNativePtr(glWindowPtr, f.Width, f.Height);
//            //-------------------
//            //setup events for glfw window
//            Glfw.SetWindowCloseCallback(glWindowPtr, s_windowCloseCb);
//            Glfw.SetWindowFocusCallback(glWindowPtr, s_windowFocusCb);
//            Glfw.SetWindowIconifyCallback(glWindowPtr, s_windowIconifyCb);
//            Glfw.SetWindowPosCallback(glWindowPtr, s_windowPosCb);
//            Glfw.SetWindowRefreshCallback(glWindowPtr, s_windowRefreshCb);
//            Glfw.SetWindowSizeCallback(glWindowPtr, s_windowSizeCb);
//            Glfw.SetCursorPosCallback(glWindowPtr, s_windowCursorPosCb);
//            Glfw.SetCursorEnterCallback(glWindowPtr, s_windowCursorEnterCb);
//            Glfw.SetMouseButtonCallback(glWindowPtr, s_windowMouseButtonCb);
//            Glfw.SetScrollCallback(glWindowPtr, s_scrollCb);
//            Glfw.SetKeyCallback(glWindowPtr, s_windowKeyCb);
//            Glfw.SetCharCallback(glWindowPtr, s_windowCharCb);

//            //-------------------           

//            s_existingForms.Add(glWindowPtr, f);
//            exitingFormList.Add(f);

//            //-------------------
//            GLFWPlatforms.CreateGLESContext(f);
//        }

//        public static bool ShouldClose()
//        {
//            for (int i = exitingFormList.Count - 1; i >= 0; --i)
//            {
//                //if we have some form that should not close
//                //then we just return
//                if (!Glfw.WindowShouldClose(exitingFormList[i].GlfwWindowPtr))
//                {
//                    return false;
//                }
//            }
//            return true;
//        }
//        public static void UpdateWindowsFrame()
//        {
//            int j = exitingFormList.Count;
//            for (int i = 0; i < j; ++i)
//            {
//                /* Render here */
//                /* Swap front and back buffers */
//                GlFwForm form = exitingFormList[i];
//                form.DrawFrame();
//                Glfw.SwapBuffers(form.GlfwWindowPtr);
//            }
//        }
//        public static void RunMainLoop()
//        {
//            while (!GlfwApp.ShouldClose())
//            {
//                //---------------
//                //render phase and swap
//                GlfwApp.UpdateWindowsFrame();
//                /* Poll for and process events */
//                Glfw.PollEvents();
//            }
//            Glfw.Terminate();
//        }
//    }

//    public class PaintEventArgs : EventArgs
//    {

//    }


//    public class GlFwForm : Form
//    {

//        internal double _latestMouseX;
//        internal double _latestMouseY;

//        Action<PaintEventArgs> _drawFrameDel;
//        string _windowTitle = "";
//        GlfwWindowPtr _nativeGlFwWindowPtr;
//        IntPtr _nativePlatformHwnd;
//        GlfwWinInfo _winInfo;

//        PaintEventArgs _renderUpdateEventArgs = new PaintEventArgs();
//        GLFWContextForOpenTK _glfwContextForOpenTK;

//        public GlFwForm(int w, int h, string title = "")
//        {
//            GlfwApp.InitGlFwForm(this, w, h, title);
//            this.SetBounds(0, 0, w, h);

//            //setup default
//            int max = Math.Max(w, h);
//            //------------------------------------
//            GL.Enable(EnableCap.Blend);
//            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
//            GL.ClearColor(1, 1, 1, 1);
//            //--------------------------------------------------------------------------------
//            //setup viewport size
//            //set up canvas  
//            GL.Viewport(0, 0, max, max);

//            SetDrawFrameDelegate(OnPaint);
//        }

//        internal void SetupNativePtr(GlfwWindowPtr glWindowPtr, int w, int h)
//        {
//            base.Width = w;
//            base.Height = h;
//            _nativeGlFwWindowPtr = glWindowPtr;
//            _nativePlatformHwnd = Glfw.GetNativePlatformWinHwnd(glWindowPtr);
//            _winInfo = new PixelFarm.GlfwWinInfo(_nativeGlFwWindowPtr);
//        }
//        internal GLFWContextForOpenTK GlfwContextForOpenTK
//        {
//            get => _glfwContextForOpenTK;
//            set => _glfwContextForOpenTK = value;
//        }

//        internal GlfwWindowPtr GlfwWindowPtr => _nativeGlFwWindowPtr;
//        internal OpenTK.Graphics.GraphicsContext OpenTKGraphicContext { get; set; }

//        public override void Close()
//        {
//            Glfw.HideWindow(_nativeGlFwWindowPtr);
//            Glfw.DestroyWindow(_nativeGlFwWindowPtr);
//        }


//        /// <summary>
//        /// get platform's native handle
//        /// </summary>
//        public override IntPtr Handle
//        {
//            get
//            {
//                CheckNativeHandle();
//                return _nativePlatformHwnd;
//            }
//        }
//        public override string Text
//        {
//            get => _windowTitle;
//            set
//            {
//                _windowTitle = value;
//                if (!_nativeGlFwWindowPtr.IsEmpty)
//                {
//                    //if not empty 
//                    //set to native window title
//                    Glfw.SetWindowTitle(_nativeGlFwWindowPtr, value);
//                }
//            }
//        }
//        void CheckNativeHandle()
//        {
//            if (_nativeGlFwWindowPtr.IsEmpty)
//            {
//                //create native glfw window
//                _nativeGlFwWindowPtr = Glfw.CreateWindow(this.Width,
//                    this.Height,
//                    this.Text,
//                    new GlfwMonitorPtr(),//default monitor
//                    new GlfwWindowPtr()); //default top window 

//            }
//        }

//        public override void Show()
//        {
//            CheckNativeHandle();
//            base.Show();
//        }
//        public override int Width
//        {
//            get => base.Width;

//            set
//            {
//                base.Width = value;
//                Glfw.SetWindowSize(_nativeGlFwWindowPtr, value, this.Height);
//            }
//        }
//        public override int Height
//        {
//            get => base.Height;

//            set
//            {
//                base.Height = value;
//                Glfw.SetWindowSize(_nativeGlFwWindowPtr, this.Width, value);
//            }
//        }
//        public void MakeCurrent()
//        {
//            //Glfw.MakeContextCurrent(_nativeGlFwWindowPtr);
//            _glfwContextForOpenTK.MakeCurrent(_winInfo);
//        }
//        public void Activate()
//        {
//            _glfwContextForOpenTK.MakeCurrent(_winInfo);

//        }
//        public void SetDrawFrameDelegate(Action<PaintEventArgs> drawFrameDel)
//        {
//            _drawFrameDel = drawFrameDel;
//        }
//        public void DrawFrame()
//        {
//            if (_drawFrameDel != null)
//            {
//                MakeCurrent();
//                _drawFrameDel(_renderUpdateEventArgs);
//            }
//        }
//    }
//}