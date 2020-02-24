//MIT, 2019-present, WinterDev
using System;
using System.Text;

namespace LayoutFarm.TextEditing
{
    public enum PlainTextLineEnd : byte
    {
        None,
        /// <summary>
        /// \r\n
        /// </summary>
        CRLF,
        /// <summary>
        /// \n
        /// </summary>
        LF,
    }


    /// <summary>
    /// immutable plain text line
    /// </summary>
    public class PlainTextLine
    {
        char[] _text;//***

        public PlainTextLine(string text)
        {
            _text = text.ToCharArray();
        }
        public PlainTextLine(char[] buffer)
        {
            _text = buffer;
        }
        public PlainTextLineEnd EndWith { get; set; }


        public string GetText() => new string(_text);

        public void CopyText(StringBuilder stbuilder)
        {
            stbuilder.Append(_text);
        }
#if DEBUG
        public override string ToString()
        {
            return GetText();
        }
#endif
        public PlainTextLine Clone()
        {
            return new PlainTextLine(_text);
        }
    }



}
