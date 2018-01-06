//Apache2, 2014-2018, WinterDev

using System;
using System.Text;
using PixelFarm.Drawing;

namespace LayoutFarm.Text
{



    class EditableTextRun : EditableRun
    {

        TextSpanStyle spanStyle;

        int[] outputUserCharAdvances = null;//TODO: review here-> change this to caret stop position
        bool _content_unparsed;

        ILineSegmentList _lineSegs;

        public EditableTextRun(RootGraphic gfx, char[] copyBuffer, TextSpanStyle style)
            : base(gfx)
        {
            //we need font info (in style) for evaluating the size fo this span
            //without font info we can't measure the size of this span
            this.spanStyle = style;
            SetNewContent(copyBuffer);
            UpdateRunWidth();
        }
        public EditableTextRun(RootGraphic gfx, char c, TextSpanStyle style)
            : base(gfx)
        {
            this.spanStyle = style;
            SetNewContent(new char[] { c });
            if (c == '\n')
            {
                //TODO: review line break span
                this.IsLineBreak = true;
            }
            //check line break?
            UpdateRunWidth();
        }
        public EditableTextRun(RootGraphic gfx, string str, TextSpanStyle style)
            : base(gfx)
        {
            this.spanStyle = style;
            if (str != null && str.Length > 0)
            {
                SetNewContent(str.ToCharArray());
                //special treament
                if (mybuffer.Length == 1 && mybuffer[0] == '\n')
                {
                    this.IsLineBreak = true;
                }
                UpdateRunWidth();
            }
            else
            {
                throw new Exception("string must be null or zero length");
            }
        }
        //each editable run has it own (dynamic) char buffer 


#if DEBUG
        char[] _mybuffer;
        char[] mybuffer
        {
            //TODO: review how to manage mem here
            get { return _mybuffer; }
        }
#else
        char[] mybuffer;
#endif
        void SetNewContent(char[] newbuffer)
        {
#if DEBUG
            _mybuffer = newbuffer;
#else
            mybuffer= newbuffer;
#endif
            _content_unparsed = true;
        }
        public override void ResetRootGraphics(RootGraphic rootgfx)
        {
            //change root graphics after create
            DirectSetRootGraphics(this, rootgfx);
        }
        public override EditableRun Clone()
        {
            return new EditableTextRun(this.Root, this.GetText(), this.SpanStyle);
        }
        public override EditableRun Copy(int startIndex)
        {
            int length = mybuffer.Length - startIndex;
            if (startIndex > -1 && length > 0)
            {
                return MakeTextRun(startIndex, length);
            }
            else
            {
                return null;
            }
        }
        EditableRun MakeTextRun(int sourceIndex, int length)
        {
            if (length > 0)
            {
                EditableRun newTextRun = null;
                char[] newContent = new char[length];
                Array.Copy(this.mybuffer, sourceIndex, newContent, 0, length);
                newTextRun = new EditableTextRun(this.Root, newContent, this.SpanStyle);
                newTextRun.IsLineBreak = this.IsLineBreak;
                newTextRun.UpdateRunWidth();
                return newTextRun;
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
            return new string(mybuffer);
        }
        internal override void UpdateRunWidth()
        {
            ITextService txServices = Root.TextServices;
            Size size;
            if (IsLineBreak)
            {
                //TODO: review here
                //we should not store this as a text run
                //if this is a linebreak it should be encoded at the end of this visual line
                size = new Size(0, (int)Math.Round(txServices.MeasureBlankLineHeight(GetFont())));
                outputUserCharAdvances = null;
            }
            else
            {
                //TODO: review here again 
                //1. after GSUB process, output glyph may be more or less 
                //than original input char buffer(mybuffer)

                if (txServices.SupportsWordBreak)
                {
                    var textBufferSpan = new TextBufferSpan(mybuffer);
                    int len = mybuffer.Length;
                    if (_content_unparsed)
                    {
                        //parse the content first 
                        _lineSegs = txServices.BreakToLineSegments(ref textBufferSpan);
                    }
                    //
                    _content_unparsed = false;
                    //output glyph position
                    outputUserCharAdvances = new int[len];

                    int outputTotalW, outputLineHeight;
                    txServices.CalculateUserCharGlyphAdvancePos(ref textBufferSpan, _lineSegs, GetFont(),
                        outputUserCharAdvances, out outputTotalW, out outputLineHeight);
                    size = new Size(outputTotalW, outputLineHeight);

                }
                else
                {

                    _content_unparsed = false;
                    int len = mybuffer.Length;
                    outputUserCharAdvances = new int[len];
                    int outputTotalW, outputLineHeight;
                    var textBufferSpan = new TextBufferSpan(mybuffer);
                    txServices.CalculateUserCharGlyphAdvancePos(ref textBufferSpan, GetFont(),
                        outputUserCharAdvances, out outputTotalW, out outputLineHeight);
                    size = new Size(outputTotalW, outputLineHeight);
                }

            }
            //---------
            this.SetSize(size.Width, size.Height);
            MarkHasValidCalculateSize();
        }
        public override char GetChar(int index)
        {
            return mybuffer[index];
        }


        public override void CopyContentToStringBuilder(StringBuilder stBuilder)
        {
            if (IsLineBreak)
            {
                stBuilder.Append("\r\n");
            }
            else
            {
                stBuilder.Append(mybuffer);
            }
        }
        public override int CharacterCount
        {
            get
            {
                return mybuffer.Length;
            }
        }
        public override TextSpanStyle SpanStyle
        {
            get
            {
                return this.spanStyle;
            }
        }
        public override void SetStyle(TextSpanStyle spanStyle)
        {
            //TODO: review this again
            //update style may affect the 'visual' layout of the span
            //the span may expand large or shrink down
            //so we invalidate graphics area pre and post

            this.InvalidateGraphics();
            this.spanStyle = spanStyle;
            this.InvalidateGraphics();
            UpdateRunWidth();
        }
        Size CalculateDrawingStringSize(int length)
        {
            if (!_content_unparsed)
            {
                //the content is parsed ***

                if (this.mybuffer.Length == length)
                {
                    return this.Size;
                }
                else
                {
                    //ca
                    int total = 0;
                    if (outputUserCharAdvances != null)
                    {
                        for (int i = 0; i < length; ++i)
                        {
                            total += this.outputUserCharAdvances[i];
                        }
                    }
                    return new Size(total, (int)Math.Round(Root.TextServices.MeasureBlankLineHeight(GetFont())));
                }
            }
            PixelFarm.Drawing.RequestFont fontInfo = GetFont();
            var textBufferSpan = new TextBufferSpan(this.mybuffer, 0, length);
            return this.Root.TextServices.MeasureString(ref textBufferSpan, fontInfo);
        }
        protected PixelFarm.Drawing.RequestFont GetFont()
        {
            if (!HasStyle)
            {
                return this.Root.DefaultTextEditFontInfo;
            }
            else
            {
                TextSpanStyle spanStyle = this.SpanStyle;
                if (spanStyle.FontInfo != null)
                {
                    return spanStyle.FontInfo;
                }
                else
                {
                    return this.Root.DefaultTextEditFontInfo;
                }
            }
        }

        public override EditableRun Copy(int startIndex, int length)
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
        static int EvaluateFontAndTextColor(DrawBoard canvas, TextSpanStyle spanStyle)
        {
            var font = spanStyle.FontInfo;
            var color = spanStyle.FontColor;
            var currentTextFont = canvas.CurrentFont;
            var currentTextColor = canvas.CurrentTextColor;
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
        protected bool HasStyle
        {
            get
            {
                return !this.SpanStyle.IsEmpty();
            }
        }
        public override void CustomDrawToThisCanvas(DrawBoard canvas, Rectangle updateArea)
        {
            int bWidth = this.Width;
            int bHeight = this.Height;
            if (!this.HasStyle)
            {
                canvas.DrawText(this.mybuffer, new Rectangle(0, 0, bWidth, bHeight), 0);
            }
            else
            {
                TextSpanStyle style = this.SpanStyle;
                switch (EvaluateFontAndTextColor(canvas, style))
                {
                    case DIFF_FONT_SAME_TEXT_COLOR:
                        {
                            RequestFont prevFont = canvas.CurrentFont;
                            canvas.DrawText(this.mybuffer,
                                new Rectangle(0, 0, bWidth, bHeight),
                                style.ContentHAlign);
                        }
                        break;
                    case DIFF_FONT_DIFF_TEXT_COLOR:
                        {
                            RequestFont prevFont = canvas.CurrentFont;
                            Color prevColor = canvas.CurrentTextColor;
                            canvas.CurrentFont = style.FontInfo;
                            canvas.CurrentTextColor = style.FontColor;
                            canvas.DrawText(this.mybuffer,
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
                            canvas.DrawText(this.mybuffer,
                                new Rectangle(0, 0, bWidth, bHeight),
                                style.ContentHAlign);
                            canvas.CurrentTextColor = prevColor;
                        }
                        break;
                    default:
                        {
                            canvas.DrawText(this.mybuffer,
                               new Rectangle(0, 0, bWidth, bHeight),
                               style.ContentHAlign);
                        }
                        break;
                }
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
                int j = outputUserCharAdvances.Length;
                int accWidth = 0; //accummulate width
                for (int i = 0; i < j; i++)
                {
                    int charW = outputUserCharAdvances[i];
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
        internal override bool IsInsertable
        {
            get
            {
                return true;
            }
        }
        public override EditableRun LeftCopy(int index)
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
            int oldLexLength = mybuffer.Length;
            char[] newBuff = new char[oldLexLength + 1];
            if (index > -1 && index < mybuffer.Length - 1)
            {
                Array.Copy(mybuffer, newBuff, index + 1);
                newBuff[index + 1] = c;
                Array.Copy(mybuffer, index + 1, newBuff, index + 2, oldLexLength - index - 1);
            }
            else if (index == -1)
            {
                newBuff[0] = c;
                Array.Copy(mybuffer, 0, newBuff, 1, mybuffer.Length);
            }
            else if (index == oldLexLength - 1)
            {
                Array.Copy(mybuffer, newBuff, oldLexLength);
                newBuff[oldLexLength] = c;
            }
            else
            {
                throw new NotSupportedException();
            }
            SetNewContent(newBuff);
            UpdateRunWidth();
        }
        internal override EditableRun Remove(int startIndex, int length, bool withFreeRun)
        {
            EditableRun freeRun = null;
            if (startIndex > -1 && length > 0)
            {
                int oldLexLength = mybuffer.Length;
                char[] newBuff = new char[oldLexLength - length];
                if (withFreeRun)
                {
                    freeRun = MakeTextRun(startIndex, length);
                }
                if (startIndex > 0)
                {
                    Array.Copy(mybuffer, 0, newBuff, 0, startIndex);
                }

                Array.Copy(mybuffer, startIndex + length, newBuff, startIndex, oldLexLength - startIndex - length);
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
