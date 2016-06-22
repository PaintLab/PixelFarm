//MIT, 2014-2016,WinterDev
using System;
using PixelFarm.Drawing;
using OpenTK.Graphics.ES20;
using Mini;
using PixelFarm.DrawingGL;
namespace OpenTkEssTest
{
    [Info(OrderCode = "106")]
    [Info("T106_SampleBrushes")]
    public class T106_SampleBrushes : PrebuiltGLControlDemoBase
    {
        CanvasGL2d canvas2d;
        protected override void OnInitGLProgram(object sender, EventArgs args)
        {
            int max = Math.Max(this.Width, this.Height);
            canvas2d = new CanvasGL2d(max, max);
        }
        protected override void DemoClosing()
        {
            canvas2d.Dispose();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            canvas2d.SmoothMode = CanvasSmoothMode.Smooth;
            canvas2d.StrokeColor = PixelFarm.Drawing.Color.Blue;
            canvas2d.ClearColorBuffer();
            canvas2d.FillRect(PixelFarm.Drawing.Color.Black, 0, 0, 150, 150);
            GLBitmap glBmp = LoadTexture("..\\logo-dark.jpg");
            var textureBrush = new TextureBrush(new GLImage(glBmp));
            canvas2d.FillPolygon(
                textureBrush,
                new float[]
                {
                    0,50,
                    50,50,
                    10,100
                }, 3 * 2);
            //------------------------------------------------------------------------- 
            var linearGrBrush2 = new LinearGradientBrush(
              new PixelFarm.Drawing.PointF(0, 50),
              PixelFarm.Drawing.Color.Red,
              new PixelFarm.Drawing.PointF(400, 100),
              PixelFarm.Drawing.Color.White);
            //fill polygon with gradient brush 


            canvas2d.FillRect(PixelFarm.Drawing.Color.Yellow, 200, 0, 150, 150);
            canvas2d.FillPolygon(
                linearGrBrush2, new float[] {
                        200, 50,
                        250, 50,
                        210, 100}, 3 * 2);
            canvas2d.FillRect(PixelFarm.Drawing.Color.Black, 400, 0, 150, 150);
            //------------------------------------------------------------------------- 

            //another  ...                
            canvas2d.FillPolygon(
                   linearGrBrush2, new float[] {
                       400, 50,
                       450, 50,
                       410, 100}, 3 * 2);
            //------------------------------------------------------------------------- 


            miniGLControl.SwapBuffers();
        }
        GLBitmap LoadTexture(string imgFileName)
        {
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(imgFileName);
            int textureId = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, textureId);
            //glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR_MIPMAP_LINEAR);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            //     glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            //glPixelStorei(GL_UNPACK_ALIGNMENT, 1);
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
            //glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, static_cast<GLsizei>(image.width), static_cast<GLsizei>(image.height), 0,
            //             GL_RGBA, GL_UNSIGNED_BYTE, image.data.data());
            var bmpdata = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp.Width, bmp.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, bmpdata.Scan0);
            bmp.UnlockBits(bmpdata);
            //glGenerateMipmap(GL_TEXTURE_2D);
            GL.GenerateMipmap(TextureTarget.Texture2D);
            return new GLBitmap(textureId, bmp.Width, bmp.Height);
        }
    }
}

