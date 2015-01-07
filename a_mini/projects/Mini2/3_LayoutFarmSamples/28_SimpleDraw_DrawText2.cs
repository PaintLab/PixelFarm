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
    [Info(OrderCode = "28")]
    [Info("DrawSample08_DrawText2")]
    public class DrawSample08_DrawText2 : DemoBase
    {
        static DrawSample08_DrawText2()
        {
            PixelFarm.Drawing.DrawingGL.CanvasGLPortal.Start();

        }
        public override void Load()
        {
            //draw 1
            FormTestWinGLControl form = new FormTestWinGLControl();
            var canvas = PixelFarm.Drawing.DrawingGL.CanvasGLPortal.P.CreateCanvas(0, 0, 800, 600);

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
                    fontinfo = PixelFarm.Drawing.DrawingGL.CanvasGLPortal.GetFontInfo("tahoma", 10, PixelFarm.Drawing.DrawingGL.FontLoadTechnique.GdiBitmapFont);
                    canvas.CurrentFont = fontinfo.ResolvedFont;
                }
                canvas.ClearSurface(PixelFarm.Drawing.Color.White);
                canvas.FillRectangle(PixelFarm.Drawing.Color.Yellow, 3, 3, 200, 200);
                canvas.DrawRectangle(PixelFarm.Drawing.Color.Red, 0, 0, 400, 400);
                //test draw text

                canvas.DrawText("AaBbCc0123 +*/%$".ToCharArray(), 0, 0);

                canvas.CurrentTextColor = PixelFarm.Drawing.Color.Red;
                canvas.DrawText("AaBbCc0123 +*/%$".ToCharArray(), 0, 50);

                canvas.CurrentTextColor = PixelFarm.Drawing.Color.Green;
                canvas.DrawText("AaBbCc0123 +*/%$".ToCharArray(), 0, 100);

                canvas.CurrentTextColor = PixelFarm.Drawing.Color.Blue;
                canvas.DrawText("AaBbCc0123 +*/%$".ToCharArray(), 0, 150);

                canvas.CurrentTextColor = PixelFarm.Drawing.Color.Black;
            });
            form.Show();
            
            
        }
    }
}