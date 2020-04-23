//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.UI;

namespace LayoutFarm.CustomWidgets
{
    struct MayBeEmptyTempContext<T> : IDisposable
    {
        internal readonly T _tool;
        internal MayBeEmptyTempContext(out T tool)
        {
            MayBeEmptyTempContext<T>.GetFreeItem(out _tool);
            tool = _tool;
        }
        public void Dispose()
        {
            if (_tool != null)
            {
                MayBeEmptyTempContext<T>.Release(_tool);
            }
        }

        public static readonly MayBeEmptyTempContext<T> Empty = new MayBeEmptyTempContext<T>();


        public delegate T CreateNewItemDelegate();
        public delegate void ReleaseItemDelegate(T item);


        [System.ThreadStatic]
        static Stack<T> s_pool;
        [System.ThreadStatic]
        static CreateNewItemDelegate s_newHandler;
        [System.ThreadStatic]
        static ReleaseItemDelegate s_releaseCleanUp;

        public static MayBeEmptyTempContext<T> Borrow(out T freeItem)
        {
            return new MayBeEmptyTempContext<T>(out freeItem);
        }

        public static void SetNewHandler(CreateNewItemDelegate newHandler, ReleaseItemDelegate releaseCleanUp = null)
        {
            //set new instance here, must set this first***
            if (s_pool == null)
            {
                s_pool = new Stack<T>();
            }
            s_newHandler = newHandler;
            s_releaseCleanUp = releaseCleanUp;
        }
        internal static void GetFreeItem(out T freeItem)
        {
            if (s_pool.Count > 0)
            {
                freeItem = s_pool.Pop();
            }
            else
            {
                freeItem = s_newHandler();
            }
        }
        internal static void Release(T item)
        {
            s_releaseCleanUp?.Invoke(item);
            s_pool.Push(item);
            //... 
        }
        public static bool IsInit()
        {
            return s_pool != null;
        }
    }

    static class LayoutTools
    {
        public static MayBeEmptyTempContext<LinkedList<T>> BorrowLinkedList<T>(out LinkedList<T> linkedlist)
        {
            if (!MayBeEmptyTempContext<LinkedList<T>>.IsInit())
            {
                MayBeEmptyTempContext<LinkedList<T>>.SetNewHandler(
                    () => new LinkedList<T>(),
                    list => list.Clear());
            }
            return MayBeEmptyTempContext<LinkedList<T>>.Borrow(out linkedlist);
        }
        public static MayBeEmptyTempContext<List<T>> BorrowList<T>(out List<T> linkedlist)
        {
            if (!MayBeEmptyTempContext<List<T>>.IsInit())
            {
                MayBeEmptyTempContext<List<T>>.SetNewHandler(
                    () => new List<T>(),
                    list => list.Clear());
            }
            return MayBeEmptyTempContext<List<T>>.Borrow(out linkedlist);
        }
    }

    /// <summary>
    /// abstract box ui element.
    /// this control provides 'primary-render-element', 
    /// keyboard-mouse-events, viewport mechanhism.
    /// </summary>
    public abstract class AbstractBox : AbstractRectUI
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

        public AbstractBox(int width, int height)
            : base(width, height)
        {
            _innerHeight = height;
            _innerWidth = width;
            _supportViewport = true;//default for all rect
            _needClipArea = true;
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


        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (_primElement == null)
            {
                //var renderE = new CustomRenderBox(rootgfx, this.Width, this.Height);

                GlobalRootGraphic.BlockGraphicsUpdate();

                var renderE = EnableDoubleBuffer ?
                    new DoubleBufferCustomRenderBox(rootgfx, this.Width, this.Height) { EnableDoubleBuffer = true } :
                    new CustomRenderBox(rootgfx, this.Width, this.Height);

                renderE.SetLocation(this.Left, this.Top);
                renderE.NeedClipArea = this.NeedClipArea;
                renderE.TransparentForMouseEvents = this.TransparentForMouseEvents;
                renderE.SetVisible(this.Visible);
                renderE.BackColor = _backColor;
                renderE.BorderColor = _borderColor;
                renderE.SetBorders(BorderLeft, BorderTop, BorderRight, BorderBottom);

#if DEBUG
                renderE.dbugBreak = this.dbugBreakMe;
#endif


                renderE.HasSpecificHeight = this.HasSpecificHeight;
                renderE.HasSpecificWidth = this.HasSpecificWidth;
                renderE.SetController(this);
                renderE.SetLocation(this.Left, this.Top);
                renderE.SetViewport(this.ViewportLeft, this.ViewportTop); 

                IUICollection<UIElement> childIter = GetDefaultChildrenIter();
                if (childIter != null && childIter.Count > 0)
                {
                    foreach (UIElement child in childIter.GetIter())
                    {
                        renderE.AddChild(child.GetPrimaryRenderElement(rootgfx));
                    }
                }

                GlobalRootGraphic.ReleaseGraphicsUpdate();
                renderE.InvalidateGraphics();

                _primElement = renderE;
            }
            return _primElement;
        }
        protected void SetPrimaryRenderElement(CustomRenderBox primElement)
        {
            _primElement = primElement;
        }
        protected void SuspendGraphicsUpdate()
        {
            _primElement?.SuspendGraphicsUpdate();
        }
        protected void ResumeGraphicsUpdate()
        {
            _primElement?.ResumeGraphicsUpdate();
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
                if (e.Delta < 0)
                {
                    //down
                    _viewportTop += 20;
                    if (_viewportTop > _innerHeight - this.Height)
                    {
                        _viewportTop = _innerHeight - this.Height;
                    }
                }
                else
                {
                    //up
                    _viewportTop -= 20;
                    if (_viewportTop < 0)
                    {
                        _viewportTop = 0;
                    }
                }
                _primElement.SetViewport(_viewportLeft, _viewportTop);
                this.InvalidateGraphics();
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
                    _primElement.LayoutHint = value;
                }

                //if (_uiList != null && _uiList.Count > 0)
                //{
                //    this.InvalidateLayout();
                //}
            }
        }
        protected override void OnContentLayout()
        {
            this.PerformContentLayout();
        }

        public bool AllowAutoContentExpand { get; set; }


        protected abstract IUICollection<UIElement> GetDefaultChildrenIter();

        public override void PerformContentLayout()
        {
            //****
            //this.InvalidateGraphics();
            //temp : arrange as vertical stack***
            Rectangle preBounds = this.Bounds;
            switch (this.ContentLayoutKind)
            {
                case BoxContentLayoutKind.VerticalStack:
                    {
                        int maxRight = 0;

                        int xpos = this.PaddingLeft; //start X at paddingLeft
                        int ypos = this.PaddingTop; //start Y at padding top

                        IUICollection<UIElement> childrenIter = GetDefaultChildrenIter();
                        if (childrenIter != null && childrenIter.Count > 0)
                        {
                            foreach (UIElement ui in childrenIter.GetIter())
                            {
                                if (ui is AbstractRectUI element)
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
                        }

                        this.SetInnerContentSize(maxRight, ypos);


                    }
                    break;
                case BoxContentLayoutKind.HorizontalStack:
                    {

                        int maxBottom = 0;

                        //experiment
                        bool allowAutoContentExpand = this.AllowAutoContentExpand;

                        int xpos = this.PaddingLeft; //start X at paddingLeft
                        int ypos = this.PaddingTop; //start Y at padding top
                        IUICollection<UIElement> childrenIter = GetDefaultChildrenIter();
                        if (childrenIter != null && childrenIter.Count > 0)
                        {
                            List<AbstractRectUI> alignToEnds = null;
                            var alignToEndsContext = MayBeEmptyTempContext<List<AbstractRectUI>>.Empty;

                            List<AbstractRectUI> notHaveSpecificWidthElems = null;
                            var notHaveSpecificWidthElemsContext = MayBeEmptyTempContext<List<AbstractRectUI>>.Empty;

                            int left_to_right_max_x = 0;

                            foreach (UIElement ui in childrenIter.GetIter())
                            {
                                if (ui is AbstractRectUI element)
                                {
                                    element.PerformContentLayout();

                                    //TODO: review Middle again
                                    if (element.Alignment == RectUIAlignment.End)
                                    {
                                        //skip this
                                        if (alignToEnds == null)
                                        {
                                            alignToEndsContext = LayoutTools.BorrowList(out alignToEnds);
                                        }
                                        alignToEnds.Add(element);
                                    }
                                    else
                                    {
                                        if (allowAutoContentExpand && !element.HasSpecificWidth)
                                        {
                                            if (notHaveSpecificWidthElems == null)
                                            {
                                                notHaveSpecificWidthElemsContext = LayoutTools.BorrowList(out notHaveSpecificWidthElems);
                                            }
                                            notHaveSpecificWidthElems.Add(element);
                                        }

                                        element.SetLocationAndSize(xpos, ypos + element.MarginTop, element.Width, element.Height); //
                                        xpos += element.Width + element.MarginLeftRight;
                                        int tmp_bottom = element.Bottom;
                                        if (tmp_bottom > maxBottom)
                                        {
                                            maxBottom = tmp_bottom;
                                        }
                                    }
                                }
                            }

                            left_to_right_max_x = xpos;

                            //--------
                            //arrange alignToEnd again
                            if (alignToEnds != null)
                            {
                                //var node = alignToEnds.Last; //start from last node
                                int n = alignToEnds.Count;
                                xpos = this.Width - PaddingRight;
                                while (n > 0)
                                {
                                    --n;
                                    AbstractRectUI rectUI = alignToEnds[n];
                                    xpos -= rectUI.Width + rectUI.MarginLeft;
                                    rectUI.SetLocationAndSize(xpos, ypos + rectUI.MarginTop, rectUI.Width, rectUI.Height); //

                                    //
                                    int tmp_bottom = rectUI.Bottom;
                                    if (tmp_bottom > maxBottom)
                                    {
                                        maxBottom = tmp_bottom;
                                    }
                                }

                                //release back to pool
                                alignToEndsContext.Dispose();
                            }
                            //--------

                            if (notHaveSpecificWidthElems != null && (xpos > left_to_right_max_x))
                            {
                                //this mean this allow content expand
                                float avaliable_w = xpos - left_to_right_max_x;
                                //distribute this 
                                float avg_w = avaliable_w / notHaveSpecificWidthElems.Count;

                                for (int m = notHaveSpecificWidthElems.Count - 1; m >= 0; --m)
                                {
                                    AbstractRectUI ui = notHaveSpecificWidthElems[m];
                                    ui.SetWidth((int)(ui.Width + avg_w));
                                }

                                //arrange location again
                                xpos = this.PaddingLeft; //start X at paddingLeft
                                foreach (UIElement ui in childrenIter.GetIter())
                                {
                                    if (ui is AbstractRectUI element && element.Alignment != RectUIAlignment.End)
                                    {
                                        //TODO: review here again
                                        element.SetLocation(xpos, ypos + element.MarginTop);
                                        xpos += element.Width + element.MarginLeftRight;
                                    }
                                }

                            }
                            notHaveSpecificWidthElemsContext.Dispose();
                            //--------
                        }

                        this.SetInnerContentSize(xpos, maxBottom);
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
                                if (ui is AbstractRectUI element)
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
            base.RaiseLayoutFinished();

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
        protected virtual void Clear() => _items?.Clear(this);
    }
}