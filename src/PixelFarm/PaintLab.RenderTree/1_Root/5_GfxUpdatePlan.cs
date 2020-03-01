//Apache2, 2020-present, WinterDev

using System.Collections.Generic;
using PixelFarm.Drawing;
namespace LayoutFarm
{
    struct SimplePool<T>
    {
        public delegate T CreateNewDel();
        public delegate void CleanupDel(T d);

        CreateNewDel _createDel;
        CleanupDel _cleanupDel;
        Stack<T> _pool;
        public SimplePool(CreateNewDel createDel, CleanupDel cleanup)
        {
            _pool = new Stack<T>();
            _createDel = createDel;
            _cleanupDel = cleanup;
        }
        public T Borrow()
        {
            return _pool.Count > 0 ? _pool.Pop() : _createDel();
        }
        public void ReleaseBack(T t)
        {
            _cleanupDel?.Invoke(t);
            _pool.Push(t);
        }
        public void Close()
        {

        }
    }


    public class GfxUpdatePlan
    {
        RootGraphic _rootgfx;
        readonly List<RenderElement> _bubbleGfxTracks = new List<RenderElement>();
        public GfxUpdatePlan(RootGraphic rootgfx)
        {
            _rootgfx = rootgfx;
        }

        static RenderElement FindFirstClipedOrOpaqueParent(RenderElement r)
        {
#if DEBUG
            RenderElement dbugBackup = r;
#endif
            r = r.ParentRenderElement;
            while (r != null)
            {
                if (r.NoClipOrBgIsNotOpaque)
                {
                    r = r.ParentRenderElement;
                }
                else
                {
                    //found 1st opaque bg parent
                    return r;
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

        public Rectangle AccumUpdateArea { get; private set; }


        /// <summary>
        /// update rect region
        /// </summary>
        class GfxUpdateRectRgn
        {
            readonly List<InvalidateGfxArgs> _invList = new List<InvalidateGfxArgs>();

#if DEBUG
            public GfxUpdateRectRgn() { }
#endif
            public void AddDetail(InvalidateGfxArgs a)
            {
                if (_invList.Count == 0)
                {
                    AccumRect = a.GlobalRect;
                }
                else
                {

                    if (AccumRect == a.GlobalRect)
                    {
                        return;
                    }

                    AccumRect = Rectangle.Union(AccumRect, a.GlobalRect);
                }
                _invList.Add(a);
            }
            public void Reset(RootGraphic rootgfx)
            {
                List<InvalidateGfxArgs> invList = _invList;
                for (int i = invList.Count - 1; i >= 0; --i)
                {
                    //release back 
                    rootgfx.ReleaseInvalidateGfxArgs(invList[i]);
                }
                invList.Clear();
                AccumRect = Rectangle.Empty;
            }

            public Rectangle AccumRect { get; private set; }
            public InvalidateGfxArgs GetDetail(int index) => _invList[index];
            public int DetailCount => _invList.Count;
        }

        readonly List<GfxUpdateRectRgn> _updateList = new List<GfxUpdateRectRgn>();
        readonly SimplePool<GfxUpdateRectRgn> _updateRectRgnPool = new SimplePool<GfxUpdateRectRgn>(() => new GfxUpdateRectRgn(), null);

        GfxUpdateRectRgn _currentUpdateRgn = null;

        public void SetCurrentUpdate(int index)
        {
            //reset

            AccumUpdateArea = Rectangle.Empty;
            _bubbleGfxTracks.Clear();
            _currentUpdateRgn = _updateList[index];

            int detailCount = _currentUpdateRgn.DetailCount;
            for (int i = 0; i < detailCount; ++i)
            {
                InvalidateGfxArgs args = _currentUpdateRgn.GetDetail(i);
                RenderElement.MarkAsGfxUpdateTip(args.StartOn);
                BubbleUpGraphicsUpdateTrack(args.StartOn, _bubbleGfxTracks);
            }

            AccumUpdateArea = _currentUpdateRgn.AccumRect;

            RenderElement.WaitForStartRenderElement = true;
        }

        public void ClearCurrentUpdate()
        {
            if (_currentUpdateRgn != null)
            {
                _currentUpdateRgn.Reset(_rootgfx);
                _updateRectRgnPool.ReleaseBack(_currentUpdateRgn);
                _currentUpdateRgn = null;
            }
            for (int i = _bubbleGfxTracks.Count - 1; i >= 0; --i)
            {
                RenderElement.ResetBubbleUpdateLocalStatus(_bubbleGfxTracks[i]);
            }
            _bubbleGfxTracks.Clear();
            RenderElement.WaitForStartRenderElement = false;
        }
        public int UpdateListCount => _updateList.Count;

        /// <summary>
        /// add to nearest update-rect-region, if not found create a new one
        /// </summary>
        /// <param name="a"></param>
        void AddToNearestRegion(InvalidateGfxArgs a)
        {
            int j = _updateList.Count;
            GfxUpdateRectRgn found = null;
            for (int i = j - 1; i >= 0; --i)
            {
                //search from latest rgn 
                GfxUpdateRectRgn existing = _updateList[i];
                if (existing.AccumRect.IntersectsWith(a.GlobalRect))
                {
                    found = existing;
                    break;
                }
            }

            if (found == null)
            {
                GfxUpdateRectRgn updateJob = _updateRectRgnPool.Borrow();
                updateJob.AddDetail(a);
                _updateList.Add(updateJob);
            }
            else
            {
                found.AddDetail(a);
            }
        }

        public void SetUpdatePlanForFlushAccum()
        {
            //create accumulative plan                
            //merge consecutive
            RenderElement.WaitForStartRenderElement = false;
            List<InvalidateGfxArgs> accumQueue = RootGraphic.GetAccumInvalidateGfxArgsQueue(_rootgfx);
            int j = accumQueue.Count;
            if (j == 0)
            {
                return;
            }
            else if (j > 10) //???
            {
                //default (original) mode                 
                System.Diagnostics.Debug.WriteLine("traditional: " + j);

                for (int i = 0; i < j; ++i)
                {
                    InvalidateGfxArgs a = accumQueue[i];
                    _rootgfx.ReleaseInvalidateGfxArgs(a);
                }
            }
            else
            {
#if DEBUG
                if (j == 2)
                {
                }
                System.Diagnostics.Debug.WriteLine("flush accum:" + j);
                //--------------
                //>>preview for debug
                if (RenderElement.dbugUpdateTrackingCount > 0)
                {
                    throw new System.NotSupportedException();
                }

                //for (int i = 0; i < j; ++i)
                //{
                //    InvalidateGfxArgs a = accumQueue[i];
                //    RenderElement srcE = a.SrcRenderElement;
                //    if (srcE.NoClipOrBgIsNotOpaque)
                //    {
                //        srcE = FindFirstClipedOrOpaqueParent(srcE);
                //        if (srcE == null)
                //        {
                //            throw new System.NotSupportedException();
                //        }
                //    }
                //    if (srcE.IsBubbleGfxUpdateTrackedTip)
                //    {
                //    }
                //}
                //<<preview for debug
                //--------------
#endif

                for (int i = 0; i < j; ++i)
                {
                    InvalidateGfxArgs a = accumQueue[i];
                    RenderElement srcE = a.SrcRenderElement;
                    if (srcE.NoClipOrBgIsNotOpaque)
                    {
                        srcE = FindFirstClipedOrOpaqueParent(srcE);
                    }
                    a.StartOn = srcE;
                    AddToNearestRegion(a);
                }
            }

            accumQueue.Clear();
        }

        public void ResetUpdatePlan()
        {
            _currentUpdateRgn = null;
            _updateList.Clear();
            RenderElement.WaitForStartRenderElement = false;
        }
    }



}