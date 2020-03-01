//
// EglWinContext.cs
//
// Author:
//       Stefanos A. <stapostol@gmail.com>
//
// Copyright (c) 2006-2014 Stefanos Apostolopoulos
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

using System;
using OpenTK.Graphics;

namespace OpenTK.Platform.Egl
{
    static class ESLib
    {
        static IntPtr s_ES1;
        static IntPtr s_ES2;
        static bool s_isLoaded;
        static void LoadLib()
        {
            if (s_isLoaded) return;
            //
            //if not
            s_ES1 = OpenTK.Platform.Windows.Functions.LoadLibrary("libGLESv1_CM");
            s_ES2 = OpenTK.Platform.Windows.Functions.LoadLibrary("libGLESv2");

            s_isLoaded = true;
        }
        public static IntPtr GetGLESv1_CM()
        {
            LoadLib();
            return s_ES1;
        }
        public static IntPtr GetGLESv2()
        {
            LoadLib();
            return s_ES2;
        }
    }

    //-----------------------
    //my extensions
    public class MyEglWin
    {

        EglWinContext _eglWinContext;
        readonly EglWindowInfo _egl_win;
        readonly IntPtr _eglDisplay;
        public MyEglWin(IntPtr hwnd, IntPtr eglDisplay, int major, int minor,
             OpenTK.Graphics.GraphicsMode mode, GraphicsContextFlags flags)
        {
            Hwnd = hwnd;
            _eglDisplay = eglDisplay;

            _egl_win = new OpenTK.Platform.Egl.EglWindowInfo(hwnd, _eglDisplay);
            _eglWinContext = new EglWinContext(mode, _egl_win, null, major, minor, flags);

        }
        public void SwapBuffers()
        {
            _eglWinContext.SwapBuffers();
        }
        public void MakeCurrent()
        {
            _eglWinContext.MakeCurrent(_egl_win);
        }
        public IntPtr Hwnd { get; }
    }


    class EglWinContext : EglContext
    {
        private IntPtr ES1 = ESLib.GetGLESv1_CM();
        private IntPtr ES2 = ESLib.GetGLESv2();


        public EglWinContext(GraphicsMode mode, EglWindowInfo window, IGraphicsContext sharedContext,
          int major, int minor, GraphicsContextFlags flags)
          : base(mode, window, sharedContext, major, minor, flags | GraphicsContextFlags.Embedded | GraphicsContextFlags.Angle)
        {

        }

        public EglWinContext(ContextHandle handle, EglWindowInfo window, IGraphicsContext sharedContext,
            int major, int minor, GraphicsContextFlags flags)
            : base(handle, window, sharedContext, major, minor, flags)
        {
        }

        protected override IntPtr GetStaticAddress(IntPtr function, RenderableFlags renderable)
        {
            if ((renderable & (RenderableFlags.ES2 | RenderableFlags.ES3)) != 0 && ES2 != IntPtr.Zero)
            {
                return Windows.Functions.GetProcAddress(ES2, function);
            }
            else if ((renderable & RenderableFlags.ES) != 0 && ES1 != IntPtr.Zero)
            {
                return Windows.Functions.GetProcAddress(ES1, function);
            }
            return IntPtr.Zero;
        }

        protected override void Dispose(bool manual)
        {
            //if (ES1 != IntPtr.Zero)
            //{
            //    Windows.Functions.FreeLibrary(ES1);
            //}
            //if (ES2 != IntPtr.Zero)
            //{
            //    Windows.Functions.FreeLibrary(ES2);
            //}

            ES1 = ES2 = IntPtr.Zero;

            base.Dispose(manual);
        }
    }
}