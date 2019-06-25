//MIT, 2019, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
namespace LayoutFarm.TextEditing
{
    public class SimpleTextSelectableLayer
    {
        List<LightLineBox> _lineBoxes = new List<LightLineBox>();
        public void SetText(string text)
        {
            _lineBoxes.Clear();
            using (System.IO.StringReader reader = new System.IO.StringReader(text))
            {
                string line = reader.ReadLine();
                while (line != null)
                {
                    LightLineBox lineBox = new LightLineBox(this);
                    _lineBoxes.Add(lineBox);
                }
            }

        }
        public void Clear()
        {

        }
    }

    public enum LightLineBoxEndWith : byte
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

    public struct WordSegment
    {
        int startAt;
        int len;
        int style;
    }

    public class LightLineBox
    {
        SimpleTextSelectableLayer _owerLayer;
        LightLineBoxEndWith _endWith;
        char[] _lineBuffer; //total line buffer .
        WordSegment[] _wordSegments;
        int _lineNumber;
        public LightLineBox(SimpleTextSelectableLayer owerLayer)
        {
            _owerLayer = owerLayer;
        }
        public void SetText(string text)
        {
            //set entire textline
        }
        public void Draw(DrawBoard drawboard)
        {

        }
    }

}
