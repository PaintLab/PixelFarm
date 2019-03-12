//MIT, 2019-present,WinterDev

using System;
using Mini;
using PixelFarm.DrawingGL;
namespace OpenTkEssTest
{

    [Info(OrderCode = "406")]
    [Info("T406_GLBitmapAtlas", AvailableOn = AvailableOn.GLES)]
    public class T406_GLBitmapAtlas : DemoBase
    {
        GLPainterContext _pcx;

        GLPainter _painter;
        BitmapAtlasPainter _bmpAtlasPainter;

        protected override void OnGLPainterReady(GLPainter painter)
        {

            _pcx = painter.PainterContext;
            _painter = painter;
            _bmpAtlasPainter = new BitmapAtlasPainter();
            string atlasInfoFile = "d:\\WImageTest\\test1_atlas";
            _bmpAtlasPainter.ChangeBitmapAtlas(atlasInfoFile);

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
            _pcx.ClearColorBuffer();

            _bmpAtlasPainter.DrawImage(_painter, @"\chk_checked.png", 0, 0);
            _bmpAtlasPainter.DrawImage(_painter, @"\chk_unchecked.png", 20, 0);

            SwapBuffers();
        }

    }

}

