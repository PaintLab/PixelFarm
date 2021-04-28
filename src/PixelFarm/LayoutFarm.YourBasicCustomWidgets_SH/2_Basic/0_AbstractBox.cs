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
    public abstract partial class AbstractBox : AbstractRectUI
    {

        //some basic rect-properties
        BoxContentLayoutKind _boxContentLayoutKind;
        Color _backColor = KnownColors.LightGray;
        Color _borderColor = Color.Transparent;

        int _innerWidth;
        int _innerHeight;

        int _viewportLeft;
        int _viewportTop;

        protected bool _supportViewport;
        protected bool _needClipArea;

        protected CustomRenderBox _primElement;
        CustomRenderBoxSpec _boxSpec;



        public AbstractBox(int width, int height)
            : base(width, height)
        {
            _innerHeight = height;
            _innerWidth = width;
            _supportViewport = true;//default for all rect
            _needClipArea = true;
        }

        public CustomRenderBoxSpec BoxSpec
        {
            get => _boxSpec;
            set
            {
                //assign box spec
                _boxSpec = value;

                if (_primElement != null && value != null)
                {
                    //set this to primary render ele  
                    //some prop in spec has effect on layout phase
                    //eg. width, height, margin,
                    if (value.BackColor.HasValue)
                    {
                        _primElement.BackColor = value.BackColor.Value;
                    }
                    if (value.BorderColor.HasValue)
                    {
                        _primElement.BorderColor = value.BorderColor.Value;
                    }

                    //TODO:...
                    //add more...
                }
            }
        }
        public bool EnableDoubleBuffer { get; set; }

        //TODO: review this fields 
        public event EventHandler<UIMouseDownEventArgs> MouseDown;
        public event EventHandler<UIMouseMoveEventArgs> MouseMove;
        public event EventHandler<UIMouseMoveEventArgs> MouseEnter;
        public event EventHandler<UIMouseMoveEventArgs> MouseDrag;
        public event EventHandler<UIMouseWheelEventArgs> MouseWheel;
        public event EventHandler<UIMouseUpEventArgs> MouseUp;

        public event EventHandler<UIMouseLeaveEventArgs> MouseLeave;
        public event EventHandler<UIMouseLostFocusEventArgs> LostMouseFocus;
        //some secondary event eg.
        //mouse-press, mouse-hover,
        //not expose in event fields
        //TODO: add MouseDrag, mouse-double-click
        //user must use it through ExternalEventListener / Behaviour


        public override RenderElement CurrentPrimaryRenderElement => _primElement;


        protected static void SetCommonProperties(CustomRenderBox renderE, AbstractBox absRect)
        {

            renderE.NeedClipArea = absRect.NeedClipArea;
            renderE.SetPadding(absRect.PaddingLeft, absRect.PaddingTop, absRect.PaddingRight, absRect.PaddingBottom);

            renderE.SetLocation(absRect.Left, absRect.Top);
            renderE.SetViewport(absRect.ViewportLeft, absRect.ViewportTop);
            renderE.SetVisible(absRect.Visible);
            renderE.SetBorders(absRect.BorderLeft, absRect.BorderTop, absRect.BorderRight, absRect.BorderBottom);

            renderE.BackColor = absRect._backColor;
            renderE.BorderColor = absRect._borderColor;

#if DEBUG
            renderE.dbugBreak = absRect.dbugBreakMe;
#endif

            renderE.SetController(absRect);
            renderE.TransparentForMouseEvents = absRect.TransparentForMouseEvents;
        }
        protected static void BuildChildren(CustomRenderBox renderE, AbstractBox absRect)
        {
            IUICollection<UIElement> childIter = absRect.GetDefaultChildrenIter();
            if (childIter != null && childIter.Count > 0)
            {

                foreach (UIElement child in childIter.GetIter())
                {
                    renderE.AddChild(child.GetPrimaryRenderElement());
                }
            }
        }
        public override RenderElement GetPrimaryRenderElement()
        {
            if (_primElement == null)
            {

                var renderE = EnableDoubleBuffer ?
                    new DoubleBufferCustomRenderBox(this.Width, this.Height) { EnableDoubleBuffer = true } :
                    new CustomRenderBox(this.Width, this.Height);

                SetCommonProperties(renderE, this);
                BuildChildren(renderE, this);


                return _primElement = renderE;
            }
            return _primElement;
        }
        protected void SetPrimaryRenderElement(CustomRenderBox primElement)
        {
            _primElement = primElement;
        }


        public override SizeF CalculateMinimumSize(LayoutUpdateArgs args)
        {
            if (this.HasSpecificWidthAndHeight)
            {
                CalculatedMinWidth = this.Width;
                CalculatedMinHeight = this.Height;
                return new SizeF(CalculatedMinWidth, CalculatedMinHeight);
            }


            if (_hasMinSize)
            {
                return new SizeF(CalculatedMinWidth, CalculatedMinHeight);
            }

            _hasMinSize = true;
            //minum size of present or children
            //so its depends on layout kind 
            IUICollection<UIElement> childIter = GetDefaultChildrenIter();
            if (childIter == null || childIter.Count == 0)
            {
                CalculatedMinWidth = PaddingLeft + PaddingRight + BorderLeft + BorderRight;
                CalculatedMinHeight = PaddingTop + PaddingBottom + BorderTop + BorderBottom;
            }

            switch (ContentLayoutKind)
            {
                default: throw new NotSupportedException();
                case BoxContentLayoutKind.Absolute:
                    {
                        //skip this
                        ////current position of all children
                        RectangleF total = RectangleF.Empty;

                        foreach (UIElement ui in childIter.GetIter())
                        {
                            SizeF minSize = ui.CalculateMinimumSize(args);
                            Point location = ui.GetLocation();
                            RectangleF elem_bounds = new RectangleF(location.X, location.Y, minSize.Width, minSize.Height);
                            total = RectangleF.Union(total, elem_bounds);
                        }

                        CalculatedMinWidth = HasSpecificWidth ? Width : (int)total.Width + PaddingLeft + PaddingRight + BorderLeft + BorderRight;
                        CalculatedMinHeight = HasSpecificHeight ? Height : (int)total.Height;
                    }
                    break;
                case BoxContentLayoutKind.VerticalStack:
                    {
                        float maxWidth = 0;
                        float totalH = 0;

                        foreach (UIElement ui in childIter.GetIter())
                        {
                            SizeF minSize = ui.CalculateMinimumSize(args);
                            if (maxWidth < minSize.Width)
                            {
                                maxWidth = minSize.Width;
                            }
                            totalH += minSize.Height;
                        }

                        CalculatedMinWidth = HasSpecificWidth ? Width : (int)maxWidth + PaddingLeft + PaddingRight + BorderLeft + BorderRight;
                        CalculatedMinHeight = HasSpecificHeight ? Height : (int)totalH;
                    }
                    break;
                case BoxContentLayoutKind.HorizontalStack:
                    {
                        float totalW = 0;
                        float maxHeight = 0;

                        foreach (UIElement ui in childIter.GetIter())
                        {
                            SizeF minSize = ui.CalculateMinimumSize(args);
                            totalW += minSize.Width;
                            if (minSize.Height > maxHeight)
                            {
                                maxHeight = minSize.Height;
                            }
                        }

                        CalculatedMinWidth = HasSpecificWidth ? Width : (int)totalW + PaddingLeft + PaddingRight + BorderLeft + BorderRight;
                        CalculatedMinHeight = HasSpecificHeight ? Height : (int)maxHeight;

                    }
                    break;
                case BoxContentLayoutKind.HorizontalFlow:
                    {
                        //assume all elem are inline elem

                        float totalW = 0;
                        float y = 0;
                        float maxHeight = 0;


                        foreach (UIElement ui in childIter.GetIter())
                        {
                            SizeF minSize = ui.CalculateMinimumSize(args);

                            if (totalW + minSize.Width > args.AvailableWidth)
                            {
                                //begin newline here
                                y += maxHeight;
                                maxHeight = 0;
                                totalW = 0;
                            }

                            if (minSize.Height > maxHeight)
                            {
                                maxHeight = minSize.Height;
                            }
                            totalW += minSize.Width;
                        }



                        CalculatedMinWidth = HasSpecificWidth ? Width : (int)totalW + PaddingLeft + PaddingRight + BorderLeft + BorderRight;
                        CalculatedMinHeight = HasSpecificHeight ? Height : (int)(y + maxHeight);
                        //
                    }
                    break;
            }

            return new SizeF(CalculatedMinWidth, CalculatedMinHeight);
        }

        public override void UpdateLayout()
        {
            base.UpdateLayout();
            IUICollection<UIElement> childIter = GetDefaultChildrenIter();
            if (childIter != null && childIter.Count > 0)
            {
                foreach (UIElement ui in childIter.GetIter())
                {
                    ui.UpdateLayout();
                }
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
        protected override void InvalidatePadding(PaddingName paddingName, ushort newValue)
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
                if (_primElement != null)
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
                if (_primElement != null)
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
                        //TODO: 
                }
            }
        }

        protected override void OnDoubleClick(UIMouseEventArgs e)
        {
            if (this.AcceptKeyboardFocus)
            {
                this.Focus();
            }
        }
        protected override void OnMouseEnter(UIMouseMoveEventArgs e)
        {
            MouseEnter?.Invoke(this, e);
            base.OnMouseEnter(e);
        }
        protected override void OnMouseDown(UIMouseDownEventArgs e)
        {
            MouseDown?.Invoke(this, e);
            base.OnMouseDown(e);

            if (this.AcceptKeyboardFocus)
            {
                this.Focus();
            }
        }
        protected override void OnMouseMove(UIMouseMoveEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.IsDragging)
            {
                MouseDrag?.Invoke(this, e);
            }
            else
            {
                MouseMove?.Invoke(this, e);
            }
        }
        protected override void OnMouseLeave(UIMouseLeaveEventArgs e)
        {
            base.OnMouseLeave(e);
            MouseLeave?.Invoke(this, e);
        }

        protected override void OnMouseUp(UIMouseUpEventArgs e)
        {
            base.OnMouseUp(e);
            MouseUp?.Invoke(this, e);
        }
        protected override void OnLostMouseFocus(UIMouseLostFocusEventArgs e)
        {
            base.OnLostMouseFocus(e);
            this.LostMouseFocus?.Invoke(this, e);
        }
        //----------------------------------------------------
        public override int ViewportLeft => _viewportLeft;
        public override int ViewportTop => _viewportTop;
        //
        public int ViewportRight => this.ViewportLeft + this.Width;
        public int ViewportBottom => this.ViewportTop + this.Height;
        //

        public override void SetViewport(int x, int y, object reqBy)
        {
            //check if viewport is changed or not
            bool isChanged = (_viewportLeft != x) || (_viewportTop != y);
            _viewportLeft = x;
            _viewportTop = y;
            if (this.HasReadyRenderElement)
            {
                _primElement.SetViewport(_viewportLeft, _viewportTop);
                if (isChanged)
                {
                    RaiseViewportChanged();
                }

            }
        }
        protected override void OnMouseWheel(UIMouseWheelEventArgs e)
        {
            //vertical scroll

            if (_innerHeight > this.Height)
            {
                int new_top = _viewportTop;

                int diff = Height / 4; //TODO: check this value again 

                if (diff < 20)
                {
                    diff = 20;
                }

                if (e.Delta < 0)
                {
                    //down
                    //TODO:
                    new_top += diff;
                    if (new_top > _innerHeight - this.Height)
                    {
                        new_top = _innerHeight - this.Height;
                    }
                }
                else
                {
                    //up
                    new_top -= diff;
                    if (new_top < 0)
                    {
                        new_top = 0;
                    }
                }
                SetViewport(_viewportLeft, new_top);
            }
            MouseWheel?.Invoke(this, e);
        }
        //-------------------
        protected override bool OnProcessDialogKey(UIKeyEventArgs e)
        {

            //KeyDown?.Invoke(this, e);
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

        public override int InnerWidth => _innerWidth;
        public override int InnerHeight => _innerHeight;
        public virtual void SetInnerContentSize(int w, int h)
        {
            _innerWidth = w;
            _innerHeight = h;
        }


        public override bool NeedContentLayout => _needContentLayout;

        public virtual BoxContentLayoutKind ContentLayoutKind
        {
            get => _boxContentLayoutKind;
            set
            {
                _boxContentLayoutKind = value; //invalidate layout after change this
                if (_primElement != null)
                {
                    //assign layout hint 
                    _primElement.ContentHitTestHint = RenderBoxes.HitTestHint.Custom;
                }
            }
        }


        public bool AllowAutoContentExpand { get; set; }


        protected abstract IUICollection<UIElement> GetDefaultChildrenIter();


        bool _preserveLineBoxes = true;
        /// <summary>
        /// preserve multiple lines 
        /// </summary>
        public bool PreserverLineBoxes
        {
            get => _preserveLineBoxes;
            set
            {

                _preserveLineBoxes = value;
                if (!value && _primElement != null)
                {
                    _primElement.Lines = null;
                }
            }
        }


        struct TempAbstractRectUI : IAbstractRect
        {

            readonly RenderElement _renderE;
            public TempAbstractRectUI(RenderElement renderE)
            {
                _renderE = renderE;
                HorizontalAlignment = RectUIAlignment.Begin;
                VerticalAlignment = VerticalAlignment.Top;
            }

            public ushort MarginLeft => 0;

            public ushort MarginTop => 0;

            public ushort MarginRight => 0;

            public ushort MarginBottom => 0;

            public int Left => _renderE.Left;

            public int Top => _renderE.Top;

            public int Width => _renderE.Width;

            public int Height => _renderE.Height;

            public RectUIAlignment HorizontalAlignment { get; set; }
            public VerticalAlignment VerticalAlignment { get; set; }

            public RenderElement GetPrimaryRenderElement() => _renderE;

            public void SetLocation(int left, int top) => _renderE.SetLocation(left, top);
            public void SetSize(int w, int h) => _renderE.SetSize(w, h);
            public void SetLocationAndSize(int left, int top, int width, int height) => _renderE.SetBounds(left, top, width, height);
            public bool HasSpecificWidth => true;
            public bool HasSpecificHeight => true;
        }



        public override void PerformContentLayout(LayoutUpdateArgs args)
        {
            //TODO: move layout algo to another class

            //****
            //this.InvalidateGraphics();
            //temp : arrange as vertical stack***
            Rectangle preBounds = this.Bounds;

            //we can use manual layout here
            //or use default built-in layout machanism

            int limitW = 99999;
            if (this.HasSpecificWidth)
            {
                limitW = this.Width;
            }

            switch (this.ContentLayoutKind)
            {
                case BoxContentLayoutKind.VerticalStack:
                    {
                        //TODO: check if non-overlap or not
                        if (_primElement != null)
                        {
                            _primElement.ContentHitTestHint = RenderBoxes.HitTestHint.VerticalColumnNonOverlap;
                        }


                        int maxRight = 0;

                        int xpos = this.PaddingLeft; //start X at paddingLeft
                        int ypos = this.PaddingTop; //start Y at padding top

                        IUICollection<UIElement> childrenIter = GetDefaultChildrenIter();
                        if (childrenIter != null && childrenIter.Count > 0)
                        {
                            int width = this.Width;
                            foreach (UIElement ui in childrenIter.GetIter())
                            {
                                if (ui is AbstractRectUI rect)
                                {
                                    rect.PerformContentLayout(args);
                                    if (!rect.HasSpecificWidth)
                                    {
                                        //expand full width 
                                        rect.SetLocationAndSize(xpos + rect.MarginLeft, ypos + rect.MarginTop, width, rect.Height);
                                    }
                                    else
                                    {
                                        rect.SetLocationAndSize(xpos + rect.MarginLeft, ypos + rect.MarginTop, rect.Width, rect.Height);
                                    }

                                    ypos += rect.Height + rect.MarginTopBottom;

                                    int tmp_right = rect.Right;
                                    if (tmp_right > maxRight)
                                    {
                                        maxRight = tmp_right;
                                    }
                                }
                            }
                        }

                        this.SetInnerContentSize(maxRight, ypos);
                    }
                    break;
                case BoxContentLayoutKind.HorizontalStack:
                    {

                        if (_primElement != null)
                        {
                            //TODO: review here again
                            _primElement.ContentHitTestHint = RenderBoxes.HitTestHint.HorizontalRowNonOverlap;
                        }

                        int maxBottom = 0;
                        //experiment
                        bool allowAutoContentExpand = this.AllowAutoContentExpand;
                        allowAutoContentExpand = true;

                        int xpos = this.PaddingLeft; //start X at paddingLeft
                        int ypos = this.PaddingTop; //start Y at padding top
                        IUICollection<UIElement> childrenIter = GetDefaultChildrenIter();

                        //check if this abstract box want to preserver line box or not
                        //if just layout, then we can use shared lineboxes 

                        bool isMixAlignment = false;

                        if (childrenIter != null && childrenIter.Count > 0)
                        {
                            using (var lineboxContext = new LineBoxesContext(null))
                            {
                                LineBox linebox = lineboxContext.AddNewLineBox();
                                int left_to_right_max_x = 0;
                                int limit_w = this.Width;
                                int max_lineHeight = 0;
                                foreach (UIElement ui in childrenIter.GetIter())
                                {
                                    IAbstractRect rect;
                                    if (ui is AbstractRectUI rectUI) //TODO: review here again
                                    {
                                        //1. measure content=> get 'default' size, minimum or specific size
                                        //

                                        rectUI.PerformContentLayout(args);
                                        rect = rectUI;
                                    }
                                    else
                                    {
                                        rect = new TempAbstractRectUI(ui.GetPrimaryRenderElement());
                                    }

                                    int marginLeftRigth = rect.MarginLeft + rect.MarginRight;
                                    int new_x = xpos + rect.Width + marginLeftRigth;

                                    rect.SetLocation(xpos, ypos + rect.MarginTop); //
                                    int tmp_bottom = rect.Top + rect.Height;

                                    xpos = new_x;

                                    if (max_lineHeight < rect.Height)
                                    {
                                        max_lineHeight = rect.Height;
                                        linebox.LineHeight = max_lineHeight;
                                    }

                                    if (tmp_bottom > maxBottom)
                                    {
                                        //start 
                                        maxBottom = tmp_bottom;
                                    }
                                    linebox.Add(rect);
                                }

                                left_to_right_max_x = xpos;
                                linebox.AdjustHorizontalAlignment(this.Width);
                                linebox.AdjustVerticalAlignment();

                                if (!isMixAlignment)
                                {
                                    isMixAlignment = linebox._mixedHorizontalAlignment;
                                }
                            }

                        }
                        if (_primElement != null && isMixAlignment)
                        {
                            //TODO: review here again
                            _primElement.ContentHitTestHint = RenderBoxes.HitTestHint.Custom;
                        }

                        this.SetInnerContentSize(xpos, maxBottom);
                    }
                    break;
                case BoxContentLayoutKind.HorizontalFlow:
                    {
                        if (_primElement != null)
                        {
                            _primElement.ContentHitTestHint = RenderBoxes.HitTestHint.Custom;
                        }


                        int maxBottom = 0;
                        //experiment
                        bool allowAutoContentExpand = this.AllowAutoContentExpand;
                        allowAutoContentExpand = true;

                        int xpos = this.PaddingLeft; //start X at paddingLeft
                        int ypos = this.PaddingTop; //start Y at padding top
                        IUICollection<UIElement> childrenIter = GetDefaultChildrenIter();

                        //check if this abstract box want to preserver line box or not
                        //if just layout, then we can use shared lineboxes 

                        _primElement?.Lines?.Clear();

                        if (childrenIter != null && childrenIter.Count > 0)
                        {
                            using (var lineboxContext = new LineBoxesContext(_preserveLineBoxes ? _primElement : null))
                            {
                                LineBox linebox = lineboxContext.AddNewLineBox();
                                int left_to_right_max_x = 0;
                                int limit_w = this.Width;
                                int max_lineHeight = 0;

                                foreach (UIElement ui in childrenIter.GetIter())
                                {
                                    IAbstractRect rect;

                                    if (ui is AbstractRectUI rectUI) //TODO: review here again
                                    {
                                        //1. measure content=> get 'default' size, minimum or specific size
                                        //
                                        rectUI.PerformContentLayout(args);
                                        rect = rectUI;
                                    }
                                    else
                                    {
                                        rect = new TempAbstractRectUI(ui.GetPrimaryRenderElement());
                                    }

                                    int marginLeftRigth = rect.MarginLeft + rect.MarginRight;
                                    int new_x = xpos + rect.Width + marginLeftRigth;

                                    if (new_x > limit_w)
                                    {
                                        //before we begin a new line
                                        //adjust current line vertical aligment

                                        linebox.AdjustHorizontalAlignment(limit_w);
                                        linebox.AdjustVerticalAlignment();

                                        xpos = PaddingLeft; //start
                                        ypos += max_lineHeight + 1;//**
                                        max_lineHeight = 0;//reset

                                        //start newline
                                        linebox = lineboxContext.AddNewLineBox();
                                        linebox.LineTop = ypos;

                                        new_x = xpos + rect.Width + marginLeftRigth;
                                    }

                                    int tmp_bottom = 0;
                                    if (_preserveLineBoxes)
                                    {
                                        //new top is relative to linetop
                                        rect.SetLocation(xpos, rect.MarginTop); //
                                        tmp_bottom = ypos + (rect.Top + rect.Height);
                                    }
                                    else
                                    {
                                        rect.SetLocation(xpos, ypos + rect.MarginTop); //
                                        tmp_bottom = rect.Top + rect.Height;
                                    }

                                    xpos = new_x;

                                    if (max_lineHeight < rect.Height)
                                    {
                                        max_lineHeight = rect.Height;
                                        linebox.LineHeight = max_lineHeight;
                                    }

                                    if (tmp_bottom > maxBottom)
                                    {
                                        //start 
                                        maxBottom = tmp_bottom;
                                    }

                                    linebox.Add(rect);

                                }

                                left_to_right_max_x = xpos;

                                linebox.AdjustHorizontalAlignment(limit_w);
                                linebox.AdjustVerticalAlignment();
                            }
                        }

                        this.SetInnerContentSize(xpos, maxBottom);

                        //this.dbugBreakMe = true;

                    }
                    break;
                default:
                    {

                        //this case : no action about paddings, margins, borders...


                        int maxRight = 0;
                        int maxBottom = 0;
                        IUICollection<UIElement> childrenIter = GetDefaultChildrenIter();
                        if (childrenIter != null && childrenIter.Count > 0)
                        {
                            foreach (UIElement ui in childrenIter.GetIter())
                            {
                                if (ui is AbstractRectUI rect)
                                {
                                    rect.PerformContentLayout(args);

                                    int tmp_right = rect.Right;// element.InnerWidth + element.Left;
                                    if (tmp_right > maxRight)
                                    {
                                        maxRight = tmp_right;
                                    }
                                    int tmp_bottom = rect.Bottom;// element.InnerHeight + element.Top;
                                    if (tmp_bottom > maxBottom)
                                    {
                                        maxBottom = tmp_bottom;
                                    }
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

#if DEBUG
            Rectangle postBounds = this.Bounds;
            if (preBounds != postBounds)
            {

            }
#endif
            //------------------------------------------------
            base.RaiseLayoutFinished(); //TODO: review here again

#if DEBUG
            if (HasReadyRenderElement)
            {
                // this.InvalidateGraphics();
            }
#endif
        }



        protected override void OnGuestMsg(UIGuestMsgEventArgs e)
        {
            //?
            //this.DragOver?.Invoke(this, e);
            base.OnGuestMsg(e);
        }

    }


    public abstract class AbstractBox<T> : AbstractBox
        where T : UIElement.IUICollection<UIElement>
    {
        protected T _items;
        public AbstractBox(int w, int h) : base(w, h)
        {
        }
        protected override IUICollection<UIElement> GetDefaultChildrenIter() => _items;
    }


    public abstract class AbstractControlBox : AbstractBox<UIElement.UIList<UIElement>>
    {
        public AbstractControlBox(int w, int h) : base(w, h) { }

        protected void AddChild(UIElement ui)
        {
            if (_items == null)
            {
                _items = new UIList<UIElement>();
            }
            _items.Add(this, ui);
        }
        protected void RemoveChild(UIElement ui)
        {
            _items.Remove(this, ui);
        }
        protected virtual void Clear() => _items?.Clear(this);
    }
}