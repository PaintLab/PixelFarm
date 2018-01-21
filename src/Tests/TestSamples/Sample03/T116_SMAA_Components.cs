//MIT, 2014-2016,WinterDev
//creadit : http://learningwebgl.com/lessons/lesson16/index.html

using System;
using Mini;
using PixelFarm.DrawingGL;
using OpenTK.Graphics.ES20;

namespace OpenTkEssTest
{
    [Info(OrderCode = "116")]
    [Info("T116_SMAA_Components")]
    public class T116_SMAA_Components : DemoBase
    {
        GLRenderSurface _glsx;
        GLPainter painter;
        FrameBuffer frameBuffer1;
        FrameBuffer frameBuffer2;
        FrameBuffer frameBuffer3;

        GLBitmap glbmp;
        bool isInit;
        bool frameBufferNeedUpdate;
        protected override void OnGLSurfaceReady(GLRenderSurface glsx, GLPainter painter)
        {
            this._glsx = glsx;
            this.painter = painter;
        }
        protected override void OnReadyForInitGLShaderProgram()
        {

            frameBuffer1 = _glsx.CreateFrameBuffer(this.Width, this.Height);
            frameBuffer2 = _glsx.CreateFrameBuffer(this.Width, this.Height);
            frameBuffer3 = _glsx.CreateFrameBuffer(this.Width, this.Height);

            frameBufferNeedUpdate = true;
            //------------ 
        }
        protected override void DemoClosing()
        {
            _glsx.Dispose();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            _glsx.SmoothMode = SmoothMode.Smooth;
            _glsx.StrokeColor = PixelFarm.Drawing.Color.Blue;
            _glsx.Clear(PixelFarm.Drawing.Color.White);
            _glsx.ClearColorBuffer();
            //-------------------------------
            if (!isInit)
            {
                glbmp = DemoHelper.LoadTexture(RootDemoPath.Path + @"\lion001.png");
                glbmp.IsBigEndianPixel = false;
                isInit = true;
            }
            if (frameBuffer1.FrameBufferId > 0)
            {
                if (frameBufferNeedUpdate)
                {
                    //step1 : draw input glbmp into frameBuffer1
                    _glsx.AttachFrameBuffer(frameBuffer1);
                    //------------------------------------------------------------------------------------                     
                    //after make the frameBuffer current
                    //then all drawing command will apply to frameBuffer
                    //do draw to frame buffer here                                        
                    _glsx.Clear(PixelFarm.Drawing.Color.Empty);
                    _glsx.DrawImageWithSMAA(glbmp, 0, 300);
                    //------------------------------------------------------------------------------------  
                    _glsx.DetachFrameBuffer();
                    //after release current, we move back to default frame buffer again***

                    //------------------------------------------------------------------------------------   
                    //step2: draw framebuffer 1 to frameBuffer2
                    _glsx.AttachFrameBuffer(frameBuffer2);
                    _glsx.Clear(PixelFarm.Drawing.Color.Empty);

                    _glsx.DrawImageWithSMAA2(frameBuffer1, 0,400);
                    _glsx.DetachFrameBuffer();
                    //------------------------------------------------------------------------------------   
                    //step3
                    _glsx.AttachFrameBuffer(frameBuffer3);
                    _glsx.Clear(PixelFarm.Drawing.Color.Empty);
                    _glsx.DrawImageWithSMAA3(frameBuffer2, glbmp, 0, 400);
                    _glsx.DetachFrameBuffer();
                    //-------------------------------------------------------------------------------------
                    frameBufferNeedUpdate = false;
                }

                _glsx.DrawFrameBuffer(frameBuffer3, 0, 400);
            }
            else
            {
                _glsx.Clear(PixelFarm.Drawing.Color.Blue);
            }
            //-------------------------------
            SwapBuffers();
        }
    }
}

