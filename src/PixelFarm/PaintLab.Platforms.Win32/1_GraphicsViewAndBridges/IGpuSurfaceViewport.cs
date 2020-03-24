//Apache2, 2014-present, WinterDev

using System;
namespace LayoutFarm.UI
{
    public interface IGpuOpenGLSurfaceView
    {
        PixelFarm.Drawing.Size GetSize();
        void MakeCurrent();
        void SwapBuffers();
        void Dispose();
        System.IntPtr NativeWindowHwnd { get; }
        void SetBounds(int left, int top, int width, int height);
        void SetSize(int width, int height);
        int Width { get; }
        int Height { get; }

        void Invalidate();
        void Refresh();

        IntPtr GetEglDisplay();
        IntPtr GetEglSurface();

        //
        Cursor CurrentCursor { get; set; }         
    }
}