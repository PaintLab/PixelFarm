//MIT, 2014-present,WinterDev
//creadit : http://learningwebgl.com/lessons/lesson16/index.html

using System;
using Mini;
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
                        for (int i = 0; i < 400;)
                        {
                            //left,top (NOT x,y) 
                            _pcx.DrawImageToQuad(_glbmp,
                                new PixelFarm.Drawing.PointF(i, i),
                                new PixelFarm.Drawing.PointF(i + _glbmp.Width / 2, i),
                                new PixelFarm.Drawing.PointF(i + _glbmp.Width / 2, i + _glbmp.Height / 2),
                                new PixelFarm.Drawing.PointF(i, i + _glbmp.Height / 2));

                            i += 50;
                        }
                        //
                        _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftBottom;
                        for (int i = 0; i < 400;)
                        {
                            _pcx.DrawImageToQuad(_glbmp,
                                       new PixelFarm.Drawing.PointF(i, i),
                                       new PixelFarm.Drawing.PointF(i + _glbmp.Width / 2, i),
                                       new PixelFarm.Drawing.PointF(i + _glbmp.Width / 2, i + _glbmp.Height / 2),
                                       new PixelFarm.Drawing.PointF(i, i + _glbmp.Height / 2));

                            i += 50;
                        }


                    }
                    break;
                case T107_1_DrawImageSet.ToQuad2:
                    {

                        _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftTop;

                        float rotateDegree = 20;

                        for (int i = 0; i < 400;)
                        {
                            //left,top (NOT x,y) 
                            float[] quad = new float[]
                            {
                                0, 0, //left-top
                                _glbmp.Width , 0, //right-top
                                _glbmp.Width , _glbmp.Height , //right-bottom
                                0, _glbmp.Height  //left bottom
                            };

                            PixelFarm.CpuBlit.VertexProcessing.Affine aff =
                                 PixelFarm.CpuBlit.VertexProcessing.Affine.NewMatix2(
                                     PixelFarm.CpuBlit.VertexProcessing.AffinePlan.Translate(-_glbmp.Width / 2, -_glbmp.Height / 2),
                                     PixelFarm.CpuBlit.VertexProcessing.AffinePlan.RotateDeg(rotateDegree),
                                     PixelFarm.CpuBlit.VertexProcessing.AffinePlan.Translate(i + _glbmp.Width / 2, i + _glbmp.Height / 2));


                            aff.Transform(ref quad[0], ref quad[1]);
                            aff.Transform(ref quad[2], ref quad[3]);
                            aff.Transform(ref quad[4], ref quad[5]);
                            aff.Transform(ref quad[6], ref quad[7]);


                            _pcx.DrawImageToQuad(_glbmp,
                                new PixelFarm.Drawing.PointF(quad[0], quad[1]),
                                new PixelFarm.Drawing.PointF(quad[2], quad[3]),
                                new PixelFarm.Drawing.PointF(quad[4], quad[5]),
                                new PixelFarm.Drawing.PointF(quad[6], quad[7]));

                            i += 50;
                        }
                        //
                        _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftBottom;
                        for (int i = 0; i < 400;)
                        {
                            //left,top (NOT x,y) 
                            float[] quad = new float[]
                            {
                                    0, 0, //left-top
                                    _glbmp.Width , 0, //right-top
                                    _glbmp.Width , -_glbmp.Height , //right-bottom
                                    0, -_glbmp.Height //left bottom
                            };

                            PixelFarm.CpuBlit.VertexProcessing.Affine aff =
                                 PixelFarm.CpuBlit.VertexProcessing.Affine.NewMatix2(
                                     PixelFarm.CpuBlit.VertexProcessing.AffinePlan.Translate(-_glbmp.Width / 2, -_glbmp.Height / 2),
                                     PixelFarm.CpuBlit.VertexProcessing.AffinePlan.RotateDeg(rotateDegree),
                                     PixelFarm.CpuBlit.VertexProcessing.AffinePlan.Translate(i + _glbmp.Width / 2, i + _glbmp.Height / 2));


                            aff.Transform(ref quad[0], ref quad[1]);
                            aff.Transform(ref quad[2], ref quad[3]);
                            aff.Transform(ref quad[4], ref quad[5]);
                            aff.Transform(ref quad[6], ref quad[7]);


                            _pcx.DrawImageToQuad(_glbmp,
                                new PixelFarm.Drawing.PointF(quad[0], quad[1]),
                                new PixelFarm.Drawing.PointF(quad[2], quad[3]),
                                new PixelFarm.Drawing.PointF(quad[4], quad[5]),
                                new PixelFarm.Drawing.PointF(quad[6], quad[7]));


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

                            PixelFarm.CpuBlit.VertexProcessing.Affine aff =
                                 PixelFarm.CpuBlit.VertexProcessing.Affine.NewMatix2(
                                     PixelFarm.CpuBlit.VertexProcessing.AffinePlan.Translate(-_glbmp.Width / 2, -_glbmp.Height / 2),
                                     PixelFarm.CpuBlit.VertexProcessing.AffinePlan.RotateDeg(rotateDegree),
                                     PixelFarm.CpuBlit.VertexProcessing.AffinePlan.Translate(i + _glbmp.Width / 2, i + _glbmp.Height / 2));

                            _pcx.DrawImageToQuad(_glbmp, aff);


                            i += 50;
                        }
                        //
                        _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftBottom;
                        for (int i = 0; i < 400;)
                        {


                            PixelFarm.CpuBlit.VertexProcessing.Affine aff =
                                 PixelFarm.CpuBlit.VertexProcessing.Affine.NewMatix2(
                                     PixelFarm.CpuBlit.VertexProcessing.AffinePlan.Translate(-_glbmp.Width / 2, -_glbmp.Height / 2),
                                     PixelFarm.CpuBlit.VertexProcessing.AffinePlan.RotateDeg(rotateDegree),
                                     PixelFarm.CpuBlit.VertexProcessing.AffinePlan.Translate(i + _glbmp.Width / 2, i + _glbmp.Height / 2));

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
                            _pcx.DrawSubImage(_glbmp, ref srcRect, i, i);
                            i += 50;
                        }
                        //
                        _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftBottom;
                        for (int i = 0; i < 400;)
                        {
                            PixelFarm.Drawing.Rectangle srcRect = new PixelFarm.Drawing.Rectangle(0, 0, _glbmp.Width, _glbmp.Height);
                            _pcx.DrawSubImage(_glbmp, ref srcRect, i, i);
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
                            _pcx.DrawSubImage(_glbmp, ref srcRect, i, i);
                            i += 50;
                        }
                        //
                        _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftBottom;
                        for (int i = 0; i < 400;)
                        {
                            PixelFarm.Drawing.Rectangle srcRect = new PixelFarm.Drawing.Rectangle(0, 0, _glbmp.Width / 2, _glbmp.Height / 2);
                            _pcx.DrawSubImage(_glbmp, ref srcRect, i, i);
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
                            _pcx.DrawSubImage(_glbmp, ref srcRect, i, i);
                            i += 50;
                        }
                        //
                        _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftBottom;
                        for (int i = 0; i < 400;)
                        {
                            PixelFarm.Drawing.Rectangle srcRect = new PixelFarm.Drawing.Rectangle(20, 20, 50, 50);
                            _pcx.DrawSubImage(_glbmp, ref srcRect, i, i);
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
                            _pcx.DrawSubImage(_glbmp, ref srcRect, i, i, 2f);
                            i += 50;
                        }
                        //
                        _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftBottom;
                        for (int i = 0; i < 400;)
                        {
                            PixelFarm.Drawing.Rectangle srcRect = new PixelFarm.Drawing.Rectangle(20, 20, 50, 50);
                            _pcx.DrawSubImage(_glbmp, ref srcRect, i, i, 2f);
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

