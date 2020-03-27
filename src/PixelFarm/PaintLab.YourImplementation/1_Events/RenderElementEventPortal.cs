//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.RenderBoxes;
namespace LayoutFarm.UI
{



    public class RenderElementEventPortal : IEventPortal
    {

        /// <summary>
        /// a helper class for mouse press monitor
        /// </summary>
        class MousePressMonitorHelper
        {
            int _ms;
            int _mousePressCount;
            IUIEventListener _currentMonitoredElem;
            readonly UITimerTask _mousePressMonitor;
            readonly UIMousePressEventArgs _mousePressEventArgs;

            public MousePressMonitorHelper(int ms)
            {
                _ms = ms;
                _mousePressCount = 0;
                _currentMonitoredElem = null;
                _mousePressEventArgs = new UIMousePressEventArgs();

                _mousePressMonitor = new UITimerTask(t =>
                {
                    if (_currentMonitoredElem != null)
                    {
                        //invoke mouse press event
                        if (_mousePressCount > 0)
                        {
                            _currentMonitoredElem.ListenMousePress(_mousePressEventArgs);
                        }
                        _mousePressCount++;
                    }
                });
                _mousePressMonitor.Enabled = true;
                _mousePressMonitor.IntervalInMillisec = ms; //interval for mouse press monitor
                UIPlatform.RegisterTimerTask(_mousePressMonitor);
            }
            public void Reset()
            {
                _currentMonitoredElem = null;
                _mousePressCount = 0;
            }
            /// <summary>
            /// set monitoed elem + invoke 1st mouse press event 
            /// </summary>
            /// <param name="ui"></param>
            public void SetMonitoredElement(IUIEventListener ui)
            {
#if DEBUG
                if (ui == null)
                {
                    System.Diagnostics.Debugger.Break();
                }
#endif

                _currentMonitoredElem = ui;
                _mousePressCount = 0;
                _currentMonitoredElem.ListenMousePress(_mousePressEventArgs);
            }
            public void AddMousePressInformation(UIMouseEventArgs importInfo)
            {
                _mousePressEventArgs.Button = importInfo.Button;
            }
            public bool HasMonitoredElem => _currentMonitoredElem != null;

        }


        //current hit chain        
        HitChain _previousChain = new HitChain();
        Stack<HitChain> _hitChainStack = new Stack<HitChain>();
        readonly RenderElement _topRenderElement;
        readonly MousePressMonitorHelper _mousePressMonitor;

#if DEBUG
        int dbugMsgChainVersion;
#endif
        public RenderElementEventPortal(RenderElement topRenderElement)
        {
            _topRenderElement = topRenderElement;
#if DEBUG
            dbugRootGraphics = (MyRootGraphic)topRenderElement.Root;
#endif
            _mousePressMonitor = new MousePressMonitorHelper(40);


        }

        HitChain GetFreeHitChain()
        {
            return (_hitChainStack.Count > 0) ? _hitChainStack.Pop() : new HitChain();
        }
        void SwapHitChain(HitChain hitChain)
        {

            if (_previousChain != null)
            {
                _hitChainStack.Push(_previousChain);
            }

            _previousChain = hitChain;
            //temp fix here 
            _previousChain.Reset();
        }

        static void SetEventOrigin(UIEventArgs e, HitChain hitChain)
        {
            int count = hitChain.Count;
            if (count > 0)
            {
                HitInfo hitInfo = hitChain.GetHitInfo(count - 1);
                e.ExactHitObject = hitInfo.HitElemAsRenderElement;
            }
        }


        //        static RenderElement HitTestOnPreviousChain(HitChain hitPointChain, HitChain previousChain, int x, int y)
        //        {
        //#if DEBUG
        //            if (hitPointChain == previousChain)
        //            {
        //                throw new NotSupportedException();
        //            }
        //#endif

        //            if (previousChain.Count > 0)
        //            {
        //                previousChain.SetStartTestPoint(x, y);
        //                //test on prev chain top to bottom
        //                int j = previousChain.Count;
        //                for (int i = 0; i < j; ++i)
        //                {
        //                    HitInfo hitInfo = previousChain.GetHitInfo(i);
        //                    RenderElement elem = hitInfo.HitElemAsRenderElement;
        //                    if (elem != null && elem.VisibleAndHasParent)
        //                    {
        //                        if (elem.Contains(hitInfo.point))
        //                        {
        //                            RenderElement found = elem.FindUnderlyingSiblingAtPoint(hitInfo.point);
        //                            if (found == null)
        //                            {
        //                                Point leftTop = elem.Location;
        //                                hitPointChain.OffsetTestPoint(leftTop.X, leftTop.Y);
        //                                hitPointChain.AddHitObject(elem);
        //                                //add to chain
        //                            }
        //                            else
        //                            {
        //                                break;
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        break;
        //                    }
        //                }
        //            }
        //            //---------------------------------
        //            if (hitPointChain.Count > 0)
        //            {
        //                var commonElement = hitPointChain.GetHitInfo(hitPointChain.Count - 1).HitElemAsRenderElement;
        //                hitPointChain.RemoveCurrentHit();
        //                return commonElement;
        //            }
        //            else
        //            {
        //                return null;
        //            }
        //        }


        void HitTestCoreWithPrevChainHint(HitChain hitPointChain, HitChain previousChain, int x, int y)
        {
            //---------------------------------
            //test on previous chain first , find common element 
            hitPointChain.Reset();
            hitPointChain.SetStartTestPoint(x, y);
#if DEBUG
            hitPointChain.dbugHitPhase = _dbugHitChainPhase;
#endif
            //if (this.dbugId > 0 && isDragging && previousChain.Count > 1)
            //{

            //}

            //RenderElement commonElement = HitTestOnPreviousChain(hitPointChain, previousChain, x, y);

            //temp fix
            //TODO: fix bug on HitTestOnPreviousChain()
            RenderElement commonElement = _topRenderElement;
            ////use root 
            //if (isDragging)
            //{
            //    if (commonElement != this.topRenderElement)
            //    {

            //    }
            //}


            //if (lastCommonElement != null && commonElement != null &&
            //    lastCommonElement != commonElement && isDragging)
            //{
            //    Console.WriteLine(commonElement.dbug_GetBoundInfo());
            //}
            //if (commonElement == null)
            //{
            //    commonElement = this.topRenderElement;
            //}

            //if (commonElement != this.topRenderElement)
            //{

            //}

            //lastCommonElement = commonElement;
            commonElement.HitTestCore(hitPointChain);
            //this.topRenderElement.HitTestCore(hitPointChain);
        }
        void IEventPortal.PortalMouseWheel(UIMouseEventArgs e)
        {
#if DEBUG
            if (this.dbugRootGraphics.dbugEnableGraphicInvalidateTrace)
            {
                this.dbugRootGraphics.dbugGraphicInvalidateTracer.WriteInfo("================");
                this.dbugRootGraphics.dbugGraphicInvalidateTracer.WriteInfo("MOUSEWHEEL");
                this.dbugRootGraphics.dbugGraphicInvalidateTracer.WriteInfo("================");
            }
#endif
            HitChain hitPointChain = GetFreeHitChain();
#if DEBUG 
            _dbugHitChainPhase = dbugHitChainPhase.MouseWheel;
#endif
            //find hit element
            HitTestCoreWithPrevChainHint(hitPointChain, _previousChain, e.X, e.Y);
            if (hitPointChain.Count > 0)
            {
                //------------------------------
                //1. origin object 
                SetEventOrigin(e, hitPointChain);
                //------------------------------  
                IUIEventListener currentMouseWheel = null;
                //portal                
                ForEachOnlyEventPortalBubbleUp(e, hitPointChain, (e1, portal) =>
                {
                    portal.PortalMouseWheel(e1);
                    //*****
                    currentMouseWheel = e1.CurrentContextElement;
                    return true;
                });
                //------------------------------
                //use events
                if (!e.CancelBubbling)
                {
                    e.CurrentContextElement = currentMouseWheel = null; //clear 
                    ForEachEventListenerBubbleUp(e, hitPointChain, (e1, listener) =>
                    {
                        //please ensure=> no local var/pararmeter capture inside lambda
                        if (listener.BypassAllMouseEvents)
                        {
                            return false;
                        }
                        currentMouseWheel = listener;
                        listener.ListenMouseWheel(e1);
                        //------------------------------------------------------- 
                        bool cancelMouseBubbling = e1.CancelBubbling;
                        //------------------------------------------------------- 
                        //retrun true to stop this loop (no further bubble up)
                        //return false to bubble this to upper control       
                        return e1.CancelBubbling || !listener.BypassAllMouseEvents;

                    });
                }
            }

            SwapHitChain(hitPointChain);
            e.StopPropagation();
        }

#if DEBUG

        dbugHitChainPhase _dbugHitChainPhase;
#endif

        IUIEventListener _prevMouseDownElement;
        IUIEventListener _currentMouseDown;


        void IEventPortal.PortalMouseDown(UIMouseEventArgs e)
        {
#if DEBUG
            if (this.dbugRootGraphics.dbugEnableGraphicInvalidateTrace)
            {
                this.dbugRootGraphics.dbugGraphicInvalidateTracer.WriteInfo("================");
                this.dbugRootGraphics.dbugGraphicInvalidateTracer.WriteInfo("MOUSEDOWN");
                this.dbugRootGraphics.dbugGraphicInvalidateTracer.WriteInfo("================");
            }
            dbugMsgChainVersion = 1;
            int local_msgVersion = 1;
#endif 
            HitChain hitPointChain = GetFreeHitChain();
#if DEBUG 
            _dbugHitChainPhase = dbugHitChainPhase.MouseDown;
#endif
            HitTestCoreWithPrevChainHint(hitPointChain, _previousChain, e.X, e.Y);
            if (hitPointChain.Count > 0)
            {
                //------------------------------
                //1. origin object 
                SetEventOrigin(e, hitPointChain);
                //------------------------------ 
                _prevMouseDownElement = e.PreviousMouseDown;
                _currentMouseDown = null;
                //portal                
                ForEachOnlyEventPortalBubbleUp(e, hitPointChain, (e1, portal) =>
                {
                    portal.PortalMouseDown(e1);
                    //*****
                    _currentMouseDown = e1.CurrentContextElement;
                    return true;
                });
                //------------------------------
                //use events
                if (!e.CancelBubbling)
                {
                    e.CurrentContextElement = _currentMouseDown = null; //clear 
                    ForEachEventListenerBubbleUp(e, hitPointChain, (e1, listener) =>
                    {
                        //please ensure=> no local var/pararmeter capture inside lambda
                        if (listener.BypassAllMouseEvents)
                        {
                            return false;
                        }
                        _currentMouseDown = listener;
                        listener.ListenMouseDown(e1);

                        if (e.CurrentMousePressMonitor != null)
                        {
                            //set snapshot data
                            _mousePressMonitor.AddMousePressInformation(e);
                            //set and invoke mouse press here                                
                            _mousePressMonitor.SetMonitoredElement(e.CurrentMousePressMonitor);
                        }
                        else
                        {
                            _mousePressMonitor.Reset();
                        }

                        //------------------------------------------------------- 
                        bool cancelMouseBubbling = e1.CancelBubbling;
                        if (_prevMouseDownElement != null &&
                            _prevMouseDownElement != listener)
                        {
                            _prevMouseDownElement.ListenLostMouseFocus(e1);
                            _prevMouseDownElement = null;//clear
                        }
                        //------------------------------------------------------- 
                        //retrun true to stop this loop (no further bubble up)
                        //return false to bubble this to upper control 
                        return e1.CancelBubbling || !listener.BypassAllMouseEvents;
                    });


                }

                if (_prevMouseDownElement != _currentMouseDown &&
                    _prevMouseDownElement != null)
                {
                    //TODO: review here, auto or manual
                    _prevMouseDownElement.ListenLostMouseFocus(e);
                    _prevMouseDownElement = null;
                }
            }
            //---------------------------------------------------------------

#if DEBUG
            RootGraphic visualroot = this.dbugRootGraphics;
            if (visualroot.dbug_RecordHitChain)
            {
                visualroot.dbug_rootHitChainMsg.Clear();
                HitInfo hitInfo;
                for (int tt = hitPointChain.Count - 1; tt >= 0; --tt)
                {
                    hitInfo = hitPointChain.GetHitInfo(tt);
                    RenderElement ve = hitInfo.HitElemAsRenderElement;
                    if (ve != null)
                    {
                        ve.dbug_WriteOwnerLayerInfo(visualroot, tt);
                        ve.dbug_WriteOwnerLineInfo(visualroot, tt);
                        string hit_info = new string('.', tt) + " [" + tt + "] "
                            + "(" + hitInfo.point.X + "," + hitInfo.point.Y + ") "
                            + ve.dbug_FullElementDescription();
                        visualroot.dbug_rootHitChainMsg.AddLast(new dbugLayoutMsg(ve, hit_info));
                    }
                }
            }
#endif

            SwapHitChain(hitPointChain);

            e.StopPropagation(); //TODO: review this again
#if DEBUG
            if (local_msgVersion != dbugMsgChainVersion)
            {
                return;
            }
            visualroot.dbugHitTracker.Write("stop-mousedown");
            visualroot.dbugHitTracker.Play = false;
#endif
        }

        bool _isFirstMouseEnter = false;
        bool _mouseMoveFoundSomeHit = false;

        void IEventPortal.PortalMouseMove(UIMouseEventArgs e)
        {

            HitChain hitPointChain = GetFreeHitChain();
#if DEBUG

            _dbugHitChainPhase = dbugHitChainPhase.MouseMove;
#endif
            HitTestCoreWithPrevChainHint(hitPointChain, _previousChain, e.X, e.Y);
            _previousChain.Reset();
            SetEventOrigin(e, hitPointChain);
            //-------------------------------------------------------
            ForEachOnlyEventPortalBubbleUp(e, hitPointChain, (e1, portal) =>
            {
                //please ensure=> no local var/pararmeter capture inside lambda
                portal.PortalMouseMove(e1);
                return true;
            });
            //-------------------------------------------------------  
            if (!e.CancelBubbling)
            {
                _mouseMoveFoundSomeHit = false;
                ForEachEventListenerBubbleUp(e, hitPointChain, (e1, listener) =>
                {
                    //please ensure=> no local var/pararmeter capture inside lambda
                    _mouseMoveFoundSomeHit = true;
                    _isFirstMouseEnter = false;
                    if (e1.CurrentMouseActive != null &&
                        e1.CurrentMouseActive != listener)
                    {
                        IUIEventListener tmp = e1.CurrentContextElement;
                        e1.CurrentContextElement = e1.CurrentMouseActive;
                        e1.CurrentMouseActive.ListenMouseLeave(e1);
                        e1.CurrentContextElement = tmp;//restore

                        _isFirstMouseEnter = true;
                    }

                    if (!e1.IsCanceled)
                    {
                        e1.CurrentMouseActive = listener;
                        e1.IsFirstMouseEnter = _isFirstMouseEnter;
                        e1.CurrentMouseActive.ListenMouseMove(e1);
                        e1.IsFirstMouseEnter = false;
                    }

                    return true;//stop
                });

                if (!_mouseMoveFoundSomeHit && e.CurrentMouseActive != null)
                {
                    IUIEventListener prev = e.CurrentContextElement;
                    e.CurrentContextElement = e.CurrentMouseActive;
                    e.CurrentMouseActive.ListenMouseLeave(e);
                    e.CurrentContextElement = prev;

                    if (!e.IsCanceled)
                    {
                        e.CurrentMouseActive = null;
                    }

                }
            }
            SwapHitChain(hitPointChain);
            e.StopPropagation();
        }
        void IEventPortal.PortalGotFocus(UIFocusEventArgs e)
        {
        }
        void IEventPortal.PortalLostFocus(UIFocusEventArgs e)
        {
        }
        void IEventPortal.PortalMouseUp(UIMouseEventArgs e)
        {
#if DEBUG
            if (this.dbugRootGraphics.dbugEnableGraphicInvalidateTrace)
            {
                this.dbugRootGraphics.dbugGraphicInvalidateTracer.WriteInfo("================");
                this.dbugRootGraphics.dbugGraphicInvalidateTracer.WriteInfo("MOUSEUP");
                this.dbugRootGraphics.dbugGraphicInvalidateTracer.WriteInfo("================");
            }
#endif

            HitChain hitPointChain = GetFreeHitChain();
#if DEBUG
            _dbugHitChainPhase = dbugHitChainPhase.MouseUp;
#endif

            _mousePressMonitor.Reset();
            HitTestCoreWithPrevChainHint(hitPointChain, _previousChain, e.X, e.Y);

            if (hitPointChain.Count > 0)
            {
                SetEventOrigin(e, hitPointChain);
                //--------------------------------------------------------------- 
                ForEachOnlyEventPortalBubbleUp(e, hitPointChain, (e1, portal) =>
                {
                    //please ensure=> no local var/pararmeter capture inside lambda
                    portal.PortalMouseUp(e1);
                    return true;
                });
                //---------------------------------------------------------------
                if (!e.CancelBubbling)
                {
                    ForEachEventListenerBubbleUp(e, hitPointChain, (e1, listener) =>
                    {
                        //please ensure=> no local var/pararmeter capture inside lambda
                        if (listener.BypassAllMouseEvents)
                        {
                            return false;
                        }
                        listener.ListenMouseUp(e1);
                        //retrun true to stop this loop (no further bubble up)
                        //return false to bubble this to upper control       
                        return e1.CancelBubbling || !listener.BypassAllMouseEvents;

                    });
                }
                //---------------------------------------------------------------
                if (e.IsAlsoDoubleClick)
                {
                    ForEachEventListenerBubbleUp(e, hitPointChain, (e1, listener) =>
                    {
                        //please ensure=> no local var/pararmeter capture inside lambda
                        listener.ListenMouseDoubleClick(e1);
                        //------------------------------------------------------- 
                        //retrun true to stop this loop (no further bubble up)
                        //return false to bubble this to upper control       
                        return e1.CancelBubbling || !listener.BypassAllMouseEvents;
                    });
                }
                if (!e.CancelBubbling)
                {
                    if (e.IsAlsoDoubleClick)
                    {
                        ForEachEventListenerBubbleUp(e, hitPointChain, (e1, listener) =>
                        {
                            //please ensure=> no local var/pararmeter capture inside lambda
                            listener.ListenMouseDoubleClick(e1);
                            //------------------------------------------------------- 
                            //retrun true to stop this loop (no further bubble up)
                            //return false to bubble this to upper control       
                            return e1.CancelBubbling || !listener.BypassAllMouseEvents;
                        });
                    }
                    else
                    {
                        //ForEachEventListenerBubbleUp(e, hitPointChain, listener =>
                        //{
                        //    listener.ListenMouseClick(e);

                        //    //retrun true to stop this loop (no further bubble up)
                        //    //return false to bubble this to upper control       
                        //    return e.CancelBubbling || !listener.BypassAllMouseEvents;
                        //});
                    }
                }
            }
            SwapHitChain(hitPointChain);
            e.StopPropagation();
        }
        void IEventPortal.PortalKeyDown(UIKeyEventArgs e)
        {
        }
        void IEventPortal.PortalKeyUp(UIKeyEventArgs e)
        {
        }
        void IEventPortal.PortalKeyPress(UIKeyEventArgs e)
        {
        }
        bool IEventPortal.PortalProcessDialogKey(UIKeyEventArgs e)
        {
            return false;
        }

        //===================================================================
        delegate bool EventPortalAction<T>(T e, IEventPortal evPortal) where T : UIEventArgs;
        delegate bool EventListenerAction<T>(T e, IUIEventListener listener) where T : UIEventArgs;
        static void ForEachOnlyEventPortalBubbleUp<T>(T e, HitChain hitPointChain, EventPortalAction<T> eventPortalAction)
            where T : UIEventArgs
        {
            for (int i = hitPointChain.Count - 1; i >= 0; --i)
            {
                HitInfo hitPoint = hitPointChain.GetHitInfo(i);
                object currentHitElement = hitPoint.HitElemAsRenderElement.GetController();
                if (currentHitElement is IEventPortal eventPortal)
                {
                    Point p = hitPoint.point;
                    e.CurrentContextElement = currentHitElement as IUIEventListener;
                    e.SetLocation(p.X, p.Y);
                    if (eventPortalAction(e, eventPortal))
                    {
                        return;
                    }
                }
            }
        }
        static void ForEachEventListenerBubbleUp<T>(T e, HitChain hitPointChain, EventListenerAction<T> listenerAction)
            where T : UIEventArgs
        {
            HitInfo hitInfo;
            for (int i = hitPointChain.Count - 1; i >= 0; --i)
            {
                hitInfo = hitPointChain.GetHitInfo(i);
                if (hitInfo.HitElemAsRenderElement.GetController() is IUIEventListener listener)
                {
                    if (e.SourceHitElement == null)
                    {
                        e.SourceHitElement = listener;
                    }

                    Point p = hitInfo.point;
                    e.SetLocation(p.X, p.Y);
                    e.CurrentContextElement = listener;
                    if (listenerAction(e, listener))
                    {
                        return;
                    }
                }
            }
        }


        //        public override void OnDragStart(UIMouseEventArgs e)
        //        {

        //#if DEBUG
        //            if (this.dbugRootGraphic.dbugEnableGraphicInvalidateTrace)
        //            {
        //                this.dbugRootGraphic.dbugGraphicInvalidateTracer.WriteInfo("================");
        //                this.dbugRootGraphic.dbugGraphicInvalidateTracer.WriteInfo("START_DRAG");
        //                this.dbugRootGraphic.dbugGraphicInvalidateTracer.WriteInfo("================");
        //            }
        //#endif

        //            HitTestCoreWithPrevChainHint(
        //              hitPointChain.LastestRootX,
        //              hitPointChain.LastestRootY);

        //            DisableGraphicOutputFlush = true;
        //            this.currentDragElem = null;

        //            //-----------------------------------------------------------------------

        //            ForEachEventListenerPreviewBubbleUp(this.hitPointChain, (hitobj, listener) =>
        //            {
        //                listener.PortalMouseMove(e);
        //                return true;
        //            });

        //            //-----------------------------------------------------------------------

        //            ForEachEventListenerBubbleUp(this.hitPointChain, (hit, listener) =>
        //            {
        //                currentDragElem = listener;
        //                listener.ListenDragEvent(UIDragEventName.DragStart, e);
        //                return true;
        //            });
        //            DisableGraphicOutputFlush = false;
        //            FlushAccumGraphicUpdate();

        //            hitPointChain.SwapHitChain();
        //        }
        //        public override void OnDrag(UIMouseEventArgs e)
        //        {
        //            if (currentDragElem == null)
        //            {
        //                return;
        //            }

        //#if DEBUG
        //            this.dbugRootGraphic.dbugEventIsDragging = true;
        //#endif

        //            //if (currentDragingElement == null)
        //            //{

        //            //    return;
        //            //}
        //            //else
        //            //{
        //            //}

        //            //--------------

        //            DisableGraphicOutputFlush = true;

        //            currentDragElem.ListenDragEvent(UIDragEventName.Dragging, e);

        //            DisableGraphicOutputFlush = false;
        //            FlushAccumGraphicUpdate();

        //            //Point globalDragingElementLocation = currentDragingElement.GetGlobalLocation();
        //            //e.TranslateCanvasOrigin(globalDragingElementLocation);
        //            //e.SourceHitElement = currentDragingElement;
        //            //Point dragPoint = hitPointChain.PrevHitPoint;
        //            //dragPoint.Offset(currentXDistanceFromDragPoint, currentYDistanceFromDragPoint);
        //            //e.Location = dragPoint;
        //            //e.DragingElement = currentDragingElement;

        //            //IEventListener ui = currentDragingElement.GetController() as IEventListener;
        //            //if (ui != null)
        //            //{
        //            //    ui.ListenDragEvent(UIDragEventName.Dragging, e);
        //            //}
        //            //e.TranslateCanvasOriginBack();


        //        }


        //        public override void OnDragStop(UIMouseEventArgs e)
        //        {

        //            if (currentDragElem == null)
        //            {
        //                return;
        //            }
        //#if DEBUG
        //            this.dbugRootGraphic.dbugEventIsDragging = false;
        //#endif

        //            DisableGraphicOutputFlush = true;

        //            currentDragElem.ListenDragEvent(UIDragEventName.DragStop, e);

        //            DisableGraphicOutputFlush = false;
        //            FlushAccumGraphicUpdate();

        //            //if (currentDragingElement == null)
        //            //{
        //            //    return;
        //            //}

        //            //DisableGraphicOutputFlush = true;

        //            //Point globalDragingElementLocation = currentDragingElement.GetGlobalLocation();
        //            //e.TranslateCanvasOrigin(globalDragingElementLocation);

        //            //Point dragPoint = hitPointChain.PrevHitPoint;
        //            //dragPoint.Offset(currentXDistanceFromDragPoint, currentYDistanceFromDragPoint);
        //            //e.Location = dragPoint;

        //            //e.SourceHitElement = currentDragingElement;
        //            //var script = currentDragingElement.GetController() as IEventListener;
        //            //if (script != null)
        //            //{
        //            //    script.ListenDragEvent(UIDragEventName.DragStop, e);
        //            //}

        //            //e.TranslateCanvasOriginBack();

        //            //UIMouseEventArgs d_eventArg = new UIMouseEventArgs();
        //            //if (hitPointChain.DragHitElementCount > 0)
        //            //{
        //            //    ForEachDraggingObjects(this.hitPointChain, (hitobj, listener) =>
        //            //    {
        //            //        //d_eventArg.TranslateCanvasOrigin(globalLocation);
        //            //        //d_eventArg.SourceHitElement = elem;
        //            //        //d_eventArg.DragingElement = currentDragingElement;

        //            //        //var script2 = elem.GetController();
        //            //        //if (script2 != null)
        //            //        //{
        //            //        //}

        //            //        //d_eventArg.TranslateCanvasOriginBack();
        //            //        return true;
        //            //    });
        //            //    //foreach (RenderElement elem in hitPointChain.GetDragHitElementIter())
        //            //    //{
        //            //    //    Point globalLocation = elem.GetGlobalLocation();
        //            //    //    d_eventArg.TranslateCanvasOrigin(globalLocation);
        //            //    //    d_eventArg.SourceHitElement = elem;
        //            //    //    d_eventArg.DragingElement = currentDragingElement;

        //            //    //    var script2 = elem.GetController();
        //            //    //    if (script2 != null)
        //            //    //    {
        //            //    //    }

        //            //    //    d_eventArg.TranslateCanvasOriginBack();
        //            //    //}
        //            //} 
        //            DisableGraphicOutputFlush = false;
        //            FlushAccumGraphicUpdate();
        //        }

#if DEBUG

        //void BroadcastDragHitEvents(UIMouseEventArgs e)
        //{
        //    //Point globalDragingElementLocation = currentDragingElement.GetGlobalLocation();
        //    //Rectangle dragRect = currentDragingElement.GetGlobalRect();

        //    //VisualDrawingChain drawingChain = this.WinRootPrepareRenderingChain(dragRect);

        //    //List<RenderElement> selVisualElements = drawingChain.selectedVisualElements;
        //    //int j = selVisualElements.Count;
        //    //LinkedList<RenderElement> underlyingElements = new LinkedList<RenderElement>();
        //    //for (int i = j - 1; i > -1; --i)
        //    //{

        //    //    if (selVisualElements[i].ListeningDragEvent)
        //    //    {
        //    //        underlyingElements.AddLast(selVisualElements[i]);
        //    //    }
        //    //}

        //    //if (underlyingElements.Count > 0)
        //    //{
        //    //    foreach (RenderElement underlyingUI in underlyingElements)
        //    //    {

        //    //        if (underlyingUI.IsDragedOver)
        //    //        {   
        //    //            hitPointChain.RemoveDragHitElement(underlyingUI);
        //    //            underlyingUI.IsDragedOver = false;
        //    //        }
        //    //    }
        //    //}
        //    //UIMouseEventArgs d_eventArg = UIMouseEventArgs.GetFreeDragEventArgs();

        //    //if (hitPointChain.DragHitElementCount > 0)
        //    //{
        //    //    foreach (RenderElement elem in hitPointChain.GetDragHitElementIter())
        //    //    {
        //    //        Point globalLocation = elem.GetGlobalLocation();
        //    //        d_eventArg.TranslateCanvasOrigin(globalLocation);
        //    //        d_eventArg.SourceVisualElement = elem;
        //    //        var script = elem.GetController();
        //    //        if (script != null)
        //    //        {
        //    //        }
        //    //        d_eventArg.TranslateCanvasOriginBack();
        //    //    }
        //    //}
        //    //hitPointChain.ClearDragHitElements();

        //    //foreach (RenderElement underlyingUI in underlyingElements)
        //    //{

        //    //    hitPointChain.AddDragHitElement(underlyingUI);
        //    //    if (underlyingUI.IsDragedOver)
        //    //    {
        //    //        Point globalLocation = underlyingUI.GetGlobalLocation();
        //    //        d_eventArg.TranslateCanvasOrigin(globalLocation);
        //    //        d_eventArg.SourceVisualElement = underlyingUI;

        //    //        var script = underlyingUI.GetController();
        //    //        if (script != null)
        //    //        {
        //    //        }

        //    //        d_eventArg.TranslateCanvasOriginBack();
        //    //    }
        //    //    else
        //    //    {
        //    //        underlyingUI.IsDragedOver = true;
        //    //        Point globalLocation = underlyingUI.GetGlobalLocation();
        //    //        d_eventArg.TranslateCanvasOrigin(globalLocation);
        //    //        d_eventArg.SourceVisualElement = underlyingUI;

        //    //        var script = underlyingUI.GetController();
        //    //        if (script != null)
        //    //        {
        //    //        }

        //    //        d_eventArg.TranslateCanvasOriginBack();
        //    //    }
        //    //}
        //    //UIMouseEventArgs.ReleaseEventArgs(d_eventArg);
        //} 
        static int dbugTotalId;
        public readonly int dbugId = dbugTotalId++;
        MyRootGraphic dbugRootGfx;
        MyRootGraphic dbugRootGraphics
        {
            get { return dbugRootGfx; }
            set
            {

                this.dbugRootGfx = value;
                _previousChain.dbugHitTracker = this.dbugRootGraphics.dbugHitTracker;
            }
        }
#endif
    }
}