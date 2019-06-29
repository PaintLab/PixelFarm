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
        RunStyle _runStyle;
        RequestFont _font;
        TextFlowRenderBox _textFlowRenderBox;
        PlainTextDocument _doc;        
        protected TextSpanStyle _defaultSpanStyle;
        public FlowLabel(int w, int h) : base(w, h)
        {
            _textColor = PixelFarm.Drawing.Color.Black; //default?, use Theme?
        }
        public RequestFont RequestFont
        {
            get => _font;
            set
            {
                _font = value;
                if (_textFlowRenderBox != null)
                {
                    //apply new font to all text in the flow render box
                }
            }
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
                if (_font == null)
                {
                    _font = new RequestFont("tahoma", 11);
                }

                _runStyle = new RunStyle(rootgfx.TextServices) { FontColor = _textColor, ReqFont = _font };

                var txtFlowRenderBox = new TextFlowRenderBox(rootgfx, this.Width, this.Height, true);
                txtFlowRenderBox.BackgroundColor = _backColor;
                txtFlowRenderBox.SetLocation(this.Left, this.Top);
                txtFlowRenderBox.SetViewport(this.ViewportLeft, this.ViewportTop);
                txtFlowRenderBox.SetVisible(this.Visible);
                txtFlowRenderBox.SetController(this);

                //
                _textFlowRenderBox = txtFlowRenderBox;
                ReloadDocument();
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
                lineCount++;
            }

            this.InvalidateGraphics();
        }

        //----------
        protected override void OnMouseDown(UIMouseEventArgs e)
        {
            this.Focus();
            e.MouseCursorStyle = MouseCursorStyle.IBeam;
            e.CancelBubbling = true;
            e.CurrentContextElement = this;
            _textFlowRenderBox.HandleMouseDown(e);
        }
        protected override void OnMouseMove(UIMouseEventArgs e)
        {
            if (e.IsDragging)
            {
                _textFlowRenderBox.HandleDrag(e);
                e.CancelBubbling = true;
                e.MouseCursorStyle = MouseCursorStyle.IBeam;
            }
        }
        protected override void OnMouseUp(UIMouseEventArgs e)
        {
            if (e.IsDragging)
            {
                _textFlowRenderBox.HandleDragEnd(e);
            }
            else
            {
                _textFlowRenderBox.HandleMouseUp(e);
            }
            e.MouseCursorStyle = MouseCursorStyle.Default;
            e.CancelBubbling = true;
        }
        protected override void OnMouseWheel(UIMouseEventArgs e)
        {
            //mouse wheel on  
            _textFlowRenderBox.HandleMouseWheel(e);
            e.CancelBubbling = true;
        }
        protected override void OnDoubleClick(UIMouseEventArgs e)
        {
            _textFlowRenderBox.HandleDoubleClick(e);
            e.CancelBubbling = true;
        }
        protected override void OnMouseLeave(UIMouseEventArgs e)
        {
            e.MouseCursorStyle = MouseCursorStyle.Arrow;
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