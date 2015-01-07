
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
    [Info(OrderCode = "22")]
    [Info("T22_PolygonTest")]
    public class T22_PolygonTest : DemoBase
    {
        static T22_PolygonTest()
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
                //if (tbrush == null)
                //{
                //    var bitmap = new System.Drawing.Bitmap("../../../SampleImages/plain01.png");
                //    bmp = new PixelFarm.Drawing.Bitmap(bitmap.Width, bitmap.Height, bitmap);
                //    tbrush = new PixelFarm.Drawing.TextureBrush(bmp);
                //}

                //fill rectangle is easy
                //canvas.FillRectangle(linearGrBrush, 0, 0, 150, 150);
                //canvas.FillRectangle(PixelFarm.Drawing.Color.Blue, 0, 0, 150, 150);
                //---------------------------------------------------- 
                //2. fill polygon with gradient brush 


                canvas.FillPolygon(
                   PixelFarm.Drawing.Color.Red,
                    new PixelFarm.Drawing.PointF[]{
                        new PixelFarm.Drawing.PointF(0, 50),
                        new PixelFarm.Drawing.PointF(50, 50),
                        new PixelFarm.Drawing.PointF(10, 100)});
                canvas.Note1 = 1; //temp


                var linearGrBrush = new LinearGradientBrush(
                    new PixelFarm.Drawing.PointF(25, 25),
                    PixelFarm.Drawing.Color.Black,
                    new PixelFarm.Drawing.PointF(100, 100),
                    PixelFarm.Drawing.Color.Blue); 

                //canvas.FillPolygon(
                //    tbrush,
                //    new PixelFarm.Drawing.PointF[]{
                //            new PixelFarm.Drawing.PointF(60, 50),
                //            new PixelFarm.Drawing.PointF(100, 50),
                //            new PixelFarm.Drawing.PointF(70, 100)});
                
                //canvas.FillPolygon(
                //    linearGrBrush,
                //    new PixelFarm.Drawing.PointF[]{
                //            new PixelFarm.Drawing.PointF(60, 50),
                //            new PixelFarm.Drawing.PointF(100, 50),
                //            new PixelFarm.Drawing.PointF(70, 100)});
                canvas.FillPolygon(
                    linearGrBrush,
                    new PixelFarm.Drawing.PointF[]{
                            new PixelFarm.Drawing.PointF(60, 50),
                            new PixelFarm.Drawing.PointF(100, 50),
                            new PixelFarm.Drawing.PointF(70, 100)});
                canvas.Note1 = 0;
                //------------------------------------------------------------------------- 
            });
            form.Show();
        }
    }
}