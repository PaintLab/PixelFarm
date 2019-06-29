//Apache2, 2014-present, WinterDev

using PixelFarm.Drawing;
using LayoutFarm.UI;
using LayoutFarm.TextEditing;
namespace LayoutFarm.CustomWidgets
{


    public class FlowLabel : AbstractRectUI
    {
        string _text;
        Color _textColor;
        Color _backColor;
        RequestFont _font;
        TextFlowRenderBox _textFlowRenderBox;
        PlainTextDocument _doc;
        RunStyle _runStyle;
        protected TextSpanStyle _defaultSpanStyle;
        public FlowLabel(int w, int h) : base(w, h)
        {

            _textColor = PixelFarm.Drawing.Color.Black; //default?, use Theme?
        }

        RunStyle GetDefaultRunStyle()
        {
            if (_runStyle == null)
            {
                return _runStyle = new RunStyle(_textFlowRenderBox.Root.TextServices)
                {
                    FontColor = Color.Black,
                    ReqFont = _font
                };
            }
            else
            {
                return _runStyle;
            }
        }

        public ContentTextSplitter TextSplitter
        {
            get;
            set;
        }
        public override RenderElement CurrentPrimaryRenderElement => _textFlowRenderBox;

        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (_textFlowRenderBox == null)
            {
                _textFlowRenderBox = new TextFlowRenderBox(rootgfx, this.Width, this.Height, true);
            }
            return _textFlowRenderBox;
        }
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                _doc = PlainTextDocumentHelper.CreatePlainTextDocument(value);
                if (_textFlowRenderBox != null)
                {
                    ReloadDocument();
                }
            }
        }
        void ReloadDocument()
        {
            if (_doc == null)
            {
                return;
            }

            _textFlowRenderBox.ClearAllChildren();
            int lineCount = 0;

            RunStyle runstyle = GetDefaultRunStyle();
            foreach (PlainTextLine line in _doc.GetLineIter())
            {
                if (lineCount > 0)
                {
                    _textFlowRenderBox.SplitCurrentLineToNewLine();
                }
                //we create an unparse text run***
                _textFlowRenderBox.AddTextLine(line);
            }

            this.InvalidateGraphics();
        }
    }

    public class Label : AbstractRectUI
    {
        string _text;
        Color _textColor;
        Color _backColor;
        RequestFont _font;

        CustomTextRun _myTextRun;
        //
        public Label(int w, int h)
            : base(w, h)
        {
            _textColor = PixelFarm.Drawing.Color.Black; //default?, use Theme?
        }

#if DEBUG
        public bool dbugBreakOnRenderElement;
#endif
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (_myTextRun == null)
            {
                var trun = new CustomTextRun(rootgfx, this.Width, this.Height);

#if DEBUG
                trun.dbugBreak = this.dbugBreakOnRenderElement;

#endif
                trun.SetLocation(this.Left, this.Top);
                trun.TextColor = _textColor;
                trun.BackColor = _backColor;
                trun.Text = this.Text;
                trun.PaddingLeft = this.PaddingLeft;
                trun.PaddingTop = this.PaddingTop;
                trun.SetVisible(this.Visible);
                trun.SetController(this);
                //
                if (_font != null)
                {
                    trun.RequestFont = _font;
                }
                _myTextRun = trun;
            }
            //-----------
            return _myTextRun;
        }
        protected override void InvalidatePadding(PaddingName paddingName, byte newValue)
        {
            if (_myTextRun == null) return;
            //
            switch (paddingName)
            {
                case PaddingName.Left:
                    _myTextRun.PaddingLeft = newValue;
                    break;
                case PaddingName.Top:
                    _myTextRun.PaddingTop = newValue;
                    break;
                case PaddingName.Right:
                    _myTextRun.PaddingRight = newValue;
                    break;
                case PaddingName.Bottom:
                    _myTextRun.PaddingBottom = newValue;
                    break;
                case PaddingName.AllSide:
                    _myTextRun.SetPaddings(this.PaddingLeft, this.PaddingTop, this.PaddingRight, this.PaddingBottom);
                    break;
                case PaddingName.AllSideSameValue:
                    _myTextRun.SetPaddings(newValue);
                    break;
            }
        }
        protected override void InvalidateMargin(MarginName marginName, short newValue)
        {
            //TODO:...
        }

        public override void SetFont(RequestFont font)
        {
            if (_myTextRun != null)
            {
                _myTextRun.RequestFont = font;
            }
            else
            {
                _font = font;
            }
        }
        //
        public override RenderElement CurrentPrimaryRenderElement => _myTextRun;
        //
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                if (_myTextRun != null)
                {
                    _myTextRun.Text = value;
                }
            }
        }
        /// <summary>
        /// text color
        /// </summary>
        public Color Color
        {
            get => _textColor;
            set
            {
                _textColor = value;
                if (_myTextRun != null)
                {
                    _myTextRun.TextColor = value;
                }
            }
        }
        public Color BackColor
        {
            get => _backColor;
            set
            {
                _backColor = value;
                if (_myTextRun != null)
                {
                    _myTextRun.BackColor = value;
                }
            }
        }
        //
        public override int InnerWidth => this.Width;
        public override int InnerHeight => this.Height;

    }
}