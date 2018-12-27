//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm.CustomWidgets
{
    /// <summary>
    /// abstract box ui element.
    /// this control provides 'primary-render-element', 
    /// keyboard-mouse-events, viewport mechanhism.
    /// </summary>
    public abstract class AbstractBox : AbstractRectUI
    {
        BoxContentLayoutKind _boxContentLayoutKind;
        bool _needContentLayout;
        Color _backColor = Color.LightGray;
        Color _borderColor = Color.Transparent;

        int _innerWidth;
        int _innerHeight;

        int _viewportX;
        int _viewportY;

        bool _supportViewport;
        bool _needClipArea;
        CustomRenderBox _primElement;

        UICollection _uiList;

        public AbstractBox(int width, int height)
            : base(width, height)
        {
            _innerHeight = height;
            _innerWidth = width;
            _supportViewport = true;
        }

        public event EventHandler<UIMouseEventArgs> MouseDown;
        public event EventHandler<UIMouseEventArgs> MouseMove;
        public event EventHandler<UIMouseEventArgs> MouseUp;
        public event EventHandler<UIMouseEventArgs> MouseDoubleClick;
        public event EventHandler<UIMouseEventArgs> MouseLeave;
        public event EventHandler<UIMouseEventArgs> MouseDrag;
        public event EventHandler<UIMouseEventArgs> MouseWheel;
        public event EventHandler<UIMouseEventArgs> LostMouseFocus;
        public event EventHandler<UIGuestTalkEventArgs> DragOver;
        public event EventHandler<UIKeyEventArgs> KeyDown;

        //
        protected override bool HasReadyRenderElement => _primElement != null;
        public override RenderElement CurrentPrimaryRenderElement => _primElement;
        // 
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (_primElement == null)
            {
                var renderE = new CustomRenderBox(rootgfx, this.Width, this.Height);
                renderE.SetLocation(this.Left, this.Top);
                renderE.NeedClipArea = this.NeedClipArea;
                renderE.TransparentForAllEvents = this.TransparentAllMouseEvents;
                renderE.SetVisible(this.Visible);
                renderE.BackColor = _backColor;
                renderE.BorderColor = _borderColor;
                renderE.SetBorders(BorderLeft, BorderTop, BorderRight, BorderBottom);

                BuildChildrenRenderElement(renderE);

                _primElement = renderE;
            }
            return _primElement;
        }
        protected void SetPrimaryRenderElement(CustomRenderBox primElement)
        {
            _primElement = primElement;
        }

        protected void BuildChildrenRenderElement(RenderElement parent)
        {
            //TODO: review here
            parent.HasSpecificHeight = this.HasSpecificHeight;
            parent.HasSpecificWidth = this.HasSpecificWidth;
            parent.SetController(this);
            parent.SetVisible(this.Visible);
            parent.SetLocation(this.Left, this.Top);
            parent.HasSpecificWidthAndHeight = true; //?
            parent.SetViewport(this.ViewportX, this.ViewportY);

            int childCount = this.ChildCount;
            for (int m = 0; m < childCount; ++m)
            {
                parent.AddChild(this.GetChild(m));
            }
        }

        public bool NeedClipArea
        {
            get => _needClipArea;
            set
            {
                _needClipArea = value;
                if (_primElement != null)
                {
                    _primElement.NeedClipArea = value;
                }
            }
        }
        protected override void InvalidatePadding(PaddingName paddingName, byte newValue)
        {
            if (_primElement == null) return;
            //
            switch (paddingName)
            {
#if DEBUG
                default: throw new NotSupportedException();
#endif
                case PaddingName.Left:
                    _primElement.PaddingLeft = newValue;
                    break;
                case PaddingName.Top:
                    _primElement.PaddingTop = newValue;
                    break;
                case PaddingName.Right:
                    _primElement.PaddingRight = newValue;
                    break;
                case PaddingName.Bottom:
                    _primElement.PaddingBottom = newValue;
                    break;
                case PaddingName.AllSide:
                    _primElement.SetPadding(
                        this.PaddingLeft,
                        this.PaddingTop,
                        this.PaddingRight,
                        this.PaddingBottom
                        );
                    break;
                case PaddingName.AllSideSameValue:
                    _primElement.SetPadding(newValue);
                    break;
            }

        }
        public Color BackColor
        {
            get => _backColor;
            set
            {
                _backColor = value;
                if (HasReadyRenderElement)
                {
                    _primElement.BackColor = value;
                }
            }
        }
        public Color BorderColor
        {
            get => _borderColor;
            set
            {
                _borderColor = value;
                if (HasReadyRenderElement)
                {
                    _primElement.BorderColor = value;
                }
            }
        }
        protected override void InvalidateBorder(BorderName borderName, byte newValue)
        {
            if (_primElement != null)
            {
                switch (borderName)
                {
                    case BorderName.AllSide:
                        _primElement.SetBorders(newValue);
                        break;
                }
            }

            base.InvalidateBorder(borderName, newValue);
        }
        public bool AcceptKeyboardFocus
        {
            get;
            set;
        }
        protected override void OnDoubleClick(UIMouseEventArgs e)
        {

            MouseDoubleClick?.Invoke(this, e);

            if (this.AcceptKeyboardFocus)
            {
                this.Focus();
            }
        }
        protected override void OnMouseDown(UIMouseEventArgs e)
        {

            MouseDown?.Invoke(this, e);
            if (this.AcceptKeyboardFocus)
            {
                this.Focus();
            }

        }
        protected override void OnMouseMove(UIMouseEventArgs e)
        {
            if (e.IsDragging)
            {
                MouseDrag?.Invoke(this, e);
            }
            else
            {
                MouseMove?.Invoke(this, e);
            }
        }
        protected override void OnMouseLeave(UIMouseEventArgs e)
        {
            MouseLeave?.Invoke(this, e);
        }
        protected override void OnMouseEnter(UIMouseEventArgs e)
        {
            base.OnMouseEnter(e);
        }
        protected override void OnMouseHover(UIMouseEventArgs e)
        {
            base.OnMouseHover(e);
        }
        protected override void OnMouseUp(UIMouseEventArgs e)
        {
            MouseUp?.Invoke(this, e);
        }
        protected override void OnLostMouseFocus(UIMouseEventArgs e)
        {
            this.LostMouseFocus?.Invoke(this, e);
        }



        //----------------------------------------------------
        public override int ViewportX => _viewportX;
        public override int ViewportY => _viewportY;
        //
        public int ViewportRight => this.ViewportX + this.Width;
        public int ViewportBottom => this.ViewportY + this.Height;
        //

        public override void SetViewport(int x, int y, object reqBy)
        {
            //check if viewport is changed or not
            bool isChanged = (_viewportX != x) || (_viewportY != y);
            _viewportX = x;
            _viewportY = y;
            if (this.HasReadyRenderElement)
            {
                _primElement.SetViewport(_viewportX, _viewportY);
                if (isChanged)
                {
                    RaiseViewportChanged();
                }

            }
        }
        protected override void OnMouseWheel(UIMouseEventArgs e)
        {
            //vertical scroll

            if (_innerHeight > this.Height)
            {
                if (e.Delta < 0)
                {
                    //down
                    _viewportY += 20;
                    if (_viewportY > _innerHeight - this.Height)
                    {
                        _viewportY = _innerHeight - this.Height;
                    }
                }
                else
                {
                    //up
                    _viewportY -= 20;
                    if (_viewportY < 0)
                    {
                        _viewportY = 0;
                    }
                }
                _primElement.SetViewport(_viewportX, _viewportY);
                this.InvalidateGraphics();
            }
            //
            MouseWheel?.Invoke(this, e);
            //
        }
        //-------------------
        protected override bool OnProcessDialogKey(UIKeyEventArgs e)
        {

            KeyDown?.Invoke(this, e);

            //return true if you want to stop event bubble to other 
            if (e.CancelBubbling)
            {
                return true;
            }
            else
            {
                return base.OnProcessDialogKey(e);
            }
        }
        protected override void OnKeyDown(UIKeyEventArgs e)
        {
            base.OnKeyDown(e);
        }
        protected override void OnKeyPress(UIKeyEventArgs e)
        {
            base.OnKeyPress(e);
        }
        protected override void OnKeyUp(UIKeyEventArgs e)
        {
            base.OnKeyUp(e);
        }
        //-------------------
        public override int InnerWidth => _innerWidth;
        public override int InnerHeight => _innerHeight;

        public virtual void SetInnerContentSize(int w, int h)
        {
            _innerWidth = w;
            _innerHeight = h;
        }

        //----------------------------------------------------
        public IEnumerable<UIElement> GetChildIter()
        {
            if (_uiList != null)
            {
                int j = _uiList.Count;
                for (int i = 0; i < j; ++i)
                {
                    yield return _uiList.GetElement(i);
                }
            }
        }

        public void BringToTopMost()
        {

            AbstractBox parentBox = this.ParentUI as AbstractBox;
            if (parentBox != null)
            {
                this.RemoveSelf();
                parentBox.AddChild(this);
            }

        }
        public virtual void AddChild(UIElement ui)
        {
            if (_uiList == null)
            {
                _uiList = new UICollection(this);
            }

            _needContentLayout = true;
            _uiList.AddUI(ui);
            if (this.HasReadyRenderElement)
            {
                _primElement.AddChild(ui);
                //if (this.panelLayoutKind != BoxContentLayoutKind.Absolute)
                //{
                //    this.InvalidateLayout();
                //}
                //check if we support
                if (_supportViewport)
                {
                    this.InvalidateLayout();
                }
            }

            if (ui.NeedContentLayout)
            {
                ui.InvalidateLayout();
            }
        }
        public virtual void RemoveChild(UIElement ui)
        {
            _needContentLayout = true;
            _uiList.RemoveUI(ui);
            if (this.HasReadyRenderElement)
            {
                //if (this.ContentLayoutKind != BoxContentLayoutKind.Absolute)
                //{
                //    this.InvalidateLayout();
                //}
                if (_supportViewport)
                {
                    this.InvalidateLayout();
                }
                _primElement.RemoveChild(ui.CurrentPrimaryRenderElement);
            }
        }
        public virtual void ClearChildren()
        {
            _needContentLayout = true;
            if (_uiList != null)
            {
                _uiList.Clear();
            }
            if (this.HasReadyRenderElement)
            {
                _primElement.ClearAllChildren();
                if (_supportViewport)
                {
                    this.InvalidateLayout();
                }
            }
        }

        public int ChildCount => (_uiList != null) ? _uiList.Count : 0;

        public UIElement GetChild(int index) => _uiList.GetElement(index);

        public override bool NeedContentLayout => _needContentLayout;

        public BoxContentLayoutKind ContentLayoutKind
        {
            get => _boxContentLayoutKind;
            set
            {
                _boxContentLayoutKind = value; //invalidate layout after change this
                if (_uiList != null && _uiList.Count > 0)
                {
                    this.InvalidateLayout();
                }
            }
        }
        protected override void OnContentLayout()
        {
            this.PerformContentLayout();
        }
        public override void PerformContentLayout()
        {
            this.InvalidateGraphics();
            //temp : arrange as vertical stack***
            switch (this.ContentLayoutKind)
            {
                case CustomWidgets.BoxContentLayoutKind.VerticalStack:
                    {
                        int count = this.ChildCount;

                        int maxRight = 0;

                        int xpos = this.PaddingLeft; //start X at paddingLeft
                        int ypos = this.PaddingTop; //start Y at padding top

                        for (int i = 0; i < count; ++i)
                        {
                            var element = this.GetChild(i) as AbstractRectUI;
                            if (element != null)
                            {
                                element.PerformContentLayout();
                                element.SetLocationAndSize(xpos + element.MarginLeft, ypos + element.MarginTop, element.Width, element.Height);
                                ypos += element.Height + element.MarginTopBottom;

                                int tmp_right = element.Right;
                                if (tmp_right > maxRight)
                                {
                                    maxRight = tmp_right;
                                }
                            }
                        }

                        this.SetInnerContentSize(maxRight, ypos);
                    }
                    break;
                case CustomWidgets.BoxContentLayoutKind.HorizontalStack:
                    {
                        int count = this.ChildCount;
                        int maxBottom = 0;

                        int xpos = this.PaddingLeft; //start X at paddingLeft
                        int ypos = this.PaddingTop; //start Y at padding top

                        for (int i = 0; i < count; ++i)
                        {
                            var element = this.GetChild(i) as AbstractRectUI;
                            if (element != null)
                            {
                                element.PerformContentLayout();

                                //element.SetLocationAndSize(xpos, ypos, element.InnerWidth, element.InnerHeight); //OLD
                                //xpos += element.InnerWidth;

                                element.SetLocationAndSize(xpos, ypos + element.MarginTop, element.Width, element.Height); //
                                xpos += element.Width + element.MarginLeftRight;

                                int tmp_bottom = element.Bottom;
                                if (tmp_bottom > maxBottom)
                                {
                                    maxBottom = tmp_bottom;
                                }
                            }
                        }
                        this.SetInnerContentSize(xpos, maxBottom);
                    }
                    break;
                default:
                    {

                        //this case : no action about paddings, margins, borders...

                        int count = this.ChildCount;
                        int maxRight = 0;
                        int maxBottom = 0;
                        for (int i = 0; i < count; ++i)
                        {
                            var element = this.GetChild(i) as AbstractRectUI;
                            if (element != null)
                            {
                                element.PerformContentLayout();
                                int tmp_right = element.Right;// element.InnerWidth + element.Left;
                                if (tmp_right > maxRight)
                                {
                                    maxRight = tmp_right;
                                }
                                int tmp_bottom = element.Bottom;// element.InnerHeight + element.Top;
                                if (tmp_bottom > maxBottom)
                                {
                                    maxBottom = tmp_bottom;
                                }
                            }
                        }

                        if (!this.HasSpecificWidth)
                        {
                            this.SetInnerContentSize(maxRight, this.InnerHeight);
                        }
                        if (!this.HasSpecificHeight)
                        {
                            this.SetInnerContentSize(this.InnerWidth, maxBottom);
                        }
                    }
                    break;
            }
            //------------------------------------------------
            base.RaiseLayoutFinished();
        }
        protected override void Describe(UIVisitor visitor)
        {
            //describe base properties
            base.Describe(visitor);
            //describe child content
            if (_uiList != null)
            {
                int j = _uiList.Count;
                for (int i = 0; i < j; ++i)
                {
                    _uiList.GetElement(i).Walk(visitor);
                }
            }
        }

        protected override void OnGuestTalk(UIGuestTalkEventArgs e)
        {
            //?
            //this.DragOver?.Invoke(this, e);
            base.OnGuestTalk(e);
        }
    }
}