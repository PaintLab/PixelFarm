using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

using PixelFarm.Agg;
using PixelFarm.DrawingGL;
using OpenTK.Graphics.OpenGL;

namespace Mini2
{
    [Info(OrderCode = "05")]
    [Info("LionFillDirectBufferDemo")]
    public class LionFillDirectBufferDemo : DemoBase
    {
        public override void Load()
        {
            //----------------------------------------------- 
            FormTestWinGLControl form = new FormTestWinGLControl();
            CanvasGL2d canvas = new CanvasGL2d(this.Width, this.Height);
            var lionFill = new LionFillSprite();
            //-----------------------------------------------

            GLBitmap bmp1 = null;
            int bmpW = 0;
            int bmpH = 0;

            form.SetGLPaintHandler((o, s) =>
            {
                canvas.Clear(PixelFarm.Drawing.Color.White);

                //since OpenGL < 3 dose not have FrameBuffer
                //so we first draw it to backbuffer 
                //then copy data from back buffer to texture
                if (bmp1 == null)
                {
                    lionFill.Draw(canvas);

                    GL.ReadBuffer(ReadBufferMode.Back);
                    Bitmap bmp = new Bitmap(800, 600, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                    byte[] buffer_output = new byte[800 * 4 * 600];
                    var bmpdata = bmp.LockBits(
                        new Rectangle(0, 0, 800, 600),
                        System.Drawing.Imaging.ImageLockMode.ReadWrite,
                        bmp.PixelFormat);
                    GL.ReadPixels(0, 0, 800, 600, PixelFormat.Bgra, PixelType.Byte, bmpdata.Scan0);
                    unsafe
                    {
                        fixed (byte* outputH = &buffer_output[0])
                        {
                            GL.ReadPixels(0, 0, 800, 600, PixelFormat.Bgra, PixelType.Byte, (IntPtr)outputH);
                        }
                    }
                    bmp.UnlockBits(bmpdata);
                    //save
                    bmp.Save("d:\\WImageTest\\glLion.png");
                    //--------- 
                    //canvas.DrawImage(GLBitmapTexture.CreateBitmapTexture(bmp), 0, 0, bmp.Width, bmp.Height);
                    bmp1 = PixelFarm.Drawing.DrawingGL.GLBitmapTextureHelper.CreateBitmapTexture(bmp);
                    bmpW = bmp.Width;
                    bmpH = bmp.Height;

                    canvas.Clear(PixelFarm.Drawing.Color.White);                     
                }
                canvas.DrawImage(bmp1, 0, 0, bmpW, bmpH);

                //canvas.Clear(PixelFarm.Drawing.Color.White);
            });
            form.Show();
        }
    }
}