//MIT, 2014-present,WinterDev

using System;
using Mini;
using PixelFarm.DrawingGL;
using PixelFarm.CpuBlit;
using PaintLab.Svg;
namespace OpenTkEssTest
{
    [Info(OrderCode = "108")]
    [Info("T108_LionFill", SupportedOn = AvailableOn.GLES)]
    public class T108_LionFill : DemoBase
    {

        GLPainterContext _pcx;
        SpriteShape _spriteShape;
        GLPainter _painter;
        protected override void OnGLPainterReady(GLPainter painter)
        {
            _pcx = painter.PainterContext;
            _painter = painter;
        }

        protected override void OnReadyForInitGLShaderProgram()
        {
            //string sampleFile = "Samples/lion.svg";
            //string sampleFile = "Samples/tiger_whiskers.svg";
            string sampleFile = "Samples/tiger002.svg";
            //string sampleFile = "Samples/tiger_wrinkles.svg"; 
            VgVisualElement vgVisElem = VgVisualDocHelper.CreateVgVisualDocFromFile(sampleFile).VgRootElem;
            _spriteShape = new SpriteShape(vgVisElem);
        }

        protected override void DemoClosing()
        {
            _pcx.Dispose();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            _pcx.SmoothMode = SmoothMode.Smooth;
            _pcx.StrokeColor = PixelFarm.Drawing.Color.Blue;
            _pcx.ClearColorBuffer();
            //------------------------------- 
            _spriteShape.Paint(_painter);
            //-------------------------------
            SwapBuffers();
        }
    }
}

