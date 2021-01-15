//Apache2, 2014-present, WinterDev

using System;
using PixelFarm.Drawing;


namespace LayoutFarm.UI
{


    /// <summary>
    /// abstract Rect UI Element
    /// </summary>
    public abstract class AbstractRectUI : UIElement, IScrollable, IBoxElement, IAcceptBehviour, IAbstractRect
    {
        //dimension only
        //no color,
        //no children

        protected enum PaddingName : byte
        {
            Left,
            Top,
            Right,
            Bottom,
            AllSideSameValue,
            AllSide
        }
        protected enum MarginName : byte
        {
            Left,
            Top,
            Right,
            Bottom,
            AllSideSameValue,
            AllSide
        }
        protected enum BorderName : byte
        {
            Left,
            Top,
            Right,
            Bottom,
            AllSideSameValue,
            AllSide
        }

        //some properties about our rect
        bool _specificWidth;
        bool _specificHeight;

        ushort _paddingLeft;
        ushort _paddingTop;
        ushort _paddingRight;
        ushort _paddingBottom;
        //
        byte _borderLeft;
        byte _borderTop;
        byte _borderRight;
        byte _borderBottom;
        //
        ushort _marginLeft;
        ushort _marginTop;
        ushort _marginRight;
        ushort _marginBottom;

        static AbstractRectUI()
        {
            if (!Temp<ViewportChangedEventArgs>.IsInit())
            {
                Temp<ViewportChangedEventArgs>.SetNewHandler(
                    () => new ViewportChangedEventArgs(),
                    null
                );
            }
        }
        public AbstractRectUI(int width, int height)
        {
            //default,           
            HasSpecificWidthAndHeight = true;

            SetElementBoundsWH(width, height);

            //default for box
            this.AutoStopMouseEventPropagation = true;
        }


        public RectUIAlignment HorizontalAlignment { get; set; }
        public VerticalAlignment VerticalAlignment { get; set; }



        public int CalculatedMinWidth { get; protected set; }
        public int CalculatedMinHeight { get; protected set; }

        public ushort UserSpecificMinWidth { get; set; }
        public ushort UserSpecificMinHeight { get; set; }

        public event EventHandler<ViewportChangedEventArgs> ViewportChanged;//TODO: review this*** => use event queue?


        protected void RaiseViewportChanged()
        {
            if (ViewportChanged != null)
            {

                using (Temp<ViewportChangedEventArgs>.Borrow(out ViewportChangedEventArgs changedEventArgs))
                {
                    changedEventArgs.Kind = ViewportChangedEventArgs.ChangeKind.Location;
                    ViewportChanged.Invoke(this, changedEventArgs);
                }
            }

        }
        protected void RaiseLayoutFinished()
        {
            if (ViewportChanged != null)
            {

                using (Temp<ViewportChangedEventArgs>.Borrow(out ViewportChangedEventArgs changedEventArgs))
                {
                    changedEventArgs.Kind = ViewportChangedEventArgs.ChangeKind.LayoutDone;
                    ViewportChanged.Invoke(this, changedEventArgs);
                }
            }

        }
        public virtual void SetFont(RequestFont font)
        {

        }
#if DEBUG
        public bool dbugBreakOnSetLocation;
#endif
        public virtual void SetLocation(int left, int top)
        {
#if DEBUG
            if (dbugBreakOnSetLocation)
            {

            }
#endif
            SetElementBoundsLT(left, top);

            //TODO: review here
            this.CurrentPrimaryRenderElement?.SetLocation(left, top);

        }

        /// <summary>
        /// set visual size (or viewport size) of this rect
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public virtual void SetSize(int width, int height)
        {
            SetElementBoundsWH(width, height);
            this.CurrentPrimaryRenderElement?.SetSize(width, height);
        }
        /// <summary>
        /// set location and visual size of this rect
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public virtual void SetLocationAndSize(int left, int top, int width, int height)
        {
            SetElementBoundsLT(left, top);
            SetElementBoundsWH(width, height);
            if (this.HasReadyRenderElement)
            {
                this.CurrentPrimaryRenderElement.SetBounds(left, top, width, height);
            }
        }
        public int Left
        {
            get
            {
                if (this.HasReadyRenderElement)
                {
                    return this.CurrentPrimaryRenderElement.X;
                }
                else
                {
                    return (int)this.BoundLeft;
                }
            }
        }
        public int Top
        {
            get
            {
                if (this.HasReadyRenderElement)
                {
                    return this.CurrentPrimaryRenderElement.Y;
                }
                else
                {
                    return (int)this.BoundTop;
                }
            }
        }
        //
        public int Right => this.Left + Width;
        //
        public int Bottom => this.Top + Height;
        //
        public Point Position
        {
            get
            {
                if (this.HasReadyRenderElement)
                {
                    return new Point(CurrentPrimaryRenderElement.X, CurrentPrimaryRenderElement.Y);
                }
                else
                {
                    return new Point((int)BoundLeft, (int)BoundTop);
                }
            }
        }
        /// <summary>
        /// visual width or viewport width
        /// </summary>
        public int Width
        {
            get
            {
                if (this.HasReadyRenderElement)
                {
                    return this.CurrentPrimaryRenderElement.Width;
                }
                else
                {
                    return (int)BoundWidth;
                }
            }
        }
        /// <summary>
        /// visual height or viewport height
        /// </summary>
        public int Height
        {
            get
            {
                if (this.HasReadyRenderElement)
                {
                    return this.CurrentPrimaryRenderElement.Height;
                }
                else
                {
                    return (int)BoundHeight;
                }
            }
        }

        //---------------------------------------------------------------
        protected virtual void InvalidatePadding(PaddingName paddingName, ushort newValue)
        {
        }
        public ushort PaddingLeft
        {
            get => _paddingLeft;
            set => InvalidatePadding(PaddingName.Left, _paddingLeft = value);
        }
        public ushort PaddingTop
        {
            get => _paddingTop;
            set => InvalidatePadding(PaddingName.Top, _paddingTop = value);
        }
        public ushort PaddingRight
        {
            get => _paddingRight;
            set => InvalidatePadding(PaddingName.Right, _paddingRight = value);
        }
        public ushort PaddingBottom
        {
            get => _paddingBottom;
            set => InvalidatePadding(PaddingName.Bottom, _paddingBottom = value);
        }
        public void SetPaddings(byte left, byte top, byte right, byte bottom)
        {
            _paddingLeft = left;
            _paddingRight = right;
            _paddingTop = top;
            _paddingBottom = bottom;
            InvalidatePadding(PaddingName.AllSide, 0);
        }
        public void SetPaddings(byte sameValue)
        {
            _paddingLeft =
                _paddingRight =
                _paddingTop =
                _paddingBottom = sameValue;
            InvalidatePadding(PaddingName.AllSideSameValue, sameValue);
        }
        //---------------------------------------------------------------
        protected virtual void InvalidateMargin(MarginName marginName, ushort newValue)
        {
        }
        public ushort MarginLeft
        {
            get => _marginLeft;
            set => InvalidateMargin(MarginName.Left, _marginLeft = value);
        }
        public ushort MarginTop
        {
            get => _marginTop;
            set => InvalidateMargin(MarginName.Top, _marginTop = value);
        }
        public ushort MarginRight
        {
            get => _marginRight;
            set => InvalidateMargin(MarginName.Right, _marginRight = value);
        }
        public ushort MarginBottom
        {
            get => _marginBottom;
            set => InvalidateMargin(MarginName.Bottom, _marginBottom = value);
        }
        public int MarginLeftRight => _marginLeft + _marginRight;
        public int MarginTopBottom => _marginTop + _marginBottom;

        public void SetMargins(ushort left, ushort top, ushort right, ushort bottom)
        {
            _marginLeft = left;
            _marginTop = top;
            _marginRight = right;
            _marginBottom = bottom;
            InvalidateMargin(MarginName.AllSide, 0);
        }
        public void SetMargins(ushort sameValue)
        {
            _marginLeft =
                _marginTop =
                _marginRight =
                _marginBottom = sameValue;
            InvalidateMargin(MarginName.AllSideSameValue, sameValue);
        }
        //---------------------------------------------------------------
        protected virtual void InvalidateBorder(BorderName borderName, byte newValue)
        {
        }
        public byte BorderLeft
        {
            get => _borderLeft;
            set => InvalidateBorder(BorderName.Left, _borderLeft = value);
        }
        public byte BorderTop
        {
            get => _borderTop;
            set => InvalidateBorder(BorderName.Top, _borderTop = value);
        }
        public byte BorderRight
        {
            get => _borderRight;
            set => InvalidateBorder(BorderName.Right, _borderRight = value);
        }
        public byte BorderBottom
        {
            get => _borderBottom;
            set => InvalidateBorder(BorderName.Bottom, _borderBottom = value);
        }
        public void SetBorders(byte left, byte top, byte right, byte bottom)
        {
            _borderLeft = left;
            _borderTop = top;
            _borderRight = right;
            _borderBottom = bottom;
            InvalidateBorder(BorderName.AllSide, 0);
        }
        public void SetBorders(byte sameValue)
        {
            _borderLeft =
                _borderTop =
                _borderRight =
                _borderBottom = sameValue;
            InvalidateBorder(BorderName.AllSideSameValue, sameValue);
        }
        //---------------------------------------------------------------

        public override void InvalidateGraphics()
        {

            //invalidate 'bubble' rect 
            //is (0,0,w,h) start invalidate from current primary render element
            this.CurrentPrimaryRenderElement?.InvalidateGraphics();

        }

        public override void GetViewport(out int left, out int top)
        {
            //AbstractRect dose not have actual viewport
            left = ViewportLeft;
            top = ViewportTop;
        }
        //
        public virtual int ViewportLeft => 0;
        //AbstractRect dose not have actual viewport
        //if you want viewport you must overide this

        public virtual int ViewportTop => 0;
        //AbstractRect dose not have actual viewport
        //if you want viewport you must overide this
        //
        int IScrollable.ViewportWidth => this.Width;

        //AbstractRect dose not have actual viewport
        //if you want viewport you must overide this

        int IScrollable.ViewportHeight => this.Height;

        //AbstractRect dose not have actual viewport
        //if you want viewport you must overide this

        public virtual void SetViewport(int x, int y, object reqBy)
        {
            //AbstractRect dose not have actual viewport
            //if you want viewport you must overide this
        }
        public void SetViewport(int x, int y)
        {
            //AbstractRect dose not have actual viewport
            //if you want viewport you must overide this
            SetViewport(x, y, this);
        }

        public virtual int InnerHeight => this.Height;
        //
        public virtual int InnerWidth => this.Width;

        public bool HasSpecificWidth
        {
            get => _specificWidth;
            set => _specificWidth = value;
        }
        public bool HasSpecificHeight
        {
            get => _specificHeight;
            set => _specificHeight = value;
        }
        public bool HasSpecificWidthAndHeight
        {
            get => _specificHeight && _specificWidth;
            set => _specificHeight = _specificWidth = value;
        }

        public Rectangle Bounds => new Rectangle(this.Left, this.Top, this.Width, this.Height);

        //-----------------------
        //for css interface
        void IBoxElement.ChangeElementSize(int w, int h) => this.SetSize(w, h);
        int IBoxElement.MinHeight => this.Height;
        //for css interface
        //TODO: use mimimum current font height ***  




        //-----------------------
        /// <summary>
        /// mouse beh instant
        /// </summary>
        MouseBehaviorInstanceBase _uiMouseBehInst;

        bool IAcceptBehviour.AttachBehviour(MouseBehaviorInstanceBase inst)
        {
            if (inst == null)
            {
                _uiMouseBehInst = null;
                return true;
            }

            if (_uiMouseBehInst == null)
            {
                _uiMouseBehInst = inst;
                return true;
            }
            else
            {
#if DEBUG
                //please clear the old listener before set a new one.
                //in this version a single AttachUIBehaviour has 1 external event listener
                System.Diagnostics.Debugger.Break();
#endif

                return false;
            }
        }


        //derived class can raise beh manually, 
        //default mode: the attached mouse behavior will be invoke inside a responsible method
        //user (derived class) can disable this but set DisableAutoBehRaising= true
        //then if they want to call it=> call it via RaiseBeh...


        /// <summary>
        /// disable automatic raising event on _uiMouseBehInst
        /// </summary>
        protected bool DisableAutoBehRaising { get; set; }

        protected void RaiseBehMouseDown(UIMouseDownEventArgs e)
        {
            _uiMouseBehInst?.ListenMouseDown(this, e);
        }
        protected void RaiseBehMouseMove(UIMouseMoveEventArgs e)
        {
            _uiMouseBehInst?.ListenMouseMove(this, e);
        }
        protected void RaiseBehMouseUp(UIMouseUpEventArgs e)
        {
            _uiMouseBehInst?.ListenMouseUp(this, e);
        }
        protected void RaiseBehMouseEnter(UIMouseMoveEventArgs e)
        {
            _uiMouseBehInst?.ListenMouseEnter(this, e);
        }
        protected void RaiseBehMouseLeave(UIMouseLeaveEventArgs e)
        {
            _uiMouseBehInst?.ListenMouseLeave(this, e);
        }
        protected void RaiseBehMousePress(UIMousePressEventArgs e)
        {
            _uiMouseBehInst?.ListenMousePress(this, e);
        }
        protected void RaiseBehMouseHover(UIMouseHoverEventArgs e)
        {
            _uiMouseBehInst?.ListenMouseHover(this, e);
        }
        protected void RaiseBehLostMouseFocus(UIMouseLostFocusEventArgs e)
        {
            _uiMouseBehInst?.ListenLostMouseFocus(this, e);
        }


        protected override void OnMouseDown(UIMouseDownEventArgs e)
        {
            if (!DisableAutoBehRaising) RaiseBehMouseDown(e);
        }
        protected override void OnMouseMove(UIMouseMoveEventArgs e)
        {
            if (!DisableAutoBehRaising) RaiseBehMouseMove(e);
        }
        protected override void OnMouseUp(UIMouseUpEventArgs e)
        {
            if (!DisableAutoBehRaising) RaiseBehMouseUp(e);
        }
        protected override void OnMouseEnter(UIMouseMoveEventArgs e)
        {
            if (!DisableAutoBehRaising) RaiseBehMouseEnter(e);
        }
        protected override void OnMouseLeave(UIMouseLeaveEventArgs e)
        {
            if (!DisableAutoBehRaising) RaiseBehMouseLeave(e);
        }
        protected override void OnMousePress(UIMousePressEventArgs e)
        {
            if (!DisableAutoBehRaising) RaiseBehMousePress(e);
        }
        protected override void OnMouseHover(UIMouseHoverEventArgs e)
        {
            if (!DisableAutoBehRaising) RaiseBehMouseHover(e);
        }
        protected override void OnLostMouseFocus(UIMouseLostFocusEventArgs e)
        {
            if (!DisableAutoBehRaising) RaiseBehLostMouseFocus(e);
        }
        protected static void AssignProperties(LayoutFarm.CustomWidgets.CustomRenderBox customRenderE, AbstractRectUI ui)
        {
            customRenderE.SetLocation(ui.Left, ui.Top);
        }

        //--------------
        //layout instance feature
        public LayoutInstance LayoutInstance { get; set; }
        public override void UpdateLayout()
        {
            //update size and bounds from its layout instance
            if (LayoutInstance != null && LayoutInstance.GetResultBounds(out RectangleF bounds))
            {
                SetLocationAndSize((int)bounds.Left, (int)bounds.Top, (int)bounds.Width, (int)bounds.Height);
            }
        }

    }

    public static class UIElementExtensions
    {
        public static void Offset(this AbstractRectUI ui, float dx, float dy)
        {
            ui.SetLocation((int)(ui.Left + dx), (int)(ui.Top + dy));
        }
        public static void SetWidth(this AbstractRectUI ui, int newW)
        {
            ui.SetSize(newW, ui.Height);
        }
        public static void SetHeight(this AbstractRectUI ui, int newH)
        {
            ui.SetSize(ui.Width, newH);
        }
    }



}