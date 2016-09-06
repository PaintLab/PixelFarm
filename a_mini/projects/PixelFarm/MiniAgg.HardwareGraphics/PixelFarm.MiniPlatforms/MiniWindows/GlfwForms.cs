//MIT, 2016, WinterDev
using System;
using System.Collections.Generic;
using Pencil.Gaming;

namespace PixelFarm.Forms
{
    public static class GlfwApp
    {
        static Dictionary<GlfwWindowPtr, GlFwForm> existingForms = new Dictionary<GlfwWindowPtr, GlFwForm>();
        static List<GlFwForm> exitingFormList = new List<GlFwForm>();
        static GlfwWindowCloseFun s_windowCloseCb;
        static GlfwWindowFocusFun s_windowFocusCb;
        static GlfwWindowIconifyFun s_windowIconifyCb;
        static GlfwWindowPosFun s_windowPosCb;
        static GlfwWindowRefreshFun s_windowRefreshCb;
        static GlfwWindowSizeFun s_windowSizeCb;
        static GlfwApp()
        {
            s_windowCloseCb = (GlfwWindowPtr wnd) =>
            {
                GlFwForm found;
                if (existingForms.TryGetValue(wnd, out found))
                {
                    //user can cancel window close here here
                    bool userCancel = false;
                    found.InvokeOnClosing(ref userCancel);
                    if (userCancel)
                    {
                        return;
                    }
                    //--------------------------------------
                    //user let this window close ***
                    Glfw.SetWindowShouldClose(wnd, true);
                    Glfw.DestroyWindow(wnd); //destroy this
                    existingForms.Remove(wnd);
                    exitingFormList.Remove(found);
                    //--------------------------------------
                }
            };
            s_windowFocusCb = (GlfwWindowPtr wnd, bool focus) =>
            {
                GlFwForm found;
                if (existingForms.TryGetValue(wnd, out found))
                {
                    found.SetFocusState(focus);
                }
            };
            s_windowIconifyCb = (GlfwWindowPtr wnd, bool iconify) =>
            {
                GlFwForm found;
                if (existingForms.TryGetValue(wnd, out found))
                {
                    found.SetIconifyState(iconify);
                }
            };

            s_windowPosCb = (GlfwWindowPtr wnd, int x, int y) =>
            {
                GlFwForm found;
                if (existingForms.TryGetValue(wnd, out found))
                {
                    found.SetWindowPos(x, y);
                }
            };
            s_windowRefreshCb = (GlfwWindowPtr wnd) =>
            {
                GlFwForm found;
                if (existingForms.TryGetValue(wnd, out found))
                {
                    found.InvokeRefresh();
                }
            };

            s_windowSizeCb = (GlfwWindowPtr wnd, int width, int height) =>
            {
                GlFwForm found;
                if (existingForms.TryGetValue(wnd, out found))
                {
                    found.SetWindowSize(width, height);
                }
            };
        }
        public static GlFwForm CreateGlfwForm(int w, int h, string title)
        {
            GlfwWindowPtr glWindowPtr = Glfw.CreateWindow(w, h,
            title,
            new GlfwMonitorPtr(),//default monitor
            new GlfwWindowPtr()); //default top window 
            GlFwForm f = new GlFwForm(glWindowPtr, w, h);
            f.Text = title;
            //-------------------
            //setup events for glfw window
            Glfw.SetWindowCloseCallback(glWindowPtr, s_windowCloseCb);
            Glfw.SetWindowFocusCallback(glWindowPtr, s_windowFocusCb);
            Glfw.SetWindowIconifyCallback(glWindowPtr, s_windowIconifyCb);
            Glfw.SetWindowPosCallback(glWindowPtr, s_windowPosCb);
            Glfw.SetWindowRefreshCallback(glWindowPtr, s_windowRefreshCb);
            Glfw.SetWindowSizeCallback(glWindowPtr, s_windowSizeCb);
            //-------------------
            existingForms.Add(glWindowPtr, f);
            exitingFormList.Add(f);
            return f;
        }


        public static bool ShouldClose()
        {
            for (int i = exitingFormList.Count - 1; i >= 0; --i)
            {
                //if we have some form that should not close
                //then we just return
                if (!Glfw.WindowShouldClose(exitingFormList[i].GlfwWindowPtr))
                {
                    return false;
                }
            }
            return true;
        }
        public static void UpdateWindowsFrame()
        {
            int j = exitingFormList.Count;
            for (int i = 0; i < j; ++i)
            {
                /* Render here */
                /* Swap front and back buffers */
                GlFwForm form = exitingFormList[i];
                form.DrawFrame();
                Glfw.SwapBuffers(form.GlfwWindowPtr);
            }

        }
    }
    public class GlFwForm : Form
    {


        SimpleAction drawFrameDel;
        string _windowTitle = "";
        GlfwWindowPtr _nativeGlFwWindowPtr;
        IntPtr _nativePlatformHwnd;

        internal GlFwForm(GlfwWindowPtr glWindowPtr, int w, int h)
        {
            this.Width = w;
            this.Height = h;
            _nativeGlFwWindowPtr = glWindowPtr;
            _nativePlatformHwnd = glWindowPtr.inner_ptr;
        }
        public GlfwWindowPtr GlfwWindowPtr
        {
            get
            {
                return _nativeGlFwWindowPtr;
            }
        }
        internal void SetFocusState(bool focus)
        {
            if (focus)
            {
                OnFocus();
            }
            else
            {
                OnLostFocus();
            }
        }
        internal void SetIconifyState(bool iconify)
        {
            OnIconify(iconify);
        }
        internal void SetWindowPos(int x, int y)
        {
            //on pos changed
        }
        internal void SetWindowSize(int w, int h)
        {
            //on pos changed
        }
        internal void InvokeRefresh()
        {
        }
        internal void InvokeOnClosing(ref bool cancel)
        {
            OnClosing(ref cancel);
        }
        protected virtual void OnIconify(bool iconify)
        {

        }
        protected virtual void OnFocus()
        {
        }
        protected virtual void OnLostFocus()
        {
        }
        protected virtual void OnClosing(ref bool cancel)
        {

        }
        /// <summary>
        /// get platform's native handle
        /// </summary>
        public override IntPtr Handle
        {
            get
            {
                CheckNativeHandle();
                return _nativePlatformHwnd;
            }
        }
        public override string Text
        {
            get
            {
                return this._windowTitle;
            }
            set
            {
                this._windowTitle = value;
                if (!_nativeGlFwWindowPtr.IsEmpty)
                {
                    //if not empty 
                    //set to native window title
                    Glfw.SetWindowTitle(this._nativeGlFwWindowPtr, value);
                }
            }
        }
        void CheckNativeHandle()
        {
            if (_nativeGlFwWindowPtr.IsEmpty)
            {
                //create native glfw window
                this._nativeGlFwWindowPtr = Glfw.CreateWindow(this.Width,
                    this.Height,
                    this.Title,
                    new GlfwMonitorPtr(),//default monitor
                    new GlfwWindowPtr()); //default top window 
                _nativePlatformHwnd = _nativeGlFwWindowPtr.inner_ptr;
            }
        }
        public override void Show()
        {
            CheckNativeHandle();
            base.Show();
        }

        public override int Width
        {
            get
            {
                return base.Width;
            }
            set
            {
                base.Width = value;

            }
        }
        public override int Height
        {
            get
            {
                return base.Height;
            }

            set
            {
                base.Height = value;
            }
        }
        public void MakeCurrent()
        {
            Glfw.MakeContextCurrent(this._nativeGlFwWindowPtr);
        }
        public void CreateOpenGLEsContext()
        {

            //make open gl es current context 
            GlfwWindowPtr currentContext = Glfw.GetCurrentContext();
            var contextHandler = new OpenTK.ContextHandle(currentContext.inner_ptr);
            var context = OpenTK.Graphics.GraphicsContext.CreateDummyContext(contextHandler);
            bool isCurrent = context.IsCurrent;
            PixelFarm.GlfwWinInfo winInfo = new PixelFarm.GlfwWinInfo(this._nativePlatformHwnd);
            context.MakeCurrent(winInfo);

        }
        public void SetDrawFrameDelegate(SimpleAction drawFrameDel)
        {
            this.drawFrameDel = drawFrameDel;
        }
        public void DrawFrame()
        {
            if (drawFrameDel != null)
            {
                MakeCurrent();
                drawFrameDel();
            }
        }
    }
}