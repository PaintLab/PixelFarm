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
        FrameBuffer _edgeFrameBuffRT;
        FrameBuffer _weightFrameBuffRT;
        FrameBuffer frameBuffer3;
        FrameBuffer _colorBuffer;

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

            //frameBuffer1 = _glsx.CreateFrameBuffer(this.Width, this.Height);
            //frameBuffer2 = _glsx.CreateFrameBuffer(this.Width, this.Height);
            //frameBuffer3 = _glsx.CreateFrameBuffer(this.Width, this.Height); 
            int frameBufferW = 800;
            int frameBufferH = 800;

            _colorBuffer = new FrameBuffer(frameBufferW, frameBufferH);

            _edgeFrameBuffRT = new FrameBuffer(frameBufferW, frameBufferH, new FrameBufferCreationParameters()
            {
                minFilter = TextureMinFilter.Linear,
                magFilter = TextureMagFilter.Linear,
                pixelFormat = PixelFormat.Rgb, //**
                pixelInternalFormat = PixelInternalFormat.Rgb
            });
            //
            _weightFrameBuffRT = new FrameBuffer(frameBufferW, frameBufferH, new FrameBufferCreationParameters()
            {
                minFilter = TextureMinFilter.Linear,
                magFilter = TextureMagFilter.Linear,
                pixelFormat = PixelFormat.Rgba
            });
            //
            frameBuffer3 = _glsx.CreateFrameBuffer(frameBufferW, frameBufferH);
            frameBufferNeedUpdate = true;
            //------------ 


            //FrameBuffer _edgesRT;//edge render target
            //FrameBuffer _weightsRT; //weight render target

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
                //glbmp = DemoHelper.LoadTexture(RootDemoPath.Path + @"\lines.png");
                glbmp = DemoHelper.LoadTexture(RootDemoPath.Path + @"\lion_no_aa.png");
                isInit = true;
            }
            if (_edgeFrameBuffRT.FrameBufferId > 0)
            {
                //if (frameBufferNeedUpdate)
                //{
                //draw input glbmp into frameBuffer1
                _glsx.AttachFrameBuffer(_colorBuffer);
                _glsx.Clear(PixelFarm.Drawing.Color.Empty);
                _glsx.DrawImage(glbmp, 0, this.Height);
                _glsx.DetachFrameBuffer();
                //-------------------------
                //post-processing AA


                //------------------------------------------------------------------------------------                     
                //step 1: ColorEdgeDetection
                //step1 : draw input glbmp into frameBuffer1
                _glsx.AttachFrameBuffer(_edgeFrameBuffRT);
                //------------------------------------------------------------------------------------                     
                //after make the frameBuffer current
                //then all drawing command will apply to frameBuffer
                //do draw to frame buffer here                                        
                _glsx.Clear(PixelFarm.Drawing.Color.Empty);
                GL.ClearStencil(0);
                GL.Disable(EnableCap.Blend);

                // We will be creating the stencil buffer for later usage. 
                //replace where rendered
                GL.StencilFunc(StencilFunction.Gequal, 1, 1);
                GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace);
                GL.Enable(EnableCap.StencilTest);

                _glsx.DrawImageWithSMAA(_colorBuffer, 0, 800);
                //------------------------------------------------------------------------------------  
                _glsx.DetachFrameBuffer();
                //after release current, we move back to default frame buffer again***

                ////------------------------------------------------------------------------------------   
                //step 2: BlendWeightCalculation
                ////step2: draw framebuffer 1 to frameBuffer2
                _glsx.AttachFrameBuffer(_weightFrameBuffRT);
                _glsx.Clear(PixelFarm.Drawing.Color.Empty);
                GL.ClearStencil(0);
                //
                GL.Disable(EnableCap.Blend);

                // Here we want to process only marked pixels.
                GL.Enable(EnableCap.StencilTest);
                //place a 1 where rendered                
                GL.StencilFunc(StencilFunction.Equal, 1, 1);
                GL.StencilOp(StencilOp.Zero, StencilOp.Zero, StencilOp.Keep);
                //replace where rendered                 

                _glsx.DrawImageWithSMAA2(_edgeFrameBuffRT, 0, 800);
                _glsx.DetachFrameBuffer();

                ////------------------------------------------------------------------------------------
                ////step3
                _glsx.AttachFrameBuffer(frameBuffer3);
                _glsx.Clear(PixelFarm.Drawing.Color.Empty);
                GL.Disable(EnableCap.Blend);

                // Here we want to process all the pixels.
                GL.Disable(EnableCap.StencilTest);               
                _glsx.DrawImageWithSMAA3(_weightFrameBuffRT, _colorBuffer, 0, 800);
                _glsx.DetachFrameBuffer();

                ////-------------------------------------------------------------------------------------
                //frameBufferNeedUpdate = false;
                //}
                //_glsx.DrawFrameBuffer(_weightFrameBuffRT, 0, this.Height);
                //_glsx.dbugDrawSMAATextArea();
                //_glsx.DrawFrameBuffer(_edgeFrameBuffRT, 0, this.Height);
                //_glsx.DrawFrameBuffer(_weightFrameBuffRT, 0, this.Height);
                //_glsx.DrawFrameBuffer(_weightFrameBuffRT, 0, this.Height);
                // _glsx.DrawFrameBuffer(_colorBuffer, 0, this.Height);
                //_glsx.DrawFrameBuffer(_edgeFrameBuffRT, 0, this.Height);
                //_glsx.DrawFrameBuffer(_weightFrameBuffRT, 0, this.Height);
                //_glsx.DrawFrameBuffer(_weightFrameBuffRT, 0, this.Height);
                _glsx.DrawFrameBuffer(frameBuffer3, 0, this.Height);
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

