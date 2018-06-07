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
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using OpenTK.Graphics;
namespace OpenTK.Platform.Egl
{
    // Holds information about an EGL window.
    class EglWindowInfo : IWindowInfo
    {

        IntPtr handle;
        IntPtr display;
        IntPtr surface;
        bool disposed;
        public EglWindowInfo(IntPtr handle, IntPtr display)
        {
            Handle = handle;
            Display = display;
        }
        public EglWindowInfo(IntPtr handle, IntPtr display, IntPtr surface)
        {
            Handle = handle;
            Display = display;
            Surface = surface;
        }
        public IntPtr Handle { get { return handle; } private set { handle = value; } }

        public IntPtr Display { get { return display; } private set { display = value; } }

        public IntPtr Surface { get { return surface; } private set { surface = value; } }



        //my extension
        static int[] eglSurfaceConfigs = null;

        public void CreateWindowSurface(IntPtr config)
        {
            //some gles2 implementation may supports more eglSurfaceConfig 
            //we 
            if (eglSurfaceConfigs == null)
            {
                IntPtr vendor = Egl.QueryString(display, Egl.EXTENSIONS);
                string eglVendor = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(vendor);
                if (eglVendor.Contains("EGL_NV_post_sub_buffer"))
                {
                    //we want some extensions ...
                    eglSurfaceConfigs = new int[]{ 
                        //key,                                                   value
                        AngleProjectEglConfig.EGL_POST_SUB_BUFFER_SUPPORTED_NV,1,
                        Egl.NONE//end config array with zero
                    };
                }
                else
                {
                    //blank config
                    eglSurfaceConfigs = new int[]{ 
                        //key,                                                   value                        
                        Egl.NONE//end config array with zero
                    };
                }
            }

            Surface = Egl.CreateWindowSurface(Display, config, Handle, eglSurfaceConfigs);
            int error = Egl.GetError();
            if (error != Egl.SUCCESS)
                throw new GraphicsContextException(String.Format("[Error] Failed to create EGL window surface, error {0}.", error));
        }

        //public void CreatePixmapSurface(EGLConfig config)
        //{
        //    Surface = Egl.CreatePixmapSurface(Display, config, Handle, null);
        //}

        //public void CreatePbufferSurface(EGLConfig config)
        //{
        //    Surface = Egl.CreatePbufferSurface(Display, config, null);
        //}

        public void DestroySurface()
        {
            if (Surface != IntPtr.Zero)
                if (!Egl.DestroySurface(Display, Surface))
                    Debug.Print("[Warning] Failed to destroy {0}:{1}.", Surface.GetType().Name, Surface);
        }



        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool manual)
        {
            if (!disposed)
            {
                if (manual)
                {
                    DestroySurface();
                    disposed = true;
                }
                else
                {
                    Debug.Print("[Warning] Failed to destroy {0}:{1}.", this.GetType().Name, Handle);
                }
            }
        }

        ~EglWindowInfo()
        {
            Dispose(false);
        }

    }
}
