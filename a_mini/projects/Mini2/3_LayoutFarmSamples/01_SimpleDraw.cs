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
            CanvasGLPortal.Start();
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

            var softCanvas = CanvasGLPortal.P.CreateCanvas(0, 0, 800, 600);

            form.SetGLPaintHandler((o, s) =>
            {
                softCanvas.ClearSurface(LayoutFarm.Drawing.Color.White);
                softCanvas.FillRectangle(
                  LayoutFarm.Drawing.Color.Blue,
                  0, 0, 200, 200);

                softCanvas.SetCanvasOrigin(50, 50);
                
                softCanvas.FillRectangle(
                  LayoutFarm.Drawing.Color.Green,
                  0, 0, 200, 200);

                softCanvas.SetCanvasOrigin(0, 0);
            });
            form.Show();
        }
    }
}