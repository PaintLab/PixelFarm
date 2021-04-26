//MIT, 2014-present, WinterDev

using System;

using Typography.Text;
namespace LayoutFarm.TextFlow
{

    /// <summary>
    /// forward only text source 
    /// </summary>
    public class ForwardOnlyCharSource
    {
        readonly TextBuilder<int> _text;
        public ForwardOnlyCharSource()
        {
            _text = new TextBuilder<int>();
        }
        /// <summary>
        /// write content to TextCopyBuffer
        /// </summary>
        /// <param name="output"></param>
        /// <param name="offset"></param>
        /// <param name="len"></param>
        internal void WriteTo(TextCopyBuffer output, int offset, int len)
        {
            //convert utf32 to utf16 
            output.Append(TextBuilder<int>.UnsafeGetInternalArray(_text), offset, len);
        }
        internal void WriteTo(TextBuilder<int> output, int offset, int len)
        {
            //convert utf32 to utf16 
            //temp
            output.Append(_text, offset, len);

        }
        internal void Copy(int srcStart, int srcLen, char[] outputArr, int outputStart)
        {
            Array.Copy(TextBuilder<int>.UnsafeGetInternalArray(_text), srcStart, outputArr, outputStart, srcLen);
        }

        public TextBufferSpan NewSpan(char[] charBuffer, int start, int len)
        {
            int s = _text.Count;
            int end = start + len;
            for (int i = start; i < end; ++i)
            {
                char c0 = charBuffer[i];
                if (char.IsHighSurrogate(c0) && i + 1 < len)
                {
                    //and not the last one
                    _text.Append(char.ConvertToUtf32(c0, charBuffer[i + 1]));
                    i++;
                }
                else
                {
                    _text.Append(c0);
                }
            }
            return new TextBufferSpan(this.UnsafeInternalArray, s, _text.Count - s);
        }
        public TextBufferSpan NewSpan(int[] charBuffer, int start, int len)
        {
            int s = _text.Count;
            _text.Append(charBuffer, start, len);
            return new TextBufferSpan(this.UnsafeInternalArray, s, len);
        }

        public TextBufferSpan NewSegment(TextBufferSpan textspan)
        {
            if (textspan.IsEmpty)
            {
                //return NewSpan(textspan.GetRawUtf32Buffer(), textspan.start, textspan.len);
                return NewSpan(textspan.GetRawUtf16Buffer(), textspan.start, textspan.len);
            }
            else
            {
                return NewSpan(textspan.GetRawUtf16Buffer(), textspan.start, textspan.len);
            }
        }
        public TextBufferSpan NewSegment(TextCopyBufferUtf32 buffer, int start, int len)
        {
            return NewSpan(TextCopyBufferUtf32.UnsafeGetInternalArray(buffer), start, len);
        }
        public TextBufferSpan NewSegment(TextCopyBufferUtf16 buffer, int start, int len)
        {
            return NewSpan(TextCopyBufferUtf16.UnsafeGetInternalArray(buffer), start, len);
        }
        public TextBufferSpan NewSegment(TextCopyBufferUtf32 buffer)
        {
            return NewSpan(TextCopyBufferUtf32.UnsafeGetInternalArray(buffer), 0, buffer.Length);
        }

        public int LatestLen => _text.Count;
        internal int[] UnsafeInternalArray => TextBuilder<int>.UnsafeGetInternalArray(_text);



        //public void CopyAndAppend(int start, int len)
        //{
        //    //copy data from srcRange
        //    //and append to the last part of _arrList
        //    _arrList.CopyAndAppend(start, len);
        //}
        //public void Append(CharBufferSegment charSpan)
        //{
        //    //append data from another charspan
        //    _arrList.Append(charSpan.UnsafeInternalCharArr, charSpan.beginAt, charSpan.len);
        //}
        //public void Append(int c)
        //{
        //    _arrList.Append(c);
        //}

    }



}
