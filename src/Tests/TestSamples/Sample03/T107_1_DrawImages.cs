//MIT, 2014-2016,WinterDev
//creadit : http://learningwebgl.com/lessons/lesson16/index.html

using System;
using Mini;
using PixelFarm.DrawingGL;
namespace OpenTkEssTest
{
    [Info(OrderCode = "117")]
    [Info("T117_1_DrawImages")]
    public class T117_1_DrawImages : DemoBase
    {
        GLRenderSurface _glsx;
        GLPainter painter;
        GLBitmap glbmp;
        bool isInit;
        protected override void OnGLSurfaceReady(GLRenderSurface glsx, GLPainter painter)
        {
            this._glsx = glsx;
            this.painter = painter;
        }
        protected override void OnReadyForInitGLShaderProgram()
        {
        }
        protected override void DemoClosing()
        {
            _glsx.Dispose();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            _glsx.SmoothMode = SmoothMode.Smooth;
            _glsx.StrokeColor = PixelFarm.Drawing.Color.Blue;
            _glsx.Clear(PixelFarm.Drawing.Color.White); //set clear color and clear all buffer
            _glsx.ClearColorBuffer(); //test , clear only color buffer
            //-------------------------------
            if (!isInit)
            {
                glbmp = DemoHelper.LoadTexture(RootDemoPath.Path + @"\logo-dark.jpg");
                isInit = true;
            }

            GLRenderSurfaceOrigin prevOrgKind = _glsx.OriginKind; //save
            _glsx.OriginKind = GLRenderSurfaceOrigin.LeftTop;
            for (int i = 0; i < 400;)
            {
                _glsx.DrawImage(glbmp, i, i); //left,top (NOT x,y)
                i += 50;
            }


            _glsx.OriginKind = GLRenderSurfaceOrigin.LeftBottom;
            for (int i = 0; i < 400;)
            {
                _glsx.DrawImage(glbmp, i, i); //left,top (NOT x,y)
                i += 50;
            }

            _glsx.OriginKind = prevOrgKind;//restore 
            //-------------------------------
            SwapBuffers();
        }
    }
}

