//Apache2, 2014-present, WinterDev

using System;
using System.Text;
using PixelFarm.Drawing;

namespace LayoutFarm.TextEditing
{

    public class EditableTextRun : EditableRun
    {
#if DEBUG
        char[] _dbugmybuffer;
        char[] _mybuffer => _dbugmybuffer;
#else
        char[] _mybuffer;
#endif

        int[] _outputUserCharAdvances = null;//TODO: review here-> change this to caret stop position
        bool _content_unparsed;
        ILineSegmentList _lineSegs;

        public EditableTextRun(RunStyle runstyle, char[] copyBuffer)
            : base(runstyle)
        {
            //we need font info (in style) for evaluating the size fo this span
            //without font info we can't measure the size of this span 
            SetNewContent(copyBuffer);
            UpdateRunWidth();
#if DEBUG
            //this.dbugBreak = true;
#endif
        }
        public EditableTextRun(RunStyle runstyle, char c)
            : base(runstyle)
        {
            if (c == '\n' || c == '\r')
            {
                throw new NotSupportedException();
                ////TODO: review line break span
                //this.IsLineBreak = true;
            }

            SetNewContent(new char[] { c });
            //check line break?
            UpdateRunWidth();
        }
        public EditableTextRun(RunStyle runstyle, string str)
            : base(runstyle)
        {

            if (str != null && str.Length > 0)
            {
                SetNewContent(str.ToCharArray());
                //special treament
                if (_mybuffer.Length == 1 && _mybuffer[0] == '\n')
                {
                    throw new NotSupportedException();
                    //this.IsLineBreak = true;
                }
                UpdateRunWidth();
            }
            else
            {
                //TODO: review here
                throw new Exception("string must be null or zero length");
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
        }

        public void UnsafeSetNewContent(char[] copyContent)
        {
            SetNewContent(copyContent);
        }

        public static void UnsafeGetRawCharBuffer(EditableTextRun textRun, out char[] rawCharBuffer)
        {
            rawCharBuffer = textRun._mybuffer;
        }
        
        public override CopyRun CreateCopy()
        {
            return new CopyRun() { RawContent = this.GetText().ToCharArray() };
        }
        public override CopyRun Copy(int startIndex)
        {
            int length = _mybuffer.Length - startIndex;
            if (startIndex > -1 && length > 0)
            {
                return MakeTextRun(startIndex, length);
            }
            else
            {
                return null;
            }
        }
        CopyRun MakeTextRun(int sourceIndex, int length)
        {
            if (length > 0)
            {
                //CopyRun newTextRun = null;
                char[] newContent = new char[length];
                Array.Copy(_mybuffer, sourceIndex, newContent, 0, length);

                CopyRun copyRun = new CopyRun();
                copyRun.RawContent = newContent;
                return copyRun;
                //newTextRun = new EditableTextRun(this.Root, newContent, this.SpanStyle);
                //newTextRun.IsLineBreak = this.IsLineBreak;
                //newTextRun.UpdateRunWidth();
                //return newTextRun;
            }
            else
            {
                throw new Exception("string must be null or zero length");
            }
        }

        public override int GetRunWidth(int charOffset)
        {
            return CalculateDrawingStringSize(charOffset).Width;
        }
        public override string GetText()
        {
            return new string(_mybuffer);
        }
        public override void UpdateRunWidth()
        {

            //if (IsLineBreak)
            //{
            //    //TODO: review here
            //    //we should not store this as a text run
            //    //if this is a linebreak it should be encoded at the end of this visual line
            //    size = new Size(0, (int)Math.Round(txServices.MeasureBlankLineHeight(GetFont())));
            //    _outputUserCharAdvances = null;
            //}
            //else
            //{
            //TODO: review here again 
            //1. after GSUB process, output glyph may be more or less 
            //than original input char buffer(mybuffer)

            var textBufferSpan = new TextBufferSpan(_mybuffer);
            _outputUserCharAdvances = new int[_mybuffer.Length];

            if (SupportWordBreak)
            {
                if (_content_unparsed)
                {
                    //parse the content first 
                    _lineSegs = BreakToLineSegs(ref textBufferSpan);
                }
                _content_unparsed = false;
                MeasureString2(ref textBufferSpan, _lineSegs, _outputUserCharAdvances,
                               out int outputTotalW, out int outputLineHeight);
                SetSize2(outputTotalW, outputLineHeight);
                InvalidateGraphics();
            }
            else
            {
                MeasureString2(ref textBufferSpan, null, _outputUserCharAdvances,
                               out int outputTotalW, out int outputLineHeight);
                SetSize2(outputTotalW, outputLineHeight);
                InvalidateGraphics();
            }
        }
        protected void AdjustClientBounds(ref Rectangle bounds)
        {
            if (this.OwnerEditableLine != null)
            {
                bounds.Offset(0, this.OwnerEditableLine.Top);
            }
        }
        public override char GetChar(int index)
        {
            return _mybuffer[index];
        }


        public override void CopyContentToStringBuilder(StringBuilder stBuilder)
        {
            stBuilder.Append(_mybuffer);
        }
        //
        public override int CharacterCount => _mybuffer.Length;
        //
        //
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
            var textBufferSpan = new TextBufferSpan(_mybuffer, 0, length);
            return MeasureString(ref textBufferSpan);
        }

        public override CopyRun Copy(int startIndex, int length)
        {
            if (startIndex > -1 && length > 0)
            {
                return MakeTextRun(startIndex, length);
            }
            else
            {
                return null;
            }
        }

        const int SAME_FONT_SAME_TEXT_COLOR = 0;
        const int SAME_FONT_DIFF_TEXT_COLOR = 1;
        const int DIFF_FONT_SAME_TEXT_COLOR = 2;
        const int DIFF_FONT_DIFF_TEXT_COLOR = 3;

        static int EvaluateFontAndTextColor(DrawBoard canvas, RunStyle runstyle)
        {
            RequestFont font = runstyle.ReqFont;
            Color color = runstyle.FontColor;
            RequestFont currentTextFont = canvas.CurrentFont;
            Color currentTextColor = canvas.CurrentTextColor;
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
        //

        //
        public override void Draw(DrawBoard canvas, Rectangle updateArea)
        {
            int bWidth = this.Width;
            int bHeight = this.Height;

#if DEBUG
            //canvas.dbug_DrawCrossRect(Color.Red, new Rectangle(0, 0, bWidth, bHeight));
            //canvas.DrawRectangle(Color.Red, 0, 0, bWidth, bHeight);
#endif

            RunStyle style = this.RunStyle;//must 
            switch (EvaluateFontAndTextColor(canvas, style))
            {
                case DIFF_FONT_SAME_TEXT_COLOR:
                    {

                        canvas.DrawText(_mybuffer,
                            new Rectangle(0, 0, bWidth, bHeight),
                            style.ContentHAlign);
                    }
                    break;
                case DIFF_FONT_DIFF_TEXT_COLOR:
                    {
                        RequestFont prevFont = canvas.CurrentFont;
                        Color prevColor = canvas.CurrentTextColor;
                        canvas.CurrentFont = style.ReqFont;
                        canvas.CurrentTextColor = style.FontColor;
                        canvas.DrawText(_mybuffer,
                             new Rectangle(0, 0, bWidth, bHeight),
                             style.ContentHAlign);
                        canvas.CurrentFont = prevFont;
                        canvas.CurrentTextColor = prevColor;
                    }
                    break;
                case SAME_FONT_DIFF_TEXT_COLOR:
                    {
                        Color prevColor = canvas.CurrentTextColor;
                        canvas.CurrentTextColor = style.FontColor;
                        canvas.DrawText(_mybuffer,
                            new Rectangle(0, 0, bWidth, bHeight),
                            style.ContentHAlign);
                        canvas.CurrentTextColor = prevColor;
                    }
                    break;
                default:
                    {
                        canvas.DrawText(_mybuffer,
                           new Rectangle(0, 0, bWidth, bHeight),
                           style.ContentHAlign);
                    }
                    break;
            }

        }

        /// <summary>
        /// find charactor from pixel offset (pixel offset starts at the begining of this run)
        /// </summary>
        /// <param name="pixelOffset"></param>
        /// <returns></returns>
        public override EditableRunCharLocation GetCharacterFromPixelOffset(int pixelOffset)
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
                            return new EditableRunCharLocation(accWidth + charW, i + 1);
                        }
                        else
                        {
                            return new EditableRunCharLocation(accWidth, i);
                        }
                    }
                    else
                    {
                        accWidth += charW;
                    }
                }

                //end of this run
                return new EditableRunCharLocation(accWidth, j);
            }
            else
            {
                //pixelOffset>=width
                //not in this run, may be next run
                return new EditableRunCharLocation(0, 1);
            }
        }
        //-------------------------------------------
        //
        internal override bool IsInsertable => true;
        //
        public override CopyRun LeftCopy(int index)
        {
            if (index > 0)
            {
                return MakeTextRun(0, index);
            }
            else
            {
                return null;
            }
        }
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
                    freeRun = MakeTextRun(startIndex, length);
                }
                if (startIndex > 0)
                {
                    Array.Copy(_mybuffer, 0, newBuff, 0, startIndex);
                }

                Array.Copy(_mybuffer, startIndex + length, newBuff, startIndex, oldLexLength - startIndex - length);
                SetNewContent(newBuff);
                UpdateRunWidth();
            }


            if (withFreeRun)
            {
                return freeRun;
            }
            else
            {
                return null;
            }
        }
    }
}
