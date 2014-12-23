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
            FontInfo fontinfo = null;
            LayoutFarm.Drawing.DrawingGL.CanvasGLPortal.P.CreateNativeFontWrapper(new System.Drawing.Font("tahoma", 24));


            form.SetGLPaintHandler((o, s) =>
            {
                if (fontinfo == null)
                {
                    fontinfo = LayoutFarm.Drawing.DrawingGL.CanvasGLPortal.P.CreateNativeFontWrapper(new System.Drawing.Font("tahoma", 24));
                    canvas.CurrentFont = fontinfo.ResolvedFont;
                }
                canvas.ClearSurface(LayoutFarm.Drawing.Color.White);
                canvas.FillRectangle(LayoutFarm.Drawing.Color.White, 0, 0, 400, 400);
                //test draw text
                canvas.Note1 = 2;
                canvas.DrawText("AaBbCc0123 +*/%$".ToCharArray(), 0, 0);
                canvas.Note1 = 0;
                canvas.DrawText("AaBbCc0123 +*/%$".ToCharArray(), 0, 100); 

            });
            form.Show();
        }
    }
}