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
    [Info(OrderCode = "04")]
    [Info("Drawing4")]
    public class DrawSample04 : DemoBase
    {
        static DrawSample04()
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

            GraphicsPath path= null;
            LayoutFarm.Drawing.Bitmap bmp = null;
            form.SetGLPaintHandler((o, s) =>
            {
                canvas.ClearSurface(LayoutFarm.Drawing.Color.White);
                canvas.FillRectangle(
                  LayoutFarm.Drawing.Color.Blue,
                 100, 10, 200, 200);
                canvas.DrawRectangle(
                 LayoutFarm.Drawing.Color.Black,
                 100, 10, 200, 200);

                //------------
                //graphics paths
                if (path == null)
                {
                    path = CurrentGraphicsPlatform.CreateGraphicPath();
                    //path.AddRectangle(new LayoutFarm.Drawing.RectangleF(10, 10, 10, 10));
                    path.AddEllipse(30, 30, 25, 25);

                }
                canvas.FillPath(path);
            });
            form.Show();
        }
    }
}