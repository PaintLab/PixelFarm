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
    [Info(OrderCode = "03")]
    [Info("Drawing3")]
    public class DrawSample03 : DemoBase
    {
        static DrawSample03()
        {
            LayoutFarm.Drawing.DrawingGL.CanvasGLPortal.Start();

        }

        static LayoutFarm.Drawing.RectangleF GenRect(float ux, float uy, float uw, float uh)
        {
            //from user coord to cartesian coord
            return new LayoutFarm.Drawing.RectangleF(ux, uy + uh, uw, uh);
        }
        public override void Load()
        {
            //draw 1
            FormTestWinGLControl form = new FormTestWinGLControl();
            //form.GetCanvasControl().Visible = false;
            //System.Windows.Forms.Panel pp = new Panel();
            //pp.Size = new System.Drawing.Size(500, 500);
            //pp.BackColor = System.Drawing.Color.Black;
            //form.Controls.Add(pp);
            //WinGdiPortal.P.CreateCanvas(0, 0, 800, 600); 

            var canvas = LayoutFarm.Drawing.DrawingGL.CanvasGLPortal.P.CreateCanvas(0, 0, 800, 600);

            LayoutFarm.Drawing.Bitmap bmp = null;
            form.SetGLPaintHandler((o, s) =>
            {
                canvas.ClearSurface(LayoutFarm.Drawing.Color.White);
                canvas.FillRectangle(
                  LayoutFarm.Drawing.Color.Blue,
                  0, 0, 200, 200);

                canvas.SetCanvasOrigin(180, 180);
                //----------------------
                canvas.FillRectangle(
                  LayoutFarm.Drawing.Color.Green,
                  0, 0, 100, 100);
                //----------------------
                canvas.SetCanvasOrigin(0, 0);
                //----------------------
                //drawimgage
                if (bmp == null)
                {
                    var bitmap = new System.Drawing.Bitmap("../../../SampleImages/plain01.png");
                    bmp = new LayoutFarm.Drawing.Bitmap(bitmap.Width, bitmap.Height, bitmap);
                }

                //draw full image to destination
                //canvas.DrawImage(bmp, GenRect(0, 0, bmp.Width / 2, bmp.Height / 2));
                canvas.DrawImage(bmp, new LayoutFarm.Drawing.RectangleF(0, 0, bmp.Width / 2, bmp.Height / 2));
                //----------------------
                //1. draw from some part of src image to dest
                //canvas.DrawImage(bmp,
                //    //dest
                //    //new LayoutFarm.Drawing.RectangleF(0, 350, bmp.Width, bmp.Height),
                //    new LayoutFarm.Drawing.RectangleF(0, 350, bmp.Width, bmp.Height),
                //    //src
                //    new LayoutFarm.Drawing.RectangleF(0, 0, 100, 100));
                canvas.DrawImage(bmp,
                    //dest
                    //new LayoutFarm.Drawing.RectangleF(0, 350, bmp.Width, bmp.Height),
                   new LayoutFarm.Drawing.RectangleF(0, 350, bmp.Width / 2, bmp.Height / 2),
                    //src
                   new LayoutFarm.Drawing.RectangleF(0, 0, bmp.Width, bmp.Height));

                ////----------------------
                canvas.DrawImage(bmp,
                    //dest 
                    new LayoutFarm.Drawing.RectangleF(350, 350, bmp.Width / 2, bmp.Height / 2),
                    //src
                    new LayoutFarm.Drawing.RectangleF(100, 100, bmp.Width - 100, bmp.Height - 100));
                ////----------------------
                //2. 

                {
                    var refBmp = new LayoutFarm.Drawing.ReferenceBitmap(bmp, 50, 50, 100, 100);
                    canvas.DrawImage(refBmp,
                        new LayoutFarm.Drawing.RectangleF(300, 100, refBmp.Width, refBmp.Height));
                }
                ////----------------------
                //3. small glyph 
                int startAt = 50;
                int lineHeight = 20;
                int drawXPos = 300;
                int drawYPos = 50;
                for (int i = 0; i < 5; ++i)
                {
                    var refBmp = new LayoutFarm.Drawing.ReferenceBitmap(bmp, 50, startAt, 50, 20);
                    canvas.DrawImage(refBmp,
                        new LayoutFarm.Drawing.RectangleF(drawXPos, drawYPos, refBmp.Width, refBmp.Height));
                    drawXPos += 50;
                    startAt += lineHeight;
                }

                canvas.StrokeColor = LayoutFarm.Drawing.Color.Blue;

                canvas.DrawLine(0, 0, 800, 800);
            });
            form.Show();
        }
    }
}