using System;
using System.Windows.Forms;
using SkiaSharp;

namespace TestSkia1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            glControl.Visible = false;
            canvas.Visible = true;

            glControl.PaintSurface += GlControl_PaintSurface;
            canvas.PaintSurface += Canvas_PaintSurface;
        }

        private void Canvas_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs e)
        {
            var skCanvas = e.Surface.Canvas;
            skCanvas.Clear(new SkiaSharp.SKColor(255, 255, 255)); 

            using (var paint = new SKPaint())
            {
                paint.TextSize = 36.0f;
                paint.IsAntialias = true;
                paint.Color = (SKColor)0xFF4281A4;
                paint.IsStroke = false; 
                skCanvas.DrawText("PixelFarm+SkiaSharp", 20, 64.0f, paint);
            } 

        }
        private void GlControl_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintGLSurfaceEventArgs e)
        {
            var skCanvas = e.Surface.Canvas;

        }
    }
}
