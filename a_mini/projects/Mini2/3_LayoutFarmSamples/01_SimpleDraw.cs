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

                canvas.DrawImage(bmp, new LayoutFarm.Drawing.RectangleF(0, 0, bmp.Width, bmp.Height));

                canvas.DrawImage(bmp,
                    //dest
                    new LayoutFarm.Drawing.RectangleF(0, 350, bmp.Width, bmp.Height),
                    //src
                    new LayoutFarm.Drawing.RectangleF(50, 50, 100, 100));
                //----------------------
                canvas.StrokeColor = LayoutFarm.Drawing.Color.Blue;

                canvas.DrawLine(0, 0, 400, 400);
            });
            form.Show();
        }
    }
}