using System;
using System.Windows.Forms;
using PixelFarm.DrawingGL;

namespace Mini
{
    public partial class FormGLTest : Form
    {
        MyMiniGLES2Control miniGLControl;
        IntPtr hh1;
        CanvasGL2d canvas2d;
        public FormGLTest()
        {
            InitializeComponent();
        }
        public void InitGLControl()
        {
            InitMiniGLControl(800, 600);
        }
        MyMiniGLES2Control InitMiniGLControl(int w, int h)
        {
            if (miniGLControl == null)
            {
                miniGLControl = new MyMiniGLES2Control();
                miniGLControl.Width = w;
                miniGLControl.Height = h;
                miniGLControl.ClearColor = PixelFarm.Drawing.Color.Blue;
                this.Controls.Add(miniGLControl);

                miniGLControl.SetGLPaintHandler(HandleGLPaint);
                hh1 = miniGLControl.Handle;
                miniGLControl.MakeCurrent();

                int max = Math.Max(this.Width, this.Height);
                canvas2d = new CanvasGL2d(max, max);
            }
            return miniGLControl;
        }
        public MyMiniGLES2Control MiniGLControl
        {
            get { return this.miniGLControl; }
        }
        void HandleGLPaint(object sender, System.EventArgs e)
        {
            canvas2d.SmoothMode = CanvasSmoothMode.Smooth;
            canvas2d.StrokeColor = PixelFarm.Drawing.Color.Blue;
            canvas2d.ClearColorBuffer();
           canvas2d.FillRect(PixelFarm.Drawing.Color.Black, 20, 20, 150, 150);
            //load bmp image 
            //------------------------------------------------------------------------- 
        
            miniGLControl.SwapBuffers();
        }
    }
}
