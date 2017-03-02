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
        public void InitGLControl()
        {
            InitMiniGLControl(800, 600);
        }
        public MyGLControl InitMiniGLControl(int w, int h)
        {
            if (miniGLControl == null)
            {
                miniGLControl = new MyGLControl();
                miniGLControl.Width = w;
                miniGLControl.Height = h;
                //miniGLControl.ClearColor = PixelFarm.Drawing.Color.Blue;
                this.Controls.Add(miniGLControl);


                //miniGLControl.SetGLPaintHandler(HandleGLPaint);
                //hh1 = miniGLControl.Handle;
                //miniGLControl.MakeCurrent();
                //int max = Math.Max(this.Width, this.Height);
                //canvas2d = PixelFarm.Drawing.GLES2.GLES2Platform.CreateCanvasGL2d(max, max);
                //canvasPainter = new GLCanvasPainter(canvas2d, max, max);
                ////create text printer for opengl 
                ////----------------------
                ////1. win gdi based
                ////var printer = new WinGdiFontPrinter(canvas2d, w, h);
                ////canvasPainter.TextPrinter = printer;
                ////----------------------
                ////2. raw vxs
                ////var printer = new PixelFarm.Drawing.Fonts.VxsTextPrinter(canvasPainter);
                ////canvasPainter.TextPrinter = printer;
                ////----------------------
                ////3. agg texture based font texture
                ////var printer = new AggFontPrinter(canvasPainter, w, h);
                ////canvasPainter.TextPrinter = printer;
                ////----------------------
                ////4. texture atlas based font texture

                ////------------
                ////resolve request font


                //var printer = new GLBmpGlyphTextPrinter(canvasPainter, YourImplementation.BootStrapWinGdi.myFontLoader);
                //canvasPainter.TextPrinter = printer;
            }
            return miniGLControl;
        }

        //public MyGLControl MiniGLControl
        //{
        //    get { return this.miniGLControl; }
        //}
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
