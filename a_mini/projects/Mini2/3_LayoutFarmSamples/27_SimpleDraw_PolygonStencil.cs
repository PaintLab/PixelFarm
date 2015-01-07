
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

using PixelFarm.Agg;
using PixelFarm.DrawingGL;
using PixelFarm.Drawing;
 

namespace Mini2
{
    [Info(OrderCode = "27")]
    [Info("DrawSample07_GradientBrush")]
    public class DrawSample07_PolygonStencil : DemoBase
    {
        static DrawSample07_PolygonStencil()
        {
            PixelFarm.Drawing.DrawingGL.CanvasGLPortal.Start();

        }
        public override void Load()
        {
            //draw 1
            FormTestWinGLControl form = new FormTestWinGLControl();
            var canvas = PixelFarm.Drawing.DrawingGL.CanvasGLPortal.P.CreateCanvas(0, 0, 800, 600);

            PixelFarm.Drawing.Bitmap bmp = null;
            PixelFarm.Drawing.TextureBrush tbrush = null;
            form.SetGLPaintHandler((o, s) =>
            {
                canvas.ClearSurface(PixelFarm.Drawing.Color.White);
                if (tbrush == null)
                {
                    var bitmap = new System.Drawing.Bitmap("../../../SampleImages/plain01.png");
                    bmp = new PixelFarm.Drawing.Bitmap(bitmap.Width, bitmap.Height, bitmap);
                    tbrush = new PixelFarm.Drawing.TextureBrush(bmp);
                }
                //2. fill polygon with gradient brush  
                canvas.FillPolygon(
                    tbrush,
                    new PixelFarm.Drawing.PointF[]{
                            new PixelFarm.Drawing.PointF(60, 50),
                            new PixelFarm.Drawing.PointF(100, 50),
                            new PixelFarm.Drawing.PointF(70, 100)});

                canvas.Note1 = 1; //temp
                canvas.FillPolygon(
                    tbrush,
                    new PixelFarm.Drawing.PointF[]{
                        new PixelFarm.Drawing.PointF(0, 50),
                        new PixelFarm.Drawing.PointF(50, 50),
                        new PixelFarm.Drawing.PointF(10, 100)});

                canvas.Note1 = 0;
                //------------------------------------------------------------------------- 
            });
            form.Show();
        }
    }
}