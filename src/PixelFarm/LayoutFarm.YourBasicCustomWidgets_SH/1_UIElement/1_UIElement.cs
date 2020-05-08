//Apache2, 2014-present, WinterDev


using PixelFarm.Drawing;
using System.Collections.Generic;
namespace LayoutFarm.UI
{
    static class UILayoutQueue
    {
        static readonly Queue<UIElement> s_layoutQueue = new Queue<UIElement>();

        static UILayoutQueue()
        {
            LayoutFarm.EventQueueSystem.CentralEventQueue.RegisterEventQueue(ClearLayoutQueue);
        }
        internal static void AddToLayoutQueue(UIElement ui)
        {
            if (ui.IsInLayoutQueue) return;
            s_layoutQueue.Enqueue(ui);
            ui.IsInLayoutQueue = true;
        }
        public static void ClearLayoutQueue()
        {
            int count = s_layoutQueue.Count;

#if DEBUG

            //if (UIElement.s_dbugBreakOnSetBounds)
            //{
            //    for (int i = count - 1; i >= 0; --i)
            //    {
            //        UIElement ui = s_layoutQueue.Dequeue();
            //        ui.IsInLayoutQueue = false;
            //        if (ui.IsInLayoutQueue)
            //        {
            //            //should not occur
            //            throw new System.NotSupportedException();
            //        }
            //    }
            //    return;
            //}

            if (count > 0)
            {
                //System.Diagnostics.Debug.WriteLine("layout_queue:" + count);
            }
#endif

            LayoutUpdateArgs layoutArgs = null;

            for (int i = count - 1; i >= 0; --i)
            {
                UIElement ui = s_layoutQueue.Dequeue();
                ui.IsInLayoutQueue = false;
                UIElement.InvokeContentLayout(ui, layoutArgs);
#if DEBUG
                if (ui.IsInLayoutQueue)
                {
                    //should not occur
                    throw new System.NotSupportedException();
                }
#endif
            }
        }
    }

    public class LayoutUpdateArgs
    {
        public int AvailableWidth { get; set; }

    }

    public abstract class LayoutInstance
    {

        public abstract bool GetResultBounds(out RectangleF rects);
    }




    public abstract partial class UIElement : IUIEventListener
    {

#if DEBUG
        public bool dbugBreakMe;
        static int s_dbugTotalId;
        public readonly int dbugId = s_dbugTotalId++;

#endif


        //bounds
        float _left;
        float _top;
        float _right;
        float _bottom;

        bool _hide;
        protected bool _needContentLayout;
        protected bool _hasMinSize;
        internal object _collectionLinkNode; //optional, eg for linked-list node, RB-tree-node

        public UIElement()
        {
            //if (dbugId == 114)
            //{ 
            //}
        }

        public UIElement ParentUI { get; set; }
        /// <summary>
        /// update layout data from layout instance
        /// </summary>
        public virtual void UpdateLayout()
        {

        }
        public abstract RenderElement GetPrimaryRenderElement();
        public abstract RenderElement CurrentPrimaryRenderElement { get; }
        protected virtual bool HasReadyRenderElement => CurrentPrimaryRenderElement != null;
        public abstract void InvalidateGraphics();
        public virtual void Focus()
        {
            //make this keyboard focusable
            CurrentPrimaryRenderElement?.GetRoot()?.SetCurrentKeyboardFocus(this.CurrentPrimaryRenderElement);
        }
        public virtual void Blur()
        {
            CurrentPrimaryRenderElement?.GetRoot()?.SetCurrentKeyboardFocus(null);
        }

        public virtual void InvalidateOuterGraphics()
        {

        }
        public virtual bool Visible
        {
            get
            {
                if (this.HasReadyRenderElement)
                {
                    return CurrentPrimaryRenderElement.Visible;
                }
                else
                {
                    return !_hide;
                }
            }
            set
            {
                _hide = !value;
                this.CurrentPrimaryRenderElement?.SetVisible(value);
            }
        }

        public PixelFarm.Drawing.Point GetGlobalLocation()
        {
            RenderElement currentRenderE = this.CurrentPrimaryRenderElement;
            if (currentRenderE != null)
            {
                return currentRenderE.GetGlobalLocation();
            }
            else
            {
                return new PixelFarm.Drawing.Point((int)_left, (int)_top);
            }             
        }

        public PixelFarm.Drawing.Point GetLocation() => new PixelFarm.Drawing.Point((int)_left, (int)_top);

        public virtual void GetViewport(out int left, out int top)
        {
            left = top = 0;
        }

        public void GetElementBounds(
           out float left,
           out float top,
           out float right,
           out float bottom)
        {
            left = _left;
            top = _top;
            right = _right;
            bottom = _bottom;
        }

        protected void SetElementBoundsWH(float width, float height)
        {
#if DEBUG
            //if (s_dbugBreakOnSetBounds)
            //{
            //    if (dbugBreakMe)
            //    {

            //    }
            //    else if (!dbugIsImgBox)
            //    {

            //    }
            //}
#endif
            _right = _left + width;
            _bottom = _top + height;
        }
        protected void SetElementBoundsLTWH(float left, float top, float width, float height)
        {
#if DEBUG
            //if (s_dbugBreakOnSetBounds)
            //{
            //    if (dbugBreakMe)
            //    {

            //    }
            //    else if (!dbugIsImgBox)
            //    {

            //    }
            //}
#endif
            //change 'TransparentBounds' => not effect visual presentation
            _left = left;
            _top = top;
            _right = left + width;
            _bottom = top + height;
        }
        protected void SetElementBounds(float left, float top, float right, float bottom)
        {
#if DEBUG
            //if (s_dbugBreakOnSetBounds)
            //{
            //    if (dbugBreakMe)
            //    {

            //    }
            //    else if (!dbugIsImgBox)
            //    {

            //    }
            //}
#endif
            //change 'TransparentBounds' => not effect visual presentation
            _left = left;
            _top = top;
            _right = right;
            _bottom = bottom;
        }
        protected void SetElementBoundsLT(float left, float top)
        {

#if DEBUG
            //if (s_dbugBreakOnSetBounds)
            //{
            //    if (dbugBreakMe)
            //    {

            //    }
            //    else if (!dbugIsImgBox)
            //    {

            //    }
            //}
#endif


            _bottom = top + (_bottom - _top);
            _right = left + (_right - _left);
            _left = left;
            _top = top;
        }

#if DEBUG
        public static bool s_dbugBreakOnSetBounds = false;
#endif
        //-------------------------------------------------------
        protected float BoundWidth => _right - _left;
        protected float BoundHeight => _bottom - _top;
        protected float BoundTop => _top;
        protected float BoundLeft => _left;
        //-------------------------------------------------------
        //layout ...
        public virtual bool NeedContentLayout => false;
        internal bool IsInLayoutQueue { get; set; }
        //-------------------------------------------------------
        //events ...
        bool _transparentAllMouseEvents; //TODO: review here
        /// <summary>
        /// transparent for all mouse event?
        /// </summary>
        public bool TransparentForMouseEvents
        {
            get => _transparentAllMouseEvents;
            set
            {
                _transparentAllMouseEvents = value;
                if (this.HasReadyRenderElement)
                {
                    this.CurrentPrimaryRenderElement.TransparentForMouseEvents = value;
                }
            }
        }
        public bool AcceptKeyboardFocus { get; set; }
        public bool DisableAutoMouseCapture { get; set; }
        public bool AutoStopMouseEventPropagation { get; set; } //TODO: review this
        //
        protected virtual void OnShown()
        {
        }
        protected virtual void OnHide()
        {
        }
        protected virtual void OnLostKeyboardFocus(UIFocusEventArgs e)
        {
        }
        protected virtual void OnLostMouseFocus(UIMouseLostFocusEventArgs e)
        {
        }
        protected virtual void OnGotKeyboardFocus(UIFocusEventArgs e)
        {
        }
        protected virtual void OnDoubleClick(UIMouseEventArgs e)
        {
        }
        //-------------------------------------------------------
        protected virtual void OnMouseDown(UIMouseDownEventArgs e)
        {
        }
        protected virtual void OnMousePress(UIMousePressEventArgs e)
        {

        }
        protected virtual void OnMouseMove(UIMouseMoveEventArgs e)
        {
        }
        protected virtual void OnMouseUp(UIMouseUpEventArgs e)
        {
        }
        protected virtual void OnMouseEnter(UIMouseMoveEventArgs e)
        {
        }
        protected virtual void OnMouseLeave(UIMouseLeaveEventArgs e)
        {
        }
        protected virtual void OnMouseWheel(UIMouseWheelEventArgs e)
        {
        }
        protected virtual void OnMouseHover(UIMouseHoverEventArgs e)
        {
        }

        //------------------------------------------------------------
        protected virtual void OnKeyDown(UIKeyEventArgs e)
        {
        }
        protected virtual void OnKeyUp(UIKeyEventArgs e)
        {
        }
        protected virtual void OnKeyPress(UIKeyEventArgs e)
        {
        }
        protected virtual bool OnProcessDialogKey(UIKeyEventArgs e)
        {
            return false;
        }
        //------------------------------------------------------------
        public void InvalidateLayout()
        {
            //add to layout queue
            UILayoutQueue.AddToLayoutQueue(this);
        }
        public void SuspendLayout()
        {
            //temp
            UILayoutQueue.AddToLayoutQueue(this);
        }
        public void ResumeLayout()
        {
            //temp
            UILayoutQueue.AddToLayoutQueue(this);
        }
        public void SuspendGraphicsUpdate()
        {
            CurrentPrimaryRenderElement?.SuspendGraphicsUpdate();

        }
        public void ResumeGraphicsUpdate()
        {
            CurrentPrimaryRenderElement?.ResumeGraphicsUpdate();
        }
        public virtual void NotifyContentUpdate(UIElement childContent)
        {
            //
        }
        internal static void InvokeContentLayout(UIElement ui, LayoutUpdateArgs args)
        {
            //called by central layout queue
            ui.PerformContentLayout(args);
        }
        public virtual void PerformContentLayout(LayoutUpdateArgs args)
        {

        }
        public virtual SizeF CalculateMinimumSize(LayoutUpdateArgs args)
        {
            return new SizeF(_right - _left, _bottom - _top);
        }
        protected virtual void OnElementChanged()
        {
        }

        protected virtual void OnGuestMsg(UIGuestMsgEventArgs e)
        {
        }
        public static void UnsafeRemoveLinkedNode(UIElement ui)
        {
            ui._collectionLinkNode = null;
        }

        //---------
        protected virtual void OnAcceptVisitor(UIVisitor visitor)
        {

        }
        public static void AcceptVisitor(UIElement ui, UIVisitor visitor)
        {
            if (visitor.ReportEnterAndExit)
            {
                UIVisitor.InvokeOnEnter(visitor, ui);
                ui.OnAcceptVisitor(visitor);
                UIVisitor.InvokeOnExit(visitor, ui);
            }
            else
            {
                ui.OnAcceptVisitor(visitor);
            }

        }
#if DEBUG
        object dbugTagObject;
        public object dbugTag
        {
            get
            {
                return this.dbugTagObject;
            }
            set
            {
                this.dbugTagObject = value;
            }
        }
#endif
    }


}