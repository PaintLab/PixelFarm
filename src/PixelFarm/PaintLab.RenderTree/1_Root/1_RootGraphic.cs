//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;

using PixelFarm.Drawing;
using LayoutFarm.RenderBoxes;

namespace LayoutFarm
{
    public interface IRenderElement
    {
        void Render(DrawBoard d, UpdateArea updateArea);
#if DEBUG
        void dbugShowRenderPart(DrawBoard d, UpdateArea r);
#endif
    }

    public static class GlobalRootGraphic
    {
        //TODO: merge this to RootGraphics?

        static int _suspendCount;
        internal static bool SuspendGraphicsUpdate;

        public static RootGraphic CurrentRootGfx;
        public static RenderElement CurrentRenderElement;

        static ITextService _textServices;
        public static ITextService TextService
        {
            get => _textServices;
            set
            {
#if DEBUG
                if (_textServices != null)
                {

                }
#endif
                _textServices = value;

            }
        }
        public static void BlockGraphicsUpdate()
        {
            _suspendCount++;
            SuspendGraphicsUpdate = true;
        }
        public static void ReleaseGraphicsUpdate()
        {
            _suspendCount--;
            SuspendGraphicsUpdate = _suspendCount > 0;
        }
        public static void ForceResumeGraphicsUpdate()
        {
            _suspendCount = 0;
            SuspendGraphicsUpdate = false;
        }
    }

    static class BubbleInvalidater
    {

        static readonly SimplePool<InvalidateGfxArgs> _invGfxPool = new SimplePool<InvalidateGfxArgs>(() => new InvalidateGfxArgs(), a => a.Reset());


        public static InvalidateGfxArgs GetInvalidateGfxArgs()
        {
#if DEBUG
            InvalidateGfxArgs invGfx = _invGfxPool.Borrow();
            invGfx.dbugWaitingInPool = false;
            return invGfx;
#else

            return _invGfxPool.Borrow();
#endif
        }
        public static void ReleaseInvalidateGfxArgs(InvalidateGfxArgs args)
        {
#if DEBUG
            if (args.dbugWaitingInPool)
            {
                //
                throw new NotSupportedException();
            }
            args.dbugWaitingInPool = true;
#endif
            _invGfxPool.ReleaseBack(args);
        }

        public static void InvalidateGraphicLocalArea(RenderElement re, Rectangle localArea)
        {
            //RELATIVE to re ***

            if (localArea.Height == 0 || localArea.Width == 0)
            {
                return;
            }

            re._propFlags &= ~RenderElementConst.IS_GRAPHIC_VALID;
            InvalidateGfxArgs inv = GetInvalidateGfxArgs();
            inv.SetReason_UpdateLocalArea(re, localArea);
            InternalBubbleUpInvalidateGraphicArea(inv);
        }


        public static void InternalBubbleUpInvalidateGraphicArea(InvalidateGfxArgs args)//RenderElement fromElement, ref Rectangle elemClientRect, bool passSourceElem)
        {
            //total bounds = total bounds at level            
            //if (this.IsInRenderPhase)
            //{
            //    ReleaseInvalidateGfxArgs(args);
            //    return;
            //}
            //--------------------------------------            
            bool hasviewportOffset = args.Reason == InvalidateReason.ViewportChanged;
            int viewport_diffLeft = args.LeftDiff;
            int viewport_diffTop = args.TopDiff;



            //bubble up ,find global rect coord
            //and then merge to accumulate rect        
            RenderElement fromElement = args.SrcRenderElement;
            Rectangle elemClientRect = args.Rect;
            bool passSourceElem = args.PassSrcElement;

            //HasViewportOffset = false;


            int globalPoint_X = 0;
            int globalPoint_Y = 0;
#if DEBUG
            //if (fromElement.dbug_ObjectNote == "panel")
            //{

            //}
            int dbug_ncount = 0;
            //dbugWriteStopGfxBubbleUp(fromElement, ref dbug_ncount, dbug_ncount, ">> :" + elemClientRect.ToString());
#endif

            for (; ; )
            {
                if (!fromElement.Visible)
                {
#if DEBUG
                    //dbugWriteStopGfxBubbleUp(fromElement, ref dbug_ncount, 0, "EARLY-RET: ");
#endif
                    ReleaseInvalidateGfxArgs(args);
                    return;
                }
                else if (fromElement.BlockGraphicUpdateBubble)
                {
#if DEBUG
                    //dbugWriteStopGfxBubbleUp(fromElement, ref dbug_ncount, 0, "BLOCKED2: ");
#endif
                    ReleaseInvalidateGfxArgs(args);
                    return;
                }
#if DEBUG
                //dbugWriteStopGfxBubbleUp(fromElement, ref dbug_ncount, dbug_ncount, ">> ");
#endif


                globalPoint_X += fromElement.X;
                globalPoint_Y += fromElement.Y;

                if (fromElement.MayHasViewport && passSourceElem)
                {
                    elemClientRect.Offset(globalPoint_X, globalPoint_Y);
                    //****
#if DEBUG
                    //TODO: review here
                    if (fromElement.HasDoubleScrollableSurface)
                    {
                        //container.VisualScrollableSurface.WindowRootNotifyInvalidArea(elementClientRect);
                    }
#endif
                    Rectangle elementRect = fromElement.RectBounds;
                    elementRect.Offset(fromElement.ViewportLeft, fromElement.ViewportTop);
                    if (fromElement.NeedClipArea)
                    {
                        elemClientRect.Intersect(elementRect);
                    }

                    globalPoint_X = -fromElement.ViewportLeft; //reset ?
                    globalPoint_Y = -fromElement.ViewportTop; //reset ?
                }


#if DEBUG
                //System.Diagnostics.Debug.WriteLine(elemClientRect.ToString());
#endif

                if (fromElement.IsTopWindow)
                {

                    break;
                }
                else
                {
#if DEBUG
                    if (fromElement.dbugParentVisualElement == null)
                    {
                        //dbugWriteStopGfxBubbleUp(fromElement, ref dbug_ncount, 0, "BLOCKED3: ");
                    }
#endif

                    if (RenderElement.RequestInvalidateGraphicsNoti(fromElement))
                    {
                        RenderElement.InvokeInvalidateGraphicsNoti(fromElement, !passSourceElem, elemClientRect);
                    }

                    IParentLink parentLink = fromElement.MyParentLink;
                    if (parentLink == null)
                    {
                        ReleaseInvalidateGfxArgs(args);
                        return;
                    }

                    parentLink.AdjustLocation(ref globalPoint_X, ref globalPoint_Y);

                    //move up
                    fromElement = parentLink.ParentRenderElement;

                    if (fromElement == null)
                    {
                        ReleaseInvalidateGfxArgs(args);
                        return;
                    }
                }

                passSourceElem = true;
            }
            //----------
            //now we are on the top of root
            RootGraphic root = fromElement.GetRoot();

#if DEBUG
            RootGraphic dbugMyroot = root;
            if (dbugMyroot.dbugEnableGraphicInvalidateTrace
             && dbugMyroot.dbugGraphicInvalidateTracer != null)
            {
                while (dbug_ncount > 0)
                {
                    dbugMyroot.dbugGraphicInvalidateTracer.PopElement();
                    dbug_ncount--;
                }
            }
#endif

            //---------------------------------------- 
            elemClientRect.Offset(globalPoint_X, globalPoint_Y);
            if (elemClientRect.Top > root.Height
                || elemClientRect.Left > root.Width
                || elemClientRect.Bottom < 0
                || elemClientRect.Right < 0)
            {


#if DEBUG
                if (dbugMyroot.dbugEnableGraphicInvalidateTrace &&
                    dbugMyroot.dbugGraphicInvalidateTracer != null)
                {
                    dbugMyroot.dbugGraphicInvalidateTracer.WriteInfo("ZERO-EEX");
                    dbugMyroot.dbugGraphicInvalidateTracer.WriteInfo("\r\n");
                }

#endif
                ReleaseInvalidateGfxArgs(args);
                return;
            }
            //--------------------------------------------------------------------------------------------------


#if DEBUG

            Rectangle previewAccum = root._accumulateInvalidRect;
            if (!root._hasAccumRect)
            {
                previewAccum = elemClientRect;
            }
            else
            {

                previewAccum = Rectangle.Union(previewAccum, elemClientRect);
            }
            //if (previewAccum.Height > 30 && previewAccum.Height < 100)
            //{

            //}
#endif 

            args.GlobalRect = elemClientRect;

            if (root.HasViewportOffset = hasviewportOffset)
            {
                root.ViewportDiffLeft = viewport_diffLeft;
                root.ViewportDiffTop = viewport_diffTop;
            }

            root._accumInvalidateQueue.Add(args);
            root._hasRenderTreeInvalidateAccumRect = true;//***

            if (!root._hasAccumRect)
            {

                root._accumulateInvalidRect = elemClientRect;
                root._hasAccumRect = true;
            }
            else
            {
#if DEBUG
                //if (_accumInvalidateQueue.Count > 50)
                //{

                //}
#endif

                //TODO: check if we should do union or separate this into another group 
                if (!root._accumulateInvalidRect.IntersectsWith(elemClientRect))
                {

                    root._accumulateInvalidRect = Rectangle.Union(root._accumulateInvalidRect, elemClientRect);
                }
                else
                {
                    root._accumulateInvalidRect = Rectangle.Union(root._accumulateInvalidRect, elemClientRect);
                }
            }


#if DEBUG
            if (dbugMyroot.dbugEnableGraphicInvalidateTrace &&
                dbugMyroot.dbugGraphicInvalidateTracer != null)
            {
                string state_str = "ACC: ";
                if (root.dbugNeedContentArrangement || root.dbugNeedReCalculateContentSize)
                {
                    state_str = "!!" + state_str;
                }
                dbugMyroot.dbugGraphicInvalidateTracer.WriteInfo("ACC: " + root._accumulateInvalidRect.ToString());
                dbugMyroot.dbugGraphicInvalidateTracer.WriteInfo("\r\n");
            }
#endif
        }
    }


    public abstract partial class RootGraphic
    {
        public delegate void PaintToOutputWindowDelegate();
        protected PaintToOutputWindowDelegate _paintToOutputWindowHandler;
        Action<Rectangle> _canvasInvalidateDelegate;
        internal Rectangle _accumulateInvalidRect;
        internal bool _hasAccumRect;
        internal bool _hasRenderTreeInvalidateAccumRect;

#if DEBUG
        static int dbugTotalId;
        public readonly int dbugId = dbugTotalId++;
#endif
        public RootGraphic(int width, int heigth)
        {
            this.Width = width;
            this.Height = heigth;
        }
        public abstract ITextService TextServices { get; }
        public abstract RequestFont DefaultTextEditFontInfo { get; }
        public abstract IRenderElement TopWindowRenderBox { get; }
        public abstract void AddChild(RenderElement renderE);
        public abstract void SetCurrentKeyboardFocus(RenderElement renderElement);
        public abstract void SetPrimaryContainerElement(RenderBoxBase renderBox);
        public int Width { get; internal set; }
        public int Height { get; internal set; }
        public int ViewportDiffLeft { get; internal set; }
        public int ViewportDiffTop { get; internal set; }
        public bool HasViewportOffset { get; internal set; }
        //---------------------------------------------      

        /// <summary>
        /// close window box root
        /// </summary>
        public abstract void CloseWinRoot();
        //--------------------------------------------- 
        //carets ...
        public abstract void CaretStartBlink();
        public abstract void CaretStopBlink();
        public bool CaretHandleRegistered { get; set; }
        //------- -------------------------------------- 

        //timers
        public abstract bool GfxTimerEnabled { get; set; }
        public abstract GraphicsTimerTask SubscribeGraphicsIntervalTask(
            object uniqueName,
            TaskIntervalPlan planName,
            int intervalMs,
            EventHandler<GraphicsTimerTaskEventArgs> tickhandler);
        public abstract void RemoveIntervalTask(object uniqueName);


        //--------------------------------------------------------------------------
        public abstract void PrepareRender();
        public abstract void TopDownRecalculateContent();
        public event EventHandler PreRenderEvent;
        protected void InvokePreRenderEvent() => PreRenderEvent?.Invoke(this, EventArgs.Empty);

        public bool IsInRenderPhase { get; private set; }
        public virtual void BeginRenderPhase() => IsInRenderPhase = true;
        public virtual void EndRenderPhase() => IsInRenderPhase = false;

        public bool HasAccumInvalidateRect => _hasAccumRect;
        public Rectangle AccumInvalidateRect => _accumulateInvalidRect;
        public bool HasRenderTreeInvalidateAccumRect => _hasRenderTreeInvalidateAccumRect;

        public abstract void ManageRenderElementRequests();
        public virtual void EnqueueRenderRequest(RenderElementRequest renderReq) { }


        public void FlushAccumGraphics()
        {
            if (!_hasAccumRect)
            {
                return;
            }

            if (this.IsInRenderPhase) { return; }

#if DEBUG
            //if (_accumulateInvalidRect.Height > 30 && _accumulateInvalidRect.Height < 100)
            //{
            //}

            //System.Diagnostics.Debug.WriteLine("flush1:" + _accumulateInvalidRect.ToString());
#endif


            //TODO review this 
            _canvasInvalidateDelegate?.Invoke(_accumulateInvalidRect);

            _paintToOutputWindowHandler();
            _hasAccumRect = false;
            _hasRenderTreeInvalidateAccumRect = false;
        }
        public void SetPaintDelegates(Action<Rectangle> canvasInvalidateDelegate, PaintToOutputWindowDelegate paintToOutputHandler)
        {
            _canvasInvalidateDelegate = canvasInvalidateDelegate;
            _paintToOutputWindowHandler = paintToOutputHandler;
        }
        public static void InvalidateRectArea(RootGraphic rootgfx, Rectangle invalidateRect)
        {
#if DEBUG
            Rectangle preview = Rectangle.Union(rootgfx._accumulateInvalidRect, invalidateRect);
            //System.Diagnostics.Debug.WriteLine("flush1:" + rootgfx._accumulateInvalidRect.ToString());
#endif
            //invalidate rect come from external UI (not from interal render tree)
            rootgfx._accumulateInvalidRect = Rectangle.Union(rootgfx._accumulateInvalidRect, invalidateRect);
#if DEBUG
            if (rootgfx._accumulateInvalidRect.Height > 30)
            {

            }
#endif

            rootgfx._hasAccumRect = true;
        }

        internal readonly List<InvalidateGfxArgs> _accumInvalidateQueue = new List<InvalidateGfxArgs>();

        public static List<InvalidateGfxArgs> GetAccumInvalidateGfxArgsQueue(RootGraphic r) => r._accumInvalidateQueue;


        protected static InvalidateGfxArgs GetInvalidateGfxArgs() => BubbleInvalidater.GetInvalidateGfxArgs();
        protected static void ReleaseInvalidateGfxArgs(InvalidateGfxArgs args) => BubbleInvalidater.ReleaseInvalidateGfxArgs(args);

        protected static void InternalBubbleup(InvalidateGfxArgs args) => BubbleInvalidater.InternalBubbleUpInvalidateGraphicArea(args);
    }




}