//MIT, 2014-present,WinterDev

using System;
using PixelFarm.Drawing;
using Mini;
using PixelFarm.DrawingGL;
using PixelFarm.Drawing.Fonts;
using Typography.FontManagement;

namespace OpenTkEssTest
{
    [Info(OrderCode = "402", AvailableOn = AvailableOn.GLES)]
    [Info("T402_BrushTest2")]
    public class T402_BrushTest2 : DemoBase
    {
        GLPainterContext _pcx;
        GLPainter _painter;
        RenderVx _glyph_vx;
        LinearGradientBrush _linearGrBrush2;
        VertexStore _tempSnap1;

        protected override void OnGLPainterReady(GLPainter painter)
        {
            _pcx = painter.PainterContext;
            _painter = painter;
        }
        protected override void OnReadyForInitGLShaderProgram()
        {


            InstalledTypefaceCollection collection = new InstalledTypefaceCollection();
            collection.LoadSystemFonts();
            InstalledTypeface tahomaFont = collection.GetInstalledTypeface("tahoma", TypefaceStyle.Regular);
            FontFace tahomaFace = OpenFontLoader.LoadFont(tahomaFont.FontPath);
            ActualFont actualFont = tahomaFace.GetFontAtPointSize(72);
            FontGlyph glyph = (FontGlyph)actualFont.GetGlyph('K');


            _glyph_vx = _painter.CreateRenderVx(_tempSnap1 = glyph.flattenVxs);

            _linearGrBrush2 = new LinearGradientBrush(
               new PointF(0, 0), new PointF(100, 100),
               Color.Red, Color.Black);

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
            _painter.FillColor = PixelFarm.Drawing.Color.Black;
            //painter.FillRectLBWH(0, 0, 150, 150);
            //GLBitmap glBmp = LoadTexture("..\\logo-dark.jpg");
            //var textureBrush = new TextureBrush(new GLImage(glBmp));
            //painter.FillRenderVx(textureBrush, polygon1);
            ////------------------------------------------------------------------------- 

            //fill
            _painter.FillColor = PixelFarm.Drawing.Color.Black;
            _painter.FillRenderVx(_linearGrBrush2, _glyph_vx);
            //painter.FillRenderVx(glyph_vx);
            //-------------------------------------------------------------------------  


            SwapBuffers();
        }
    }
}

