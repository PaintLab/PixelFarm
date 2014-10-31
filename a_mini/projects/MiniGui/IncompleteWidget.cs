//----------------------------------------------------------------------------
// Anti-Grain Geometry - Version 2.4
// Copyright (C) 2002-2005 Maxim Shemanarev (http://www.antigrain.com)
//
// C# Port port by: Lars Brubaker
//                  larsbrubaker@gmail.com
// Copyright (C) 2007
//
// Permission to copy, use, modify, sell and distribute this software 
// is granted provided this copyright notice appears in all copies. 
// This software is provided "as is" without express or implied
// warranty, and with no claim as to its suitability for any purpose.
//
//----------------------------------------------------------------------------
// Contact: mcseem@antigrain.com
//          mcseemagg@yahoo.com
//          http://www.antigrain.com
//----------------------------------------------------------------------------
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using PixelFarm.Agg.Image;
using PixelFarm.Agg.Transform;
using PixelFarm.VectorMath;

namespace PixelFarm.Agg.UI
{


    public enum UnderMouseState { NotUnderMouse, UnderMouseNotFirst, FirstUnderMouse };

    /// <summary>
    /// incomplete widget, for test Agg Core Only
    /// </summary>
    public class IncompleteWidget
    {
        // this should probably some type of dirty rects with the current invalid set stored.
        bool isCurrentlyInvalid = true;

        public static bool DebugBoundsUnderMouse = false;


        ActualImage backBuffer;

        bool debugShowBounds = false;

        public bool DebugShowBounds
        {
            get
            {
                if (DebugBoundsUnderMouse)
                {
                    if (UnderMouseState != UI.UnderMouseState.NotUnderMouse)
                    {
                        return true;
                    }
                }

                return debugShowBounds;
            }

            set
            {
                debugShowBounds = value;
            }
        }


        private UnderMouseState underMouseState = UnderMouseState.NotUnderMouse;

        public UnderMouseState UnderMouseState
        {
            get
            {
                return underMouseState;
            }
        }

        static public bool DefaultEnforceIntegerBounds
        {
            get;
            set;
        }


        public bool FirstWidgetUnderMouse
        {
            get { return this.UnderMouseState == UnderMouseState.FirstUnderMouse; }
        }

        private bool visible = true;
        private bool enabled = true;

        bool selectable = true;
        public bool Selectable
        {
            get { return selectable; }
        }

        enum MouseCapturedState { NotCaptured, ChildHasMouseCaptured, ThisHasMouseCaptured };
        private MouseCapturedState mouseCapturedState;


        ColorRGBA backgroundColor = new ColorRGBA();
        public ColorRGBA BackgroundColor
        {
            get { return backgroundColor; }

        }

        [ConditionalAttribute("DEBUG")]
        public static void ThrowExceptionInDebug(string description)
        {
            throw new Exception(description);
        }





        protected Transform.Affine parentToChildTransform = Affine.IdentityMatrix;
        List<IncompleteWidget> children = new List<IncompleteWidget>();

        private bool containsFocus = false;




        public delegate void MouseEventHandler(object sender, MouseEventArgs mouseEvent);
        /// <summary>
        /// The mouse has gone down while in the bounds of this widget
        /// </summary>
        public event MouseEventHandler MouseDownInBounds;
        /// <summary>
        /// The mouse has gon down on this widget. This will not trigger if a child of this widget gets the down message.
        /// </summary>
        public event MouseEventHandler MouseDown;
        public event MouseEventHandler MouseUp;
        public event MouseEventHandler MouseWheel;
        public event MouseEventHandler MouseMove;

        /// <summary>
        /// The mouse has entered the bounds of this widget.  It may also be over a child.
        /// </summary>
        public event EventHandler MouseEnterBounds;
        /// <summary>
        /// The mouse has left the bounds of this widget.
        /// </summary>
        public event EventHandler MouseLeaveBounds;


        /// <summary>
        /// The mouse has left this widget but may still be over the bounds, it could be above a child.
        /// </summary>
        public event EventHandler MouseLeave;


        public event EventHandler BoundsChanged;





        static readonly RectangleDouble largestValidBounds = new RectangleDouble(-1000000, -1000000, 1000000, 1000000);


        public virtual List<IncompleteWidget> Children
        {
            get
            {
                return children;
            }
        }




        /// <summary>
        /// This will return the backBuffer object for widgets that are double buffered.  It will return null if they are not.
        /// </summary>
        public ImageBase BackBuffer
        {
            get
            {
                if (DoubleBuffer)
                {
                    return backBuffer;
                }

                return null;
            }
        }

        bool doubleBuffer;
        public bool DoubleBuffer
        {
            get
            {
                return doubleBuffer;
            }
        }
        private Vector2 minimumSize = new Vector2();
        public virtual Vector2 MinimumSize
        {
            get
            {
                return minimumSize;
            }
        }



        private Vector2 maximumSize = new Vector2(double.MaxValue, double.MaxValue);
        public Vector2 MaximumSize
        {
            get
            {
                return maximumSize;
            }
        }

        public virtual Vector2 OriginRelativeParent
        {
            get
            {
                Affine tempLocalToParentTransform = ParentToChildTransform;
                Vector2 originRelParent = new Vector2(tempLocalToParentTransform.tx, tempLocalToParentTransform.ty);
                return originRelParent;
            }
        }


        RectangleDouble localBounds;
        public virtual RectangleDouble LocalBounds
        {
            get
            {
                return localBounds;
            }
        }



        public string Name
        {
            get;
            set;
        }
        public bool MouseCaptured
        {
            get { return (mouseCapturedState == MouseCapturedState.ThisHasMouseCaptured); }
        }

        public bool ChildHasMouseCaptured
        {
            get { return (mouseCapturedState == MouseCapturedState.ChildHasMouseCaptured); }
        }

        public bool Visible
        {
            get
            {
                return visible;
            }

        }

        public bool Enabled
        {
            get
            {
                IncompleteWidget curGUIWidget = this;
                while (curGUIWidget != null)
                {
                    if (!curGUIWidget.enabled)
                    {
                        return false;
                    }
                    curGUIWidget = curGUIWidget.Parent;
                }

                return true;
            }

        }




        private IncompleteWidget parentBackingStore = null;


        public IncompleteWidget Parent
        {
            get
            {
                return parentBackingStore;
            }
        }


        public virtual double Width
        {
            get
            {
                return LocalBounds.Width;
            }
        }

        public virtual double Height
        {
            get
            {
                return LocalBounds.Height;
            }
        }

       
        public virtual bool PositionWithinLocalBounds(double x, double y)
        {
            if (LocalBounds.Contains(x, y))
            {
                return true;
            }
            return false;
        }

        public void Invalidate()
        {
            Invalidate(LocalBounds);
        }

        public virtual void Invalidate(RectangleDouble rectToInvalidate)
        {
            isCurrentlyInvalid = true;
            if (Parent != null)
            {
                rectToInvalidate.Offset(OriginRelativeParent);
                Parent.Invalidate(rectToInvalidate);
            }


        }

        public virtual bool Focus()
        {
            if (CanFocus && CanSelect && !Focused)
            {
                List<IncompleteWidget> allWidgetsThatWillContainFocus = new List<IncompleteWidget>();
                List<IncompleteWidget> allWidgetsThatCurrentlyHaveFocus = new List<IncompleteWidget>();

                IncompleteWidget widgetNeedingFocus = this;
                while (widgetNeedingFocus != null)
                {
                    allWidgetsThatWillContainFocus.Add(widgetNeedingFocus);
                    widgetNeedingFocus = widgetNeedingFocus.Parent;
                }

                IncompleteWidget currentWithFocus = allWidgetsThatWillContainFocus[allWidgetsThatWillContainFocus.Count - 1];
                while (currentWithFocus != null)
                {
                    allWidgetsThatCurrentlyHaveFocus.Add(currentWithFocus);
                    IncompleteWidget childWithFocus = null;
                    foreach (IncompleteWidget child in currentWithFocus.Children)
                    {
                        if (child.ContainsFocus)
                        {
                            if (childWithFocus != null)
                            {
                                ThrowExceptionInDebug("Two children should never have focus.");
                            }
                            childWithFocus = child;
                        }
                    }
                    currentWithFocus = childWithFocus;
                }

                // Try to remove all the widgets we are giving focus to from all the ones that have it.
                // This will leave us with a list of all the widgets that need to lose focus.
                foreach (IncompleteWidget childThatWillNeedFocus in allWidgetsThatWillContainFocus)
                {
                    if (allWidgetsThatCurrentlyHaveFocus.Contains(childThatWillNeedFocus))
                    {
                        allWidgetsThatCurrentlyHaveFocus.Remove(childThatWillNeedFocus);
                    }
                }

                // take the focus away from all the widgets that will not have it after this focus.
                foreach (IncompleteWidget childThatIsLosingFocus in allWidgetsThatCurrentlyHaveFocus)
                {
                    childThatIsLosingFocus.Unfocus();
                }

                // and give focus to everything in our dirrect parent chain (including this).
                IncompleteWidget curWidget = this;
                do
                {
                    curWidget.containsFocus = true;
                    curWidget = curWidget.Parent;
                } while (curWidget != null);

                // finally call any delegates
                //OnFocus(null);

                return true;
            }

            return false;
        }

        public void Unfocus()
        {
            if (containsFocus == true)
            {
                if (Focused)
                {
                    containsFocus = false;
                    //OnLostFocus(null);
                    return;
                }

                containsFocus = false;
                foreach (IncompleteWidget child in Children)
                {
                    child.Unfocus();
                }
            }
        }



        public bool CanSelect
        {
            get
            {
                if (Selectable && Parent != null && AllParentsVisibleAndEnabled())
                {
                    return true;
                }

                return false;
            }
        }

        bool AllParentsVisibleAndEnabled()
        {
            IncompleteWidget curGUIWidget = this;
            while (curGUIWidget != null)
            {
                if (!curGUIWidget.Visible || !curGUIWidget.Enabled)
                {
                    return false;
                }
                curGUIWidget = curGUIWidget.Parent;
            }

            return true;
        }

        public virtual bool CanFocus
        {
            get { return Visible && Enabled; }
        }

        public bool Focused
        {
            get
            {
                if (ContainsFocus && CanFocus)
                {
                    foreach (IncompleteWidget child in Children)
                    {
                        if (child.ContainsFocus)
                        {
                            return false;
                        }
                    }

                    // we contain focus and none of our children do so we are focused.
                    return true;
                }

                return false;
            }
        }

        public bool ContainsFocus
        {
            get
            {
                return containsFocus;
            }
        }







        /// <summary>
        /// This is called before the OnDraw method.  
        /// When overriding OnPaintBackground in a derived class it is not necessary to call the base class's OnPaintBackground.
        /// </summary>
        /// <param name="graphics2D"></param>
        public virtual void OnDrawBackground(Graphics2D graphics2D)
        {
            if (BackgroundColor.Alpha0To255 > 0)
            {
                graphics2D.FillRectangle(LocalBounds, BackgroundColor);
            }
        }

        public virtual void OnDraw(Graphics2D graphics2D)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                IncompleteWidget child = Children[i];
                if (child.Visible)
                {
                    RectangleDouble oldClippingRect = graphics2D.GetClippingRect();
                    graphics2D.PushTransform();
                    {
                        Affine currentGraphics2DTransform = graphics2D.GetTransform();
                        Affine accumulatedTransform = currentGraphics2DTransform * child.ParentToChildTransform;
                        graphics2D.SetTransform(accumulatedTransform);

                        RectangleDouble currentScreenClipping;
                        if (child.CurrentScreenClipping(out currentScreenClipping))
                        {
                            currentScreenClipping.Left = Math.Floor(currentScreenClipping.Left);
                            currentScreenClipping.Right = Math.Ceiling(currentScreenClipping.Right);
                            currentScreenClipping.Bottom = Math.Floor(currentScreenClipping.Bottom);
                            currentScreenClipping.Top = Math.Ceiling(currentScreenClipping.Top);
                            if (currentScreenClipping.Right < currentScreenClipping.Left || currentScreenClipping.Top < currentScreenClipping.Bottom)
                            {
                                ThrowExceptionInDebug("Right is less than Left or Top is less than Bottom");
                            }

                            graphics2D.SetClippingRect(currentScreenClipping);

                            if (child.DoubleBuffer)
                            {
                                Vector2 offsetToRenderSurface = new Vector2(currentGraphics2DTransform.tx, currentGraphics2DTransform.ty);
                                offsetToRenderSurface += child.OriginRelativeParent;

                                double yFraction = offsetToRenderSurface.y - (int)offsetToRenderSurface.y;
                                double xFraction = offsetToRenderSurface.x - (int)offsetToRenderSurface.x;
                                int xOffset = (int)Math.Floor(child.LocalBounds.Left);
                                int yOffset = (int)Math.Floor(child.LocalBounds.Bottom);
                                if (child.isCurrentlyInvalid)
                                {
                                    Graphics2D childBackBufferGraphics2D = Graphics2D.CreateFromImage(child.backBuffer);//.NewGraphics2D();
                                    childBackBufferGraphics2D.Clear(new ColorRGBA(0, 0, 0, 0));
                                    Affine transformToBuffer = Affine.NewTranslation(-xOffset + xFraction, -yOffset + yFraction);
                                    childBackBufferGraphics2D.SetTransform(transformToBuffer);
                                    child.OnDrawBackground(childBackBufferGraphics2D);
                                    child.OnDraw(childBackBufferGraphics2D);

                                    child.backBuffer.MarkImageChanged();
                                    child.isCurrentlyInvalid = false;
                                }

                                offsetToRenderSurface.x = (int)offsetToRenderSurface.x + xOffset;
                                offsetToRenderSurface.y = (int)offsetToRenderSurface.y + yOffset;
                                // The transform to draw the backbuffer to the graphics2D must not have a factional amount
                                // or we will get aliasing in the image and we want our back buffer pixels to map 1:1 to the next buffer
                                if (offsetToRenderSurface.x - (int)offsetToRenderSurface.x != 0
                                    || offsetToRenderSurface.y - (int)offsetToRenderSurface.y != 0)
                                {
                                    ThrowExceptionInDebug("The transform for a back buffer must be integer to avoid aliasing.");
                                }
                                graphics2D.SetTransform(Affine.NewTranslation(offsetToRenderSurface));

                                graphics2D.Render(child.backBuffer, 0, 0);
                            }
                            else
                            {
                                child.OnDrawBackground(graphics2D);
                                child.OnDraw(graphics2D);
                            }
                        }
                    }
                    graphics2D.PopTransform();
                    graphics2D.SetClippingRect(oldClippingRect);
                }
            }

            if (DebugShowBounds)
            {
                graphics2D.dbugLine(LocalBounds.Left, LocalBounds.Bottom, LocalBounds.Right, LocalBounds.Top, ColorRGBA.Green);
                graphics2D.dbugLine(LocalBounds.Left, LocalBounds.Top, LocalBounds.Right, LocalBounds.Bottom, ColorRGBA.Green);
                graphics2D.Rectangle(LocalBounds, ColorRGBA.Red);
            }
            if (showSize)
            {
                graphics2D.DrawString(string.Format("{4} {0}, {1} : {2}, {3}", (int)MinimumSize.x, (int)MinimumSize.y, (int)LocalBounds.Width, (int)LocalBounds.Height, Name),
                    Width / 2, Math.Max(Height - 16, Height / 2 - 16 * graphics2D.TransformStackCount), color: ColorRGBA.Magenta, justification: Font.Justification.Center);
            }
        }

        static bool showSize = false;

        protected virtual bool CurrentScreenClipping(out RectangleDouble screenClipping)
        {
            screenClipping = TransformRectangleToScreenSpace(LocalBounds);

            if (Parent != null)
            {
                RectangleDouble screenParentClipping;
                if (!Parent.CurrentScreenClipping(out screenParentClipping))
                {
                    // the parent is completely clipped away, so this is too.
                    return false;
                }

                RectangleDouble intersectionRect = new RectangleDouble();
                if (!intersectionRect.IntersectRectangles(screenClipping, screenParentClipping))
                {
                    // this rect is clipped away by the parent rect so return false.
                    return false;
                }
                screenClipping = intersectionRect;
            }

            return true;
        }





        public RectangleDouble TransformRectangleToScreenSpace(RectangleDouble rectangleToTransform)
        {
            IncompleteWidget prevGUIWidget = this;
            while (prevGUIWidget != null)
            {
                rectangleToTransform.Offset(prevGUIWidget.OriginRelativeParent);
                prevGUIWidget = prevGUIWidget.Parent;
            }

            return rectangleToTransform;
        }




        void DoMouseMovedOffWidgetRecursive(MouseEventArgs mouseEvent)
        {
            foreach (IncompleteWidget child in Children)
            {
                double childX = mouseEvent.X;
                double childY = mouseEvent.Y;
                child.ParentToChildTransform.InverseTransform(ref childX, ref childY);
                MouseEventArgs childMouseEvent = new MouseEventArgs(mouseEvent, childX, childY);
                child.DoMouseMovedOffWidgetRecursive(childMouseEvent);
            }

            bool needToCallLeaveBounds = underMouseState != UI.UnderMouseState.NotUnderMouse;
            bool needToCallLeave = UnderMouseState == UI.UnderMouseState.FirstUnderMouse;

            underMouseState = UI.UnderMouseState.NotUnderMouse;

            if (needToCallLeave)
            {
                OnMouseLeave(mouseEvent);
            }

            if (needToCallLeaveBounds)
            {
                OnMouseLeaveBounds(mouseEvent);
            }
        }



        public virtual void OnMouseDown(MouseEventArgs mouseEvent)
        {
            if (PositionWithinLocalBounds(mouseEvent.X, mouseEvent.Y))
            {
                bool childHasAcceptedThisEvent = false;
                bool childHasTakenFocus = false;
                for (int i = Children.Count - 1; i >= 0; i--)
                {
                    IncompleteWidget child = Children[i];
                    double childX = mouseEvent.X;
                    double childY = mouseEvent.Y;
                    child.ParentToChildTransform.InverseTransform(ref childX, ref childY);

                    MouseEventArgs childMouseEvent = new MouseEventArgs(mouseEvent, childX, childY);
                    if (childHasAcceptedThisEvent)
                    {
                        // another child already took the down so no one else can.
                        child.DoMouseMovedOffWidgetRecursive(childMouseEvent);
                    }
                    else
                    {
                        if (child.Visible && child.Enabled && child.CanSelect)
                        {
                            if (child.PositionWithinLocalBounds(childX, childY))
                            {
                                childHasAcceptedThisEvent = true;
                                child.OnMouseDown(childMouseEvent);
                                if (child.ContainsFocus)
                                {
                                    childHasTakenFocus = true;
                                }
                            }
                            else
                            {
                                child.DoMouseMovedOffWidgetRecursive(childMouseEvent);
                                child.Unfocus();
                            }
                        }
                    }
                }

                bool mouseEnteredBounds = underMouseState == UI.UnderMouseState.NotUnderMouse;

                if (childHasAcceptedThisEvent)
                {
                    mouseCapturedState = MouseCapturedState.ChildHasMouseCaptured;
                    if (UnderMouseState == UI.UnderMouseState.FirstUnderMouse)
                    {
                        underMouseState = UI.UnderMouseState.NotUnderMouse;
                        OnMouseLeave(mouseEvent);
                    }
                    underMouseState = UI.UnderMouseState.UnderMouseNotFirst;
                }
                else
                {
                    mouseCapturedState = MouseCapturedState.ThisHasMouseCaptured;
                    if (!FirstWidgetUnderMouse)
                    {
                        underMouseState = UI.UnderMouseState.FirstUnderMouse;
                        //OnMouseEnter(mouseEvent);
                    }

                    if (MouseDown != null)
                    {
                        MouseDown(this, mouseEvent);
                    }
                }

                if (mouseEnteredBounds)
                {
                    OnMouseEnterBounds(mouseEvent);
                }

                if (!childHasTakenFocus)
                {
                    if (CanFocus)
                    {
                        Focus();
                    }
                }

                if (MouseDownInBounds != null)
                {
                    MouseDownInBounds(this, mouseEvent);
                }
            }
            else if (UnderMouseState != UI.UnderMouseState.NotUnderMouse)
            {
                Unfocus();
                mouseCapturedState = MouseCapturedState.NotCaptured;

                OnMouseLeaveBounds(mouseEvent);
                if (UnderMouseState == UI.UnderMouseState.FirstUnderMouse)
                {
                    OnMouseLeave(mouseEvent);
                }
                DoMouseMovedOffWidgetRecursive(mouseEvent);
            }
        }

        internal bool mouseMoveEventHasBeenAcceptedByOther = false;
        public virtual void OnMouseMove(MouseEventArgs mouseEvent)
        {
            mouseMoveEventHasBeenAcceptedByOther = false;
            if (mouseCapturedState == MouseCapturedState.NotCaptured)
            {
                OnMouseMoveNotCaptured(mouseEvent);
            }
            else // either this or a child has the mouse captured
            {
                OnMouseMoveWhenCaptured(mouseEvent);
            }
        }

        private void OnMouseMoveWhenCaptured(MouseEventArgs mouseEvent)
        {
            if (mouseCapturedState == MouseCapturedState.ChildHasMouseCaptured)
            {
                int countOfChildernThatThinkTheyHaveTheMouseCaptured = 0;
                foreach (IncompleteWidget child in Children)
                {
                    double childX = mouseEvent.X;
                    double childY = mouseEvent.Y;
                    child.ParentToChildTransform.InverseTransform(ref childX, ref childY);
                    MouseEventArgs childMouseEvent = new MouseEventArgs(mouseEvent, childX, childY);
                    if (child.mouseCapturedState != MouseCapturedState.NotCaptured)
                    {
                        child.OnMouseMove(childMouseEvent);
                        countOfChildernThatThinkTheyHaveTheMouseCaptured++;
                    }
                }

                if (countOfChildernThatThinkTheyHaveTheMouseCaptured < 1 || countOfChildernThatThinkTheyHaveTheMouseCaptured > 1)
                {
                    ThrowExceptionInDebug("One and only one child should ever have the mouse captured.");
                }
            }
            else
            {
                if (mouseCapturedState != MouseCapturedState.ThisHasMouseCaptured)
                {
                    ThrowExceptionInDebug("You should only ever get here if you have the mouse captured.");
                }

                if (PositionWithinLocalBounds(mouseEvent.X, mouseEvent.Y))
                {
                    if (!FirstWidgetUnderMouse)
                    {
                        underMouseState = UI.UnderMouseState.FirstUnderMouse;
                        //OnMouseEnter(mouseEvent);
                        OnMouseEnterBounds(mouseEvent);
                    }
                    else if (underMouseState == UI.UnderMouseState.NotUnderMouse)
                    {
                        underMouseState = UI.UnderMouseState.FirstUnderMouse;
                        OnMouseEnterBounds(mouseEvent);
                    }

                    underMouseState = UI.UnderMouseState.FirstUnderMouse;
                }
                else
                {
                    if (FirstWidgetUnderMouse)
                    {
                        underMouseState = UI.UnderMouseState.NotUnderMouse;
                        OnMouseLeave(mouseEvent);
                        OnMouseLeaveBounds(mouseEvent);
                    }
                    else if (underMouseState != UI.UnderMouseState.NotUnderMouse)
                    {
                        underMouseState = UI.UnderMouseState.NotUnderMouse;
                        OnMouseLeaveBounds(mouseEvent);
                    }

                    underMouseState = UI.UnderMouseState.NotUnderMouse;
                }

                if (MouseMove != null)
                {
                    MouseMove(this, mouseEvent);
                }
            }
        }

        private void OnMouseMoveNotCaptured(MouseEventArgs mouseEvent)
        {
            if (Parent != null && Parent.mouseMoveEventHasBeenAcceptedByOther)
            {
                mouseMoveEventHasBeenAcceptedByOther = true;
            }

            for (int i = Children.Count - 1; i >= 0; i--)
            {
                IncompleteWidget child = Children[i];
                double childX = mouseEvent.X;
                double childY = mouseEvent.Y;
                child.ParentToChildTransform.InverseTransform(ref childX, ref childY);
                MouseEventArgs childMouseEvent = new MouseEventArgs(mouseEvent, childX, childY);
                if (child.Visible && child.Enabled && child.CanSelect)
                {
                    child.OnMouseMove(childMouseEvent);
                    if (child.PositionWithinLocalBounds(childX, childY))
                    {
                        mouseMoveEventHasBeenAcceptedByOther = true;
                    }
                }
            }

            if (PositionWithinLocalBounds(mouseEvent.X, mouseEvent.Y))
            {
                bool needToCallEnterBounds = underMouseState == UI.UnderMouseState.NotUnderMouse;

                if (mouseMoveEventHasBeenAcceptedByOther)
                {
                    if (UnderMouseState == UI.UnderMouseState.FirstUnderMouse)
                    {
                        // set it before we call the function to have the state right to the callee
                        underMouseState = UI.UnderMouseState.UnderMouseNotFirst;
                        OnMouseLeave(mouseEvent);
                    }
                    underMouseState = UI.UnderMouseState.UnderMouseNotFirst;
                }
                else
                {
                    if (!FirstWidgetUnderMouse)
                    {
                        if (mouseMoveEventHasBeenAcceptedByOther)
                        {
                            underMouseState = UI.UnderMouseState.UnderMouseNotFirst;
                        }
                        else
                        {
                            underMouseState = UI.UnderMouseState.FirstUnderMouse;
                            //OnMouseEnter(mouseEvent);
                        }
                    }
                    else // we are the first under mouse
                    {
                        if (mouseMoveEventHasBeenAcceptedByOther)
                        {
                            underMouseState = UI.UnderMouseState.UnderMouseNotFirst;
                            OnMouseLeave(mouseEvent);
                        }
                    }
                }

                if (needToCallEnterBounds)
                {
                    OnMouseEnterBounds(mouseEvent);
                }

                if (MouseMove != null)
                {
                    MouseMove(this, mouseEvent);
                }
            }
            else if (UnderMouseState != UI.UnderMouseState.NotUnderMouse)
            {
                if (FirstWidgetUnderMouse)
                {
                    underMouseState = UI.UnderMouseState.NotUnderMouse;
                    OnMouseLeave(mouseEvent);
                }
                underMouseState = UI.UnderMouseState.NotUnderMouse;
                OnMouseLeaveBounds(mouseEvent);
            }
        }

        int childrenLockedInMouseUpCount = 0;
        public virtual void OnMouseUp(MouseEventArgs mouseEvent)
        {
            if (childrenLockedInMouseUpCount != 0)
            {
                ThrowExceptionInDebug("This should not be locked.");
            }

            childrenLockedInMouseUpCount++;
            if (mouseCapturedState == MouseCapturedState.NotCaptured)
            {
                if (PositionWithinLocalBounds(mouseEvent.X, mouseEvent.Y))
                {
                    bool childHasAcceptedThisEvent = false;
                    for (int i = Children.Count - 1; i >= 0; i--)
                    {
                        IncompleteWidget child = Children[i];
                        double childX = mouseEvent.X;
                        double childY = mouseEvent.Y;
                        child.ParentToChildTransform.InverseTransform(ref childX, ref childY);
                        MouseEventArgs childMouseEvent = new MouseEventArgs(mouseEvent, childX, childY);
                        if (child.Visible && child.Enabled && child.CanSelect)
                        {
                            if (child.PositionWithinLocalBounds(childX, childY))
                            {
                                childHasAcceptedThisEvent = true;
                                child.OnMouseUp(childMouseEvent);
                                i = -1;
                            }
                            else
                            {
                                if (UnderMouseState != UI.UnderMouseState.NotUnderMouse)
                                {
                                    if (FirstWidgetUnderMouse)
                                    {
                                        OnMouseLeave(mouseEvent);
                                    }
                                    DoMouseMovedOffWidgetRecursive(mouseEvent);
                                    underMouseState = UI.UnderMouseState.NotUnderMouse;
                                }
                            }
                        }
                    }

                    if (!childHasAcceptedThisEvent)
                    {
                        if (MouseUp != null)
                        {
                            MouseUp(this, mouseEvent);
                        }
                    }
                }
            }
            else // either this or a child has the mouse captured
            {
                if (mouseCapturedState == MouseCapturedState.ChildHasMouseCaptured)
                {
                    if (childrenLockedInMouseUpCount != 1)
                    {
                        ThrowExceptionInDebug("The mouse should always be locked while in mouse up.");
                    }

                    int countOfChildernThatThinkTheyHaveTheMouseCaptured = 0;
                    foreach (IncompleteWidget child in Children)
                    {
                        if (childrenLockedInMouseUpCount != 1)
                        {
                            ThrowExceptionInDebug("The mouse should always be locked while in mouse up.");
                        }

                        double childX = mouseEvent.X;
                        double childY = mouseEvent.Y;
                        child.ParentToChildTransform.InverseTransform(ref childX, ref childY);
                        MouseEventArgs childMouseEvent = new MouseEventArgs(mouseEvent, childX, childY);
                        if (child.mouseCapturedState != MouseCapturedState.NotCaptured)
                        {
                            if (countOfChildernThatThinkTheyHaveTheMouseCaptured > 0)
                            {
                                ThrowExceptionInDebug("One and only one child should ever have the mouse captured.");
                            }
                            child.OnMouseUp(childMouseEvent);
                            countOfChildernThatThinkTheyHaveTheMouseCaptured++;
                        }
                    }
                }
                else
                {
                    if (mouseCapturedState != MouseCapturedState.ThisHasMouseCaptured)
                    {
                        ThrowExceptionInDebug("You should only ever get here if you have the mouse captured.");
                    }
                    if (MouseUp != null)
                    {
                        MouseUp(this, mouseEvent);
                    }
                }

                if (!PositionWithinLocalBounds(mouseEvent.X, mouseEvent.Y))
                {
                    if (UnderMouseState != UI.UnderMouseState.NotUnderMouse)
                    {
                        if (FirstWidgetUnderMouse)
                        {
                            underMouseState = UI.UnderMouseState.NotUnderMouse;
                            OnMouseLeave(mouseEvent);
                            OnMouseLeaveBounds(mouseEvent);
                        }
                        else
                        {
                            underMouseState = UI.UnderMouseState.NotUnderMouse;
                            OnMouseLeaveBounds(mouseEvent);
                        }
                        DoMouseMovedOffWidgetRecursive(mouseEvent);
                    }
                }

                ClearCapturedState();
            }
            childrenLockedInMouseUpCount--;

            if (childrenLockedInMouseUpCount != 0)
            {
                ThrowExceptionInDebug("This should not be locked.");
            }
        }




        public virtual void OnMouseLeave(MouseEventArgs mouseEvent)
        {
            if (MouseLeave != null)
            {
                MouseLeave(this, mouseEvent);
            }
        }



        protected virtual void OnMouseEnterBounds(MouseEventArgs mouseEvent)
        {
            if (MouseEnterBounds != null)
            {
                MouseEnterBounds(this, mouseEvent);
            }
        }

        protected virtual void OnMouseLeaveBounds(MouseEventArgs mouseEvent)
        {
            if (MouseLeaveBounds != null)
            {
                MouseLeaveBounds(this, mouseEvent);
            }
        }

        void ClearCapturedState()
        {
            if (MouseCaptured || ChildHasMouseCaptured)
            {
                foreach (IncompleteWidget child in Children)
                {
                    child.ClearCapturedState();
                }

                IncompleteWidget parent = this;
                while (parent != null)
                {
                    parent.mouseCapturedState = MouseCapturedState.NotCaptured;
                    parent = parent.Parent;
                }
            }
        }




        public Affine ParentToChildTransform
        {
            get
            {
                return parentToChildTransform;
            }

            set
            {
                parentToChildTransform = value;
            }
        }

    }
}
