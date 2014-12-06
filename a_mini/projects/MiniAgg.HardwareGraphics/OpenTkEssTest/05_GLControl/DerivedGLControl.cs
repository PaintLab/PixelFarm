using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

using System.Text;
using System.Windows.Forms;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace OpenTkEssTest
{
    public partial class DerivedGLControl : GLControl
    {
        LayoutFarm.Drawing.Color clearColor;
        EventHandler glPaintHandler;
        public DerivedGLControl()
        {
            this.InitializeComponent();
        }
        public void SetGLPaintHandler(EventHandler glPaintHandler)
        {
            this.glPaintHandler = glPaintHandler;
        }
        public LayoutFarm.Drawing.Color ClearColor
        {
            get { return clearColor; }
            set
            {
                clearColor = value;

                if (!this.DesignMode)
                {
                    MakeCurrent();
                    GL.ClearColor(clearColor);
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (!this.DesignMode)
            {

                MakeCurrent();
                //GL.Clear(ClearBufferMask.ColorBufferBit);
                if (glPaintHandler != null)
                {
                    glPaintHandler(this, e);
                }              
                SwapBuffers();
            }
        }
    }
}
