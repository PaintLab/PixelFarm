//MIT, 2014-present,WinterDev

using System;
using Mini;
using PixelFarm.DrawingGL;
using PixelFarm.Drawing.Fonts;
using PixelFarm.Contours;
namespace OpenTkEssTest
{
    [Info(OrderCode = "404")]
    [Info("T404_FontAtlas", AvailableOn = AvailableOn.GLES)]
    public class T404_FontAtlas : DemoBase
    {
        GLPainterContext _pcx;
        bool _resInit;
        GLBitmap _msdf_bmp;
        GLPainter _painter;
        SimpleFontAtlas _fontAtlas;
        PixelFarm.CpuBlit.MemBitmap _totalBmp;

        protected override void OnGLPainterReady(GLPainter painter)
        {
            _pcx = painter.PainterContext;
            _painter = painter;
        }
        protected override void OnReadyForInitGLShaderProgram()
        {

            //---------------------  
            var atlasBuilder = new Typography.Rendering.SimpleFontAtlasBuilder();
            string font_atlas = "Source Sans Pro_-1783789996";

            using (System.IO.FileStream fs = new System.IO.FileStream("Samples\\" + font_atlas + ".info", System.IO.FileMode.Open))
            {
                _fontAtlas = atlasBuilder.LoadFontAtlasInfo(fs)[0];
            }

            PixelFarm.CpuBlit.MemBitmap actualImg = null;
            using (System.IO.FileStream fs = new System.IO.FileStream("Samples\\" + font_atlas + ".info.png", System.IO.FileMode.Open))
            {
                actualImg = PixelFarm.CpuBlit.MemBitmap.LoadBitmap(fs);
            }
            _totalBmp = actualImg;
            _fontAtlas.TotalGlyph = PixelFarm.CpuBlit.MemBitmap.CreateFromCopy(actualImg);
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
            if (!_resInit)
            {
                // msdf_bmp = LoadTexture(@"..\msdf_75.png");
                //msdf_bmp = LoadTexture(@"a001_x1_66.png");
                _msdf_bmp = DemoHelper.LoadTexture(_totalBmp);
                //msdf_bmp = LoadTexture(@"a001_x1.png");
                //msdf_bmp = LoadTexture(@"msdf_65.png"); 
                _resInit = true;
            }

            _painter.Clear(PixelFarm.Drawing.Color.White);
            //var f = painter.CurrentFont;

            //painter.DrawString("hello!", 0, 20);
            //canvas2d.DrawImageWithSubPixelRenderingMsdf(msdf_bmp, 200, 500, 15f);

            Typography.Rendering.TextureGlyphMapData glyphData;

            byte[] codepoint = System.Text.Encoding.UTF8.GetBytes("AB");
            _fontAtlas.TryGetGlyphMapData(codepoint[0], out glyphData);
            PixelFarm.Drawing.Rectangle r =
                   new PixelFarm.Drawing.Rectangle(glyphData.Left,
                   glyphData.Top,
                   glyphData.Width,
                   glyphData.Height);
            //canvas2d.DrawSubImageWithMsdf(msdf_bmp, ref r, 100, 500);
            _pcx.DrawSubImageWithMsdf(_msdf_bmp, ref r, 100, 500);

            _fontAtlas.TryGetGlyphMapData(codepoint[1], out glyphData);
            PixelFarm.Drawing.Rectangle r2 = new PixelFarm.Drawing.Rectangle(glyphData.Left,
                   glyphData.Top,
                   glyphData.Width,
                   glyphData.Height);
            _pcx.DrawSubImageWithMsdf(_msdf_bmp, ref r2, 100 + r.Width - 10, 500);

            //full image
            _pcx.DrawImage(_msdf_bmp, 100, 300);
            SwapBuffers();
        }
        static PixelFarm.Drawing.Rectangle ConvToRect(Rectangle r)
        {
            return PixelFarm.Drawing.Rectangle.FromLTRB(r.Left, r.Top, r.Right, r.Bottom);
        }
    }

}

