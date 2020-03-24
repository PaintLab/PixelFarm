//MIT, 2014-present,WinterDev
//creadit : http://learningwebgl.com/lessons/lesson16/index.html

using System;
using Mini;
using PixelFarm.CpuBlit.VertexProcessing;
using PixelFarm.DrawingGL;
namespace OpenTkEssTest
{

    public enum T107_1_DrawImageSet
    {
        //for test only!
        Full,
        Half,
        ToRect,
        ToQuad1,
        ToQuad2,
        ToQuad3,
        //
        SubImages0,
        SubImages1,
        SubImages2,
        SubImagesWithScale,
        //
        SubImageWithBlurX,
        SubImageWithBlurY,
        DrawWithConv3x3,
    }

    [Info(OrderCode = "107")]
    [Info("T107_1_DrawImages", AvailableOn = AvailableOn.GLES)]
    public class T107_1_DrawImages : DemoBase
    {
        GLPainterContext _pcx;
        GLPainter _painter;
        GLBitmap _glbmp;
        bool _isInit;
        //
        protected override void OnGLPainterReady(GLPainter painter)
        {
            _pcx = painter.PainterContext;
            _painter = painter;
        }
        [DemoConfig]
        public T107_1_DrawImageSet DrawSet
        {
            get;
            set;
        }
        protected override void OnReadyForInitGLShaderProgram()
        {
        }
        protected override void DemoClosing()
        {
            _pcx.Dispose();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            _pcx.SmoothMode = SmoothMode.Smooth;
            _pcx.StrokeColor = PixelFarm.Drawing.Color.Blue;
            _pcx.Clear(PixelFarm.Drawing.Color.White); //set clear color and clear all buffer
            _pcx.ClearColorBuffer(); //test , clear only color buffer
            //-------------------------------
            if (!_isInit)
            {
                _glbmp = DemoHelper.LoadTexture(RootDemoPath.Path + @"\logo-dark.jpg");
                _isInit = true;
            }

            PixelFarm.Drawing.RenderSurfaceOrientation prevOrgKind = _pcx.OriginKind; //save
            switch (DrawSet)
            {
                default:
                case T107_1_DrawImageSet.Full:
                    {
                        _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftTop;
                        for (int i = 0; i < 400;)
                        {
                            _pcx.DrawImage(_glbmp, i, i); //left,top (NOT x,y)
                            i += 50;
                        }
                        //
                        _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftBottom;
                        for (int i = 0; i < 400;)
                        {
                            _pcx.DrawImage(_glbmp, i, i); //left,top (NOT x,y)
                            i += 50;
                        }
                    }
                    break;
                case T107_1_DrawImageSet.Half:
                    {
                        _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftTop;
                        for (int i = 0; i < 400;)
                        {
                            //left,top (NOT x,y)
                            _pcx.DrawImage(_glbmp, i, i, _glbmp.Width / 2, _glbmp.Height / 2);
                            i += 50;
                        }
                        //
                        _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftBottom;
                        for (int i = 0; i < 400;)
                        {
                            _pcx.DrawImage(_glbmp, i, i, _glbmp.Width / 2, _glbmp.Height / 2); //left,top (NOT x,y)
                            i += 50;
                        }
                    }
                    break;
                case T107_1_DrawImageSet.ToRect:
                    {

                        _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftTop;
                        for (int i = 0; i < 400;)
                        {
                            //left,top (NOT x,y)
                            //PixelFarm.Drawing.RectangleF srcRect = new PixelFarm.Drawing.RectangleF(i, i, _glbmp.Width, _glbmp.Height);
                            _pcx.DrawImage(_glbmp, i, i, _glbmp.Width / 2, _glbmp.Height / 2);
                            i += 50;
                        }
                        //
                        _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftBottom;
                        for (int i = 0; i < 400;)
                        {
                            //PixelFarm.Drawing.RectangleF srcRect = new PixelFarm.Drawing.RectangleF(i, i, _glbmp.Width, _glbmp.Height);
                            _pcx.DrawImage(_glbmp, i, i, _glbmp.Width / 2, _glbmp.Height / 2);
                            i += 50;
                        }
                    }
                    break;
                case T107_1_DrawImageSet.ToQuad1:
                    {

                        _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftTop;

                        Quad2f quad = new Quad2f();
                        quad.SetCornersFromRect(0, 0, _glbmp.Width / 2, _glbmp.Height / 2);//half size

                        for (int i = 0; i < 400;)
                        {
                            //left,top (NOT x,y)  

                            _pcx.DrawImageToQuad(_glbmp, quad);

                            quad.Offset(50, 50);
                            i += 50;
                        }
                        //
                        _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftBottom;
                        quad.SetCornersFromRect(0, 0, _glbmp.Width / 2, _glbmp.Height / 2);//half size
                        for (int i = 0; i < 400;)
                        {
                            _pcx.DrawImageToQuad(_glbmp, quad);
                            quad.Offset(50, 50);
                            i += 50;
                        }


                    }
                    break;
                case T107_1_DrawImageSet.ToQuad2:
                    {

                        _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftTop;

                        float rotateDegree = 20;

                        //float[] quad = new float[8];

                        Quad2f quad = new Quad2f();

                        for (int i = 0; i < 400;)
                        {
                            //left,top (NOT x,y) 
                            quad.SetCornersFromRect(0, 0, _glbmp.Width, _glbmp.Height);

                            AffineMat aff = AffineMat.Iden;
                            aff.Translate(-_glbmp.Width / 2, -_glbmp.Height / 2);//move to bitmap's center
                            aff.RotateDeg(rotateDegree);
                            aff.Translate(i + _glbmp.Width / 2, i + _glbmp.Height / 2);

                            quad.Transform(aff);

                            _pcx.DrawImageToQuad(_glbmp, quad);

                            i += 50;
                        }
                        //
                        _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftBottom;


                        for (int i = 0; i < 400;)
                        {
                            //left,top (NOT x,y) 
                            quad.SetCornersFromRect(0, 0, _glbmp.Width, -_glbmp.Height);

                            AffineMat aff = AffineMat.Iden;
                            aff.Translate(-_glbmp.Width / 2, -_glbmp.Height / 2);//move to bitmap's center
                            aff.RotateDeg(rotateDegree);
                            aff.Translate(i + _glbmp.Width / 2, i + _glbmp.Height / 2);

                            quad.Transform(aff);

                            _pcx.DrawImageToQuad(_glbmp, quad);

                            i += 50;
                        }

                    }
                    break;
                case T107_1_DrawImageSet.ToQuad3:
                    {
                        _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftTop;

                        float rotateDegree = 60;

                        for (int i = 0; i < 400;)
                        {

                            AffineMat aff = AffineMat.Iden;
                            aff.Translate(-_glbmp.Width / 2, -_glbmp.Height / 2);//move to bitmap's center
                            aff.RotateDeg(rotateDegree);
                            aff.Translate(i + _glbmp.Width / 2, i + _glbmp.Height / 2);

                            _pcx.DrawImageToQuad(_glbmp, aff);
                            i += 50;
                        }
                        //
                        _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftBottom;
                        for (int i = 0; i < 400;)
                        {

                            AffineMat aff = AffineMat.Iden;
                            aff.Translate(-_glbmp.Width / 2, -_glbmp.Height / 2);//move to bitmap's center
                            aff.RotateDeg(rotateDegree);
                            aff.Translate(i + _glbmp.Width / 2, i + _glbmp.Height / 2);

                            _pcx.DrawImageToQuad(_glbmp, aff);


                            i += 50;
                        }
                    }
                    break;
                case T107_1_DrawImageSet.SubImages0:
                    {
                        _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftTop;
                        for (int i = 0; i < 400;)
                        {
                            //left,top (NOT x,y)
                            PixelFarm.Drawing.Rectangle srcRect = new PixelFarm.Drawing.Rectangle(0, 0, _glbmp.Width, _glbmp.Height);
                            _pcx.DrawSubImage(_glbmp, srcRect, i, i);
                            i += 50;
                        }
                        //
                        _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftBottom;
                        for (int i = 0; i < 400;)
                        {
                            PixelFarm.Drawing.Rectangle srcRect = new PixelFarm.Drawing.Rectangle(0, 0, _glbmp.Width, _glbmp.Height);
                            _pcx.DrawSubImage(_glbmp, srcRect, i, i);
                            i += 50;
                        }
                    }
                    break;
                case T107_1_DrawImageSet.SubImages1:
                    {
                        _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftTop;
                        for (int i = 0; i < 400;)
                        {
                            //left,top (NOT x,y)
                            PixelFarm.Drawing.Rectangle srcRect = new PixelFarm.Drawing.Rectangle(0, 0, _glbmp.Width / 2, _glbmp.Height / 2);
                            _pcx.DrawSubImage(_glbmp, srcRect, i, i);
                            i += 50;
                        }
                        //
                        _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftBottom;
                        for (int i = 0; i < 400;)
                        {
                            PixelFarm.Drawing.Rectangle srcRect = new PixelFarm.Drawing.Rectangle(0, 0, _glbmp.Width / 2, _glbmp.Height / 2);
                            _pcx.DrawSubImage(_glbmp, srcRect, i, i);
                            i += 50;
                        }
                    }
                    break;
                case T107_1_DrawImageSet.SubImages2:
                    {
                        _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftTop;
                        for (int i = 0; i < 400;)
                        {
                            //left,top (NOT x,y)
                            PixelFarm.Drawing.Rectangle srcRect = new PixelFarm.Drawing.Rectangle(20, 20, 50, 50);
                            _pcx.DrawSubImage(_glbmp, srcRect, i, i);
                            i += 50;
                        }
                        //
                        _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftBottom;
                        for (int i = 0; i < 400;)
                        {
                            PixelFarm.Drawing.Rectangle srcRect = new PixelFarm.Drawing.Rectangle(20, 20, 50, 50);
                            _pcx.DrawSubImage(_glbmp, srcRect, i, i);
                            i += 50;
                        }
                    }
                    break;
                case T107_1_DrawImageSet.SubImagesWithScale:
                    {
                        _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftTop;
                        for (int i = 0; i < 400;)
                        {
                            //left,top (NOT x,y)
                            PixelFarm.Drawing.Rectangle srcRect = new PixelFarm.Drawing.Rectangle(20, 20, 50, 50);
                            _pcx.DrawSubImage(_glbmp, srcRect, i, i, 2f);
                            i += 50;
                        }
                        //
                        _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftBottom;
                        for (int i = 0; i < 400;)
                        {
                            PixelFarm.Drawing.Rectangle srcRect = new PixelFarm.Drawing.Rectangle(20, 20, 50, 50);
                            _pcx.DrawSubImage(_glbmp, srcRect, i, i, 2f);
                            i += 50;
                        }
                    }
                    break;
                case T107_1_DrawImageSet.SubImageWithBlurX:
                    {

                        _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftTop;
                        for (int i = 0; i < 400;)
                        {
                            //left,top (NOT x,y) 
                            _pcx.DrawImageWithBlurX(_glbmp, i, i);
                            i += 50;
                        }
                        //
                        _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftBottom;
                        for (int i = 0; i < 400;)
                        {
                            _pcx.DrawImageWithBlurX(_glbmp, i, i);
                            i += 50;
                        }
                    }
                    break;
                case T107_1_DrawImageSet.SubImageWithBlurY:
                    {

                        _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftTop;
                        for (int i = 0; i < 400;)
                        {
                            //left,top (NOT x,y) 
                            _pcx.DrawImageWithBlurY(_glbmp, i, i);
                            i += 50;
                        }
                        //
                        _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftBottom;
                        for (int i = 0; i < 400;)
                        {
                            _pcx.DrawImageWithBlurY(_glbmp, i, i);
                            i += 50;
                        }

                        //
                    }
                    break;
                case T107_1_DrawImageSet.DrawWithConv3x3:
                    {
                        _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftTop;
                        for (int i = 0; i < 400;)
                        {
                            //left,top (NOT x,y) 
                            _pcx.DrawImageWithConv3x3(_glbmp, Mat3x3ConvGen.sobelHorizontal, i, i);
                            i += 50;
                        }
                        //
                        _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftBottom;
                        for (int i = 0; i < 400;)
                        {
                            _pcx.DrawImageWithConv3x3(_glbmp, Mat3x3ConvGen.emboss, i, i);
                            i += 50;
                        }

                    }
                    break;
            }
            _pcx.OriginKind = prevOrgKind;//restore  

        }
    }

}

