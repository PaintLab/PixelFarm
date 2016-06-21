
using System;
using PixelFarm.Drawing;
using OpenTK.Graphics.ES20;
using Mini;
using PixelFarm.DrawingGL;
namespace OpenTkEssTest
{
    [Info(OrderCode = "107")]
    [Info("T107_SampleDrawImage")]
    public class T107_SampleDrawImage : PrebuiltGLControlDemoBase
    {
        CanvasGL2d canvas2d;
        protected override void OnInitGLProgram(object sender, EventArgs args)
        {
            //--------------------------------------------------------------------------------
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.ClearColor(1, 1, 1, 1);
            //setup viewport size
            int max = Math.Max(this.Width, this.Height);
            canvas2d = new CanvasGL2d(max, max);
            //square viewport
            GL.Viewport(0, 0, max, max);
        }
        protected override void DemoClosing()
        {
            canvas2d.Dispose();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            canvas2d.SmoothMode = CanvasSmoothMode.Smooth;
            canvas2d.StrokeColor = PixelFarm.Drawing.Color.Blue;
            //canvas2d.FillRect(PixelFarm.Drawing.Color.Black, 0, 0, 150, 150);
            //load bmp image 
            //------------------------------------------------------------------------- 
            GLBitmap glbmp = LoadTexture(@"..\logo-dark.jpg");
            canvas2d.DrawImage(glbmp, 100, 300);
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

