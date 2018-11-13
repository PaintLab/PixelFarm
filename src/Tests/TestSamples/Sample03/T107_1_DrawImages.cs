//MIT, 2014-2016,WinterDev
//creadit : http://learningwebgl.com/lessons/lesson16/index.html

using System;
using Mini;
using PixelFarm.DrawingGL;
namespace OpenTkEssTest
{

    public enum T107DrawImageSet
    {
        //for test only!
        Full,
        Half,
        FromRect,
        //
        SubImages1,
        SubImagesWithScale,
        //
        SubImageWithBlurX,
        SubImageWithBlurY,
        DrawWithConv3x3,
    }

    [Info(OrderCode = "107")]
    [Info("T107_1_DrawImages")]
    public class T107_1_DrawImages : DemoBase
    {
        GLRenderSurface _glsx;
        GLPainter _painter;
        GLBitmap _glbmp;
        bool _isInit;
        //
        protected override void OnGLSurfaceReady(GLRenderSurface glsx, GLPainter painter)
        {
            this._glsx = glsx;
            this._painter = painter;
        }
        [DemoConfig]
        public T107DrawImageSet DrawSet
        {
            get;
            set;
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
            if (!_isInit)
            {
                _glbmp = DemoHelper.LoadTexture(RootDemoPath.Path + @"\logo-dark.jpg");
                _isInit = true;
            }

            GLRenderSurfaceOrigin prevOrgKind = _glsx.OriginKind; //save
            switch (DrawSet)
            {
                default:
                case T107DrawImageSet.Full:
                    {
                        _glsx.OriginKind = GLRenderSurfaceOrigin.LeftTop;
                        for (int i = 0; i < 400;)
                        {
                            _glsx.DrawImage(_glbmp, i, i); //left,top (NOT x,y)
                            i += 50;
                        }
                        //
                        _glsx.OriginKind = GLRenderSurfaceOrigin.LeftBottom;
                        for (int i = 0; i < 400;)
                        {
                            _glsx.DrawImage(_glbmp, i, i); //left,top (NOT x,y)
                            i += 50;
                        }
                    }
                    break;
                case T107DrawImageSet.Half:
                    {
                        _glsx.OriginKind = GLRenderSurfaceOrigin.LeftTop;
                        for (int i = 0; i < 400;)
                        {
                            //left,top (NOT x,y)
                            _glsx.DrawImage(_glbmp, i, i, _glbmp.Width / 2, _glbmp.Height / 2);
                            i += 50;
                        }
                        //
                        _glsx.OriginKind = GLRenderSurfaceOrigin.LeftBottom;
                        for (int i = 0; i < 400;)
                        {
                            _glsx.DrawImage(_glbmp, i, i, _glbmp.Width / 2, _glbmp.Height / 2); //left,top (NOT x,y)
                            i += 50;
                        }
                    }
                    break;
                case T107DrawImageSet.FromRect:
                    {

                        _glsx.OriginKind = GLRenderSurfaceOrigin.LeftTop;
                        for (int i = 0; i < 400;)
                        {
                            //left,top (NOT x,y)
                            PixelFarm.Drawing.RectangleF srcRect = new PixelFarm.Drawing.RectangleF(i, i, _glbmp.Width, _glbmp.Height);
                            _glsx.DrawImage(_glbmp, srcRect, i, i, _glbmp.Width / 2, _glbmp.Height / 2);
                            i += 50;
                        }
                        //
                        _glsx.OriginKind = GLRenderSurfaceOrigin.LeftBottom;
                        for (int i = 0; i < 400;)
                        {
                            PixelFarm.Drawing.RectangleF srcRect = new PixelFarm.Drawing.RectangleF(i, i, _glbmp.Width, _glbmp.Height);
                            _glsx.DrawImage(_glbmp, srcRect, i, i, _glbmp.Width / 2, _glbmp.Height / 2);
                            i += 50;
                        }
                    }
                    break;
                case T107DrawImageSet.SubImages1:
                    {
                        _glsx.OriginKind = GLRenderSurfaceOrigin.LeftTop;
                        for (int i = 0; i < 400;)
                        {
                            //left,top (NOT x,y)
                            PixelFarm.Drawing.Rectangle srcRect = new PixelFarm.Drawing.Rectangle(20, 20, 50, 50);
                            _glsx.DrawSubImage(_glbmp, ref srcRect, i, i);
                            i += 50;
                        }
                        //
                        _glsx.OriginKind = GLRenderSurfaceOrigin.LeftBottom;
                        for (int i = 0; i < 400;)
                        {
                            PixelFarm.Drawing.Rectangle srcRect = new PixelFarm.Drawing.Rectangle(20, 20, 50, 50);
                            _glsx.DrawSubImage(_glbmp, ref srcRect, i, i);
                            i += 50;
                        }
                    }
                    break;
                case T107DrawImageSet.SubImagesWithScale:
                    {
                        _glsx.OriginKind = GLRenderSurfaceOrigin.LeftTop;
                        for (int i = 0; i < 400;)
                        {
                            //left,top (NOT x,y)
                            PixelFarm.Drawing.Rectangle srcRect = new PixelFarm.Drawing.Rectangle(20, 20, 50, 50);
                            _glsx.DrawSubImage(_glbmp, ref srcRect, i, i, 2f);
                            i += 50;
                        }
                        //
                        _glsx.OriginKind = GLRenderSurfaceOrigin.LeftBottom;
                        for (int i = 0; i < 400;)
                        {
                            PixelFarm.Drawing.Rectangle srcRect = new PixelFarm.Drawing.Rectangle(20, 20, 50, 50);
                            _glsx.DrawSubImage(_glbmp, ref srcRect, i, i, 2f);
                            i += 50;
                        }
                    }
                    break;
                case T107DrawImageSet.SubImageWithBlurX:
                    {

                        _glsx.OriginKind = GLRenderSurfaceOrigin.LeftTop;
                        for (int i = 0; i < 400;)
                        {
                            //left,top (NOT x,y) 
                            _glsx.DrawImageWithBlurX(_glbmp, i, i);
                            i += 50;
                        }
                        //
                        _glsx.OriginKind = GLRenderSurfaceOrigin.LeftBottom;
                        for (int i = 0; i < 400;)
                        {
                            _glsx.DrawImageWithBlurX(_glbmp, i, i);
                            i += 50;
                        }
                    }
                    break;
                case T107DrawImageSet.SubImageWithBlurY:
                    {

                        _glsx.OriginKind = GLRenderSurfaceOrigin.LeftTop;
                        for (int i = 0; i < 400;)
                        {
                            //left,top (NOT x,y) 
                            _glsx.DrawImageWithBlurY(_glbmp, i, i);
                            i += 50;
                        }
                        //
                        _glsx.OriginKind = GLRenderSurfaceOrigin.LeftBottom;
                        for (int i = 0; i < 400;)
                        {
                            _glsx.DrawImageWithBlurY(_glbmp, i, i);
                            i += 50;
                        }

                        //
                    }
                    break;
                case T107DrawImageSet.DrawWithConv3x3:
                    {
                        _glsx.OriginKind = GLRenderSurfaceOrigin.LeftTop;
                        for (int i = 0; i < 400;)
                        {
                            //left,top (NOT x,y) 
                            _glsx.DrawImageWithConv3x3(_glbmp, Mat3x3ConvGen.sobelHorizontal, i, i);
                            i += 50;
                        }
                        //
                        _glsx.OriginKind = GLRenderSurfaceOrigin.LeftBottom;
                        for (int i = 0; i < 400;)
                        {
                            _glsx.DrawImageWithConv3x3(_glbmp, Mat3x3ConvGen.emboss, i, i);
                            i += 50;
                        }

                    }
                    break;
            }
            _glsx.OriginKind = prevOrgKind;//restore  

        }
    }

}

