using System;
using System.Windows.Forms; 

namespace OpenTK
{
    public partial class MyGLControl : GLControl
    {
        EventHandler _externalGLPaintHandler;

        static OpenTK.Graphics.GraphicsMode s_gfxmode = new OpenTK.Graphics.GraphicsMode(
             DisplayDevice.Default.BitsPerPixel,//default 32 bits color
             16,//depth buffer => 16
             8, //stencil buffer => 8 (set this if you want to use stencil buffer toos)
             0, //number of sample of FSAA (not always work)
             0, //accum buffer
             2, // n buffer, 2=> double buffer
             false);//sterio
        //
        public MyGLControl()
            : base(s_gfxmode,
                  MinimalGLContextVersion.GLES_MAJOR,
                  MinimalGLContextVersion.GLES_MINOR,
                  OpenTK.Graphics.GraphicsContextFlags.Embedded |
                  Graphics.GraphicsContextFlags.Angle |
                  Graphics.GraphicsContextFlags.AngleD3D11 |
                  Graphics.GraphicsContextFlags.AngleD3D9)
        {

            this.InitializeComponent();
        }
       
        public void SetExternalGLPaintHandler(EventHandler externalGLPaintHandler)
        {
            _externalGLPaintHandler = externalGLPaintHandler;
        }
        
        
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (!this.DesignMode)
            {
                if (_externalGLPaintHandler != null)
                {
                    MakeCurrent();
                    _externalGLPaintHandler(this, e);
                    SwapBuffers();
                }
            }
        }
    }
}
