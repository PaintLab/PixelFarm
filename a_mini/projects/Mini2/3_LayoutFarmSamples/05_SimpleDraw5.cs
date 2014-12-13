using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

using PixelFarm.Agg;
using LayoutFarm.DrawingGL;
using LayoutFarm.Drawing;


namespace Mini2
{
    [Info(OrderCode = "05")]
    [Info("Drawing5")]
    public class DrawSample05 : DemoBase
    {
        static DrawSample05()
        {
            LayoutFarm.Drawing.DrawingGL.CanvasGLPortal.Start();

        }
        public override void Load()
        {
            //draw 1
            FormTestWinGLControl form = new FormTestWinGLControl();
            var canvas = LayoutFarm.Drawing.DrawingGL.CanvasGLPortal.P.CreateCanvas(0, 0, 800, 600);


            form.SetGLPaintHandler((o, s) =>
            {
                //test gradient brush
                canvas.ClearSurface(LayoutFarm.Drawing.Color.White);
                LinearGradientBrush linearGrBrush = new LinearGradientBrush(
                     new LayoutFarm.Drawing.PointF(25, 25),
                     LayoutFarm.Drawing.Color.Black,
                     new LayoutFarm.Drawing.PointF(100, 100),
                     LayoutFarm.Drawing.Color.Blue);

                canvas.FillRectangle(
                    linearGrBrush,
                    0, 0, 200, 200); 

            });
            form.Show();
        }
    }
}