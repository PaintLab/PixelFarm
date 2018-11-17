using System;
using System.Windows.Forms;
#if ENABLE_DESKTOP_OPENGL
using OpenTK.Graphics.OpenGL;
#else
using OpenTK.Graphics.ES20;
#endif
namespace OpenTK
{
    public partial class MyGLControl : GLControl
    {


        const int GLES_MAJOR = 3;
        const int GLES_MINOR = 0;

        EventHandler _glPaintHandler;

        static OpenTK.Graphics.GraphicsMode gfxmode = new OpenTK.Graphics.GraphicsMode(
             DisplayDevice.Default.BitsPerPixel,//default 32 bits color
             16,//depth buffer => 16
             8, //stencil buffer => 8 (set this if you want to use stencil buffer toos)
             0, //number of sample of FSAA (not always work)
             0, //accum buffer
             2, // n buffer, 2=> double buffer
             false);//sterio
        public MyGLControl()
            : base(gfxmode, GLES_MAJOR, GLES_MINOR, OpenTK.Graphics.GraphicsContextFlags.Embedded)
        {
            this.InitializeComponent();
        }
        public void InitSetup2d(int x, int y, int w, int h)
        {
            //TODO review here again

        }
        public void SetGLPaintHandler(EventHandler glPaintHandler)
        {
            this._glPaintHandler = glPaintHandler;
        }
        public void ClearSurface(OpenTK.Graphics.Color4 color)
        {
            MakeCurrent();
            GL.ClearColor(color);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {

            base.OnPaint(e);
            if (!this.DesignMode)
            {
                if (_glPaintHandler != null)
                {
                    MakeCurrent();
                    _glPaintHandler(this, e);
                    SwapBuffers();
                }
            }
        }
    }
}
