using System;
using System.Windows.Forms;
using SkiaSharp;
using Mini;
using PixelFarm.Drawing.Skia;
namespace TestSkia1
{
    partial class FormSkia1 : Form
    {
        public enum SkiaBackend
        {
            Memory,
            GLES
        }
        DemoBase exampleBase;
        SkiaBackend selectedBackend;
        SkiaCanvasPainter painter;
        public FormSkia1()
        {
            InitializeComponent();
            canvas.Visible = false;
            canvas.PaintSurface += Canvas_PaintSurface;
            glControl.Visible = true;
            glControl.PaintSurface += GlControl_PaintSurface;
            painter = new SkiaCanvasPainter(canvas.Width, canvas.Height);
            painter.SmoothingMode = PixelFarm.Drawing.SmoothingMode.AntiAlias;

        }
        public void SelectBackend(SkiaBackend backend)
        {
            switch (this.selectedBackend = backend)
            {
                default: throw new NotSupportedException();
                case SkiaBackend.Memory:
                    glControl.Visible = false;
                    canvas.Visible = true;
                    break;
                case SkiaBackend.GLES:
                    canvas.Visible = false;
                    glControl.Visible = true;
                    break;
            }

        }
        private void Canvas_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
        {
            if (exampleBase != null)
            {
                painter.Canvas = e.Surface.Canvas;
                exampleBase.Draw(painter); 
            }

        }
        private void GlControl_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintGLSurfaceEventArgs e)
        {
            if (exampleBase != null)
            {
                painter.Canvas = e.Surface.Canvas;
                exampleBase.Draw(painter);
            }
            //var skCanvas = e.Surface.Canvas;
            //skCanvas.Clear(new SkiaSharp.SKColor(255, 255, 255));

            //using (var paint = new SKPaint())
            //{
            //    paint.TextSize = 36.0f;
            //    paint.IsAntialias = true;
            //    paint.Color = (SKColor)0xFF4281A4;
            //    paint.IsStroke = false;
            //    skCanvas.DrawText("PixelFarm+SkiaSharp", 20, 64.0f, paint);
            //    paint.StrokeWidth = 3;
            //    skCanvas.DrawLine(0, 0, 100, 80, paint);
            //}
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
