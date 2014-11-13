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
            this.derivedGLControl1.SetBounds(0, 0, 800, 600);

            this.Load += new EventHandler(FormTestWinGLControl_Load);
        }
        void FormTestWinGLControl_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            this.derivedGLControl1.ClearColor = LayoutFarm.Drawing.Color.White;

            if (!this.DesignMode)
            {
                //for 2d 
                var screenBound = Screen.PrimaryScreen.Bounds;
                int max = Math.Max(screenBound.Width, screenBound.Height);
                GL.Viewport(0, 0, max, max);
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadIdentity();
                GL.Ortho(0, max, 0, max, 0.0, 100.0);

                GL.MatrixMode(MatrixMode.Modelview);
                GL.LoadIdentity();

                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            }

        }
        public void SetGLPaintHandler(EventHandler handler)
        {
            this.derivedGLControl1.SetGLPaintHandler(handler);

        }
    }
}
