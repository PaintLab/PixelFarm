//BSD, 2014-present, WinterDev

//MatterHackers: BSD
// Much of the ui to the drawing functions still needs to be C#'ed and cleaned up.  A lot of
// it still follows the originall agg function names.  I have been cleaning these up over time
// and intend to do much more refactoring of these things over the long term.

using System;
using System.Collections.Generic;

using PixelFarm.Drawing;
using PixelFarm;
using PixelFarm.Drawing.Fonts;
using Mini;


using Typography.OpenFont;
using Typography.Rendering;
using Typography.TextLayout;
namespace PixelFarm.CpuBlit.Sample_Draw
{

    [Info(OrderCode = "01")]
    public class DrawSample04 : DemoBase
    {


        LayoutFarm.OpenFontTextService _textServices;
        BitmapFontManager<ActualBitmap> _bmpFontMx;
        SimpleFontAtlas _fontAtlas;
        RequestFont _font;
        ActualBitmap _fontBmp;



        public override void Init()
        {
            //steps : detail ... 
            //1. create a text service (or get it from a singleton class)       

            _textServices = new LayoutFarm.OpenFontTextService();

            //2. create manager
            _bmpFontMx = new BitmapFontManager<ActualBitmap>(
                TextureKind.StencilLcdEffect,
                _textServices,
                atlas =>
                {
                    GlyphImage totalGlyphImg = atlas.TotalGlyph;
                    return new ActualBitmap(totalGlyphImg.Width, totalGlyphImg.Height, totalGlyphImg.GetImageBuffer());
                }
            );
            _bmpFontMx.SetCurrentScriptLangs(new ScriptLang[]
            {
                ScriptLangs.Latin
            });

            //3.  
            _font = new RequestFont("tahoma", 10);
            _fontAtlas = _bmpFontMx.GetFontAtlas(_font, out _fontBmp);


            //----------
        }



        float _finalTextureScale = 1;


        public void DrawString(Painter p, string text, double x, double y)
        {
            if (text != null)
            {
                DrawString(p, text.ToCharArray(), 0, text.Length, x, y);
            }

        }
        public void DrawString(Painter p, char[] buffer, int startAt, int len, double x, double y)
        {

            //if (_stencilBmp == null)
            //{
            //    //create a stencil bmp
            //    _stencilBmp = new ActualBitmap(p.Width, p.Height);
            //    _stencilBlender = new PixelProcessing.SubBitmapBlender(_stencilBmp, new PixelProcessing.PixelBlenderBGRA());
            //    _backPainter = AggPainter.Create(_stencilBmp);
            //    //------
            //}

            int j = buffer.Length;
            //create temp buffer span that describe the part of a whole char buffer
            TextBufferSpan textBufferSpan = new TextBufferSpan(buffer, startAt, len);

            //ask text service to parse user input char buffer and create a glyph-plan-sequence (list of glyph-plan) 
            //with specific request font
            GlyphPlanSequence glyphPlanSeq = _textServices.CreateGlyphPlanSeq(ref textBufferSpan, _font);

            Typeface typeface = _textServices.ResolveTypeface(_font);
            float scale = typeface.CalculateScaleToPixelFromPointSize(_font.SizeInPoints);

            int recommendLineSpacing = (int)_font.LineSpacingInPx;
            //--------------------------
            //TODO:
            //if (x,y) is left top
            //we need to adjust y again
            y -= ((_font.LineSpacingInPx) * scale);

            // 
            float scaleFromTexture = _finalTextureScale;
            TextureKind textureKind = _fontAtlas.TextureKind;

            float g_x = 0;
            float g_y = 0;
            int baseY = (int)Math.Round(y);

            //-------------------------------------
            //load texture 
            //_glsx.LoadTexture1(_glBmp);
            //-------------------------------------


            float acc_x = 0;
            float acc_y = 0;
            p.DrawImage(_fontBmp, 100, 100);

            int seqLen = glyphPlanSeq.Count;
            for (int i = 0; i < seqLen; ++i)
            {
                UnscaledGlyphPlan glyph = glyphPlanSeq[i];
                TextureGlyphMapData glyphData;
                if (!_fontAtlas.TryGetGlyphMapData(glyph.glyphIndex, out glyphData))
                {
                    //if no glyph data, we should render a missing glyph ***
                    continue;
                }
                //--------------------------------------
                //TODO: review precise height in float
                //-------------------------------------- 
                int srcX, srcY, srcW, srcH;
                glyphData.GetRect(out srcX, out srcY, out srcW, out srcH);

                float ngx = acc_x + (float)Math.Round(glyph.OffsetX * scale);
                float ngy = acc_y + (float)Math.Round(glyph.OffsetY * scale);
                //NOTE:
                // -glyphData.TextureXOffset => restore to original pos
                // -glyphData.TextureYOffset => restore to original pos 
                //--------------------------
                g_x = (float)(x + (ngx - glyphData.TextureXOffset) * scaleFromTexture); //ideal x
                g_y = (float)(y + (ngy - glyphData.TextureYOffset + srcH) * scaleFromTexture);

                acc_x += (float)Math.Round(glyph.AdvanceX * scale);
                //g_x = (float)Math.Round(g_x);
                g_y = (float)Math.Floor(g_y);

                //p.RenderQuality = RenderQualtity.Fast;

                //*** the atlas is inverted so...
                //p.DrawImage(_fontBmp, g_x, g_y, srcX, _fontBmp.Height - (srcY), srcW, srcH);
                //p.DrawImage(_fontBmp, g_x, g_y);

                //1. draw to back buffer 
                //_backPainter.DrawImage(_fontBmp, g_x, g_y, srcX, _fontBmp.Height - (srcY), srcW, srcH);

                //2. then copy content to this

                //p.DrawImage(_stencilBmp, 100, 100);
                p.DrawImage(_fontBmp, g_x, g_y, srcX, _fontBmp.Height - (srcY + srcH), srcW, srcH);
                switch (textureKind)
                {
                    default:
                        break;
                    case TextureKind.StencilLcdEffect:
                        {
                        }
                        break;
                }



                //copy some part from the bitmap 
                //switch (textureKind)
                //{
                //    case TextureKind.Msdf: 
                //        _glsx.DrawSubImageWithMsdf(_glBmp,
                //            ref srcRect,
                //            g_x,
                //            g_y,
                //            scaleFromTexture); 
                //        break;
                //    case TextureKind.StencilGreyScale: 
                //        //stencil gray scale with fill-color
                //        _glsx.DrawGlyphImageWithStecil(_glBmp,
                //         ref srcRect,
                //            g_x,
                //            g_y,
                //            scaleFromTexture); 
                //        break;
                //    case TextureKind.Bitmap:
                //        _glsx.DrawSubImage(_glBmp,
                //         ref srcRect,
                //            g_x,
                //            g_y,
                //            scaleFromTexture);
                //        break;
                //    case TextureKind.StencilLcdEffect: 
                //        _glsx.WriteVboToList(
                //          _vboBufferList,
                //          _indexList,
                //          ref srcRect,
                //          g_x,
                //          g_y,
                //          scaleFromTexture);

                //        break;
                //}
            }
            //-------
            //we create vbo first 
            //then render 

        }

        [DemoConfig]
        public bool UseBitmapExt
        {
            get;
            set;
        }
        public override void Draw(Painter p)
        {

            if (UseBitmapExt)
            {
                p.RenderQuality = RenderQualtity.Fast;
            }
            else
            {
                p.RenderQuality = RenderQualtity.HighQuality;
            }

            p.Orientation = DrawBoardOrientation.LeftBottom;


            //clear the image to white         
            // draw a circle
            p.Clear(Drawing.Color.White);
            p.FillColor = Color.Black;
            //-------- 

            DrawString(p, "1234567890", 10, 20);

        }
    }


}