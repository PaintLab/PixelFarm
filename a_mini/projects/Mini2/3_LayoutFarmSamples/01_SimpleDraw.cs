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

        public override void Load()
        {
            //draw 1
            FormTestWinGLControl form = new FormTestWinGLControl();
            CanvasGL2d canvas = new CanvasGL2d();

            var gdiCanvas = WinGdiPortal.P.CreateCanvas(0, 0, 0, 0, 800, 600);

            form.SetGLPaintHandler((o, s) =>
            {



            });
            form.Show();
        }
    }
}