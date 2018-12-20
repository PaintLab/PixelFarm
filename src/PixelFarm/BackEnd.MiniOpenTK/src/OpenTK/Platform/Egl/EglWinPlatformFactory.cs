//
// The Open Toolkit Library License
//
// Copyright (c) 2006 - 2009 the Open Toolkit library.
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
using OpenTK.Graphics;
using OpenTK.Platform.Windows;

namespace OpenTK.Platform.Egl
{
    // EGL factory for the Windows platform.
    internal class EglWinPlatformFactory : WinFactory
    {

        static bool isFirstTimeEval = true;
        static bool useD3D9Only = false;

        public EglWinPlatformFactory()
        {

        }

        IntPtr TryInitEglDisplay(IWindowInfo window, ref int major, ref int minor, ref GraphicsContextFlags flags)
        {
            //see https://github.com/Microsoft/angle/wiki/Initializing-ANGLE-on-D3D11-Feature-Level-9
            //IntPtr egl_display1 = Egl.GetPlatformDisplayEXT(Egl.PLATFORM_ANGLE_ANGLE, IntPtr.Zero, new int[]{
            //    Egl.PLATFORM_ANGLE_TYPE_ANGLE,
            //    Egl.PLATFORM_ANGLE_TYPE_D3D11_ANGLE,
            //    Egl.PLATFORM_ANGLE_MAX_VERSION_MAJOR_ANGLE, 9,
            //    Egl.PLATFORM_ANGLE_MAX_VERSION_MINOR_ANGLE, 3,
            //    Egl.NONE,
            //});
            //if (!Egl.Initialize(egl_display1, out int m1, out int m2))
            //{
            //} 
            //1. use default openTK default 
            WinWindowInfo win_win = (WinWindowInfo)window;
            bool success = false;
            int eglMajor, eglMinor;
            IntPtr egl_display = IntPtr.Zero;
            if (isFirstTimeEval)
            {

                isFirstTimeEval = false;
                egl_display = GetDisplay(win_win.DeviceContext);
                success = Egl.Initialize(egl_display, out eglMajor, out eglMinor);

                //-------------------
                //we found that some old machine can't init with D3D11
                //so ... we use D3D9 instead 
                if (!success)
                {
                    egl_display = Egl.GetPlatformDisplayEXT(Egl.PLATFORM_ANGLE_ANGLE, IntPtr.Zero, new int[]{
                    Egl.PLATFORM_ANGLE_TYPE_ANGLE,
                    Egl.PLATFORM_ANGLE_TYPE_D3D9_ANGLE,
                    Egl.NONE,
                    });
                    if (Egl.Initialize(egl_display, out eglMajor, out eglMinor))
                    {
                        useD3D9Only = true;
                        success = true;
                        //in this case 
                        major = 2; //GLES2
                        minor = 1;
                        flags = flags & ~GraphicsContextFlags.AngleD3D11;//can't use D3D11
                    }
                    else
                    {
                        throw new GraphicsContextException(String.Format("Failed to initialize EGL, error {0}.", Egl.GetError()));
                    }
                }
            }
            else
            {
                //not first time
                if (useD3D9Only)
                {
                    //no D3D11
                    egl_display = Egl.GetPlatformDisplayEXT(Egl.PLATFORM_ANGLE_ANGLE, IntPtr.Zero, new int[]{
                    Egl.PLATFORM_ANGLE_TYPE_ANGLE,
                    Egl.PLATFORM_ANGLE_TYPE_D3D9_ANGLE,
                    Egl.NONE,
                    });
                    if (Egl.Initialize(egl_display, out eglMajor, out eglMinor))
                    {
                        useD3D9Only = true;
                        success = true;
                        //in this case 
                        major = 2; //GLES2
                        minor = 1;
                        flags = flags & ~GraphicsContextFlags.AngleD3D11;//can't use D3D11
                    }
                    else
                    {
                        throw new GraphicsContextException(String.Format("Failed to initialize EGL, error {0}.", Egl.GetError()));
                    }
                }
                else
                {
                    //use D3D11
                    egl_display = GetDisplay(win_win.DeviceContext);
                    success = Egl.Initialize(egl_display, out eglMajor, out eglMinor);

                    //-------------------
                    //we found that some old machine can't init with D3D11
                    //so ... we use D3D9 instead 
                    if (!success)
                    {
                        //???
                        throw new GraphicsContextException(String.Format("Failed to initialize EGL, error {0}.", Egl.GetError()));
                    }
                }
            }
            return egl_display;
        }
        public override IGraphicsContext CreateGLContext(GraphicsMode mode, IWindowInfo window, IGraphicsContext shareContext, bool directRendering, int major, int minor, GraphicsContextFlags flags)
        {
            WinWindowInfo win_win = (WinWindowInfo)window;
            IntPtr egl_display = TryInitEglDisplay(window, ref major, ref minor, ref flags);
            EglWindowInfo egl_win = new OpenTK.Platform.Egl.EglWindowInfo(win_win.Handle, egl_display);
            return new EglWinContext(mode, egl_win, shareContext, major, minor, flags);

            //WinWindowInfo win_win = (WinWindowInfo)window;
            //IntPtr egl_display = GetDisplay(win_win.DeviceContext);
            //EglWindowInfo egl_win = new OpenTK.Platform.Egl.EglWindowInfo(win_win.Handle, egl_display);
            //return new EglWinContext(mode, egl_win, shareContext, major, minor, flags);
        }
        public override IGraphicsContext CreateGLContext(ContextHandle handle, IWindowInfo window, IGraphicsContext shareContext, bool directRendering, int major, int minor, GraphicsContextFlags flags)
        {


            WinWindowInfo win_win = (WinWindowInfo)window;
            IntPtr egl_display = TryInitEglDisplay(window, ref major, ref minor, ref flags);
            EglWindowInfo egl_win = new OpenTK.Platform.Egl.EglWindowInfo(win_win.Handle, egl_display);
            return new EglWinContext(handle, egl_win, shareContext, major, minor, flags);

            //WinWindowInfo win_win = (WinWindowInfo)window;
            //IntPtr egl_display = GetDisplay(win_win.DeviceContext);
            //EglWindowInfo egl_win = new OpenTK.Platform.Egl.EglWindowInfo(win_win.Handle, egl_display);
            //return new EglWinContext(handle, egl_win, shareContext, major, minor, flags);
        }

        public override GraphicsContext.GetCurrentContextDelegate CreateGetCurrentGraphicsContext()
        {
            return (GraphicsContext.GetCurrentContextDelegate)delegate
            {
                return new ContextHandle(Egl.GetCurrentContext());
            };
        }

        private IntPtr GetDisplay(IntPtr dc)
        {
            IntPtr display = Egl.GetDisplay(dc);
            if (display == IntPtr.Zero)
            {
                display = Egl.GetDisplay(IntPtr.Zero);
            }

            return display;
        }
    }
}
