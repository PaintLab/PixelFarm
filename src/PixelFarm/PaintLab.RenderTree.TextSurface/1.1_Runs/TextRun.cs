//Apache2, 2014-present, WinterDev

using System;
using System.Text;
using PixelFarm.Drawing;

namespace LayoutFarm.TextEditing
{

    class TextRun : Run, IDisposable
    {
        //text run is a collection of words that has the same presentation format (font, style, color, etc).
        //a run may contain multiple words

#if DEBUG
        char[] _dbugmybuffer;
        char[] _mybuffer => _dbugmybuffer;
#else
        char[] _mybuffer;
#endif

        int[] _outputUserCharAdvances = null;//TODO: review here-> change this to caret stop position
        bool _content_unparsed;
        ILineSegmentList _lineSegs;
        RenderVxFormattedString _renderVxFormattedString;

        public TextRun(RunStyle runstyle, char[] copyBuffer)
            : base(runstyle)
        {
#if DEBUG
            if (copyBuffer.Length == 0)
            {

            }
#endif
            //we need font info (in style) for evaluating the size fo this span
            //without font info we can't measure the size of this span 
            SetNewContent(copyBuffer);
            UpdateRunWidth();
        }
        public bool DelayFormattedString { get; set; }
        public void Dispose()
        {
            if (_renderVxFormattedString != null)
            {
                _renderVxFormattedString.Dispose();
                _renderVxFormattedString = null;
            }
        }

        //each editable run has it own (dynamic) char buffer 
        void SetNewContent(char[] newbuffer)
        {
#if DEBUG
            _dbugmybuffer = newbuffer;
#else
            _mybuffer = newbuffer;
#endif

            _content_unparsed = true;

            _renderVxFormattedString?.Dispose(); //clear old _renderVxFormattedString
            _renderVxFormattedString = null;
        }
        public override CopyRun CreateCopy()
        {
            char[] copyBuffer = new char[_mybuffer.Length];
            Array.Copy(_mybuffer, copyBuffer, copyBuffer.Length);
            return new CopyRun(copyBuffer);
        }
        public override void WriteTo(StringBuilder stbuilder) => stbuilder.Append(_mybuffer);

        public override CopyRun Copy(int startIndex)
        {
            int length = _mybuffer.Length - startIndex;
            if (startIndex > -1 && length > 0)
            {
                return MakeCopy(startIndex, length);
            }
            else
            {
                return null;
            }
        }
        CopyRun MakeCopy(int sourceIndex, int length)
        {
            if (length > 0)
            {
                //CopyRun newTextRun = null;
                char[] newContent = new char[length];
                Array.Copy(_mybuffer, sourceIndex, newContent, 0, length);
                return new CopyRun(newContent);
            }
            else
            {
                throw new Exception("string must be null or zero length");
            }
        }

        public override int GetRunWidth(int charOffset) => CalculateDrawingStringSize(charOffset).Width;

        public override string GetText() => new string(_mybuffer);

        public override void UpdateRunWidth()
        {

            var textBufferSpan = new TextBufferSpan(_mybuffer);

            //TODO: review here, 
            //1. if mybuffer lenght is not changed,we don't need to alloc new array?

            _outputUserCharAdvances = new int[_mybuffer.Length];

            if (_renderVxFormattedString != null)
            {
                _renderVxFormattedString.Dispose();
                _renderVxFormattedString = null;
            }

            var measureResult = new TextSpanMeasureResult();
            measureResult.outputXAdvances = _outputUserCharAdvances;

            if (SupportWordBreak)
            {
                if (_content_unparsed)
                {
                    //parse the content first 
                    _lineSegs = BreakToLineSegs(textBufferSpan);
                }
                _content_unparsed = false;
                MeasureString2(textBufferSpan, _lineSegs, ref measureResult);
            }
            else
            {
                MeasureString2(textBufferSpan, null, ref measureResult);
            }


            SetSize(measureResult.outputTotalW, measureResult.lineHeight);

            InvalidateGraphics();
        }
        protected void AdjustClientBounds(ref Rectangle bounds)
        {
            if (this.OwnerLine != null)
            {
                bounds.Offset(0, this.OwnerLine.Top);
            }
        }
        public override char GetChar(int index) => _mybuffer[index];

        public override void CopyContentToStringBuilder(StringBuilder stBuilder) => stBuilder.Append(_mybuffer);

        public override int CharacterCount => _mybuffer.Length;

        public override void SetStyle(RunStyle runstyle)
        {
            //TODO: review this again
            //update style may affect the 'visual' layout of the span
            //the span may expand large or shrink down
            //so we invalidate graphics area pre and post

            //TODO: review here***
            this.InvalidateGraphics();
            base.SetStyle(runstyle);
            this.InvalidateGraphics();
            UpdateRunWidth();
        }
        Size CalculateDrawingStringSize(int length)
        {
            if (!_content_unparsed)
            {
                //the content is parsed ***

                if (_mybuffer.Length == length)
                {
                    return this.Size;
                }
                else
                {
                    //ca
                    int total = 0;
                    if (_outputUserCharAdvances != null)
                    {
                        for (int i = 0; i < length; ++i)
                        {
                            total += _outputUserCharAdvances[i];
                        }
                    }
                    return new Size(total, MeasureLineHeightInt32());
                }
            }

            return MeasureString(new TextBufferSpan(_mybuffer, 0, length));
        }

        public override CopyRun Copy(int startIndex, int length) => (startIndex > -1 && length > 0) ? MakeCopy(startIndex, length) : null;

        const int SAME_FONT_SAME_TEXT_COLOR = 0;
        const int SAME_FONT_DIFF_TEXT_COLOR = 1;
        const int DIFF_FONT_SAME_TEXT_COLOR = 2;
        const int DIFF_FONT_DIFF_TEXT_COLOR = 3;

        static int EvaluateFontAndTextColor(DrawBoard d, RunStyle runstyle)
        {
            RequestFont font = runstyle.ReqFont;
            Color color = runstyle.FontColor;
            RequestFont currentTextFont = d.CurrentFont;
            Color currentTextColor = d.CurrentTextColor;
            if (font != null && font != currentTextFont)
            {
                if (currentTextColor != color)
                {
                    return DIFF_FONT_DIFF_TEXT_COLOR;
                }
                else
                {
                    return DIFF_FONT_SAME_TEXT_COLOR;
                }
            }
            else
            {
                if (currentTextColor != color)
                {
                    return SAME_FONT_DIFF_TEXT_COLOR;
                }
                else
                {
                    return SAME_FONT_SAME_TEXT_COLOR;
                }
            }
        }

        internal static RenderElement s_currentRenderE;

        void DrawText(DrawBoard d)
        {

            //d.DrawText(_mybuffer,
            //     new Rectangle(0, 0, bWidth, bHeight),
            //     style.ContentHAlign);

            if (_renderVxFormattedString == null)
            {
                _renderVxFormattedString = d.CreateFormattedString(_mybuffer, 0, _mybuffer.Length, DelayFormattedString);
            }

            switch (_renderVxFormattedString.State)
            {
                case RenderVxFormattedString.VxState.Ready:
                    d.DrawRenderVx(_renderVxFormattedString, 0, 0);
                    break;
                case RenderVxFormattedString.VxState.NoStrip:
                    {
                        //put this to the update queue system
                        //(TODO: add extension method for this)
                        GlobalRootGraphic.CurrentRootGfx.EnqueueRenderRequest(
                            new RenderBoxes.RenderElementRequest(
                                s_currentRenderE,
                                RenderBoxes.RequestCommand.ProcessFormattedString,
                                _renderVxFormattedString));
                    }
                    break;
            }
        }
        public override void Draw(DrawBoard d, UpdateArea updateArea)
        {
            int bWidth = this.Width;
            int bHeight = this.Height;

#if DEBUG
            //canvas.dbug_DrawCrossRect(Color.Red, new Rectangle(0, 0, bWidth, bHeight));
            //canvas.DrawRectangle(Color.Red, 0, 0, bWidth, bHeight);
#endif

            RunStyle style = this.RunStyle;//must 

            //set style to the canvas  
            switch (EvaluateFontAndTextColor(d, style))
            {
                case DIFF_FONT_DIFF_TEXT_COLOR:
                    {
                        RequestFont prevFont = d.CurrentFont;
                        Color prevColor = d.CurrentTextColor;
                        d.CurrentFont = style.ReqFont;
                        d.CurrentTextColor = style.FontColor;

                        DrawText(d);

                        d.CurrentFont = prevFont;
                        d.CurrentTextColor = prevColor;
                    }
                    break;
                case DIFF_FONT_SAME_TEXT_COLOR:
                    {
                        RequestFont prevFont = d.CurrentFont;
                        d.CurrentFont = style.ReqFont;

                        DrawText(d);

                        d.CurrentFont = prevFont;
                    }
                    break;

                case SAME_FONT_DIFF_TEXT_COLOR:
                    {
                        Color prevColor = d.CurrentTextColor;
                        d.CurrentTextColor = style.FontColor;
                        DrawText(d);
                        d.CurrentTextColor = prevColor;
                    }
                    break;
                default:
                    {
                        DrawText(d);
                    }
                    break;
            }

        }

        /// <summary>
        /// find charactor from pixel offset (pixel offset starts at the begining of this run)
        /// </summary>
        /// <param name="pixelOffset"></param>
        /// <returns></returns>
        public override CharLocation GetCharacterFromPixelOffset(int pixelOffset)
        {
            if (pixelOffset < Width)
            {
                int j = _outputUserCharAdvances.Length;
                int accWidth = 0; //accummulate width
                for (int i = 0; i < j; i++)
                {
                    int charW = _outputUserCharAdvances[i];
                    if (accWidth + charW > pixelOffset)
                    {
                        //stop
                        //then decide that if width > char/w => please consider stop at next char
                        if (pixelOffset - accWidth > (charW / 2))
                        {
                            //this run is no select 
                            return new CharLocation(accWidth + charW, i + 1);
                        }
                        else
                        {
                            return new CharLocation(accWidth, i);
                        }
                    }
                    else
                    {
                        accWidth += charW;
                    }
                }

                //end of this run
                return new CharLocation(accWidth, j);
            }
            else
            {
                //pixelOffset>=width
                //not in this run, may be next run
                return new CharLocation(0, 1);
            }
        }
        //-------------------------------------------
        //
        internal override bool IsInsertable => true;
        //
        public override CopyRun LeftCopy(int index) => (index > 0) ? MakeCopy(0, index) : null;

        internal override void InsertAfter(int index, char c)
        {
            int oldLexLength = _mybuffer.Length;
            char[] newBuff = new char[oldLexLength + 1];
            if (index > -1 && index < _mybuffer.Length - 1)
            {
                Array.Copy(_mybuffer, newBuff, index + 1);
                newBuff[index + 1] = c;
                Array.Copy(_mybuffer, index + 1, newBuff, index + 2, oldLexLength - index - 1);
            }
            else if (index == -1)
            {
                newBuff[0] = c;
                Array.Copy(_mybuffer, 0, newBuff, 1, _mybuffer.Length);
            }
            else if (index == oldLexLength - 1)
            {
                Array.Copy(_mybuffer, newBuff, oldLexLength);
                newBuff[oldLexLength] = c;
            }
            else
            {
                throw new NotSupportedException();
            }

            SetNewContent(newBuff);
            InvalidateOwnerLineCharCount();
            UpdateRunWidth();
        }
        internal override CopyRun Remove(int startIndex, int length, bool withFreeRun)
        {
            CopyRun freeRun = null;
            if (startIndex > -1 && length > 0)
            {
                int oldLexLength = _mybuffer.Length;
                char[] newBuff = new char[oldLexLength - length];
                if (withFreeRun)
                {
                    freeRun = MakeCopy(startIndex, length);
                }
                if (startIndex > 0)
                {
                    Array.Copy(_mybuffer, 0, newBuff, 0, startIndex);
                }

                Array.Copy(_mybuffer, startIndex + length, newBuff, startIndex, oldLexLength - startIndex - length);
                SetNewContent(newBuff);
                InvalidateOwnerLineCharCount();
                UpdateRunWidth();
            }
            return withFreeRun ? freeRun : null;
        }

#if DEBUG
        public override string ToString()
        {
            return new string(_mybuffer);
        }
#endif

    }
}
