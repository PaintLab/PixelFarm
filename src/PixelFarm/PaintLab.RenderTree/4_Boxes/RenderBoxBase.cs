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
    public enum RectUIAlignment : byte
    {
        Begin, //left, if host is horizontal stack ,or top if host is vertical stack
        Middle,
        End, //right, if host is horizontal stack ,or bottom if host is vertical stack
    }

    public interface IContainerRenderElement
    {
        void AddChild(RenderElement renderE);
        void AddFirst(RenderElement renderE);
        void InsertAfter(RenderElement afterElem, RenderElement renderE);
        void InsertBefore(RenderElement beforeElem, RenderElement renderE);
        void RemoveChild(RenderElement renderE);
        void ClearAllChildren();

    }

    public static class RenderElemHelper
    {

        public static void DrawChildContent(HitTestHint hitTestHint, IEnumerable<RenderElement> drawingIter, DrawBoard d, UpdateArea updateArea)
        {
            int enter_canvas_x = d.OriginX;
            int enter_canvas_y = d.OriginY;

            switch (hitTestHint)
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
                case HitTestHint.HorizontalRowNonOverlap:
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
                case HitTestHint.VerticalColumnNonOverlap:
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

        public static bool HitTestCore(HitChain hitTestHint, HitTestHint layoutHint, IEnumerable<RenderElement> hitTestIter)
        {
            switch (layoutHint)
            {
                default:
                    {
                        foreach (RenderElement renderE in hitTestIter)
                        {
                            if (renderE.HitTestCore(hitTestHint))
                            {
                                return true;
                            }
                        }
                    }
                    return false;
                case HitTestHint.HorizontalRowNonOverlap:
                    {
                        foreach (RenderElement renderE in hitTestIter)
                        {

                            if (renderE.HitTestCore(hitTestHint))
                            {
                                return true;
                            }
                            else if (renderE.Right < hitTestHint.TestPointX)
                            {
                                //hitTestIter iterates from right to left
                                //so in this case (eg. we have whitespace between each elem)
                                //this should be stop

                                return false;
                            }
                        }

                    }
                    return false;
                case HitTestHint.VerticalColumnNonOverlap:
                    {
                        foreach (RenderElement renderE in hitTestIter)
                        {


                            if (renderE.HitTestCore(hitTestHint))
                            {
                                return true;
                            }
                            else if (renderE.Bottom < hitTestHint.TestPointY)
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

        public AbstractRectRenderElement(int width, int height)
             : base(width, height)
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
                if (!this.BlockGraphicUpdateBubble)
                {
                    InvalidateGfxArgs args = BubbleInvalidater.GetInvalidateGfxArgs();
                    args.SetReason_ChangeViewport(this, diffLeft, diffTop);
                    this.InvalidateGraphics(args);
                }
            }
        }

    } 

    //#if DEBUG
    //    [System.Diagnostics.DebuggerDisplay("RenderBoxBase")]
    //#endif
    public abstract class RenderBoxBase : AbstractRectRenderElement, IContainerRenderElement
    {

        RenderElementCollection _elements;
        public RenderBoxBase(int width, int height)
            : base(width, height)
        {
            this.MayHasChild = true;
        }

        public override void ChildrenHitTestCore(HitChain hitChain)
        {
            if (_elements != null)
            {
                RenderElemHelper.HitTestCore(hitChain, ContentHitTestHint, _elements.GetHitTestIter());
#if DEBUG
                debug_RecordLayerInfo(_elements.dbugGetLayerInfo());
#endif
            }
        } 


        public virtual void AddChild(RenderElement renderE)
        {
            if (_elements == null)
            {
                _elements = new RenderElementCollection();
            }

            _elements.AddChild(this, renderE);
        }
        public virtual void AddFirst(RenderElement renderE)
        {
            if (_elements == null)
            {
                _elements = new RenderElementCollection();
            }

            _elements.AddFirst(this, renderE);
        }

        public virtual void InsertAfter(RenderElement afterElem, RenderElement renderE)
        {
            _elements.InsertChildAfter(this, afterElem, renderE);
        }
        public virtual void InsertBefore(RenderElement beforeElem, RenderElement renderE)
        {
            _elements.InsertChildBefore(this, beforeElem, renderE);
        }
        public virtual void RemoveChild(RenderElement renderE)
        {
            _elements?.RemoveChild(this, renderE);
        }
        public virtual void ClearAllChildren()
        {
            _elements?.Clear(this);

        }

        //TODO: review inner content size again

        public override Size InnerContentSize => this.Size; 

        internal RenderElementCollection GetElemCollection() => _elements;
        protected IEnumerable<RenderElement> GetDrawingIter() => _elements?.GetDrawingIter();

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
                    ContentHitTestHint,
                    _elements.GetDrawingIter(),
                    d, updateArea);
            }
        }


        public HitTestHint ContentHitTestHint { get; set; }

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
            //dbug_EnterReArrangeContent(this);
            //dbug_topDownReArrContentPass++;
            //this.dbug_BeginArr++;
            //debug_PushTopDownElement(this);
            //this.MarkValidContentArrangement();
            //////IsInTopDownReArrangePhase = true;
            ////if (_defaultLayer != null)
            ////{
            ////    _defaultLayer.TopDownReArrangeContent();
            ////}

            //// BoxEvaluateScrollBar();


            //this.dbug_FinishArr++;
            //debug_PopTopDownElement(this);
            //dbug_ExitReArrangeContent();
        }
        public void dbugTopDownReArrangeContentIfNeed()
        {
            //bool isIncr = false;
            //if (!this.NeedContentArrangement)
            //{
            //    if (!this.dbugFirstArrangementPass)
            //    {
            //        this.dbugFirstArrangementPass = true;
            //        dbug_WriteInfo(dbugVisitorMessage.PASS_FIRST_ARR);
            //    }
            //    else
            //    {
            //        isIncr = true;
            //        this.dbugVRoot.dbugNotNeedArrCount++;
            //        this.dbugVRoot.dbugNotNeedArrCountEpisode++;
            //        dbug_WriteInfo(dbugVisitorMessage.NOT_NEED_ARR);
            //        this.dbugVRoot.dbugNotNeedArrCount--;
            //    }
            //    return;
            //}

            //dbugForceTopDownReArrangeContent();
            //if (isIncr)
            //{
            //    this.dbugVRoot.dbugNotNeedArrCount--;
            //}
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
