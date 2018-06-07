//MIT, 2018, WinterDev

using System;
using System.Collections.Generic;

using PixelFarm.Drawing;
using PixelFarm.Agg;
using LayoutFarm.RenderBoxes;

namespace LayoutFarm.UI
{
    class SvgRenderElement : RenderElement
    {
        public SvgRenderElement(RootGraphic rootGfx, int width, int height)
            : base(rootGfx, width, height)
        {

            //this.dbug_ObjectNote = "AAA";
            //this.NeedClipArea = false;
            this.MayHasChild = true;
            this.TransparentForAllEvents = true;

        }
        public SvgRenderVx RenderVx { get; set; }

        public override void ChildrenHitTestCore(HitChain hitChain)
        {
            RectD bound = RenderVx.GetBounds();

            if (bound.Contains(hitChain.TestPoint.x, hitChain.TestPoint.y))
            {
                //check exact hit or the vxs part
                if (HitTestOnSubPart(RenderVx, hitChain.TextPointX, hitChain.TextPointY))
                {
                    hitChain.AddHitObject(this);
                    return; //return after first hit
                }
            }

            base.ChildrenHitTestCore(hitChain);
        }
        static bool HitTestOnSubPart(SvgRenderVx _svgRenderVx, float x, float y)
        {
            int partCount = _svgRenderVx.SvgVxCount;


            for (int i = partCount - 1; i >= 0; --i)
            {
                //we do hittest top to bottom => (so => iter backward)

                SvgPart vx = _svgRenderVx.GetInnerVx(i);
                if (vx.Kind != SvgRenderVxKind.Path)
                {
                    continue;
                }
                VertexStore innerVxs = vx.GetVxs();
                //fine tune
                //hit test ***
                if (VertexHitTester.IsPointInVxs(innerVxs, x, y))
                {
                    return true;
                }
            }
            return false;
        }
        public override void CustomDrawToThisCanvas(DrawBoard canvas, Rectangle updateArea)
        {
            if (RenderVx != null)
            {
                canvas.DrawRenderVx(RenderVx, this.X, this.Y);
            }
        }
        public override void ResetRootGraphics(RootGraphic rootgfx)
        {

        }
    }

    public class UISprite : UIElement
    {

        SvgRenderElement _svgRenderElement;
        SvgRenderVx _svgRenderVx;

#if DEBUG
        static int dbugTotalId;
        public readonly int dbugId = dbugTotalId++;
#endif
        public UISprite(float width, float height)
        {
            SetElementBoundsWH(width, height);
            this.AutoStopMouseEventPropagation = true;

        }
        public void LoadSvg(SvgRenderVx renderVx)
        {
            _svgRenderVx = renderVx;
            if (_svgRenderElement != null)
            {
                _svgRenderElement.RenderVx = renderVx;
                RectD bound = renderVx.GetBounds();
                this.SetSize((float)bound.Width, (float)bound.Height);
            }
        }
        protected override void OnElementChanged()
        {

            if (_svgRenderElement != null)
            {
                _svgRenderVx.SetBitmapSnapshot(null);

                _svgRenderElement.RenderVx = _svgRenderVx;
                _svgRenderVx.InvalidateBounds();
                RectD bound1 = _svgRenderVx.GetBounds();

            }
        }
        protected override void OnMouseDown(UIMouseEventArgs e)
        {
            base.OnMouseDown(e);
        }
        public override void Walk(UIVisitor visitor)
        {

        }
        protected override bool HasReadyRenderElement
        {
            get { return _svgRenderElement != null; }
        }
        public override RenderElement CurrentPrimaryRenderElement
        {
            get { return _svgRenderElement; }
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (_svgRenderElement == null)
            {
                _svgRenderElement = new SvgRenderElement(rootgfx, 10, 10);
                _svgRenderElement.SetController(this);
                if (_svgRenderVx != null)
                {
                    _svgRenderElement.RenderVx = _svgRenderVx;
                    RectD bound = _svgRenderVx.GetBounds();
                    this.SetSize((int)bound.Width, (int)bound.Height);
                }
            }
            return _svgRenderElement;
        }
        public virtual void SetLocation(float left, float top)
        {
            SetElementBoundsLT(left, top);
            if (this.HasReadyRenderElement)
            {
                //TODO: review rounding here
                this.CurrentPrimaryRenderElement.SetLocation((int)left, (int)top);
            }
        }

        public virtual void SetSize(float width, float height)
        {
            SetElementBoundsWH(width, height);
            if (this.HasReadyRenderElement)
            {
                this.CurrentPrimaryRenderElement.SetSize((int)width, (int)height);
            }
        }

        public void SetBounds(float left, float top, float width, float height)
        {
            SetLocation(left, top);
            SetSize(width, height);
        }
        public float Left
        {
            get
            {
                if (this.HasReadyRenderElement)
                {
                    return this.CurrentPrimaryRenderElement.X;
                }
                else
                {
                    return BoundLeft;
                }
            }
        }
        public float Top
        {
            get
            {
                if (this.HasReadyRenderElement)
                {
                    return this.CurrentPrimaryRenderElement.Y;
                }
                else
                {
                    return BoundTop;
                }
            }
        }
        public float Right
        {
            get { return this.Left + Width; }
        }
        public float Bottom
        {
            get { return this.Top + Height; }
        }

        public float Width
        {
            get
            {
                if (this.HasReadyRenderElement)
                {
                    return this.CurrentPrimaryRenderElement.Width;
                }
                else
                {
                    return BoundWidth;
                }
            }
        }
        public float Height
        {
            get
            {
                if (this.HasReadyRenderElement)
                {
                    return this.CurrentPrimaryRenderElement.Height;
                }
                else
                {
                    return BoundHeight;
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
        public virtual void PerformContentLayout()
        {
        }
        protected virtual void Describe(UIVisitor visitor)
        {
            visitor.Attribute("left", this.Left);
            visitor.Attribute("top", this.Top);
            visitor.Attribute("width", this.Width);
            visitor.Attribute("height", this.Height);
        }


    }
}