//BSD, 2018-present, WinterDev  

using System;
using System.Collections.Generic;

using PixelFarm.Drawing;

using PixelFarm.CpuBlit.PixelProcessing;
using PixelFarm.CpuBlit.BitmapAtlas;

using Typography.TextLayout;

using Mini;

namespace PixelFarm.CpuBlit.Sample_LionAlphaMask
{


    [Info(OrderCode = "00")]
    public class FontTextureDemo2 : DemoBase
    {
        AggPainter _maskBufferPainter;
        PixelBlenderWithMask _maskPixelBlender = new PixelBlenderWithMask();
        PixelBlenderPerColorComponentWithMask _maskPixelBlenderPerCompo = new PixelBlenderPerColorComponentWithMask();


        OpenFontTextService _textServices;
        TextServiceClient _txtClient;
        BitmapFontManager<MemBitmap> _bmpFontMx;
        SimpleBitmapAtlas _fontAtlas;
        RequestFont _font;
        MemBitmap _fontBmp;
        MemBitmap _alphaBmp;
        float _finalTextureScale = 1;

        public FontTextureDemo2()
        {
            this.Width = 800;
            this.Height = 600;
        }
        public override void Init()
        {
            //steps : detail ... 
            //1. create a text service (or get it from a singleton class)       

            _textServices = new OpenFontTextService();
            _txtClient = _textServices.CreateNewServiceClient();
            //2. create manager
            _bmpFontMx = new BitmapFontManager<MemBitmap>(
                _textServices,
                atlas => MemBitmap.CreateFromCopy(atlas.MainBitmap)
            );

            //3.  
            _font = new RequestFont("tahoma", 16);
            _fontAtlas = _bmpFontMx.GetFontAtlas(_font, out _fontBmp);
        }

        bool _pixelBlenderSetup = false;

        public void DrawString(Painter p, string text, double x, double y)
        {
            if (text != null)
            {
                DrawString(p, text.ToCharArray(), 0, text.Length, x, y);
            }

        }
        public void DrawString(Painter p, char[] buffer, int startAt, int len, double x, double y)
        {
            if (!(p is AggPainter painter)) return;
            //

            int width = painter.Width;
            int height = painter.Height;
            if (!_pixelBlenderSetup)
            {
                SetupMaskPixelBlender(width, height);
                _pixelBlenderSetup = true;
            }

            int j = buffer.Length;
            //create temp buffer span that describe the part of a whole char buffer
            var textBufferSpan = new TextBufferSpan(buffer, startAt, len);
            //ask text service to parse user input char buffer and create a glyph-plan-sequence (list of glyph-plan) 
            //with specific request font

            ResolvedFont font = _textServices.ResolveFont(_font);
            GlyphPlanSequence glyphPlanSeq = _txtClient.CreateGlyphPlanSeq(textBufferSpan, _font);

            float scale = 1;// _fontAtlas.TargetTextureScale;
            int recommendLineSpacing = (int)font.LineSpacingInPixels;
            //--------------------------
            //TODO:
            //if (x,y) is left top
            //we need to adjust y again
            y -= font.LineSpacingInPixels;

            // 
            float scaleFromTexture = _finalTextureScale;
            TextureKind textureKind = _fontAtlas.TextureKind;

            float gx = 0;
            float gy = 0;
            int baseY = (int)Math.Round(y);

            float acc_x = 0;
            float acc_y = 0;

            int lineHeight = (int)font.LineSpacingInPixels;//temp
            //painter.DestBitmapBlender.OutputPixelBlender = maskPixelBlenderPerCompo; //change to new blender 
            painter.DestBitmapBlender.OutputPixelBlender = _maskPixelBlenderPerCompo; //change to new blender  

            int seqLen = glyphPlanSeq.Count;
            for (int i = 0; i < seqLen; ++i)
            {
                UnscaledGlyphPlan glyph = glyphPlanSeq[i];
                if (!_fontAtlas.TryGetItem(glyph.glyphIndex, out AtlasItem atlaItem))
                {
                    //if no glyph data, we should render a missing glyph ***
                    continue;
                }
                //--------------------------------------
                //TODO: review precise height in float
                //-------------------------------------- 

                atlaItem.GetRect(out int srcX, out int srcY, out int srcW, out int srcH);

                float ngx = acc_x + (float)Math.Round(glyph.OffsetX * scale);
                float ngy = acc_y + (float)Math.Round(glyph.OffsetY * scale);
                //NOTE:
                // -glyphData.TextureXOffset => restore to original pos
                // -glyphData.TextureYOffset => restore to original pos 
                //--------------------------
                gx = (float)(x + (ngx - atlaItem.TextureXOffset) * scaleFromTexture); //ideal x
                gy = (float)(y + (ngy - atlaItem.TextureYOffset - srcH + lineHeight) * scaleFromTexture);

                acc_x += (float)Math.Round(glyph.AdvanceX * scale);
                gy = (float)Math.Floor(gy) + lineHeight;

                //clear with solid black color 
                //_maskBufferPainter.Clear(Color.Black);
                _maskBufferPainter.FillRect(gx - 1, gy - 1, srcW + 2, srcH + 2, Color.Black);
                //draw 'stencil' glyph on mask-buffer                
                //_maskBufferPainter.DrawImage(_fontBmp, gx, gy, srcX, _fontBmp.Height - (srcY), srcW, srcH);
                _maskBufferPainter.DrawImage(_fontBmp, gx, gy, srcX, srcY, srcW, srcH);

                //select component to render this need to render 3 times for lcd technique
                //1. B
                _maskPixelBlenderPerCompo.SelectedMaskComponent = PixelBlenderColorComponent.B;
                _maskPixelBlenderPerCompo.EnableOutputColorComponent = EnableOutputColorComponent.B;
                painter.FillRect(gx + 1, gy, srcW, srcH);
                //2. G
                _maskPixelBlenderPerCompo.SelectedMaskComponent = PixelBlenderColorComponent.G;
                _maskPixelBlenderPerCompo.EnableOutputColorComponent = EnableOutputColorComponent.G;
                painter.FillRect(gx + 1, gy, srcW, srcH);
                //3. R
                _maskPixelBlenderPerCompo.SelectedMaskComponent = PixelBlenderColorComponent.R;
                _maskPixelBlenderPerCompo.EnableOutputColorComponent = EnableOutputColorComponent.R;
                painter.FillRect(gx + 1, gy, srcW, srcH);
            }
        }

        void SetupMaskPixelBlender(int width, int height)
        {
            //----------
            //same size
            _alphaBmp = new MemBitmap(width, height);
            _maskBufferPainter = AggPainter.Create(_alphaBmp, new PixelBlenderBGRA());
            _maskBufferPainter.Clear(Color.Black);
            //------------ 
            //draw glyph bmp to _alpha bmp
            //_maskBufferPainter.DrawImage(_glyphBmp, 0, 0);
            _maskPixelBlender.SetMaskBitmap(_alphaBmp);
            _maskPixelBlenderPerCompo.SetMaskBitmap(_alphaBmp);
        }
        [DemoConfig]
        public PixelProcessing.PixelBlenderColorComponent SelectedComponent
        {
            get
            {
                if (_maskPixelBlender != null)
                {
                    return _maskPixelBlender.SelectedMaskComponent;
                }
                else
                {
                    return PixelProcessing.PixelBlenderColorComponent.R;//default
                }
            }
            set
            {

                _maskPixelBlender.SelectedMaskComponent = value;
                _maskPixelBlenderPerCompo.SelectedMaskComponent = value;
                this.InvalidateGraphics();
            }
        }


        public override void Draw(Painter p)
        {

            p.RenderQuality = RenderQuality.HighQuality;
            p.Orientation = PixelFarm.Drawing.RenderSurfaceOriginKind.LeftBottom;


            //clear the image to white         
            // draw a circle
            p.Clear(Drawing.Color.White);
            p.FillColor = Color.Black;

            //TODO: review here again
            ResolvedFont resolvedFont = _textServices.ResolveFont(_font);

            int lineSpaceInPx = resolvedFont.LineSpacingInPixels;
            int ypos = 0;
            DrawString(p, "Hello World", 10, ypos);
            ypos += lineSpaceInPx;
            //--------  

            p.FillColor = Color.Green;
            DrawString(p, "Hello World", 10, ypos);
            ypos += lineSpaceInPx;

            p.FillColor = Color.Blue;
            DrawString(p, "Hello World", 10, ypos);
            ypos += lineSpaceInPx;

            p.FillColor = Color.Red;
            DrawString(p, "Hello World", 10, ypos);
            ypos += lineSpaceInPx;

            p.FillColor = Color.Yellow;
            DrawString(p, "Hello World", 10, ypos);
            ypos += lineSpaceInPx;

            p.FillColor = KnownColors.Gray;
            DrawString(p, "Hello World", 10, ypos);
            ypos += lineSpaceInPx;

            p.FillColor = Color.Black;
            DrawString(p, "Hello World", 10, ypos);
            ypos += lineSpaceInPx;
        }




    }


}
