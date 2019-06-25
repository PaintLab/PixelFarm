//MIT, 2019, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
namespace LayoutFarm.TextEditing
{
    public class SimpleTextSelectableLayer
    {
        RenderBoxBase _owner;
        List<LightLineBox> _lineBoxes = new List<LightLineBox>();
        RootGraphic _rootgfx;
        bool _invalidLayout;
        int _innerContentHeight;

        public SimpleTextSelectableLayer(RootGraphic rootgfx)
        {
            _rootgfx = rootgfx;
            _invalidLayout = true;
        }

        public int InnerContentHeight => _innerContentHeight;
        public int LineCount => _lineBoxes.Count;
        public LightLineBox GetLine(int index)
        {
            return _lineBoxes[index];
        }
        public void SetOwner(RenderBoxBase owner)
        {
            _owner = owner;
        }
        public void SetText(string text)
        {
            _lineBoxes.Clear();
            using (System.IO.StringReader reader = new System.IO.StringReader(text))
            {
                string line = reader.ReadLine();
                while (line != null)
                {
                    LightLineBox lineBox = new LightLineBox(_rootgfx, 10, 10);//                     
                    lineBox.SetText(line);
                    _lineBoxes.Add(lineBox);
                    line = reader.ReadLine(); //***
                }
                //arrange all
                _invalidLayout = true;
            }

            ArrangeLines();
        }
        public void ArrangeLines()
        {
            if (!_invalidLayout) return;
            //---------------------------
            //arrange all
            int j = _lineBoxes.Count;
            int left = 0;
            int top = 0;
            int defaultLineHeight = 19;
            int ownerW = _owner.Width;

            for (int i = 0; i < j; ++i)
            {
                LightLineBox linebox = _lineBoxes[i];
                linebox.SetBounds(left, top, ownerW, defaultLineHeight);
                top += defaultLineHeight;
                //TODO: inter-line space?
            }

            _invalidLayout = false;
        }
        public void Draw(DrawBoard drawboard, Rectangle updateArea)
        {
            int j = _lineBoxes.Count;
            //
            for (int i = 0; i < j; ++i)
            {
                LightLineBox linebox = _lineBoxes[i];
                if (updateArea.IntersectsWith(linebox.RectBounds))
                {
                    linebox.Draw(drawboard);
                }
            }
        }
        public void Clear()
        {
            _lineBoxes.Clear();
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

    public class LightLineBox : RenderElement
    {
        SimpleTextSelectableLayer _owerLayer;

        char[] _lineBuffer; //total line buffer
        WordSegment[] _wordSegments; //set this later
        public LightLineBox(RootGraphic rootgfx, int w, int h)
            : base(rootgfx, w, h)
        {

        }
        public void SetOwnerLayer(SimpleTextSelectableLayer ownerLayer)
        {
            _owerLayer = ownerLayer;
        }
        public int LineNumber { get; internal set; }
        public LightLineBoxEndWith EndWith { get; set; }
        public void SetText(string text)
        {
            //set entire textline
            _lineBuffer = text.ToCharArray();
        }
        public void Draw(DrawBoard drawboard)
        {
            drawboard.DrawText(_lineBuffer, X, Y);
        }
        public override void CustomDrawToThisCanvas(DrawBoard d, Rectangle updateArea)
        {
            d.DrawText(_lineBuffer, X, Y);
        }
        public override void ResetRootGraphics(RootGraphic rootgfx)
        {

        }
    }

}
