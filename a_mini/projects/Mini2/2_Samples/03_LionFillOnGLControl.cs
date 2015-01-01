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
            CanvasGL2d canvas = new CanvasGL2d(this.Width, this.Height);
            var lionFill = new LionFillSprite();
            //----------
            //draw lion on software layer
            ActualImage actualImage = new ActualImage(800, 600, PixelFarm.Agg.Image.PixelFormat.Rgba32);
            Graphics2D g2d = Graphics2D.CreateFromImage(actualImage);
            GLBitmap bmp = null;

            form.SetGLPaintHandler((o, s) =>
            {
                canvas.Clear(LayoutFarm.Drawing.Color.White);
                //-------------------------------------
                // draw lion from bitmap to GL screen
                if (bmp == null)
                {
                    lionFill.OnDraw(g2d);
                    bmp = new GLBitmap(new LazyAggBitmapBufferProvider(actualImage));

                }
                //lion is inverted from software layer ,
                //so... we use DrawImageInvert()
                int xpos = 0;
                int w = bmp.Width;
                int h = bmp.Height;

                for (int i = 0; i < 3; ++i)
                {
                    canvas.DrawImage(bmp, xpos, 50, w, h);
                    w = (int)(w * 1.2);
                    h = (int)(h * 1.2);
                    xpos += 150;
                }
                w = bmp.Width;
                h = bmp.Height;
                xpos = 0;
                for (int i = 0; i < 2; ++i)
                {

                    w = (int)(w * 0.75);
                    h = (int)(h * 0.75);
                    xpos -= 50;
                    canvas.DrawImage(bmp, xpos, 50, w, h);

                }
            });

            form.Show();

        }
    }
}