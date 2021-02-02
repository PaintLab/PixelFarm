//MIT, 2016-present, WinterDev
//Apache2, https://xmlgraphics.apache.org/

using System;
using PixelFarm.Drawing;

namespace PixelFarm.DrawingGL
{
    public interface IGLTextPrinter
    {
        /// <summary>
        /// render from RenderVxFormattedString object to specific pos
        /// </summary>
        /// <param name="renderVx"></param>
        /// <param name="left"></param>
        /// <param name="top"></param>
        void DrawString(GLRenderVxFormattedString renderVx, double left, double top);
        void PrepareStringForRenderVx(GLRenderVxFormattedString renderVx, char[] text, int startAt, int len);
        void PrepareStringForRenderVx(GLRenderVxFormattedString renderVx, int[] text, int startAt, int len);
        void PrepareStringForRenderVx(GLRenderVxFormattedString renderVx, IFormattedGlyphPlanList formattedGlyphPlans);


        void ChangeFont(RequestFont font);
        void ChangeFillColor(Color fillColor);
        void ChangeStrokeColor(Color strokColor);
        TextBaseline TextBaseline { get; set; }
        void DrawString(char[] text, int startAt, int len, double left, double top);

        /// <summary>
        /// text drawing technique
        /// </summary>
        GlyphTexturePrinterDrawingTechnique TextDrawingTechnique { get; set; }
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
            char[] buff = text.ToCharArray();
            _textPrinter?.DrawString(buff, 0, buff.Length, left, top);
        }
        public override RenderVxFormattedString CreateRenderVx(string textspan)
        {

            if (_textPrinter != null)
            {
                char[] buffer = textspan.ToCharArray();
                var fmtstr = new GLRenderVxFormattedString();
#if DEBUG
                fmtstr.dbugText = textspan;
#endif
                _textPrinter?.PrepareStringForRenderVx(fmtstr, buffer, 0, buffer.Length);
                fmtstr.ReleaseIntermediateStructures();
                return fmtstr;
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
        public override RenderVxFormattedString CreateRenderVx(IFormattedGlyphPlanList formattedGlyphPlans)
        {
            if (_textPrinter != null)
            {
                var fmtstr = new GLRenderVxFormattedString();
                _textPrinter?.PrepareStringForRenderVx(fmtstr, formattedGlyphPlans);
                fmtstr.ReleaseIntermediateStructures();
                return fmtstr;
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
                var fmtstr = new GLRenderVxFormattedString();
#if DEBUG
                fmtstr.dbugText = new string(textspanBuff, startAt, len);
#endif
                _textPrinter?.PrepareStringForRenderVx(fmtstr, textspanBuff, startAt, len);
                fmtstr.ReleaseIntermediateStructures();
                return fmtstr;
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


        int _creationCycle;

        internal void CreateWordStrips(System.Collections.Generic.List<DrawingGL.GLRenderVxFormattedString> fmtStringList)
        {
            //create a set of frm string
            //
            if (_creationCycle > ushort.MaxValue)
            {
                _creationCycle = 0;//reset
            }
            _creationCycle++;

            int j = fmtStringList.Count;
            
            for (int i = j - 1; i >= 0; --i)
            {
                //change state before send to the drawboard
                GLRenderVxFormattedString vxFmtStr = fmtStringList[i];
                vxFmtStr.UseWithWordPlate = true;
                vxFmtStr.Delay = false;
                if (vxFmtStr.CreationCycle > 0 && vxFmtStr.CreationCycle == _creationCycle - 1)
                {
                    vxFmtStr.SkipCreation = true;
                    continue;
                }
                vxFmtStr.SkipCreation = false;
                vxFmtStr.CreationCycle = _creationCycle;
            }

#if DEBUG
            int diff = j - fmtStringList.Count;
            if (diff > 5)
            {

            }
#endif

            //
            j = fmtStringList.Count;//reset  
            if (j == 0)
            {
                return;
            }


            RequestFont prevFont = CurrentFont; //save
            WordPlate latestWordplate = null;

            //if (j > 20)
            //{
            //    j = 20;
            //}
            for (int i = 0; i < j; ++i)
            {
                GLRenderVxFormattedString vxFmtStr = fmtStringList[i];
                if (vxFmtStr.OwnerPlate != null || vxFmtStr.SkipCreation)
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
#if DEBUG
                if (vxFmtStr.StripCount == 0)
                {

                }
#endif

                if (!wordPlate.CreateWordStrip(this, vxFmtStr))
                {
                    //we have some error?
                    throw new NotSupportedException();
                }
                vxFmtStr.IsReset = false;

                vxFmtStr.State = RenderVxFormattedString.VxState.Ready;
            }
            if (latestWordplate != null)
            {
                _drawBoard.ExitCurrentDrawboardBuffer();
            }

            this.CurrentFont = prevFont; //restore
        }

#if DEBUG
        System.Diagnostics.Stopwatch dbugsw2 = new System.Diagnostics.Stopwatch();
#endif
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
#if DEBUG
            dbugsw2.Reset();
            dbugsw2.Start();
#endif
            _drawBoard.EnterNewDrawboardBuffer(wordPlate._backBuffer);

            //ensure font info for each vx formatter string ?

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
            fmtString.IsReset = false;

#if DEBUG
            dbugsw2.Stop();
            long ms = dbugsw2.ElapsedMilliseconds;
            if (ms > 3)
            {
                //Console.WriteLine("enter-exit:" + ms);
            }
#endif
            _drawBoard.CurrentFont = backupFont;//restore
            fmtString.CreationState = GLRenderVxFormattedStringState.S2_TextureStrip;
            return fmtString.OwnerPlate != null;
        }
    }
}