
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
    [Info(OrderCode = "30")]
    [Info("LeftTopWindowGradientBrush")]
    public class LeftTopWindowGradientBrush : DemoBase
    {
        static LeftTopWindowGradientBrush()
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
                canvas.Orientation = CanvasOrientation.LeftTop;
                canvas.ClearSurface(LayoutFarm.Drawing.Color.White);


                var linearGrBrush = new LinearGradientBrush(
                    new LayoutFarm.Drawing.PointF(25, 25),
                    LayoutFarm.Drawing.Color.Black,
                    new LayoutFarm.Drawing.PointF(100, 100),
                    LayoutFarm.Drawing.Color.Blue);
                 
                ////fill rectangle is easy
                ////canvas.FillRectangle(linearGrBrush, 0, 0, 150, 150);
                canvas.FillRectangle(LayoutFarm.Drawing.Color.Black, 0, 0, 150, 150);
                ////---------------------------------------------------- 

                var linearGrBrush2 = new LinearGradientBrush(
                    new LayoutFarm.Drawing.PointF(0, 50),
                    LayoutFarm.Drawing.Color.Red,
                    new LayoutFarm.Drawing.PointF(25, 100),
                    LayoutFarm.Drawing.Color.OrangeRed);
                //2. fill polygon with gradient brush 

                canvas.Note1 = 1; //temp
                canvas.FillPolygon(
                    linearGrBrush2,
                    new LayoutFarm.Drawing.PointF[]{
                        new LayoutFarm.Drawing.PointF(0, 50),
                        new LayoutFarm.Drawing.PointF(50, 50),
                        new LayoutFarm.Drawing.PointF(10, 100)});
                canvas.Note1 = 0;
                ////-------------------------------------------------------------------------

                ////another  ...                
                canvas.FillRectangle(LayoutFarm.Drawing.Color.Yellow, 200, 0, 150, 150);

                canvas.Note1 = 1; //temp
                canvas.FillPolygon(
                    linearGrBrush2,
                    new LayoutFarm.Drawing.PointF[]{
                        new LayoutFarm.Drawing.PointF(200, 50),
                        new LayoutFarm.Drawing.PointF(250, 50),
                        new LayoutFarm.Drawing.PointF(210, 100)});
                canvas.Note1 = 0;
                //-------------------------------------------------------------------------

                canvas.FillRectangle(LayoutFarm.Drawing.Color.White, 400, 0, 150, 150);

                canvas.Note1 = 1; //temp
                canvas.FillPolygon(
                    linearGrBrush2,
                    new LayoutFarm.Drawing.PointF[]{
                        new LayoutFarm.Drawing.PointF(400, 50),
                        new LayoutFarm.Drawing.PointF(450, 50),
                        new LayoutFarm.Drawing.PointF(410, 100)});
                canvas.Note1 = 0;
                //-------------------------------------------------------------------------



            });
            form.Show();
        }
    }
}