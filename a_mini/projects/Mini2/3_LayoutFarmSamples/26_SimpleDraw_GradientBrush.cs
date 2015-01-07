
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

using PixelFarm.Agg;
using PixelFarm.DrawingGL;
using PixelFarm.Drawing;

using OpenTK.Graphics.OpenGL;

namespace Mini2
{
    [Info(OrderCode = "26")]
    [Info("DrawSample06_GradientBrush")]
    public class DrawSample06_GradientBrush : DemoBase
    {
        static DrawSample06_GradientBrush()
        {
            PixelFarm.Drawing.DrawingGL.CanvasGLPortal.Start();

        }
        public override void Load()
        {
            //draw 1
            FormTestWinGLControl form = new FormTestWinGLControl();
            var canvas = PixelFarm.Drawing.DrawingGL.CanvasGLPortal.P.CreateCanvas(0, 0, 800, 600);

            form.SetGLPaintHandler((o, s) =>
            {
                canvas.ClearSurface(PixelFarm.Drawing.Color.White);

                var linearGrBrush = new LinearGradientBrush(
                    new PixelFarm.Drawing.PointF(25, 25),
                    PixelFarm.Drawing.Color.Black,
                    new PixelFarm.Drawing.PointF(100, 100),
                    PixelFarm.Drawing.Color.Blue);
                //1. 
                //fill rectangle is easy
                //canvas.FillRectangle(linearGrBrush, 0, 0, 150, 150);
                canvas.FillRectangle(PixelFarm.Drawing.Color.Black, 0, 0, 150, 150);
                //---------------------------------------------------- 
                var linearGrBrush2 = new LinearGradientBrush(
                    new PixelFarm.Drawing.PointF(0, 50),
                    PixelFarm.Drawing.Color.Red,
                    new PixelFarm.Drawing.PointF(25, 100),
                    PixelFarm.Drawing.Color.OrangeRed);
                //2. fill polygon with gradient brush 

                canvas.Note1 = 1; //temp
                canvas.FillPolygon(
                    linearGrBrush2,
                    new PixelFarm.Drawing.PointF[]{
                        new PixelFarm.Drawing.PointF(0, 50),
                        new PixelFarm.Drawing.PointF(50, 50),
                        new PixelFarm.Drawing.PointF(10, 100)});
                canvas.Note1 = 0;
                //-------------------------------------------------------------------------

                //another  ...                
                canvas.FillRectangle(PixelFarm.Drawing.Color.Yellow, 200, 0, 150, 150);

                canvas.Note1 = 1; //temp
                canvas.FillPolygon(
                    linearGrBrush2,
                    new PixelFarm.Drawing.PointF[]{
                        new PixelFarm.Drawing.PointF(200, 50),
                        new PixelFarm.Drawing.PointF(250, 50),
                        new PixelFarm.Drawing.PointF(210, 100)});
                canvas.Note1 = 0;
                //-------------------------------------------------------------------------

                canvas.FillRectangle(PixelFarm.Drawing.Color.White, 400, 0, 150, 150);

                canvas.Note1 = 1; //temp
                canvas.FillPolygon(
                    linearGrBrush2,
                    new PixelFarm.Drawing.PointF[]{
                        new PixelFarm.Drawing.PointF(400, 50),
                        new PixelFarm.Drawing.PointF(450, 50),
                        new PixelFarm.Drawing.PointF(410, 100)});
                canvas.Note1 = 0;
                //-------------------------------------------------------------------------



            });
            form.Show();
        }
    }
}