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
        GLPainterContext _pcx;
        GLPainter _painter;

        protected override void OnGLPainterReady(GLPainterContext pcx, GLPainter painter)
        {
            _pcx = pcx;
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
            _pcx.Dispose();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            _pcx.SmoothMode = SmoothMode.Smooth;
            _painter.StrokeColor = _pcx.StrokeColor = PixelFarm.Drawing.Color.Blue;

            _pcx.Clear(PixelFarm.Drawing.Color.White); //set clear color and clear all buffer
            _pcx.ClearColorBuffer(); //test , clear only color buffer
                                      //------------------------------- 

            float prevStrokeW = _pcx.StrokeWidth;
            switch (T102_1_StrokeWidth)
            {
                default: throw new NotSupportedException();
                case T102_1_StrokeWidth.HalfPx:
                    _pcx.StrokeWidth = 0.5f;
                    break;
                case T102_1_StrokeWidth.OnePx:
                    _pcx.StrokeWidth = 1;
                    break;
                case T102_1_StrokeWidth.TwoPx:
                    _pcx.StrokeWidth = 2;
                    break;
                case T102_1_StrokeWidth.ThreePx:
                    _pcx.StrokeWidth = 3;
                    break;
                case T102_1_StrokeWidth.FourPx:
                    _pcx.StrokeWidth = 4;
                    break;
                case T102_1_StrokeWidth.FivePx:
                    _pcx.StrokeWidth = 5;
                    break;


            } 
            PixelFarm.Drawing.RenderSurfaceOrientation prevOrgKind = _pcx.OriginKind; //save
            switch (DrawSet)
            {
                default:
                case T102_1_Set.Lines:
                    {
                        _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftTop;
                        for (int i = 0; i < 400;)
                        {
                            _pcx.DrawLine(i + 10, i + 10, i + 30, i + 50);
                            i += 50;
                        }
                        //
                        _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftBottom;
                        for (int i = 0; i < 400;)
                        {
                            _pcx.DrawLine(i + 10, i + 10, i + 30, i + 50);
                            i += 50;
                        }
                    }
                    break;
                case T102_1_Set.FillRect:
                    {
                        _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftTop;
                        for (int i = 0; i < 400;)
                        {
                            _pcx.FillRect(PixelFarm.Drawing.Color.Red, i, i, 50, 50);
                            i += 50;
                        }
                        //
                        _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftBottom;
                        for (int i = 0; i < 400;)
                        {
                            _pcx.FillRect(PixelFarm.Drawing.Color.Red, i, i, 50, 50);
                            i += 50;
                        }
                    }
                    break;
                case T102_1_Set.DrawRect:
                    {
                        _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftTop;
                        for (int i = 0; i < 400;)
                        {
                            _painter.DrawRect(i, i, 50, 50);
                            i += 50;
                        }
                        //
                        _pcx.OriginKind = PixelFarm.Drawing.RenderSurfaceOrientation.LeftBottom;
                        for (int i = 0; i < 400;)
                        {
                            _painter.DrawRect(i, i, 50, 50);
                            i += 50;
                        }
                    }
                    break;
            }
            _pcx.OriginKind = prevOrgKind;//restore  


            _pcx.StrokeWidth = prevStrokeW;
        }
    }

}

