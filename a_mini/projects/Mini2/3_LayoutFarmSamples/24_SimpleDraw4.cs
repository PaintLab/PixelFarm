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
    [Info(OrderCode = "24")]
    [Info("DrawSample04")]
    public class DrawSample04 : DemoBase
    {
        static DrawSample04()
        {
            PixelFarm.Drawing.DrawingGL.CanvasGLPortal.Start();

        }
        public override void Load()
        {
            //draw 1
            FormTestWinGLControl form = new FormTestWinGLControl();
            var platform = PixelFarm.Drawing.DrawingGL.CanvasGLPortal.P;
            var canvas = platform.CreateCanvas(0, 0, 800, 600);
            GraphicsPath path = null;
            FontInfo fontinfo = platform.GetFont("tahoma", 24, PixelFarm.Drawing.FontStyle.Regular);
            canvas.CurrentFont = fontinfo.ResolvedFont;

            form.SetGLPaintHandler((o, s) =>
            {
                //if (fontinfo == null)
                //{
                //    fontinfo = platform.CreateNativeFontWrapper(new System.Drawing.Font("tahoma", 24));
                //    canvas.CurrentFont = fontinfo.ResolvedFont;
                //}
                canvas.ClearSurface(PixelFarm.Drawing.Color.White);
                canvas.FillRectangle(
                  PixelFarm.Drawing.Color.Blue,
                 100, 10, 200, 200);
                canvas.DrawRectangle(
                 PixelFarm.Drawing.Color.Black,
                 100, 10, 200, 200);

                //------------
                //graphics paths
                if (path == null)
                {
                    path = platform.CreateGraphicsPath();
                    //path.AddRectangle(new PixelFarm.Drawing.RectangleF(10, 10, 10, 10));
                    path.AddEllipse(30, 30, 25, 25);

                }
                //canvas.FillPath(path);
                canvas.DrawPath(path);
                //------------
                //test draw text
                canvas.DrawText("ABCD".ToCharArray(), 100, 500);
                //------------

            });
            form.Show();
        }
    }
}