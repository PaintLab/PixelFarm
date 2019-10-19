//Apache2, 2014-present, WinterDev

namespace LayoutFarm.UI
{
    public interface IGpuOpenGLSurfaceView
    {
        PixelFarm.Drawing.Size GetSize();
        void MakeCurrent();
        void SwapBuffers();
        void Dispose();
        //
        //TODO, review this again
        System.Windows.Forms.Cursor Cursor { get; set; }
    }
}