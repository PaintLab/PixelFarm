
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
    [Info(OrderCode = "22")]
    [Info("T22_PolygonTest")]
    public class T22_PolygonTest : DemoBase
    {
        static T22_PolygonTest()
        {
            LayoutFarm.Drawing.DrawingGL.CanvasGLPortal.Start();

        }
        public override void Load()
        {
            //draw 1
            FormTestWinGLControl form = new FormTestWinGLControl();
            var canvas = LayoutFarm.Drawing.DrawingGL.CanvasGLPortal.P.CreateCanvas(0, 0, 800, 600);

            LayoutFarm.Drawing.Bitmap bmp = null;
            LayoutFarm.Drawing.TextureBrush tbrush = null;
            form.SetGLPaintHandler((o, s) =>
            {
                canvas.ClearSurface(LayoutFarm.Drawing.Color.White);
                //if (tbrush == null)
                //{
                //    var bitmap = new System.Drawing.Bitmap("../../../SampleImages/plain01.png");
                //    bmp = new LayoutFarm.Drawing.Bitmap(bitmap.Width, bitmap.Height, bitmap);
                //    tbrush = new LayoutFarm.Drawing.TextureBrush(bmp);
                //}

                //fill rectangle is easy
                //canvas.FillRectangle(linearGrBrush, 0, 0, 150, 150);
                //canvas.FillRectangle(LayoutFarm.Drawing.Color.Blue, 0, 0, 150, 150);
                //---------------------------------------------------- 
                //2. fill polygon with gradient brush 


                canvas.FillPolygon(
                   LayoutFarm.Drawing.Color.Red,
                    new LayoutFarm.Drawing.PointF[]{
                        new LayoutFarm.Drawing.PointF(0, 50),
                        new LayoutFarm.Drawing.PointF(50, 50),
                        new LayoutFarm.Drawing.PointF(10, 100)});
                canvas.Note1 = 1; //temp


                var linearGrBrush = new LinearGradientBrush(
                    new LayoutFarm.Drawing.PointF(25, 25),
                    LayoutFarm.Drawing.Color.Black,
                    new LayoutFarm.Drawing.PointF(100, 100),
                    LayoutFarm.Drawing.Color.Blue); 

                //canvas.FillPolygon(
                //    tbrush,
                //    new LayoutFarm.Drawing.PointF[]{
                //            new LayoutFarm.Drawing.PointF(60, 50),
                //            new LayoutFarm.Drawing.PointF(100, 50),
                //            new LayoutFarm.Drawing.PointF(70, 100)});
                
                //canvas.FillPolygon(
                //    linearGrBrush,
                //    new LayoutFarm.Drawing.PointF[]{
                //            new LayoutFarm.Drawing.PointF(60, 50),
                //            new LayoutFarm.Drawing.PointF(100, 50),
                //            new LayoutFarm.Drawing.PointF(70, 100)});
                canvas.FillPolygon(
                    linearGrBrush,
                    new LayoutFarm.Drawing.PointF[]{
                            new LayoutFarm.Drawing.PointF(60, 50),
                            new LayoutFarm.Drawing.PointF(100, 50),
                            new LayoutFarm.Drawing.PointF(70, 100)});
                canvas.Note1 = 0;
                //------------------------------------------------------------------------- 
            });
            form.Show();
        }
    }
}