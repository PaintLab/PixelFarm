//MIT, 2014-2016,WinterDev
//creadit : http://learningwebgl.com/lessons/lesson16/index.html

using System;
using Mini;
using PixelFarm.DrawingGL;
namespace OpenTkEssTest
{
    [Info(OrderCode = "113")]
    [Info("T113_FrameBuffer")]
    public class T113_FrameBufferWithBlur : DemoBase
    {
        CanvasGL2d canvas2d;
        GLCanvasPainter painter;
        FrameBuffer frameBuffer;
        FrameBuffer frameBuffer2;
        GLBitmap glbmp;
        bool isInit;
        bool frameBufferNeedUpdate;
        protected override void OnGLContextReady(CanvasGL2d canvasGL, GLCanvasPainter painter)
        {
            this.canvas2d = canvasGL;
            this.painter = painter;
        }
        protected override void OnReadyForInitGLShaderProgram()
        {
            frameBuffer = canvas2d.CreateFrameBuffer(this.Width, this.Height);
            frameBufferNeedUpdate = true;
            //------------ 
            frameBuffer2 = canvas2d.CreateFrameBuffer(this.Width, this.Height);
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
                glbmp = DemoHelper.LoadTexture(RootDemoPath.Path + @"\logo-dark.png");
                isInit = true;
            }
            if (frameBuffer.FrameBufferId > 0)
            {
                if (frameBufferNeedUpdate)
                {
                    //------------------------------------------------------------------------------------           
                    //framebuffer
                    canvas2d.AttachFrameBuffer(frameBuffer);
                    //after make the frameBuffer current
                    //then all drawing command will apply to frameBuffer
                    //do draw to frame buffer here                                        
                    canvas2d.Clear(PixelFarm.Drawing.Color.Red);
                    canvas2d.DrawImageWithBlurX(glbmp, 0, 300);
                    canvas2d.DetachFrameBuffer();
                    //------------------------------------------------------------------------------------  
                    //framebuffer2
                    canvas2d.AttachFrameBuffer(frameBuffer2);
                    GLBitmap bmp2 = new GLBitmap(frameBuffer.TextureId, frameBuffer.Width, frameBuffer.Height);
                    bmp2.IsBigEndianPixel = true;
                    canvas2d.DrawImageWithBlurY(bmp2, 0, 300);
                    canvas2d.DetachFrameBuffer();
                    //------------------------------------------------------------------------------------  
                    //after release current, we move back to default frame buffer again***
                    frameBufferNeedUpdate = false;

                }
                canvas2d.DrawFrameBuffer(frameBuffer2, 15, 300);
            }
            else
            {
                canvas2d.Clear(PixelFarm.Drawing.Color.Blue);
            }
            //-------------------------------
            SwapBuffers();
        }
    }
}

