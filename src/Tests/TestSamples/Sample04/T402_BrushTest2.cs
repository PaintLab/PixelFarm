//MIT, 2014-2016,WinterDev

using System;
using PixelFarm.Drawing;
using Mini;
using PixelFarm.DrawingGL;
using PixelFarm.Drawing.Fonts;
using Typography.TextServices;

namespace OpenTkEssTest
{
    [Info(OrderCode = "402")]
    [Info("T402_BrushTest2")]
    public class T402_BrushTest2 : DemoBase
    {
        GLRenderSurface _glsx;
        GLPainter painter;
        RenderVx glyph_vx;
        LinearGradientBrush linearGrBrush2;
        VertexStoreSnap tempSnap1;

        protected override void OnGLSurfaceReady(GLRenderSurface glsx, GLPainter painter)
        {
            this._glsx = glsx;
            this.painter = painter;
        }
        protected override void OnReadyForInitGLShaderProgram()
        {


            InstalledFontCollection collection = new InstalledFontCollection();
            collection.LoadSystemFonts();
            InstalledFont tahomaFont = collection.GetFont("tahoma", InstalledFontStyle.Normal);
            FontFace tahomaFace = OpenFontLoader.LoadFont(tahomaFont.FontPath);
            ActualFont actualFont = tahomaFace.GetFontAtPointSize(72);
            FontGlyph glyph = (FontGlyph)actualFont.GetGlyph('K');


            glyph_vx = painter.CreateRenderVx(tempSnap1 = new PixelFarm.Drawing.VertexStoreSnap(glyph.flattenVxs));

            linearGrBrush2 = new LinearGradientBrush(
               new PointF(0, 0), Color.Red,
               new PointF(100, 100), Color.Black);

        }

        protected override void DemoClosing()
        {
            _glsx.Dispose();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            _glsx.SmoothMode = SmoothMode.Smooth;
            _glsx.StrokeColor = PixelFarm.Drawing.Color.Blue;
            _glsx.ClearColorBuffer();
            painter.FillColor = PixelFarm.Drawing.Color.Black;
            //painter.FillRectLBWH(0, 0, 150, 150);
            //GLBitmap glBmp = LoadTexture("..\\logo-dark.jpg");
            //var textureBrush = new TextureBrush(new GLImage(glBmp));
            //painter.FillRenderVx(textureBrush, polygon1);
            ////------------------------------------------------------------------------- 

            //fill
            painter.FillColor = PixelFarm.Drawing.Color.Black;
            painter.FillRenderVx(linearGrBrush2, glyph_vx);
            //painter.FillRenderVx(glyph_vx);
            //-------------------------------------------------------------------------  


            SwapBuffers();
        }
    }
}

