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
    [Info(OrderCode = "03")]
    [Info("LionFill on GL")]
    public class LionFillOnGL : DemoBase
    {

        public override void Load()
        {
            //lion fill test 
            FormTestWinGLControl form = new FormTestWinGLControl();
            CanvasGL2d canvas = new CanvasGL2d();
            var lionFill = new LionFillSprite();
            //----------
            //draw lion on software layer
            ActualImage actualImage = new ActualImage(800, 600, PixelFarm.Agg.Image.PixelFormat.Rgba32);
            Graphics2D g2d = Graphics2D.CreateFromImage(actualImage);
            lionFill.OnDraw(g2d);

            GLBitmapTexture bmp = null;

            form.SetGLPaintHandler((o, s) =>
            {
                canvas.Clear(LayoutFarm.Drawing.Color.White);
                //-------------------------------------
                //draw lion from bitmap to GL screen
                if (bmp == null)
                {
                    bmp = GLBitmapTexture.CreateBitmapTexture(actualImage);
                }
                //lion is inverted from software layer ,
                //so... we use DrawImageInvert()
                canvas.DrawImageInvert(bmp, 50, 50);
            });

            form.Show();
            
        }
    }
}