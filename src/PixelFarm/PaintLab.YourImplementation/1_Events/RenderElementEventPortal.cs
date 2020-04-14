//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.RenderBoxes;
using LayoutFarm.UI.ForImplementator;
namespace LayoutFarm.UI
{
    public class RenderElementEventPortal : IEventPortal
    {


        /// <summary>
        /// a helper class for mouse press monitor
        /// </summary>
        class MousePressMonitorHelper
        {
            /// <summary>
            /// interval in millisec for mouse press
            /// </summary>
            int _intervalMs;
            int _mousePressCount;
            IUIEventListener _currentMonitoredElem;
            readonly UITimerTask _mousePressMonitor;
            readonly UIMousePressEventArgs _mousePressEventArgs;

            public MousePressMonitorHelper(int intervalMs)
            {
                _intervalMs = intervalMs;
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
                _mousePressMonitor.IntervalInMillisec = intervalMs; //interval for mouse press monitor
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
                _mousePressEventArgs.CurrentContextElement = ui;
                _currentMonitoredElem.ListenMousePress(_mousePressEventArgs);
            }
            public void AddMousePressInformation(UIMouseDownEventArgs importInfo)
            {
                _mousePressEventArgs.Button = importInfo.Buttons;
            }
            public bool HasMonitoredElem => _currentMonitoredElem != null;

        }


        //current hit chain        
        HitChain _previousChain = new HitChain();
        Stack<HitChain> _hitChainStack = new Stack<HitChain>();
        readonly RenderElement _topRenderElement;
        readonly MousePressMonitorHelper _mousePressMonitor;
        readonly UIMouseLeaveEventArgs _mouseLeaveEventArgs = new UIMouseLeaveEventArgs();
        readonly UIMouseLostFocusEventArgs _mouseLostFocusArgs = new UIMouseLostFocusEventArgs();
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
                e.SetExactHitObject(hitChain.GetHitInfo(count - 1).HitElemAsRenderElement);
            }
            else
            {
                e.SetExactHitObject(null);
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

        IUIEventListener _currentMouseWheel = null;
        void IEventPortal.PortalMouseWheel(UIMouseWheelEventArgs e)
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

                //portal                
                ForEachOnlyEventPortalBubbleUp(e, hitPointChain, (e1, portal) =>
                {
                    //please ensure=> no local var/pararmeter capture inside lambda
                    portal.PortalMouseWheel(e1);
                    //*****
                    _currentMouseWheel = e1.CurrentContextElement;
                    return true;
                });
                //------------------------------
                //use events
                if (!e.CancelBubbling)
                {
                    _currentMouseWheel = null;
                    e.SetCurrentContextElement(null);//clear

                    ForEachEventListenerBubbleUp(e, hitPointChain, (e1, listener) =>
                    {
                        //please ensure=> no local var/pararmeter capture inside lambda
                        if (listener.BypassAllMouseEvents)
                        {
                            return false;
                        }
                        _currentMouseWheel = listener;
                        listener.ListenMouseWheel(e1);

#if DEBUG
                        if (e1.CancelBubbling)
                        {

                        }
#endif

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


        internal IUIEventListener _prevMouseDownElement;
        IUIEventListener _currentMouseDown;

        void IEventPortal.PortalMouseDown(UIMouseDownEventArgs e)
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

                _currentMouseDown = null;
                //portal                
                ForEachOnlyEventPortalBubbleUp(e, hitPointChain, (e1, portal) =>
                {
                    //please ensure=> no local var/pararmeter capture inside lambda
                    portal.PortalMouseDown(e1);
                    //*****
                    _currentMouseDown = e1.CurrentContextElement;
                    return true;
                });
                //------------------------------
                //use events
                if (!e.CancelBubbling)
                {
                    _currentMouseDown = null; //clear 
                    e.SetCurrentContextElement(null);

                    ForEachEventListenerBubbleUp(e, hitPointChain, (e1, listener) =>
                    {
                        //please ensure=> no local var/pararmeter capture inside lambda
                        if (listener.BypassAllMouseEvents)
                        {
                            return false;
                        }
                        _currentMouseDown = listener;
                        listener.ListenMouseDown(e1);

                        //------------------------------------------------------- 
                        //auto begin monitor mouse press 
                        _mousePressMonitor.AddMousePressInformation(e);
                        _mousePressMonitor.SetMonitoredElement(listener);
                        //------------------------------------------------------- 
                        bool cancelMouseBubbling = e1.CancelBubbling;
                        if (_prevMouseDownElement != null &&
                            _prevMouseDownElement != listener)
                        {
                            _prevMouseDownElement.ListenLostMouseFocus(_mouseLostFocusArgs);
                            _prevMouseDownElement = null;//clear
                        }
                        //------------------------------------------------------- 
                        //retrun true to stop this loop (no further bubble up)
                        //return false to bubble this to upper control 
                        return e1.CancelBubbling || !listener.BypassAllMouseEvents;
                    });

                    if (_currentMouseDown == null)
                    {
                        _mousePressMonitor.Reset();
                    }

                }

                if (_prevMouseDownElement != _currentMouseDown &&
                    _prevMouseDownElement != null)
                {
                    //TODO: review here, auto or manual
                    _prevMouseDownElement.ListenLostMouseFocus(_mouseLostFocusArgs);
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


        bool _mouseMoveFoundSomeHit = false;


        internal IUIEventListener _latestMouseActive;

        void IEventPortal.PortalMouseLeaveFromViewport()
        {
            //mouse out from viewport
            
            if (_latestMouseActive != null)
            {
                _mouseLeaveEventArgs.IsDragging = false;
                UIMouseLeaveEventArgs.SetDiff(_mouseLeaveEventArgs, 0, 0);
                _mouseLeaveEventArgs.SetCurrentContextElement(null);

                _latestMouseActive.ListenMouseLeave(_mouseLeaveEventArgs);
                _latestMouseActive = null;
            }
        }

        bool _mouseMoveFoundLastMouseActive;
        void IEventPortal.PortalMouseMove(UIMouseMoveEventArgs e)
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
                _mouseMoveFoundLastMouseActive = false;
                ForEachEventListenerBubbleUp(e, hitPointChain, (e1, listener) =>
                {
                    //please ensure=> no local var/pararmeter capture inside lambda
                    _mouseMoveFoundSomeHit = true; 

                    if (_latestMouseActive != listener && !_mouseMoveFoundLastMouseActive)
                    {
                        //----------                        
                        listener.ListenMouseEnter(e1);
                        //----------

                        if (_latestMouseActive != null)
                        {
                            _mouseLeaveEventArgs.SetCurrentContextElement(_latestMouseActive);
                            UIMouseLeaveEventArgs.SetDiff(_mouseLeaveEventArgs, e1.XDiff, e1.YDiff);
                            _latestMouseActive.ListenMouseLeave(_mouseLeaveEventArgs);
                        }

                        _latestMouseActive = listener;
                    }


                    if (!e1.IsCanceled)
                    {
                        //TODO: review here
                        e1.CancelBubbling = true;
                        listener.ListenMouseMove(e1);

                        if (!_mouseMoveFoundLastMouseActive)
                        {
                            _latestMouseActive = e1.CurrentContextElement;
                        }
                    }

                    if (!e1.CancelBubbling)
                    {
                        _mouseMoveFoundLastMouseActive = true;
                    }
                    return e1.CancelBubbling;
                });

                if (!_mouseMoveFoundSomeHit)
                {

                    if (_latestMouseActive != null)
                    {
                        _mouseLeaveEventArgs.IsDragging = e.IsDragging;
                        UIMouseLeaveEventArgs.SetDiff(_mouseLeaveEventArgs, e.XDiff, e.YDiff);
                        _mouseLeaveEventArgs.SetCurrentContextElement(_latestMouseActive);

                        _latestMouseActive.ListenMouseLeave(_mouseLeaveEventArgs);
                        _latestMouseActive = null;
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
        void IEventPortal.PortalMouseUp(UIMouseUpEventArgs e)
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

                        //return true to stop this loop (no further bubble up)
                        //return false to bubble this to upper control       
                        //click or double click
                        if (e1.CurrentContextElement == _currentMouseDown)
                        {
                            if (e.IsAlsoDoubleClick)
                            {
                                listener.ListenMouseDoubleClick(e);
                            }
                            else
                            {
                                listener.ListenMouseClick(e);
                            }
                        }

                        return e1.CancelBubbling || !listener.BypassAllMouseEvents;
                    });
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
                object currentHitObj = hitPoint.HitElemAsRenderElement.GetController();
                if (currentHitObj is IEventPortal eventPortal)
                {
                    Point p = hitPoint.point;
                    e.SetCurrentContextElement(currentHitObj as IUIEventListener);
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
                        e.SetSourceHitObject(listener);
                    }

                    Point p = hitInfo.point;
                    e.SetLocation(p.X, p.Y);
                    e.SetCurrentContextElement(listener);
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