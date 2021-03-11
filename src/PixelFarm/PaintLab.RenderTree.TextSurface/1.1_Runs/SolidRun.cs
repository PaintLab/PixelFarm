//Apache2, 2014-present, WinterDev

using System;
using PixelFarm.Drawing;
using Typography.Text;

namespace LayoutFarm.TextFlow
{
    public class SolidRun : Run
    {
        //TODO: review here=> who should store/handle this handle? , owner TextBox or this run?
        Action<SolidRun, DrawBoard, UpdateArea> _externalCustomDraw;
        TextBufferSpan _mybuffer;

        public SolidRun(TextBufferSpan copyBuffer, RunStyle style)
            : base(style)
        {
            //check line break? 
            _mybuffer = copyBuffer;
            UpdateRunWidth();
        }

        public void SetCustomExternalDraw(Action<SolidRun, DrawBoard, UpdateArea> externalCustomDraw) => _externalCustomDraw = externalCustomDraw;
        public RenderElement ExternalRenderElement { get; set; }

        public string RawText { get; set; } //TODO: review here

        public override void WriteTo(Typography.Text.TextCopyBuffer output)
        {
            _mybuffer.WriteTo(output);
        }
        //public override void WriteTo(Typography.Text.TextCopyBuffer output, int start)
        //{
        //    //write data to buffer 
        //    output.Append(_mybuffer); 
        //}
        //public override void WriteTo(Typography.Text.TextCopyBuffer output, int start, int len)
        //{
        //    throw new NotSupportedException();
        //    //if (start == 0)
        //    //{
        //    //    int length = _mybuffer.Count - start;
        //    //    if (start > -1 && length > 0)
        //    //    {
        //    //        output.Append(_mybuffer, start, len);
        //    //    }
        //    //}
        //}

        public override int GetRunWidth(int charOffset) => CalculateDrawingStringSize(0, charOffset).Width;
        //public override int GetRunWidth(int startAtCharOffset, int count)
        //{
        //    return MeasureString(_mybuffer.CreateSubspan(startAtCharOffset, count)).Width;
        //}

        public override void UpdateRunWidth()
        {
            Size size = CalculateDrawingStringSize(0, _mybuffer.len);
            SetSize(size.Width, size.Height);
        }
        public override int GetChar(int index) => _mybuffer.GetChar(index);

        public override int CharacterCount => _mybuffer.len;

        Size CalculateDrawingStringSize(int start, int length) => MeasureString(_mybuffer.CreateSubspan(start, length));

        const int SAME_FONT_SAME_TEXT_COLOR = 0;
        const int SAME_FONT_DIFF_TEXT_COLOR = 1;
        const int DIFF_FONT_SAME_TEXT_COLOR = 2;
        const int DIFF_FONT_DIFF_TEXT_COLOR = 3;

        static int EvaluateFontAndTextColor(DrawBoard d, RunStyle spanStyle)
        {
            RequestFont font = spanStyle.ReqFont;
            Color color = spanStyle.FontColor;
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
        // 
        public override void Draw(DrawBoard d, UpdateArea updateArea)
        {
            if (_externalCustomDraw != null)
            {
                _externalCustomDraw(this, d, updateArea);
                return;
            }
            else if (ExternalRenderElement != null)
            {
                RenderElement.Render(ExternalRenderElement, d, updateArea);
                return;
            }

            int bWidth = this.Width;
            int bHeight = this.Height;

            //1. bg, yellow for test**
            d.FillRectangle(Color.Yellow, 0, 0, bWidth, bHeight);

            //if (!this.HasStyle)
            //{
            //    canvas.DrawText(_mybuffer, new Rectangle(0, 0, bWidth, bHeight), 0);
            //}
            //else
            //{
            //TODO: review here, we don't need to do this


            //TODO: /...
            RunStyle style = this.RunStyle;
            switch (EvaluateFontAndTextColor(d, style))
            {
                case DIFF_FONT_SAME_TEXT_COLOR:
                    {
                        //var prevFont = d.CurrentFont;
                        //d.CurrentFont = style.ReqFont;
                        //d.DrawText(_mybuffer,
                        //   new Rectangle(0, 0, bWidth, bHeight),
                        //   style.ContentHAlign);
                        //d.CurrentFont = prevFont;
                    }
                    break;
                case DIFF_FONT_DIFF_TEXT_COLOR:
                    {
                        //var prevFont = d.CurrentFont;
                        //var prevColor = d.CurrentTextColor;
                        //d.CurrentFont = style.ReqFont;
                        //d.CurrentTextColor = style.FontColor;
                        //d.DrawText(_mybuffer,
                        //   new Rectangle(0, 0, bWidth, bHeight),
                        //   style.ContentHAlign);
                        //d.CurrentFont = prevFont;
                        //d.CurrentTextColor = prevColor;
                    }
                    break;
                case SAME_FONT_DIFF_TEXT_COLOR:
                    {
                        //var prevColor = d.CurrentTextColor;
                        //d.DrawText(_mybuffer,
                        //    new Rectangle(0, 0, bWidth, bHeight),
                        //    style.ContentHAlign);
                        //d.CurrentTextColor = prevColor;
                    }
                    break;
                default:
                    {
                        //d.DrawText(_mybuffer,
                        //   new Rectangle(0, 0, bWidth, bHeight),
                        //   style.ContentHAlign);
                    }
                    break;
            }
            //}
        }


        public override CharLocation GetCharacterFromPixelOffset(int pixelOffset)
        {
            if (pixelOffset < Width)
            {
                return new CharLocation(0, 0);
            }
            else
            {
                //exceed than the bound of this run
                return new CharLocation(0, 1);
            }
        }

        //public override TextSegment Copy(int startIndex)
        //{
        //    if (startIndex == 0)
        //    {

        //        int length = _mybuffer.Count - startIndex;
        //        if (startIndex > -1 && length > 0)
        //        {
        //            return _mybuffer.MakeSubSpan(startIndex, length);
        //        }
        //    }
        //    return new TextSegment();//empty
        //}
        //public override TextSegment Copy(int startIndex, int length)
        //{
        //    return _mybuffer.MakeSubSpan(startIndex, length);
        //}
        //public override TextSegment LeftCopy(int index)
        //{
        //    if (index == 0)
        //    {
        //        return new TextSegment();//empty
        //    }

        //    if (index > -1)
        //    {
        //        return _mybuffer.MakeLeftSubSpan(index);
        //    }
        //    else
        //    {
        //        return new TextSegment();//empty
        //    }
        //}
        //public override CopyRun Copy(int startIndex)
        //{
        //    if (startIndex == 0)
        //    {
        //        int length = _mybuffer.Length - startIndex;
        //        if (startIndex > -1 && length > 0)
        //        {
        //            return MakeTextRun(startIndex, length);
        //        }
        //    }
        //    return null;
        //}
        //CopyRun MakeTextRun(int sourceIndex, int length)
        //{
        //    if (length > 0)
        //    {
        //        sourceIndex = 0;
        //        length = _mybuffer.Length;

        //        char[] newContent = new char[length];
        //        Array.Copy(_mybuffer, sourceIndex, newContent, 0, length);
        //        return new CopyRun(newContent) { RunKind = RunKind.Solid };
        //    }
        //    else
        //    {
        //        throw new Exception("string must be null or zero length");
        //    }
        //}

        //-------------------------------------------
        //
        internal override bool IsInsertable => false;
        //
        //    public override CopyRun LeftCopy(int index)
        //    {
        //        if (index == 0)
        //        {
        //            return null;
        //        }

        //        if (index > -1)
        //        {
        //            return MakeTextRun(0, _mybuffer.Length);
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        ////    internal override void InsertAfter(int index, char c)
        //    {
        //        //TODO: review here
        //        //solid text run should not be editable
        //        int oldLexLength = _mybuffer.Length;
        //        char[] newBuff = new char[oldLexLength + 1];
        //        if (index > -1 && index < _mybuffer.Length - 1)
        //        {
        //            Array.Copy(_mybuffer, newBuff, index + 1);
        //            newBuff[index + 1] = c;
        //            Array.Copy(_mybuffer, index + 1, newBuff, index + 2, oldLexLength - index - 1);
        //        }
        //        else if (index == -1)
        //        {
        //            newBuff[0] = c;
        //            Array.Copy(_mybuffer, 0, newBuff, 1, _mybuffer.Length);
        //        }
        //        else if (index == oldLexLength - 1)
        //        {
        //            Array.Copy(_mybuffer, newBuff, oldLexLength);
        //            newBuff[oldLexLength] = c;
        //        }
        //        else
        //        {
        //            throw new NotSupportedException();
        //        }
        //        _mybuffer = newBuff;
        //        InvalidateOwnerLineCharCount();
        //        UpdateRunWidth();
        //    }

        //    internal override CopyRun Remove(int startIndex, int length, bool withFreeRun)
        //    {
        //        if (startIndex == _mybuffer.Length)
        //        {
        //            //at the end
        //            return null;
        //        }

        //        //
        //        startIndex = 0; //***
        //        length = _mybuffer.Length;
        //        CopyRun freeRun = null;
        //        if (startIndex > -1 && length > 0)
        //        {
        //            int oldLexLength = _mybuffer.Length;
        //            char[] newBuff = new char[oldLexLength - length];
        //            if (withFreeRun)
        //            {
        //                freeRun = MakeTextRun(startIndex, length);
        //            }
        //            if (startIndex > 0)
        //            {
        //                Array.Copy(_mybuffer, 0, newBuff, 0, startIndex);
        //            }

        //            Array.Copy(_mybuffer, startIndex + length, newBuff, startIndex, oldLexLength - startIndex - length);
        //            _mybuffer = newBuff;
        //            InvalidateOwnerLineCharCount();
        //            UpdateRunWidth();
        //        }

        //        return withFreeRun ? freeRun : null;
        //    }
    }
}
