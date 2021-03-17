//MIT, 2019-present, WinterDev

using PixelFarm.Drawing;
using LayoutFarm.UI;
using LayoutFarm.TextFlow;

namespace LayoutFarm.CustomWidgets
{
    public class TextFlowLabel : AbstractRectUI
    {

        Color _backColor;
        TextSpanStyle _textSpanStyle;

        protected TextFlowRenderBox _textFlowRenderBox;

        protected PlainTextDocument _doc;

        public TextFlowLabel() : this(16, 16)
        {
            //if user does not provide width and height,
            //we use default first, and set HasSpecificWidthAndHeight=false
            //(this feature  found in label, image box, and text-flow-label) 
            HasSpecificWidthAndHeight = false;
        }

        public TextFlowLabel(int w, int h) : base(w, h)
        {
            AcceptKeyboardFocus = true;

            _textSpanStyle = new TextSpanStyle();
            _textSpanStyle.FontColor = Color.Black; //default, use theme

        }
        public RequestFont RequestFont
        {
            get => _textSpanStyle.ReqFont;
            set
            {
                _textSpanStyle.ReqFont = value;
                if (_textFlowRenderBox != null)
                {
                    //set new style
                    _textFlowRenderBox.CurrentTextSpanStyle = _textSpanStyle;
                }
            }
        }
        public Color BackColor
        {
            get => _backColor;
            set
            {
                _backColor = value;
                if (_textFlowRenderBox != null)
                {
                    //set new style
                    _textFlowRenderBox.BackgroundColor = _backColor;
                }
            }
        }
        public Color TextColor
        {
            get => _textSpanStyle.FontColor;
            set
            {
                _textSpanStyle.FontColor = value;
                //update
                if (_textFlowRenderBox != null)
                {
                    //set new style
                    _textFlowRenderBox.CurrentTextSpanStyle = _textSpanStyle;
                }
            }
        }

        public override RenderElement CurrentPrimaryRenderElement => _textFlowRenderBox;

        public override RenderElement GetPrimaryRenderElement()
        {
            if (_textFlowRenderBox == null)
            {
                var txtFlowRenderBox = new TextFlowRenderBox(this.Width, this.Height, true);
                txtFlowRenderBox.BackgroundColor = _backColor;

                if (_textSpanStyle.ReqFont == null)
                {
                    _textSpanStyle.ReqFont = GlobalRootGraphic.CurrentRootGfx.DefaultTextEditFontInfo;
                }

                txtFlowRenderBox.CurrentTextSpanStyle = _textSpanStyle;
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


        string _orgText;
        public string Text
        {
            get => _orgText;
            set
            {
                _orgText = value;
                _doc = new PlainTextDocument(value);
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
            _textFlowRenderBox.Reload(_doc); 
            this.InvalidateGraphics();
        }


        static TextFlowLabel s_currentFlowLabel;
        //----------
        public override void Focus()
        {

            if (s_currentFlowLabel != null)
            {
                if (s_currentFlowLabel == this)
                {
                    return;//already focus
                }
                else
                {
                    s_currentFlowLabel.Blur();
                    s_currentFlowLabel = this;
                }
            }
            else
            {
                s_currentFlowLabel = this;
            }

            base.Focus();
        }
        public override void Blur()
        {
            base.Blur();

            if (_textFlowRenderBox != null)
            {
                _textFlowRenderBox.CancelSelection();
                _textFlowRenderBox.InvalidateGraphics();
            }
        }
        protected override void OnMouseDown(UIMouseDownEventArgs e)
        {

            this.Focus();
            e.MouseCursorStyle = MouseCursorStyle.IBeam;
            e.CancelBubbling = true;

            _textFlowRenderBox.HandleMouseDown(e);
        }
        protected override void OnMouseMove(UIMouseMoveEventArgs e)
        {
            if (e.IsDragging)
            {
                _textFlowRenderBox.HandleDrag(e);
                e.CancelBubbling = true;
                e.MouseCursorStyle = MouseCursorStyle.IBeam;
            }
        }
        protected override void OnMouseUp(UIMouseUpEventArgs e)
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
        protected override void OnMouseWheel(UIMouseWheelEventArgs e)
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
        protected override void OnMouseLeave(UIMouseLeaveEventArgs e)
        {
            e.MouseCursorStyle = MouseCursorStyle.Arrow;
        }

        //
        protected override void OnKeyPress(UIKeyEventArgs e)
        {
            //eg. mask text
            //we collect actual key and send the mask to to the background  
            _textFlowRenderBox.HandleKeyPress(e);
            e.CancelBubbling = true;
        }
        protected override void OnKeyDown(UIKeyEventArgs e)
        {
            _textFlowRenderBox.HandleKeyDown(e);
            e.CancelBubbling = true;
        }
        protected override void OnKeyUp(UIKeyEventArgs e)
        {
            _textFlowRenderBox.HandleKeyUp(e);
            e.CancelBubbling = true;
        }
        protected override bool OnProcessDialogKey(UIKeyEventArgs e)
        {
            if (_textFlowRenderBox.HandleProcessDialogKey(e))
            {
                e.CancelBubbling = true;
                return true;
            }
            return false;
        }
    }
}