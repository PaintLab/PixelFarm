﻿//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;

using PixelFarm.Drawing;
using LayoutFarm.RenderBoxes;

namespace LayoutFarm
{
    public interface IRenderElement
    {
        void Render(DrawBoard canvas, UpdateArea updateArea);
#if DEBUG
        void dbugShowRenderPart(DrawBoard canvas, UpdateArea r);
#endif
    }

    public static class GlobalRootGraphic
    {
        //TODO: merge this to RootGraphics?

        static int _suspendCount;
        internal static bool SuspendGraphicsUpdate;

        public static RootGraphic CurrentRootGfx;
        public static RenderElement CurrentRenderElement;

        //public static RenderElement StartWithRenderElement; //temp fix
        //public static bool WaitForFirstRenderElement;


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

    public abstract partial class RootGraphic
    {
        public delegate void PaintToOutputWindowDelegate();
        protected PaintToOutputWindowDelegate _paintToOutputWindowHandler;
        CanvasInvalidateDelegate _canvasInvalidateDelegate;
        Rectangle _accumulateInvalidRect;
        bool _hasAccumRect;
        bool _hasRenderTreeInvalidateAccumRect;

#if DEBUG
        static int dbugTotalId;
        public readonly int dbugId = dbugTotalId++;
#endif
        public RootGraphic(int width, int heigth)
        {
            this.Width = width;
            this.Height = heigth;
        }
        public bool HasAccumInvalidateRect => _hasAccumRect;
        public Rectangle AccumInvalidateRect => _accumulateInvalidRect;
        public abstract ITextService TextServices { get; }
        public abstract RequestFont DefaultTextEditFontInfo { get; }
        public abstract void TopDownRecalculateContent();
        public abstract IRenderElement TopWindowRenderBox { get; }
        public abstract void AddChild(RenderElement renderE);

        public abstract void SetPrimaryContainerElement(RenderBoxBase renderBox);
        public int Width { get; internal set; }
        public int Height { get; internal set; }
        /// <summary>
        /// close window box root
        /// </summary>
        public abstract void CloseWinRoot();
        public abstract void ManageRenderElementRequests();

        public event EventHandler PreRenderEvent;
        protected void InvokePreRenderEvent()
        {
            PreRenderEvent?.Invoke(this, EventArgs.Empty);
        }
        public abstract void SetCurrentKeyboardFocus(RenderElement renderElement);


        //--------------------------------------------------------------------------
        //timers
        public abstract bool GfxTimerEnabled { get; set; }
        public abstract GraphicsTimerTask SubscribeGraphicsIntervalTask(
            object uniqueName,
            TaskIntervalPlan planName,
            int intervalMs,
            EventHandler<GraphicsTimerTaskEventArgs> tickhandler);
        public abstract void RemoveIntervalTask(object uniqueName);



        //--------------------------------------------------------------------------
#if DEBUG

        bool dbugNeedContentArrangement { get; set; }
        bool dbugNeedReCalculateContentSize { get; set; }
        public static void dbugResetAccumRect(RootGraphic rootgfx)
        {
            rootgfx._hasAccumRect = false;
        }
#endif
        //--------------------------------------------------------------------------

        public abstract void PrepareRender();

        public bool HasRenderTreeInvalidateAccumRect => _hasRenderTreeInvalidateAccumRect;



        public virtual void EnqueueRenderRequest(RenderElementRequest renderReq) { }


        List<InvalidateGraphicsArgs> _tmpInvalidatePlans = new List<InvalidateGraphicsArgs>();
        List<RenderElement> _bubbleGfxTracks = new List<RenderElement>();
 
        public void SetUpdatePlanForFlushAccum(UpdateArea u)
        {

            bool flushPlanClearBG = true;
            RenderElement singleRenderE = null;

            //create accumulative plan                
            //merge consecutive
            int j = _accumInvalidateQueue.Count;

            //make a plan
            if (j == 1)
            {
                //This is a special case
                InvalidateGraphicsArgs a = _accumInvalidateQueue.Dequeue();

                //1. check if global update area is in the queue or not
                //if not, we can ignore this

                //2. do bubble up render tracking
                a.SrcRenderElement.InvalidateGraphics();

                if (a.SrcRenderElement.BgIsNotOpaque)
                {
                }
                else
                {
                    switch (a.Reason)
                    {
                        case InvalidateReason.ViewportChanged:
                            {
                                flushPlanClearBG = false;
                                singleRenderE = a.SrcRenderElement;
                            }
                            break;
                        case InvalidateReason.UpdateLocalArea:
                            {
                                //Do bubble tracking up
                                BubbleUpGraphicsUpdateTrack(a.SrcRenderElement, _bubbleGfxTracks);

                                flushPlanClearBG = false;
                                singleRenderE = a.SrcRenderElement;
                            }
                            break;
                    }
                }
                ReleaseInvalidateGfxArgs(a);
            }
            else if (j > 0)
            {
                //default
                for (int i = 0; i < j; ++i)
                {
                    InvalidateGraphicsArgs a = _accumInvalidateQueue.Dequeue();
                    //if (a.SrcRenderElement.BgIsNotOpaque)
                    //{

                    //}
                    //else
                    //{
                    //    switch (a.Reason)
                    //    {
                    //        case InvalidateReason.ViewportChanged:
                    //            FlushPlanClearBG = false;
                    //            SingleRenderE = a.SrcRenderElement;
                    //            break;
                    //        case InvalidateReason.UpdateLocalArea:
                    //            FlushPlanClearBG = false;
                    //            SingleRenderE = a.SrcRenderElement;
                    //            break;
                    //    }
                    //}
                    ReleaseInvalidateGfxArgs(a);
                }

                _tmpInvalidatePlans.Clear();
            }

            u.SetStartRenderElement(singleRenderE);
            u.CurrentRect = this.AccumInvalidateRect;
            u.ClearRootBackground = flushPlanClearBG;
        }
        public void ResetUpdatePlan(UpdateArea u)
        {

        }
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
            //TODO: check _canvasInvalidateDelegate== null, 


            _canvasInvalidateDelegate?.Invoke(_accumulateInvalidRect);

            _paintToOutputWindowHandler();
            _hasAccumRect = false;
            _hasRenderTreeInvalidateAccumRect = false;

            if (_bubbleGfxTracks.Count > 0)
            {
                //clear tracking elems
                for (int i = _bubbleGfxTracks.Count - 1; i >= 0; --i)
                {
                    RenderElement.ResetBubbleUpdateLocalStatus(_bubbleGfxTracks[i]);
                }
                _bubbleGfxTracks.Clear();
            }
        }
        public void SetPaintDelegates(CanvasInvalidateDelegate canvasInvalidateDelegate, PaintToOutputWindowDelegate paintToOutputHandler)
        {
            _canvasInvalidateDelegate = canvasInvalidateDelegate;
            _paintToOutputWindowHandler = paintToOutputHandler;
        }
#if DEBUG
        void dbugWriteStopGfxBubbleUp(RenderElement fromElement, ref int dbug_ncount, int nleftOnStack, string state_str)
        {
            RootGraphic dbugMyroot = this;
            if (dbugMyroot.dbugEnableGraphicInvalidateTrace && dbugMyroot.dbugGraphicInvalidateTracer != null)
            {
                if (this.dbugNeedContentArrangement || this.dbugNeedReCalculateContentSize)
                {
                    state_str = "!!" + state_str;
                }
                dbugMyroot.dbugGraphicInvalidateTracer.WriteInfo(state_str, fromElement);
                while (dbug_ncount > nleftOnStack)
                {
                    dbugMyroot.dbugGraphicInvalidateTracer.PopElement();
                    dbug_ncount--;
                }
            }
        }
#endif



        public static void InvalidateRectArea(RootGraphic rootgfx, Rectangle invalidateRect)
        {
#if DEBUG
            Rectangle preview = Rectangle.Union(rootgfx._accumulateInvalidRect, invalidateRect);
            if (preview.Height > 30 && preview.Height < 100)
            {

            }
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
        public int ViewportDiffLeft { get; private set; }
        public int ViewportDiffTop { get; private set; }
        public bool HasViewportOffset { get; private set; }


        readonly Queue<InvalidateGraphicsArgs> _reusableInvalidateGfxs = new Queue<InvalidateGraphicsArgs>();
        readonly Queue<InvalidateGraphicsArgs> _accumInvalidateQueue = new Queue<InvalidateGraphicsArgs>();

        public InvalidateGraphicsArgs GetInvalidateGfxArgs()
        {
#if DEBUG
            //System.Diagnostics.Debug.Write("inv args count:" + _reusableInvalidateGfxs.Count);
#endif

            if (_reusableInvalidateGfxs.Count == 0)
            {
                return new InvalidateGraphicsArgs();
            }
            else
            {
                return _reusableInvalidateGfxs.Dequeue();
            }
        }

        //----------

        void ReleaseInvalidateGfxArgs(InvalidateGraphicsArgs args)
        {
            args.Reset();
            _reusableInvalidateGfxs.Enqueue(args);
        }


        public void BubbleUpInvalidateGraphicArea(InvalidateGraphicsArgs args)
        {
            bool hasviewportOffset = false;
            if (args.Reason == InvalidateReason.ViewportChanged)
            {
                ViewportDiffLeft = args.LeftDiff;
                ViewportDiffTop = args.TopDiff;
                hasviewportOffset = true;
            }
            //-------------- 
            InternalBubbleUpInvalidateGraphicArea(args);//.SrcRenderElement, ref args.Rect, args.PassSrcElement); 
            HasViewportOffset = hasviewportOffset;
        }


        static void BubbleUpGraphicsUpdateTrack(RenderElement r, List<RenderElement> trackedElems)
        {
            while (r != null)
            {
                if (r.IsBubbleGfxUpdateTracked)
                {
                    return;//stop here
                }

                RenderElement.TrackBubbleUpdateLocalStatus(r);
                trackedElems.Add(r);
                r = r.ParentRenderElement;
            }          
            
        }
        void InternalBubbleUpInvalidateGraphicArea(InvalidateGraphicsArgs args)//RenderElement fromElement, ref Rectangle elemClientRect, bool passSourceElem)
        {
            //total bounds = total bounds at level            
            if (this.IsInRenderPhase)
            {
                ReleaseInvalidateGfxArgs(args);
                return;
            }
            //--------------------------------------            
            //bubble up ,find global rect coord
            //and then merge to accumulate rect        


            RenderElement fromElement = args.SrcRenderElement;
            Rectangle elemClientRect = args.Rect;
            bool passSourceElem = args.PassSrcElement;

            HasViewportOffset = false;
            _hasRenderTreeInvalidateAccumRect = true;//***

            int globalPoint_X = 0;
            int globalPoint_Y = 0;
#if DEBUG
            //if (fromElement.dbug_ObjectNote == "panel")
            //{

            //}
            int dbug_ncount = 0;
            dbugWriteStopGfxBubbleUp(fromElement, ref dbug_ncount, dbug_ncount, ">> :" + elemClientRect.ToString());
#endif

            for (; ; )
            {
                if (!fromElement.Visible)
                {
#if DEBUG
                    dbugWriteStopGfxBubbleUp(fromElement, ref dbug_ncount, 0, "EARLY-RET: ");
#endif
                    ReleaseInvalidateGfxArgs(args);
                    return;
                }
                else if (fromElement.BlockGraphicUpdateBubble)
                {
#if DEBUG
                    dbugWriteStopGfxBubbleUp(fromElement, ref dbug_ncount, 0, "BLOCKED2: ");
#endif
                    ReleaseInvalidateGfxArgs(args);
                    return;
                }
#if DEBUG
                dbugWriteStopGfxBubbleUp(fromElement, ref dbug_ncount, dbug_ncount, ">> ");
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
                        dbugWriteStopGfxBubbleUp(fromElement, ref dbug_ncount, 0, "BLOCKED3: ");
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
#if DEBUG
            var dbugMyroot = this;
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


            if (elemClientRect.Top > this.Height
                || elemClientRect.Left > this.Width
                || elemClientRect.Bottom < 0
                || elemClientRect.Right < 0)
            {
                //no intersect with  

#if DEBUG
                if (dbugMyroot.dbugEnableGraphicInvalidateTrace &&
                    dbugMyroot.dbugGraphicInvalidateTracer != null)
                {
                    dbugMyroot.dbugGraphicInvalidateTracer.WriteInfo("ZERO-EEX");
                    dbugMyroot.dbugGraphicInvalidateTracer.WriteInfo("\r\n");
                }
#endif
                return;
            }
            //--------------------------------------------------------------------------------------------------


#if DEBUG

            Rectangle previewAccum = _accumulateInvalidRect;
            if (!_hasAccumRect)
            {
                previewAccum = elemClientRect;
            }
            else
            {
                previewAccum = Rectangle.Union(previewAccum, elemClientRect);
            }

            if (previewAccum.Height > 30 && previewAccum.Height < 100)
            {

            }
#endif 

            args.GlobalRect = elemClientRect;
            if (!_hasAccumRect)
            {
                _accumInvalidateQueue.Enqueue(args);
                _accumulateInvalidRect = elemClientRect;
                _hasAccumRect = true;
            }
            else
            {
#if DEBUG
                if (_accumInvalidateQueue.Count > 50)
                {

                }
#endif
                _accumInvalidateQueue.Enqueue(args);
                _accumulateInvalidRect = Rectangle.Union(_accumulateInvalidRect, elemClientRect);
            }


#if DEBUG
            if (dbugMyroot.dbugEnableGraphicInvalidateTrace &&
                dbugMyroot.dbugGraphicInvalidateTracer != null)
            {
                string state_str = "ACC: ";
                if (this.dbugNeedContentArrangement || this.dbugNeedReCalculateContentSize)
                {
                    state_str = "!!" + state_str;
                }
                dbugMyroot.dbugGraphicInvalidateTracer.WriteInfo("ACC: " + _accumulateInvalidRect.ToString());
                dbugMyroot.dbugGraphicInvalidateTracer.WriteInfo("\r\n");
            }
#endif

        }
        public bool IsInRenderPhase { get; set; }
        //--------------------------------------------- 
        //carets ...
        public abstract void CaretStartBlink();
        public abstract void CaretStopBlink();
        public bool CaretHandleRegistered { get; set; }
        //--------------------------------------------- 

    }






}