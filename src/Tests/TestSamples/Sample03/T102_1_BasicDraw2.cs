//MIT, 2014-present,WinterDev
//creadit : http://learningwebgl.com/lessons/lesson16/index.html

using System;
using Mini;
using PixelFarm.DrawingGL;
namespace OpenTkEssTest
{

    public enum T102_1_Set
    {
        //for test only!
        DrawRect,
        FillRect,
        Lines,
    }
    public enum T102_1_StrokeWidth
    {
        HalfPx,
        OnePx,
        TwoPx,
        ThreePx,
        FourPx,
        FivePx,
    }
    [Info(OrderCode = "102", SupportedOn = AvailableOn.GLES)]
    [Info("T102_1_BasicDraw2")]
    public class T102_1_BasicDraw2 : DemoBase
    {
        GLPainterContext _glsx;
        GLPainter _painter;

        protected override void OnGLSurfaceReady(GLPainterContext glsx, GLPainter painter)
        {
            _glsx = glsx;
            _painter = painter;

        }
        [DemoConfig]
        public T102_1_Set DrawSet
        {
            get;
            set;
        }
        [DemoConfig]
        public T102_1_StrokeWidth T102_1_StrokeWidth
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
            _painter.StrokeColor = _glsx.StrokeColor = PixelFarm.Drawing.Color.Blue;

            _glsx.Clear(PixelFarm.Drawing.Color.White); //set clear color and clear all buffer
            _glsx.ClearColorBuffer(); //test , clear only color buffer
                                      //------------------------------- 

            float prevStrokeW = _glsx.StrokeWidth;
            switch (T102_1_StrokeWidth)
            {
                default: throw new NotSupportedException();
                case T102_1_StrokeWidth.HalfPx:
                    _glsx.StrokeWidth = 0.5f;
                    break;
                case T102_1_StrokeWidth.OnePx:
                    _glsx.StrokeWidth = 1;
                    break;
                case T102_1_StrokeWidth.TwoPx:
                    _glsx.StrokeWidth = 2;
                    break;
                case T102_1_StrokeWidth.ThreePx:
                    _glsx.StrokeWidth = 3;
                    break;
                case T102_1_StrokeWidth.FourPx:
                    _glsx.StrokeWidth = 4;
                    break;
                case T102_1_StrokeWidth.FivePx:
                    _glsx.StrokeWidth = 5;
                    break;


            } 
            PixelFarm.Drawing.RenderSurfaceOrientation prevOrgKind = _glsx.OriginKind; //save
            switch (DrawSet)
            {
                default:
                case T102_1_Set.Lines:
                    {
                        _glsx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftTop;
                        for (int i = 0; i < 400;)
                        {
                            _glsx.DrawLine(i + 10, i + 10, i + 30, i + 50);
                            i += 50;
                        }
                        //
                        _glsx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftBottom;
                        for (int i = 0; i < 400;)
                        {
                            _glsx.DrawLine(i + 10, i + 10, i + 30, i + 50);
                            i += 50;
                        }
                    }
                    break;
                case T102_1_Set.FillRect:
                    {
                        _glsx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftTop;
                        for (int i = 0; i < 400;)
                        {
                            _glsx.FillRect(PixelFarm.Drawing.Color.Red, i, i, 50, 50);
                            i += 50;
                        }
                        //
                        _glsx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftBottom;
                        for (int i = 0; i < 400;)
                        {
                            _glsx.FillRect(PixelFarm.Drawing.Color.Red, i, i, 50, 50);
                            i += 50;
                        }
                    }
                    break;
                case T102_1_Set.DrawRect:
                    {
                        _glsx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftTop;
                        for (int i = 0; i < 400;)
                        {
                            _painter.DrawRect(i, i, 50, 50);
                            i += 50;
                        }
                        //
                        _glsx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftBottom;
                        for (int i = 0; i < 400;)
                        {
                            _painter.DrawRect(i, i, 50, 50);
                            i += 50;
                        }
                    }
                    break;
            }
            _glsx.OriginKind = prevOrgKind;//restore  


            _glsx.StrokeWidth = prevStrokeW;
        }
    }

}

