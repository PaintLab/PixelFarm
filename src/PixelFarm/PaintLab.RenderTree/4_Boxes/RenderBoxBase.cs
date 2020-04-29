//Apache2, 2014-present, WinterDev

using PixelFarm.Drawing;
using LayoutFarm.RenderBoxes;

using System.Collections.Generic;


namespace LayoutFarm
{
    public enum BoxContentLayoutKind : byte
    {
        Absolute,
        VerticalStack,
        HorizontalStack,
        HorizontalFlow,
    }
    public enum VerticalAlignment : byte
    {
        Top,
        Middle,
        Bottom,
        UserSpecific
    }

    public interface IContainerRenderElement
    {
        void AddChild(RenderElement renderE);
        void AddFirst(RenderElement renderE);
        void InsertAfter(RenderElement afterElem, RenderElement renderE);
        void InsertBefore(RenderElement beforeElem, RenderElement renderE);
        void RemoveChild(RenderElement renderE);
        void ClearAllChildren();
        RootGraphic Root { get; }
    }

    static class RenderElemHelper
    {

        public static void DrawChildContent(LayoutHint layoutHint, IEnumerable<RenderElement> drawingIter, DrawBoard d, UpdateArea updateArea)
        {
            int enter_canvas_x = d.OriginX;
            int enter_canvas_y = d.OriginY;

            switch (layoutHint)
            {
                default:
                    {
                        foreach (RenderElement child in drawingIter)
                        {
                            if (child.IntersectsWith(updateArea) ||
                               !child.NeedClipArea)
                            {
                                //if the child not need clip
                                //its children (if exist) may intersect 
                                int x = child.X;
                                int y = child.Y;

                                d.SetCanvasOrigin(enter_canvas_x + x, enter_canvas_y + y);
                                updateArea.Offset(-x, -y);
                                RenderElement.Render(child, d, updateArea);
                                updateArea.Offset(x, y);
                            }
                        }

                        //restore
                        d.SetCanvasOrigin(enter_canvas_x, enter_canvas_y);
                    }
                    break;
                case LayoutHint.HorizontalRowNonOverlap:
                    {
                        bool found = false;
                        foreach (RenderElement child in drawingIter)
                        {
                            if (child.IntersectsWith(updateArea))
                            {
                                found = true;
                                //if the child not need clip
                                //its children (if exist) may intersect 
                                int x = child.X;
                                int y = child.Y;

                                d.SetCanvasOrigin(enter_canvas_x + x, enter_canvas_y + y);
                                updateArea.Offset(-x, -y);
                                RenderElement.Render(child, d, updateArea);
                                updateArea.Offset(x, y);
                            }
                            else if (found)
                            {
                                break;
                            }
                        }

                        //restore
                        d.SetCanvasOrigin(enter_canvas_x, enter_canvas_y);
                    }
                    break;
                case LayoutHint.VerticalColumnNonOverlap:
                    {
                        bool found = false;
                        foreach (RenderElement child in drawingIter)
                        {
                            if (child.IntersectsWith(updateArea))
                            {
                                found = true;
                                //if the child not need clip
                                //its children (if exist) may intersect 
                                int x = child.X;
                                int y = child.Y;

                                d.SetCanvasOrigin(enter_canvas_x + x, enter_canvas_y + y);
                                updateArea.Offset(-x, -y);
                                RenderElement.Render(child, d, updateArea);
                                updateArea.Offset(x, y);
                            }
                            else if (found)
                            {
                                break;
                            }
                        }
                        d.SetCanvasOrigin(enter_canvas_x, enter_canvas_y);
                    }
                    break;
            }
        }

        public static bool HitTestCore(HitChain hitChain, LayoutHint layoutHint, IEnumerable<RenderElement> hitTestIter)
        {
            switch (layoutHint)
            {
                default:
                    {
                        foreach (RenderElement renderE in hitTestIter)
                        {
                            if (renderE.HitTestCore(hitChain))
                            {
                                return true;
                            }
                        }
                    }
                    return false;
                case LayoutHint.HorizontalRowNonOverlap:
                    {
                        foreach (RenderElement renderE in hitTestIter)
                        {

                            if (renderE.HitTestCore(hitChain))
                            {
                                return true;
                            }
                            else if (renderE.Right < hitChain.TestPointX)
                            {
                                //hitTestIter iterates from right to left
                                //so in this case (eg. we have whitespace between each elem)
                                //this should be stop

                                return false;
                            }
                        }

                    }
                    return false;
                case LayoutHint.VerticalColumnNonOverlap:
                    {
                        foreach (RenderElement renderE in hitTestIter)
                        {


                            if (renderE.HitTestCore(hitChain))
                            {
                                return true;
                            }
                            else if (renderE.Bottom < hitChain.TestPointY)
                            {
                                //hitTestIter iterates from bottom to top
                                //so in this case (eg. we have whitespace between each elem)
                                //this should be stop
                                return false;
                            }
                        }
                    }
                    return false;
            }
        }
    }

    public abstract class AbstractRectRenderElement : RenderElement
    {
        protected int _viewportLeft;
        protected int _viewportTop;

        public AbstractRectRenderElement(RootGraphic rootgfx, int width, int height)
             : base(rootgfx, width, height)
        {
            this.MayHasViewport = true;
        }
        public override sealed int ViewportLeft => _viewportLeft;
        public override sealed int ViewportTop => _viewportTop;
        public override sealed void SetViewport(int viewportLeft, int viewportTop)
        {
            int diffLeft = viewportLeft - _viewportLeft;
            int diffTop = viewportTop - _viewportTop;

            if (diffLeft != 0 || diffTop != 0)
            {
                _viewportLeft = viewportLeft;
                _viewportTop = viewportTop;
                //

                InvalidateGfxArgs args = RootGetInvalidateGfxArgs();
                args.SetReason_ChangeViewport(this, diffLeft, diffTop);
                this.InvalidateGraphics(args);
            }
        }
    }
     

#if DEBUG
    [System.Diagnostics.DebuggerDisplay("RenderBoxBase {dbugGetCssBoxInfo}")]
#endif
    public abstract class RenderBoxBase : AbstractRectRenderElement, IContainerRenderElement
    {

        RenderElementCollection _elements;
        bool _layoutValid;

        public RenderBoxBase(RootGraphic rootgfx, int width, int height)
            : base(rootgfx, width, height)
        {
            this.MayHasChild = true;
        }

        public override void ChildrenHitTestCore(HitChain hitChain)
        {
            if (_elements != null)
            {
                RenderElemHelper.HitTestCore(hitChain, ContentLayoutHint, _elements.GetHitTestIter());
#if DEBUG
                debug_RecordLayerInfo(_elements.dbugGetLayerInfo());
#endif
            }
        }

        public override sealed void TopDownReCalculateContentSize()
        {
            //if (!ForceReArrange && this.HasCalculatedSize)
            //{
            //    return;
            //}
#if DEBUG
            dbug_EnterTopDownReCalculateContent(this);
#endif
            int cHeight = this.Height;
            int cWidth = this.Width;
            Size ground_contentSize = Size.Empty;
            if (_elements != null)
            {
                _elements.TopDownReCalculateContentSize();
                ground_contentSize = _elements.CalculatedContentSize;
            }
            int finalWidth = ground_contentSize.Width;
            if (finalWidth == 0)
            {
                finalWidth = this.Width;
            }
            int finalHeight = ground_contentSize.Height;
            if (finalHeight == 0)
            {
                finalHeight = this.Height;
            }
            switch (GetLayoutSpecificDimensionType(this))
            {
                case RenderElementConst.LY_HAS_SPC_HEIGHT:
                    {
                        finalHeight = cHeight;
                    }
                    break;
                case RenderElementConst.LY_HAS_SPC_WIDTH:
                    {
                        finalWidth = cWidth;
                    }
                    break;
                case RenderElementConst.LY_HAS_SPC_SIZE:
                    {
                        finalWidth = cWidth;
                        finalHeight = cHeight;
                    }
                    break;
            }


            SetCalculatedSize(this, finalWidth, finalHeight);
#if DEBUG
            dbug_ExitTopDownReCalculateContent(this);
#endif

        }
        public override void ResetRootGraphics(RootGraphic rootgfx)
        {
            if (this.Root != rootgfx)
            {
                DirectSetRootGraphics(this, rootgfx);
                if (_elements != null)
                {
                    foreach (var r in _elements.GetRenderElementIter())
                    {
                        r.ResetRootGraphics(rootgfx);
                    }
                }
            }
        }

        public virtual void AddChild(RenderElement renderE)
        {
            if (_elements == null)
            {
                _elements = new RenderElementCollection();
            }
            _layoutValid = false;
            _elements.AddChild(this, renderE);
        }
        public virtual void AddFirst(RenderElement renderE)
        {
            if (_elements == null)
            {
                _elements = new RenderElementCollection();
            }
            _layoutValid = false;
            _elements.AddFirst(this, renderE);
        }

        public virtual void InsertAfter(RenderElement afterElem, RenderElement renderE)
        {
            _layoutValid = false;
            _elements.InsertChildAfter(this, afterElem, renderE);
        }
        public virtual void InsertBefore(RenderElement beforeElem, RenderElement renderE)
        {
            _layoutValid = false;
            _elements.InsertChildBefore(this, beforeElem, renderE);
        }
        public virtual void RemoveChild(RenderElement renderE)
        {
            _layoutValid = false;
            _elements?.RemoveChild(this, renderE);
        }
        public virtual void ClearAllChildren()
        {
            _layoutValid = false;
            _elements?.Clear();
            this.InvalidateGraphics();
        }

        public override RenderElement FindUnderlyingSiblingAtPoint(Point point)
        {
            return this.MyParentLink?.FindOverlapedChildElementAtPoint(this, point);
        }

        public override Size InnerContentSize
        {
            get
            {
                if (_elements != null)
                {
                    Size s1 = _elements.CalculatedContentSize;
                    int s1_w = s1.Width;
                    int s1_h = s1.Height;

                    if (s1_w < this.Width)
                    {
                        s1_w = this.Width;
                    }
                    if (s1_h < this.Height)
                    {
                        s1_h = this.Height;
                    }
                    return new Size(s1_w, s1_h);
                }
                else
                {
                    return this.Size;
                }
            }
        }

        internal RenderElementCollection GetElemCollection() => _elements;

        protected override void RenderClientContent(DrawBoard d, UpdateArea updateArea)
        {
            if (_elements != null)
            {
#if DEBUG
                if (!debugBreaK1)
                {
                    debugBreaK1 = true;
                }
#endif

                //***                

                RenderElemHelper.DrawChildContent(
                    ContentLayoutHint,
                    _elements.GetDrawingIter(),
                    d, updateArea);
            }
        }


        public LayoutHint ContentLayoutHint { get; set; }

#if DEBUG
        public bool debugDefaultLayerHasChild
        {
            get
            {
                return _elements != null && _elements.dbugChildCount > 0;
            }
        }

        public static bool debugBreaK1;
        //-----------------------------------------------------------------
        public void dbugForceTopDownReArrangeContent()
        {
            dbug_EnterReArrangeContent(this);
            dbug_topDownReArrContentPass++;
            this.dbug_BeginArr++;
            debug_PushTopDownElement(this);
            this.MarkValidContentArrangement();
            ////IsInTopDownReArrangePhase = true;
            //if (_defaultLayer != null)
            //{
            //    _defaultLayer.TopDownReArrangeContent();
            //}

            // BoxEvaluateScrollBar();


            this.dbug_FinishArr++;
            debug_PopTopDownElement(this);
            dbug_ExitReArrangeContent();
        }
        public void dbugTopDownReArrangeContentIfNeed()
        {
            bool isIncr = false;
            if (!this.NeedContentArrangement)
            {
                if (!this.dbugFirstArrangementPass)
                {
                    this.dbugFirstArrangementPass = true;
                    dbug_WriteInfo(dbugVisitorMessage.PASS_FIRST_ARR);
                }
                else
                {
                    isIncr = true;
                    this.dbugVRoot.dbugNotNeedArrCount++;
                    this.dbugVRoot.dbugNotNeedArrCountEpisode++;
                    dbug_WriteInfo(dbugVisitorMessage.NOT_NEED_ARR);
                    this.dbugVRoot.dbugNotNeedArrCount--;
                }
                return;
            }

            dbugForceTopDownReArrangeContent();
            if (isIncr)
            {
                this.dbugVRoot.dbugNotNeedArrCount--;
            }
        }
        public override void dbug_DumpVisualProps(dbugLayoutMsgWriter writer)
        {
            base.dbug_DumpVisualProps(writer);
            writer.EnterNewLevel();
            writer.LeaveCurrentLevel();
        }
        void debug_RecordLayerInfo(dbugLayoutInfo layer)
        {
            RootGraphic visualroot = RootGraphic.dbugCurrentGlobalVRoot;
            if (visualroot.dbug_RecordDrawingChain)
            {
                visualroot.dbug_AddDrawLayer(layer);
            }
        }
        static int dbug_topDownReArrContentPass = 0;
#endif

    }

}
