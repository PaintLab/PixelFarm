//Apache2, 2014-present, WinterDev

using System;
using PixelFarm.Drawing;


namespace LayoutFarm.UI
{

    /// <summary>
    /// abstract Rect UI Element
    /// </summary>
    public abstract class AbstractRectUI : UIElement, IScrollable, IBoxElement
    {
        protected enum PaddingName
        {
            Left,
            Top,
            Right,
            Bottom,
            AllSideSameValue,
            AllSide
        }
        protected enum MarginName
        {
            Left,
            Top,
            Right,
            Bottom,
            AllSideSameValue,
            AllSide
        }
        protected enum BorderName
        {
            Left,
            Top,
            Right,
            Bottom,
            AllSideSameValue,
            AllSide
        }
        bool _specificWidth;
        bool _specificHeight;


        byte _paddingLeft;
        byte _paddingTop;
        byte _paddingRight;
        byte _paddingBottom;
        //
        byte _borderLeft;
        byte _borderTop;
        byte _borderRight;
        byte _borderBottom;
        //
        short _marginLeft;
        short _marginTop;
        short _marginRight;
        short _marginBottom;
        // 
#if DEBUG
        static int dbugTotalId;
        public readonly int dbugId = dbugTotalId++;
#endif
        public AbstractRectUI(int width, int height)
        {
            SetElementBoundsWH(width, height);
            //default for box
            this.AutoStopMouseEventPropagation = true;
        }

        public event EventHandler LayoutFinished;
        public event EventHandler ViewportChanged;
        protected virtual void RaiseViewportChanged()
        {
            ViewportChanged?.Invoke(this, EventArgs.Empty);
        }
        protected void RaiseLayoutFinished()
        {
            LayoutFinished?.Invoke(this, EventArgs.Empty);

        }
        public virtual void SetFont(RequestFont font)
        {

        }
        public virtual void SetLocation(int left, int top)
        {
            SetElementBoundsLT(left, top);
            if (this.HasReadyRenderElement)
            {
                //TODO: review here
                this.CurrentPrimaryRenderElement.SetLocation(left, top);
            }
        }


        /// <summary>
        /// set visual size (or viewport size) of this rect
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public virtual void SetSize(int width, int height)
        {
            SetElementBoundsWH(width, height);
            if (this.HasReadyRenderElement)
            {
                this.CurrentPrimaryRenderElement.SetSize(width, height);
            }
        }
        /// <summary>
        /// set location and visual size of this rect
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetLocationAndSize(int left, int top, int width, int height)
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
        protected virtual void InvalidatePadding(PaddingName paddingName, byte newValue)
        {
        }
        public byte PaddingLeft
        {
            get => _paddingLeft;
            set => InvalidatePadding(PaddingName.Left, _paddingLeft = value);
        }
        public byte PaddingTop
        {
            get => _paddingTop;
            set => InvalidatePadding(PaddingName.Top, _paddingTop = value);
        }
        public byte PaddingRight
        {
            get => _paddingRight;
            set => InvalidatePadding(PaddingName.Right, _paddingRight = value);
        }
        public byte PaddingBottom
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
        protected virtual void InvalidateMargin(MarginName marginName, short newValue)
        {
        }
        public short MarginLeft
        {
            get => _marginLeft;
            set => InvalidateMargin(MarginName.Left, _marginLeft = value);
        }
        public short MarginTop
        {
            get => _marginTop;
            set => InvalidateMargin(MarginName.Top, _marginTop = value);
        }
        public short MarginRight
        {
            get => _marginRight;
            set => InvalidateMargin(MarginName.Right, _marginRight = value);
        }
        public short MarginBottom
        {
            get => _marginBottom;
            set => InvalidateMargin(MarginName.Bottom, _marginBottom = value);
        }
        public int MarginLeftRight => _marginLeft + _marginRight;
        public int MarginTopBottom => _marginTop + _marginBottom;

        public void SetMargins(byte left, byte top, byte right, byte bottom)
        {
            _marginLeft = left;
            _marginTop = top;
            _marginRight = right;
            _marginBottom = bottom;
            InvalidateMargin(MarginName.AllSide, 0);
        }
        public void SetMargins(short sameValue)
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
            if (this.HasReadyRenderElement)
            {
                //invalidate 'bubble' rect 
                //is (0,0,w,h) start invalidate from current primary render element
                this.CurrentPrimaryRenderElement.InvalidateGraphics();
            }
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

        //------------------------------
        public virtual void PerformContentLayout()
        {
            //AbstractRect dose not have content
        }
        //
        public virtual int InnerHeight => this.Height;
        //
        public virtual int InnerWidth => this.Width;
        //
        protected virtual void Describe(UIVisitor visitor)
        {
            visitor.Attribute("left", this.Left);
            visitor.Attribute("top", this.Top);
            visitor.Attribute("width", this.Width);
            visitor.Attribute("height", this.Height);
        }


        public bool HasSpecificWidth
        {
            get => _specificWidth;
            set
            {
                _specificWidth = value;
                if (this.CurrentPrimaryRenderElement != null)
                {
                    CurrentPrimaryRenderElement.HasSpecificWidth = value;
                }
            }
        }
        public bool HasSpecificHeight
        {
            get => _specificHeight;
            set
            {
                _specificHeight = value;
                if (this.CurrentPrimaryRenderElement != null)
                {
                    CurrentPrimaryRenderElement.HasSpecificHeight = value;
                }
            }
        }
        public bool HasSpecificWidthAndHeight
        {
            get => _specificHeight && _specificWidth;
            set
            {
                _specificHeight = _specificWidth = value;

                if (this.CurrentPrimaryRenderElement != null)
                {
                    CurrentPrimaryRenderElement.HasSpecificHeight = value;
                    CurrentPrimaryRenderElement.HasSpecificWidth = value;
                }
            }
        }
        public override void Walk(UIVisitor visitor)
        {

        }
        public Rectangle Bounds => new Rectangle(this.Left, this.Top, this.Width, this.Height);

        //-----------------------
        //for css interface
        void IBoxElement.ChangeElementSize(int w, int h) => this.SetSize(w, h);
        int IBoxElement.MinHeight => this.Height;
        //for css interface
        //TODO: use mimimum current font height *** 
    }

    public static class UIElementExtensions
    {
        public static void Offset(this AbstractRectUI ui, float dx, float dy)
        {
            ui.SetLocation((int)(ui.Left + dx), (int)(ui.Top + dy));
        }
    }
}