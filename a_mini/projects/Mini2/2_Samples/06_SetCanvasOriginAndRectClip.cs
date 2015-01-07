using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

using PixelFarm.Agg;
using PixelFarm.DrawingGL;
namespace Mini2
{
    [Info(OrderCode = "06")]
    [Info("SetCanvasOriginAndRectClip")]
    public class SetCanvasOriginAndRectClip : DemoBase
    {
        public override void Load()
        {
            //-----------------------------------------------
            FormTestWinGLControl form = new FormTestWinGLControl();
            CanvasGL2d canvas = new CanvasGL2d(this.Width, this.Height);
            var lionFill = new LionFillSprite();
            //----------------------------------------------- 
            form.SetGLPaintHandler((o, s) =>
            {
                //canvas.Orientation = PixelFarm.Drawing.CanvasOrientation.LeftTop;
                canvas.Clear(PixelFarm.Drawing.Color.White);
                canvas.FillRect(PixelFarm.Drawing.Color.Blue, 0, 0, 400, 400);
                //draw vxs direct to GL surface 
                //lionFill.Draw(canvas); //before offset

                canvas.EnableClipRect();
                canvas.SetClipRectRel(200, 250, 100, 100);

                lionFill.Draw(canvas); 
                //offset
                canvas.SetCanvasOrigin(50, 50); 
                //test clipping                
                canvas.SetClipRectRel(200, 250, 100, 100);

                lionFill.Draw(canvas);
                canvas.DisableClipRect();


                canvas.SetCanvasOrigin(0, 0);
            });
            form.Show();
        }
    }
}