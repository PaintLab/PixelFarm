using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Drawing;
using System.Text;
using System.Windows.Forms;

using OpenTK.Graphics.OpenGL;
using Mini;

namespace OpenTkEssTest
{

    public partial class FormTestWinGLControl2 : Form
    {

        public FormTestWinGLControl2()
        {
            InitializeComponent();
            this.Load += new EventHandler(FormTestWinGLControl_Load);
        }
        void FormTestWinGLControl_Load(object sender, EventArgs e)
        {
            this.derivedGLControl1.ClearColor = LayoutFarm.Drawing.Color.White;

            if (!this.DesignMode)
            {
                //for 2d 
                GL.Viewport(0, 0, 800, 800);
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadIdentity();
                GL.Ortho(0, 10.0, 0, 10.0, 0.0, 100.0);

                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadIdentity();
            }

        }
        public void SetGLPaintHandler(EventHandler handler)
        {
            this.derivedGLControl1.SetGLPaintHandler(handler);

        }
    }
}
