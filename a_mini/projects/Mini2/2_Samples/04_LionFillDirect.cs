using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

using PixelFarm.Agg;
using LayoutFarm.DrawingGL;
namespace Mini2
{
    [Info(OrderCode = "04")]
    [Info("LionFillDirect")]
    public class LionFillDirectDemo : DemoBase
    {
        public override void Load()
        {
            //-----------------------------------------------
            //test draw with LayoutFarm.Canvas interface ***
            //with OpenGL backend ***
            //-----------------------------------------------
            FormTestWinGLControl form = new FormTestWinGLControl();
            CanvasGL2d canvas = new CanvasGL2d();
            var lionFill = new LionFillSprite();
            //----------
            //draw lion on software layer
            ActualImage actualImage = new ActualImage(800, 600, PixelFarm.Agg.Image.PixelFormat.Rgba32);
            Graphics2D g2d = Graphics2D.CreateFromImage(actualImage);
            lionFill.OnDraw(g2d);
 
             
            form.SetGLPaintHandler((o, s) =>
            {

                canvas.Clear(LayoutFarm.Drawing.Color.White);
                //draw vxs direct to GL surface
                lionFill.Draw(canvas);

            });
            form.Show();
        }
    }
}