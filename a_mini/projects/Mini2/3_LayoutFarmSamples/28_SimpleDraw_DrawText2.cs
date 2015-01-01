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
    [Info(OrderCode = "28")]
    [Info("DrawSample08_DrawText2")]
    public class DrawSample08_DrawText2 : DemoBase
    {
        static DrawSample08_DrawText2()
        {
            LayoutFarm.Drawing.DrawingGL.CanvasGLPortal.Start();

        }
        public override void Load()
        {
            //draw 1
            FormTestWinGLControl form = new FormTestWinGLControl();
            var canvas = LayoutFarm.Drawing.DrawingGL.CanvasGLPortal.P.CreateCanvas(0, 0, 800, 600);

            form.SetOnDestroyHandler((o, s) =>
            {
                canvas.Dispose();
            });



            FontInfo fontinfo = null; 
            form.SetGLPaintHandler((o, s) =>
            {
                canvas.Orientation = CanvasOrientation.LeftTop;
                if (fontinfo == null)
                {
                    fontinfo = LayoutFarm.Drawing.DrawingGL.CanvasGLPortal.GetFontInfo("tahoma", 10, LayoutFarm.Drawing.DrawingGL.FontLoadTechnique.GdiBitmapFont);
                    canvas.CurrentFont = fontinfo.ResolvedFont;
                }
                canvas.ClearSurface(LayoutFarm.Drawing.Color.White);
                canvas.FillRectangle(LayoutFarm.Drawing.Color.Yellow, 3, 3, 200, 200);
                canvas.DrawRectangle(LayoutFarm.Drawing.Color.Red, 0, 0, 400, 400);
                //test draw text

                canvas.DrawText("AaBbCc0123 +*/%$".ToCharArray(), 0, 0);

                canvas.CurrentTextColor = LayoutFarm.Drawing.Color.Red;
                canvas.DrawText("AaBbCc0123 +*/%$".ToCharArray(), 0, 50);

                canvas.CurrentTextColor = LayoutFarm.Drawing.Color.Green;
                canvas.DrawText("AaBbCc0123 +*/%$".ToCharArray(), 0, 100);

                canvas.CurrentTextColor = LayoutFarm.Drawing.Color.Blue;
                canvas.DrawText("AaBbCc0123 +*/%$".ToCharArray(), 0, 150);

                canvas.CurrentTextColor = LayoutFarm.Drawing.Color.Black;
            });
            form.Show();
            
            
        }
    }
}