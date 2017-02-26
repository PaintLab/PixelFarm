using System;
using System.Windows.Forms;
using PixelFarm.DrawingGL;
namespace Mini
{
    partial class FormGLTest : Form
    {
        MyMiniGLES2Control miniGLControl;
        IntPtr hh1;
        CanvasGL2d canvas2d;
        DemoBase exampleBase;
        GLCanvasPainter canvasPainter;
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
                canvas2d = PixelFarm.Drawing.GLES2.GLES2Platform.CreateCanvasGL2d(max, max);
                canvasPainter = new GLCanvasPainter(canvas2d, max, max);
                //create text printer for opengl

                //1. win gdi based
                //var printer = new WinGdiFontPrinter(canvas2d, w, h);
                //canvasPainter.TextPrinter = printer;
                //----------------------
                //2. raw vxs
                var printer = new PixelFarm.Drawing.Fonts.TextPrinter(canvasPainter);
                canvasPainter.TextPrinter = printer;
                //----------------------
                //3. agg texture based font texture
                //var printer = new AggFontPrinter(canvasPainter, w, h);
                //canvasPainter.TextPrinter = printer;
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
    }
}
