//Apache2, 2014-present, WinterDev

using System.Collections.Generic;

using PixelFarm.Drawing;
namespace LayoutFarm
{

    public enum InvalidateReason
    {
        //TODO:  move to another source code file
        Empty,
        ViewportChanged,
        UpdateLocalArea,
    }

    public class InvalidateGraphicsArgs
    {
        //TODO:  move to another source code file 
        internal InvalidateGraphicsArgs() { }
        public InvalidateReason Reason { get; private set; }
        public bool PassSrcElement { get; private set; }
        public int LeftDiff { get; private set; }
        public int TopDiff { get; private set; }
        internal Rectangle Rect;
        internal Rectangle GlobalRect;

        internal RenderElement StartOn { get; set; }
        public RenderElement SrcRenderElement { get; private set; }
        public void Reset()
        {
            LeftDiff = TopDiff = 0;
            GlobalRect = Rect = Rectangle.Empty;
            SrcRenderElement = null;
            Reason = InvalidateReason.Empty;
            PassSrcElement = false;
            StartOn = null;
        }
        /// <summary>
        /// set info about this invalidate args
        /// </summary>
        /// <param name="srcElem"></param>
        /// <param name="leftDiff"></param>
        /// <param name="topDiff"></param>
        public void Reason_ChangeViewport(RenderElement srcElem, int leftDiff, int topDiff)
        {
            SrcRenderElement = srcElem;
            LeftDiff = leftDiff;
            TopDiff = topDiff;
            Reason = InvalidateReason.ViewportChanged;
        }
        public void Reason_UpdateLocalArea(RenderElement srcElem, Rectangle localBounds)
        {
            SrcRenderElement = srcElem;
            Rect = localBounds;
            Reason = InvalidateReason.UpdateLocalArea;
        }

#if DEBUG
        public override string ToString() => Reason.ToString() + " " + SrcRenderElement.dbug_obj_id.ToString();
#endif
    }



    public class GfxUpdatePlan
    {
        //TODO:  move to another source code file

        RootGraphic _rootgfx;
        readonly List<RenderElement> _bubbleGfxTracks = new List<RenderElement>();

        public GfxUpdatePlan(RootGraphic rootgfx)
        {
            _rootgfx = rootgfx;
        }
        static RenderElement FindFirstOpaqueParent(RenderElement r)
        {
            RenderElement parent = r.ParentRenderElement;
            while (parent != null)
            {
                if (parent.BgIsNotOpaque)
                {
                    parent = r.ParentRenderElement;
                }
                else
                {
                    //found 1st opaque bg parent
                    return parent;
                }
            }
            return null; //not found
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

        class GfxUpdateJob
        {
            internal List<InvalidateGraphicsArgs> _invList = new List<InvalidateGraphicsArgs>();
        }

        readonly List<GfxUpdateJob> _gfxUpdateJobList = new List<GfxUpdateJob>();
        GfxUpdateJob _currentJob = null;
        Rectangle _accumUpdateArea;

        public void SetCurrentJob(int jobIndex)
        {
            //reset
            _accumUpdateArea = Rectangle.Empty;
            _bubbleGfxTracks.Clear();

            _currentJob = _gfxUpdateJobList[jobIndex];
            List<InvalidateGraphicsArgs> list = _currentJob._invList;
            if (list.Count > 1)
            {

            }
            else
            {
                InvalidateGraphicsArgs args = list[0];
                BubbleUpGraphicsUpdateTrack(args.StartOn, _bubbleGfxTracks);
                _accumUpdateArea = args.GlobalRect;
            }
        }

        public Rectangle AccumUpdateArea => _accumUpdateArea;
        public void ClearCurrentJob()
        {
            if (_currentJob != null)
            {
                List<InvalidateGraphicsArgs> invList = _currentJob._invList;
                for (int i = invList.Count - 1; i >= 0; --i)
                {
                    //release back 
                    _rootgfx.ReleaseInvalidateGfxArgs(invList[i]);
                }
                invList.Clear();
            }
            for (int i = _bubbleGfxTracks.Count - 1; i >= 0; --i)
            {
                RenderElement.ResetBubbleUpdateLocalStatus(_bubbleGfxTracks[i]);
            }
            _bubbleGfxTracks.Clear();

        }
        public int JobCount => _gfxUpdateJobList.Count;

        void AddNewJob(InvalidateGraphicsArgs a)
        {
            GfxUpdateJob updateJob = new GfxUpdateJob();
            updateJob._invList.Add(a);
            _gfxUpdateJobList.Add(updateJob);
        }
        public void SetUpdatePlanForFlushAccum()
        {
            //create accumulative plan                
            //merge consecutive
            List<InvalidateGraphicsArgs> accumQueue = RootGraphic.GetAccumInvalidateGfxArgsQueue(_rootgfx);
            int j = accumQueue.Count;
            if (j == 0)
            {
                return;
            }
            else if (j > 0) //???
            {
                //default mode                 
                for (int i = 0; i < j; ++i)
                {
                    InvalidateGraphicsArgs a = accumQueue[i];
                    _rootgfx.ReleaseInvalidateGfxArgs(a);
                }
            }
            else
            {
#if DEBUG
                //--------------
                //>>preview for debug
                if (RenderElement.dbugUpdateTrackingCount > 0)
                {
                    throw new System.NotSupportedException();
                }

                for (int i = 0; i < j; ++i)
                {
                    InvalidateGraphicsArgs a = accumQueue[i];
                    RenderElement srcE = a.SrcRenderElement;
                    if (srcE.BgIsNotOpaque)
                    {
                        srcE = FindFirstOpaqueParent(srcE);

                        if (srcE == null)
                        {
                            throw new System.NotSupportedException();
                        }
                    }
                    if (srcE.IsBubbleGfxUpdateTrackedTip)
                    {
                    }
                }
                //<<preview for debug
                //--------------
#endif

                for (int i = 0; i < j; ++i)
                {
                    InvalidateGraphicsArgs a = accumQueue[i];
                    RenderElement srcE = a.SrcRenderElement;

                    if (srcE.BgIsNotOpaque)
                    {
                        srcE = FindFirstOpaqueParent(srcE);
                    }

                    if (!srcE.IsBubbleGfxUpdateTrackedTip)
                    {
                        //if(srcE is not a tip)=> track this
                        //if srcE is already a tip , => not need to track
                        //
                        a.StartOn = srcE;
                        RenderElement.MarkAsGfxUpdateTip(srcE);
                        AddNewJob(a);
                    }
                    else
                    {
                        //already track ??
                        //we so need to 
#if DEBUG
                        if (_gfxUpdateJobList.Count == 0)
                        {

                        }
#endif
                    }
                }
            }

            accumQueue.Clear();
        }

        public void ResetUpdatePlan()
        {

            _currentJob = null;
            _gfxUpdateJobList.Clear();
        }
    }


    partial class RenderElement
    {
        internal void InvalidateGraphics(InvalidateGraphicsArgs args)
        {
            //RELATIVE to this ***
            _propFlags &= ~RenderElementConst.IS_GRAPHIC_VALID;
            if ((_uiLayoutFlags & RenderElementConst.LY_SUSPEND_GRAPHIC) != 0)
            {
#if DEBUG
                dbugVRoot.dbug_PushInvalidateMsg(RootGraphic.dbugMsg_BLOCKED, this);
#endif
                return;
            }

            if (!GlobalRootGraphic.SuspendGraphicsUpdate)
            {
                Rectangle rect = new Rectangle(0, 0, _b_width, _b_height);
                args.Rect = rect;

                //RELATIVE to this***
                //1.
                _propFlags &= ~RenderElementConst.IS_GRAPHIC_VALID;
                //2.  
                _rootGfx.BubbleUpInvalidateGraphicArea(args);
            }
            else
            {

            }
        }
        public void InvalidateGraphics()
        {
            //RELATIVE to this ***
            _propFlags &= ~RenderElementConst.IS_GRAPHIC_VALID;
            if ((_uiLayoutFlags & RenderElementConst.LY_SUSPEND_GRAPHIC) != 0)
            {
#if DEBUG
                dbugVRoot.dbug_PushInvalidateMsg(RootGraphic.dbugMsg_BLOCKED, this);
#endif
                return;
            }

            if (!GlobalRootGraphic.SuspendGraphicsUpdate)
            {
                Rectangle rect = new Rectangle(0, 0, _b_width, _b_height);
                InvalidateGraphicLocalArea(this, rect);
            }
            else
            {

            }
        }

        public void InvalidateParentGraphics()
        {
            //RELATIVE to its parent
            this.InvalidateParentGraphics(this.RectBounds);
        }

        protected virtual void OnInvalidateGraphicsNoti(bool fromMe, ref Rectangle totalBounds) { }

        public void InvalidateParentGraphics(Rectangle totalBounds)
        {
            //RELATIVE to its parent***

            _propFlags &= ~RenderElementConst.IS_GRAPHIC_VALID;
            RenderElement parent = this.ParentRenderElement; //start at parent ****

            //--------------------------------------- 
            if ((_uiLayoutFlags & RenderElementConst.LY_REQ_INVALIDATE_RECT_EVENT) != 0)
            {
                OnInvalidateGraphicsNoti(true, ref totalBounds);
            }
            //
            if (parent != null)
            {
                if (!GlobalRootGraphic.SuspendGraphicsUpdate)
                {
                    InvalidateGraphicsArgs arg = _rootGfx.GetInvalidateGfxArgs();
                    arg.Reason_UpdateLocalArea(parent, totalBounds);

                    _rootGfx.BubbleUpInvalidateGraphicArea(arg);//RELATIVE to its parent***
                }
                else
                {

                }
            }
        }
        internal static bool RequestInvalidateGraphicsNoti(RenderElement re)
        {
            return (re._uiLayoutFlags & RenderElementConst.LY_REQ_INVALIDATE_RECT_EVENT) != 0;
        }
        internal static void InvokeInvalidateGraphicsNoti(RenderElement re, bool fromMe, Rectangle totalBounds)
        {
            re.OnInvalidateGraphicsNoti(fromMe, ref totalBounds);
        }

        public static void InvalidateGraphicLocalArea(RenderElement re, Rectangle localArea)
        {
            //RELATIVE to re ***

            if (localArea.Height == 0 || localArea.Width == 0)
            {
                return;
            }

            re._propFlags &= ~RenderElementConst.IS_GRAPHIC_VALID;
            InvalidateGraphicsArgs inv = re._rootGfx.GetInvalidateGfxArgs();
            inv.Reason_UpdateLocalArea(re, localArea);
            re._rootGfx.BubbleUpInvalidateGraphicArea(inv);
        }


        /// <summary>
        ///TODO: review this again
        /// </summary>
        protected bool ForceReArrange
        {
            get { return true; }
            set { }
        }
        public void SuspendGraphicsUpdate()
        {
            _uiLayoutFlags |= RenderElementConst.LY_SUSPEND_GRAPHIC;
        }
        public void ResumeGraphicsUpdate()
        {
            _uiLayoutFlags &= ~RenderElementConst.LY_SUSPEND_GRAPHIC;
        }
        internal bool BlockGraphicUpdateBubble
        {
            get
            {
#if DEBUG
                return (_uiLayoutFlags & RenderElementConst.LY_SUSPEND_GRAPHIC) != 0;
#else
                return (_uiLayoutFlags & RenderElementConst.LY_SUSPEND_GRAPHIC) != 0;
#endif
            }
        }

        public static bool WaitForStartRenderElement { get; set; }
        public static bool UnlockForStartRenderElement(RenderElement re)
        {
            if ((re._propFlags & RenderElementConst.TRACKING_GFX_TIP) != 0)
            {
                WaitForStartRenderElement = false;//unlock
                return true;
            }
            return false;
        }
    }
}