//MIT, 2016-present, WinterDev
//Apache2, https://xmlgraphics.apache.org/

using System;
using PixelFarm.Drawing;

namespace PixelFarm.DrawingGL
{
    public interface IGLTextPrinter : ITextPrinter
    {
        /// <summary>
        /// render from RenderVxFormattedString object to specific pos
        /// </summary>
        /// <param name="renderVx"></param>
        /// <param name="left"></param>
        /// <param name="top"></param>
        void DrawString(GLRenderVxFormattedString renderVx, double left, double top);
        void PrepareStringForRenderVx(GLRenderVxFormattedString renderVx, char[] text, int startAt, int len);
    }

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
        public IGLTextPrinter TextPrinter
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
#if DEBUG
                if (value.SizeInPoints == 0)
                {

                }
#endif

                _requestFont = value;
                _textPrinter?.ChangeFont(value);
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
#if DEBUG
                throw new NotSupportedException();
#else
                return null;
#endif
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
#if DEBUG
                throw new NotSupportedException();
#else
                return null;
#endif
            }
        }
        public override void DrawString(RenderVxFormattedString renderVx, double x, double y)
        {
            _textPrinter?.DrawString((GLRenderVxFormattedString)renderVx, x, y);
        }

        internal void CreateWordStrips(System.Collections.Generic.List<DrawingGL.GLRenderVxFormattedString> fmtStringList)
        {

            //
            int j = fmtStringList.Count;
            for (int i = 0; i < j; ++i)
            {
                //change state before send to the drawboard
                GLRenderVxFormattedString vxFmtStr = fmtStringList[i];
                vxFmtStr.UseWithWordPlate = true;
                vxFmtStr.Delay = false;
            }
            //


            RequestFont prevFont = CurrentFont; //save
            WordPlate latestWordplate = null;

            for (int i = 0; i < j; ++i)
            {
                GLRenderVxFormattedString vxFmtStr = fmtStringList[i];
                if (vxFmtStr.OwnerPlate != null)
                {
                    continue;
                }

                WordPlate wordPlate = _wordPlateMx.GetWordPlate(vxFmtStr);

                if (latestWordplate != wordPlate)
                {
                    if (latestWordplate != null)
                    {
                        _drawBoard.ExitCurrentDrawboardBuffer();
                    }

                    latestWordplate = wordPlate;
                    _drawBoard.EnterNewDrawboardBuffer(wordPlate._backBuffer);
                }

                //if (vxFmtStr.RequestFont != null)
                //{
                //    _drawBoard.CurrentFont = vxFmtStr.RequestFont;
                //}
                //else
                //{
                //    //use current font 
                //}

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

            this.CurrentFont = prevFont; //restore
        }
        internal bool TryCreateWordStrip(GLRenderVxFormattedString fmtString)
        {

            WordPlate wordPlate = _wordPlateMx.GetWordPlate(fmtString);
            if (wordPlate == null)
            {
#if DEBUG
                throw new NotSupportedException();
#else
                return false;
#endif
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


            //ensure font info for each vx formatter string?
            //if (fmtString.RequestFont != null)
            //{
            //    _drawBoard.CurrentFont = fmtString.RequestFont;
            //}
            //else
            //{

            //}


            if (!wordPlate.CreateWordStrip(this, fmtString))
            {
                //we have some error?
#if DEBUG
                throw new NotSupportedException();
#else
                return false;
#endif
            }
            fmtString.State = RenderVxFormattedString.VxState.Ready;

            _drawBoard.ExitCurrentDrawboardBuffer();
            _drawBoard.CurrentFont = backupFont;//restore
            return fmtString.OwnerPlate != null;
        }
    }
}