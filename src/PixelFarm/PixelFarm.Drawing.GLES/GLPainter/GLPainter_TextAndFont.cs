//MIT, 2016-present, WinterDev
//Apache2, https://xmlgraphics.apache.org/

using System;
using PixelFarm.Drawing;

namespace PixelFarm.DrawingGL
{
    partial class GLPainter
    {
        WordPlateMx _wordPlateMx = new WordPlateMx();
        GLBitmapGlyphTextPrinter _bmpTextPrinter;
        GlyphTexturePrinterDrawingTechnique _textPrinterDrawingTech = GlyphTexturePrinterDrawingTechnique.Stencil;
        Color _textBgColorHint = Color.White;

        public Color TextBgColorHint
        {
            get => _textBgColorHint;
            set
            {

#if DEBUG
                if (value.A == 0)
                {

                }
#endif
                _textBgColorHint = value;
            }
        }
        internal bool PreparingWordStrip { get; set; }
        public Color FontFillColor
        {
            get => _pcx.FontFillColor;
            set => _pcx.FontFillColor = value;
        }
        public GlyphTexturePrinterDrawingTechnique TextPrinterDrawingTechnique
        {
            get => _textPrinterDrawingTech;
            set
            {
                _textPrinterDrawingTech = value;
                if (_bmpTextPrinter != null)
                {
                    _bmpTextPrinter.TextDrawingTechnique = value;
                }
            }
        }
        public ITextPrinter TextPrinter
        {
            get => _textPrinter;
            set
            {
                _textPrinter = value;

                _bmpTextPrinter = value as GLBitmapGlyphTextPrinter;
                if (_bmpTextPrinter != null)
                {
                    _bmpTextPrinter.TextDrawingTechnique = _textPrinterDrawingTech;
                }

                if (value != null && _requestFont != null)
                {
                    _textPrinter.ChangeFont(_requestFont);
                }
            }
        }
        public override RequestFont CurrentFont
        {
            get => _requestFont;
            set
            {
                _requestFont = value;
                if (_textPrinter != null)
                {
                    _textPrinter.ChangeFont(value);
                }
            }
        }
        public override void DrawString(string text, double left, double top)
        {
            _textPrinter?.DrawString(text, left, top);
        }
        public override RenderVxFormattedString CreateRenderVx(string textspan)
        {

            if (_textPrinter != null)
            {
                char[] buffer = textspan.ToCharArray();
                var renderVxFmtStr = new GLRenderVxFormattedString();

#if DEBUG
                renderVxFmtStr.dbugText = textspan;
#endif
                _textPrinter?.PrepareStringForRenderVx(renderVxFmtStr, buffer, 0, buffer.Length);
                return renderVxFmtStr;
            }
            else
            {
                return null;
            }
        }
        public override RenderVxFormattedString CreateRenderVx(char[] textspanBuff, int startAt, int len)
        {
            if (_textPrinter != null)
            {
                var renderVxFmtStr = new GLRenderVxFormattedString();
#if DEBUG
                renderVxFmtStr.dbugText = new string(textspanBuff, startAt, len);
#endif
                _textPrinter?.PrepareStringForRenderVx(renderVxFmtStr, textspanBuff, startAt, len);
                return renderVxFmtStr;
            }
            else
            {
                return null;
            }
        }
        public override void DrawString(RenderVxFormattedString renderVx, double x, double y)
        {
            _textPrinter?.DrawString(renderVx, x, y);
        }

        internal void CreateWordStrips(System.Collections.Generic.List<DrawingGL.GLRenderVxFormattedString> fmtStringList)
        {
            int j = fmtStringList.Count;

            RequestFont backupFont = CurrentFont;
            WordPlate latestWordplate = null;

            for (int i = 0; i < j; ++i)
            {
                GLRenderVxFormattedString vxFmtStr = fmtStringList[i];
                if (vxFmtStr.OwnerPlate != null)
                {
                    continue;
                }

                WordPlate wordPlate = _wordPlateMx.GetNewWordPlate(vxFmtStr);

                if (latestWordplate != wordPlate)
                {
                    if (latestWordplate != null)
                    {
                        _drawBoard.ExitCurrentDrawboardBuffer();
                    }

                    latestWordplate = wordPlate;
                    _drawBoard.EnterNewDrawboardBuffer(wordPlate._backBuffer);
                }

                if (vxFmtStr.RequestFont != null)
                {
                    _drawBoard.CurrentFont = vxFmtStr.RequestFont;
                }
                else
                {

                }

                if (!wordPlate.CreateWordStrip(this, vxFmtStr))
                {
                    //we have some error?
                    throw new NotSupportedException();
                }

                vxFmtStr.State = RenderVxFormattedString.VxState.Ready;
            }
            if (latestWordplate != null)
            {
                _drawBoard.ExitCurrentDrawboardBuffer();
            }

            this.CurrentFont = backupFont; //restore
        }
        internal void CreateWordStrip(GLRenderVxFormattedString fmtString)
        {

            WordPlate wordPlate = _wordPlateMx.GetNewWordPlate(fmtString);
            if (wordPlate == null)
            {
                throw new NotSupportedException();
            }


            //{
            //    //save output
            //    using (Image img = wordPlate._backBuffer.CopyToNewMemBitmap())
            //    {
            //        MemBitmap memBmp = img as MemBitmap;
            //        if (memBmp != null)
            //        {
            //            memBmp.SaveImage("testx_01.png");
            //        }
            //    }
            //}

            RequestFont backupFont = _drawBoard.CurrentFont; //backup
            _drawBoard.EnterNewDrawboardBuffer(wordPlate._backBuffer);

            if (fmtString.RequestFont != null)
            {
                _drawBoard.CurrentFont = fmtString.RequestFont;
            }
            else
            {

            }
            if (!wordPlate.CreateWordStrip(this, fmtString))
            {
                //we have some error?
                throw new NotSupportedException();
            }
            fmtString.State = RenderVxFormattedString.VxState.Ready;



            _drawBoard.ExitCurrentDrawboardBuffer();
            _drawBoard.CurrentFont = backupFont;//restore
        }
    }
}