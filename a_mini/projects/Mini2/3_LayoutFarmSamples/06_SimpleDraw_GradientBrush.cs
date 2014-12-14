
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

using PixelFarm.Agg;
using LayoutFarm.DrawingGL;
using LayoutFarm.Drawing;

using OpenTK.Graphics.OpenGL;

namespace Mini2
{
    [Info(OrderCode = "06")]
    [Info("DrawSample06_GradientBrush")]
    public class DrawSample06_GradientBrush : DemoBase
    {
        static DrawSample06_GradientBrush()
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
                canvas.ClearSurface(LayoutFarm.Drawing.Color.White);
                var linearGrBrush = new LinearGradientBrush(
                    new LayoutFarm.Drawing.PointF(25, 25),
                    LayoutFarm.Drawing.Color.Black,
                    new LayoutFarm.Drawing.PointF(100, 100),
                    LayoutFarm.Drawing.Color.Blue);
                //1. 
                //fill rectangle is easy
                canvas.FillRectangle(linearGrBrush,
                    0, 0, 150, 150);
                //---------------------------------------------------- 
                var linearGrBrush2 = new LinearGradientBrush(
                    new LayoutFarm.Drawing.PointF(0, 200),
                    LayoutFarm.Drawing.Color.Yellow,
                    new LayoutFarm.Drawing.PointF(25, 250),
                    LayoutFarm.Drawing.Color.Blue);

                //2. fill polygon with gradient brush
                canvas.CurrentBrush = linearGrBrush2;
                canvas.FillPolygon(
                    new LayoutFarm.Drawing.PointF[]{
                        new LayoutFarm.Drawing.PointF(0, 200),
                        new LayoutFarm.Drawing.PointF(50, 200),
                        new LayoutFarm.Drawing.PointF(25, 250)});


            });
            form.Show();
        }
    }
}