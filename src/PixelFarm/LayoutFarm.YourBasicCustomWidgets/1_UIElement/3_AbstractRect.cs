//Apache2, 2014-2018, WinterDev

using System;
using PixelFarm.Drawing;
namespace LayoutFarm.UI
{
    /// <summary>
    /// abstract Rect UI Element
    /// </summary>
    public abstract class AbstractRect : UIElement, IScrollable, IBoxElement
    {

        bool specificWidth, specificHeight;
        public event EventHandler LayoutFinished;
        public event EventHandler ViewportChanged;

#if DEBUG
        static int dbugTotalId;
        public readonly int dbugId = dbugTotalId++;
#endif
        public AbstractRect(int width, int height)
        {
            SetElementBoundsWH(width, height);
            //default for box
            this.AutoStopMouseEventPropagation = true;
        }

        protected void RaiseViewportChanged()
        {
            if (ViewportChanged != null)
            {
                ViewportChanged(this, EventArgs.Empty);
            }
        }
        protected void RaiseLayoutFinished()
        {
            if (this.LayoutFinished != null)
            {
                this.LayoutFinished(this, EventArgs.Empty);
            }
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

        public virtual void SetSize(int width, int height)
        {

            SetElementBoundsWH(width, height);
            if (this.HasReadyRenderElement)
            {
                this.CurrentPrimaryRenderElement.SetSize(width, height);
            }
        }

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
        public int Right
        {
            get { return this.Left + Width; }
        }
        public int Bottom
        {
            get { return this.Top + Height; }
        }

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

        public override void InvalidateGraphics()
        {
            if (this.HasReadyRenderElement)
            {
                this.CurrentPrimaryRenderElement.InvalidateGraphics();
            }
        }
        public void InvalidateOuterGraphics()
        {
            if (this.CurrentPrimaryRenderElement != null)
            {
                this.CurrentPrimaryRenderElement.InvalidateGraphicBounds();
            }
        }
        public override void GetViewport(out int x, out int y)
        {
            x = ViewportX;
            y = ViewportY;
        }
        public virtual int ViewportX
        {
            get { return 0; }
        }
        public virtual int ViewportY
        {
            get { return 0; }
        }
        int IScrollable.ViewportWidth
        {
            get { return this.Width; }
        }
        int IScrollable.ViewportHeight
        {
            get { return this.Height; }
        }
        public virtual void SetViewport(int x, int y, object reqBy)
        {
        }
        public void SetViewport(int x, int y)
        {
            SetViewport(x, y, this);
        }
        //------------------------------
        public virtual void PerformContentLayout()
        {
        }

        public virtual int DesiredHeight
        {
            get
            {
                return this.Height;
            }
        }
        public virtual int DesiredWidth
        {
            get
            {
                return this.Width;
            }
        }


        protected virtual void Describe(UIVisitor visitor)
        {
            visitor.Attribute("left", this.Left);
            visitor.Attribute("top", this.Top);
            visitor.Attribute("width", this.Width);
            visitor.Attribute("height", this.Height);
        }


        public bool HasSpecificWidth
        {
            get { return this.specificWidth; }
            set
            {
                this.specificWidth = value;
                if (this.CurrentPrimaryRenderElement != null)
                {
                    CurrentPrimaryRenderElement.HasSpecificWidth = value;
                }
            }
        }
        public bool HasSpecificHeight
        {
            get { return this.specificHeight; }
            set
            {
                this.specificHeight = value;
                if (this.CurrentPrimaryRenderElement != null)
                {
                    CurrentPrimaryRenderElement.HasSpecificHeight = value;
                }
            }
        }
        public bool HasSpecificSize
        {
            get { return this.specificHeight && specificWidth; }
            set
            {
                this.specificHeight = this.specificWidth = value;

                if (this.CurrentPrimaryRenderElement != null)
                {
                    CurrentPrimaryRenderElement.HasSpecificHeight = value;
                    CurrentPrimaryRenderElement.HasSpecificWidth = value;
                }
            }
        }
        public Rectangle Bounds
        {
            get { return new Rectangle(this.Left, this.Top, this.Width, this.Height); }
        }

        //-----------------------
        void IBoxElement.ChangeElementSize(int w, int h)
        {
            //for css interface
            this.SetSize(w, h);
        }
        int IBoxElement.MinHeight
        {
            get
            {
                //for css interface
                //TODO: use mimimum current font height ***
                return this.Height;
            }
        }
    }
}