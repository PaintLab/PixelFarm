//Apache2, 2014-present, WinterDev

using PixelFarm.Drawing;
using LayoutFarm.RenderBoxes;
namespace LayoutFarm
{
    public enum BoxContentLayoutKind : byte
    {
        Absolute,
        VerticalStack,
        HorizontalStack
    }


#if DEBUG
    [System.Diagnostics.DebuggerDisplay("RenderBoxBase {dbugGetCssBoxInfo}")]
#endif
    public abstract class RenderBoxBase : RenderElement
    {
        int _viewportLeft;
        int _viewportTop;
        protected PlainLayer _defaultLayer;
        protected bool _disableDefaultLayer;

        public RenderBoxBase(RootGraphic rootgfx, int width, int height)
            : base(rootgfx, width, height)
        {
            this.MayHasViewport = true;
            this.MayHasChild = true;
        }
        protected abstract PlainLayer CreateDefaultLayer();
        //
        public bool UseAsFloatWindow { get; set; }
        //
        public override void SetViewport(int viewportLeft, int viewportTop)
        {
            int diffLeft = viewportLeft - _viewportLeft;
            int diffTop = viewportTop - _viewportTop;

            if (diffLeft != 0 || diffTop != 0)
            {
                _viewportLeft = viewportLeft;
                _viewportTop = viewportTop;
                //
                InvalidateGraphicsArgs args = new InvalidateGraphicsArgs();
                args.LeftDiff = diffLeft;
                args.TopDiff = diffTop;
                this.InvalidateGraphics(args);
            }
        }
        //
        public override int ViewportLeft => _viewportLeft;
        public override int ViewportTop => _viewportTop;
        //
        public sealed override void CustomDrawToThisCanvas(DrawBoard canvas, Rectangle updateArea)
        {
            if (this.NeedClipArea)
            {
                if (canvas.PushClipAreaRect(this.Width, this.Height, ref updateArea))
                {
                    if (_viewportLeft == 0 && _viewportTop == 0)
                    {
                        this.DrawBoxContent(canvas, updateArea);
                    }
                    else
                    {
                        canvas.OffsetCanvasOrigin(-_viewportLeft, -_viewportTop);
                        updateArea.Offset(_viewportLeft, _viewportTop);
                        this.DrawBoxContent(canvas, updateArea);
#if DEBUG
                        //for debug
                        // canvas.dbug_DrawCrossRect(Color.Red,updateArea);
#endif
                        canvas.OffsetCanvasOrigin(_viewportLeft, _viewportTop);
                        updateArea.Offset(-_viewportLeft, -_viewportTop);
                    }
                }
                canvas.PopClipAreaRect();
            }
            else
            {
                if (_viewportLeft == 0 && _viewportTop == 0)
                {
                    this.DrawBoxContent(canvas, updateArea);
                }
                else
                {
                    canvas.OffsetCanvasOrigin(-_viewportLeft, -_viewportTop);
                    updateArea.Offset(_viewportLeft, _viewportTop);
                    this.DrawBoxContent(canvas, updateArea);
#if DEBUG
                    //for debug
                    // canvas.dbug_DrawCrossRect(Color.Red,updateArea);
#endif
                    canvas.OffsetCanvasOrigin(_viewportLeft, _viewportTop);
                    updateArea.Offset(-_viewportLeft, -_viewportTop);
                }

            }
        }

        public override void ChildrenHitTestCore(HitChain hitChain)
        {
            if (_defaultLayer != null)
            {
                _defaultLayer.HitTestCore(hitChain);
#if DEBUG
                debug_RecordLayerInfo(_defaultLayer);
#endif
            }
        }

        public override sealed void TopDownReCalculateContentSize()
        {
            if (!ForceReArrange && this.HasCalculatedSize)
            {
                return;
            }
#if DEBUG
            dbug_EnterTopDownReCalculateContent(this);
#endif
            int cHeight = this.Height;
            int cWidth = this.Width;
            Size ground_contentSize = Size.Empty;
            if (_defaultLayer != null)
            {
                _defaultLayer.TopDownReCalculateContentSize();
                ground_contentSize = _defaultLayer.PostCalculateContentSize;
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
                if (_defaultLayer != null)
                {
                    foreach (var r in _defaultLayer.GetRenderElementIter())
                    {
                        r.ResetRootGraphics(rootgfx);
                    }
                }
            }
        }

        public override void AddChild(RenderElement renderE)
        {
            if (_disableDefaultLayer) return;

            if (_defaultLayer == null)
            {
                _defaultLayer = CreateDefaultLayer();
            }
            _defaultLayer.AddChild(renderE);
        }
        public override void AddFirst(RenderElement renderE)
        {
            if (_disableDefaultLayer) return;

            if (_defaultLayer == null)
            {
                _defaultLayer = CreateDefaultLayer();
            }
            _defaultLayer.AddFirst(renderE);
        }

        public override void InsertAfter(RenderElement afterElem, RenderElement renderE)
        {
            _defaultLayer.InsertChildAfter(afterElem, renderE);
        }
        public override void InsertBefore(RenderElement beforeElem, RenderElement renderE)
        {
            _defaultLayer.InsertChildBefore(beforeElem, renderE);
        }
        public override void RemoveChild(RenderElement renderE)
        {
            _defaultLayer?.RemoveChild(renderE);
        }
        public override void ClearAllChildren()
        {
            _defaultLayer?.Clear();
        }

        public override RenderElement FindUnderlyingSiblingAtPoint(Point point)
        {
            if (this.MyParentLink != null)
            {
                return this.MyParentLink.FindOverlapedChildElementAtPoint(this, point);
            }

            return null;
        }

        public override Size InnerContentSize
        {
            get
            {
                if (_defaultLayer != null)
                {
                    Size s1 = _defaultLayer.PostCalculateContentSize;
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

        protected abstract void DrawBoxContent(DrawBoard canvas, Rectangle updateArea);
        //
        protected bool HasDefaultLayer => _defaultLayer != null;

        protected void DrawDefaultLayer(DrawBoard canvas, ref Rectangle updateArea)
        {
            if (_defaultLayer != null)
            {
#if DEBUG
                if (!debugBreaK1)
                {
                    debugBreaK1 = true;
                }
#endif
                _defaultLayer.DrawChildContent(canvas, updateArea);
            }
        }

#if DEBUG
        public bool debugDefaultLayerHasChild
        {
            get
            {
                return _defaultLayer != null && _defaultLayer.dbugChildCount > 0;
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
            //IsInTopDownReArrangePhase = true;
            if (_defaultLayer != null)
            {
                _defaultLayer.TopDownReArrangeContent();
            }

            // BoxEvaluateScrollBar();


            this.dbug_FinishArr++;
            debug_PopTopDownElement(this);
            dbug_ExitReArrangeContent();
        }
        public void dbugTopDownReArrangeContentIfNeed()
        {
            bool isIncr = false;
            if (!ForceReArrange && !this.NeedContentArrangement)
            {
                if (!this.FirstArrangementPass)
                {
                    this.FirstArrangementPass = true;
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
        void debug_RecordLayerInfo(RenderElementLayer layer)
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
