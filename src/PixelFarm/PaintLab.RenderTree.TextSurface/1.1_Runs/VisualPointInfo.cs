//Apache2, 2014-present, WinterDev

namespace LayoutFarm.TextFlow
{
    public abstract class VisualPointInfo
    {
        readonly int _lineCharIndex;
        int _caretXPos;
        int _onTextRunCharOffset;
        int _onTextRunPixelOffset;
        public VisualPointInfo(int lineCharIndex)
        {
            _lineCharIndex = lineCharIndex;
        }
        public void SetAdditionVisualInfo(int onTextRunCharOffset, int caretXPos, int textRunPixelOffset)
        {
            _caretXPos = caretXPos;
            _onTextRunCharOffset = onTextRunCharOffset;
            _onTextRunPixelOffset = textRunPixelOffset;
        }
        public int LineCharIndex => _lineCharIndex;


        public bool IsOnTheBeginOfLine => _lineCharIndex == 0;
        public abstract int LineId { get; }
        public abstract int LineTop { get; }
        public abstract int ActualLineHeight { get; }
        public abstract int LineNumber { get; }
        public abstract int CurrentWidth { get; }

        //public int TextRunCharOffset => _onTextRunCharOffset;
        //public int RunLocalSelectedIndex => _lineCharIndex - _onTextRunCharOffset;

        public int X => _caretXPos;

        //public int TextRunPixelOffset => _onTextRunPixelOffset;
    }



    public class EditableVisualPointInfo : VisualPointInfo
    {
        readonly TextLineBox _line;
        internal EditableVisualPointInfo(TextLineBox line, int lineCharIndex)
            : base(lineCharIndex)
        {
            _line = line;
        }

        internal TextLineBox Line => _line;
        public override int LineTop => _line.Top;

        public override int CurrentWidth => _line.LineWidth;

        public override int ActualLineHeight => _line.ActualLineHeight;

        public override int LineNumber => _line.LineNumber;

        public override int LineId => _line.LineNumber;

    }
}
