//MIT, 2014-2016,WinterDev
//creadit : http://learningwebgl.com/lessons/lesson16/index.html
using System;
using Mini;
using PixelFarm.DrawingGL;
using OpenTK.Graphics.ES20;
namespace OpenTkEssTest
{
    [Info(OrderCode = "112")]
    [Info("T112_FrameBuffer")]
    public class T112_FrameBuffer : PrebuiltGLControlDemoBase
    {
        CanvasGL2d canvas2d;
        GLCanvasPainter painter;
        FrameBuffer frameBuffer;
        GLBitmap glbmp;
        GLBitmap frameBufferBmp;

        bool isInit;
        bool isFrameBufferReady;
        protected override void OnInitGLProgram(object sender, EventArgs args)
        {
            int max = Math.Max(this.Width, this.Height);
            canvas2d = new CanvasGL2d(max, max);
            painter = new GLCanvasPainter(canvas2d, max, max);
            frameBuffer = canvas2d.CreateFrameBuffer(max, max);
            //------------ 
        }
        protected override void DemoClosing()
        {
            canvas2d.Dispose();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            canvas2d.SmoothMode = CanvasSmoothMode.Smooth;
            canvas2d.StrokeColor = PixelFarm.Drawing.Color.Blue;
            canvas2d.Clear(PixelFarm.Drawing.Color.White);
            canvas2d.ClearColorBuffer();
            //-------------------------------
            if (!isInit)
            {
                glbmp = LoadTexture(@"..\logo-dark.jpg");
                isInit = true;
            }
            if (frameBuffer.FrameBufferId > 0)
            {
                if (!isFrameBufferReady)
                {
                    //------------------------------------------------------------------------------------
                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, frameBuffer.FrameBufferId);
                    //--------
                    //do draw to frame buffer here
                    GL.ClearColor(OpenTK.Graphics.Color4.Red);
                    GL.Clear(ClearBufferMask.ColorBufferBit);
                    //------------------------------------------------------------------------------------
                    canvas2d.DrawImage(glbmp, 0, 300);
                    //------------------------------------------------------------------------------------ 
                    GL.BindTexture(TextureTarget.Texture2D, frameBuffer.TextureId);
                    GL.GenerateMipmap(TextureTarget.Texture2D);
                    GL.BindTexture(TextureTarget.Texture2D, 0); //unbind texture
                    GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0); //switch back to default -framebuffer


                    frameBufferBmp = new GLBitmap(frameBuffer.TextureId, frameBuffer.Width, frameBuffer.Height);
                    frameBufferBmp.DontSwapRedBlueChannel = true;

                    isFrameBufferReady = true;
                }                
                canvas2d.DrawImage(frameBufferBmp, 15, 300);

            }
            else
            {
                canvas2d.Clear(PixelFarm.Drawing.Color.Blue);
            }
            //-------------------------------
            miniGLControl.SwapBuffers();
        }
        static GLBitmap LoadTexture(string imgFileName)
        {
            using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(imgFileName))
            {
                var bmpdata = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                int stride = bmpdata.Stride;
                byte[] buffer = new byte[stride * bmp.Height];
                System.Runtime.InteropServices.Marshal.Copy(bmpdata.Scan0, buffer, 0, buffer.Length);
                bmp.UnlockBits(bmpdata);
                return new GLBitmap(bmp.Width, bmp.Height, buffer, false);
            }
        }
    }
}

