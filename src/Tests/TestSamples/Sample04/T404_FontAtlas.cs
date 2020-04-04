//MIT, 2014-present,WinterDev

using System;
using Mini;
using PixelFarm.DrawingGL;
using PixelFarm.CpuBlit.BitmapAtlas;
using PixelFarm.Contours;

using Typography.OpenFont;

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
        SimpleBitmapAtlas _fontAtlas;


        Typeface _typeface;
        ushort _glyphIndex_0;
        ushort _glyphIndex_1;
        protected override void OnGLPainterReady(GLPainter painter)
        {
            _pcx = painter.PainterContext;
            _painter = painter;
        }
        protected override void OnReadyForInitGLShaderProgram()
        {

            //just an example
            //this is slow on init.
            //since we must wait until msdf texture generation is complete.
            //in real-world, we should use caching.

            using (System.IO.FileStream fs = new System.IO.FileStream("Samples\\SourceSansPro-Regular.ttf", System.IO.FileMode.Open))
            {
                _typeface = new OpenFontReader().Read(fs);
            }

            var reqFont = new PixelFarm.Drawing.RequestFont("Source Sans Pro", 32);

            //1. create glyph-texture-bitmap generator
            var glyphTextureGen = new GlyphTextureBitmapGenerator();
            glyphTextureGen.MsdfGenVersion = 3;

            //2. generate the glyphs
            SimpleBitmapAtlasBuilder atlasBuilder = glyphTextureGen.CreateTextureFontFromBuildDetail(
                _typeface,
                reqFont.SizeInPoints,
                 TextureKind.Msdf,
                GlyphTextureCustomConfigs.TryGetGlyphTextureBuildDetail(reqFont, false, false)
            );




            //3. set information before write to font-info
            atlasBuilder.FontFilename = reqFont.Name;//TODO: review here, check if we need 'filename' or 'fontname'
            atlasBuilder.FontKey = reqFont.FontKey;
            atlasBuilder.SpaceCompactOption = SimpleBitmapAtlasBuilder.CompactOption.ArrangeByHeight;

            //4. merge all glyph in the builder into a single image
            PixelFarm.CpuBlit.MemBitmap totalGlyphsImg = atlasBuilder.BuildSingleImage(true);
            //-------------------------------------------------------------

            //5. create a simple font atlas from information inside this atlas builder.
            _fontAtlas = atlasBuilder.CreateSimpleBitmapAtlas();
            _fontAtlas.SetMainBitmap(totalGlyphsImg, true);

            byte[] codepoint = System.Text.Encoding.UTF8.GetBytes("AB");
            _glyphIndex_0 = _typeface.GetGlyphIndex(codepoint[0]);
            _glyphIndex_1 = _typeface.GetGlyphIndex(codepoint[1]);
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
                _msdf_bmp = DemoHelper.LoadTexture(_fontAtlas.MainBitmap);
                _resInit = true;
            }

            _painter.Clear(PixelFarm.Drawing.Color.White);


            _fontAtlas.TryGetItem(_glyphIndex_0, out AtlasItem glyphData);
            PixelFarm.Drawing.Rectangle r =
                   new PixelFarm.Drawing.Rectangle(glyphData.Left,
                   glyphData.Top,
                   glyphData.Width,
                   glyphData.Height);

            _pcx.DrawSubImageWithMsdf(_msdf_bmp, ref r, 100, 40);

            _fontAtlas.TryGetItem(_glyphIndex_1, out glyphData);
            PixelFarm.Drawing.Rectangle r2 = new PixelFarm.Drawing.Rectangle(glyphData.Left,
                   glyphData.Top,
                   glyphData.Width,
                   glyphData.Height);
            _pcx.DrawSubImageWithMsdf(_msdf_bmp, ref r2, 100 + r.Width, 40);

            //full image
            _pcx.DrawImage(_msdf_bmp, 0, 100);
            SwapBuffers();
        }
        
    }

}

