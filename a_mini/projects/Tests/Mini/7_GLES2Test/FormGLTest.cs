//MIT, 2014-2017, WinterDev
using System;

using System.Windows.Forms;
using PixelFarm.DrawingGL;
using OpenTK;

namespace Mini
{
    partial class FormGLTest : Form
    {
        MyGLControl miniGLControl;
        IntPtr hh1;
        CanvasGL2d canvas2d;
        DemoBase exampleBase;
        GLCanvasPainter canvasPainter;
        public FormGLTest()
        {
            InitializeComponent();
        }
        public void LoadExample(ExampleAndDesc exAndDesc)
        {
            DemoBase exBase = Activator.CreateInstance(exAndDesc.Type) as DemoBase;
            if (exBase == null)
            {
                return;
            }

            this.Text = exAndDesc.ToString();
            //----------------------------------------------------------------------------
            this.exampleBase = exBase;
            exampleBase.Init();
        }
        public MyGLControl InitMiniGLControl(int w, int h)
        {
            if (miniGLControl == null)
            {
                miniGLControl = new MyGLControl();
                miniGLControl.Width = w;
                miniGLControl.Height = h;
                this.Controls.Add(miniGLControl); 
            }
            return miniGLControl;
        }

        void HandleGLPaint(object sender, System.EventArgs e)
        {
            canvas2d.SmoothMode = CanvasSmoothMode.Smooth;
            canvas2d.StrokeColor = PixelFarm.Drawing.Color.Black;
            canvas2d.ClearColorBuffer();
            //example
            canvasPainter.FillColor = PixelFarm.Drawing.Color.Black;
            canvasPainter.FillRectLBWH(20, 20, 150, 150);
            //load bmp image 
            //------------------------------------------------------------------------- 
            if (exampleBase != null)
            {
                exampleBase.Draw(canvasPainter);
            }
            miniGLControl.SwapBuffers();
        }
        
    }
}
