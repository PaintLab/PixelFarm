//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.RenderBoxes;
namespace LayoutFarm.UI
{
    public sealed class MyRootGraphic : RootGraphic, ITopWindowEventRootProvider
    {

        List<ToNotifySizeChangedEvent> _tobeNotifySizeChangedList = new List<ToNotifySizeChangedEvent>();
        List<RenderElementRequest> _renderRequestList = new List<RenderElementRequest>();
        GraphicsTimerTaskManager _graphicTimerTaskMan;

        static object _normalUpdateTask = new object();
        readonly TopWindowEventRoot _topWindowEventRoot;
        readonly RenderBoxBase _topWindowRenderBox;

        RenderBoxBase _primaryContainerElement;

        RequestFont _defaultTextEditFont; //TODO: review here
        ITextService _ifonts;

        public MyRootGraphic(
            int width, int height,
            ITextService ifonts)
            : base(width, height)
        {


            this._ifonts = ifonts;
            this._graphicTimerTaskMan = new GraphicsTimerTaskManager(this);
            _defaultTextEditFont = new RequestFont("tahoma", 10);

#if DEBUG
            dbugCurrentGlobalVRoot = this;
            dbug_Init(null, null, null);
#endif

            //create default render box***
            this._topWindowRenderBox = new TopWindowRenderBox(this, width, height);
            this._topWindowEventRoot = new TopWindowEventRoot(this._topWindowRenderBox);
            this.SubscribeGraphicsIntervalTask(_normalUpdateTask,
                TaskIntervalPlan.Animation,
                20,
                (s, e) =>
                {
                    this.PrepareRender();
                    this.FlushAccumGraphics();
                });

            _primaryContainerElement = _topWindowRenderBox;
        }
        public override ITextService TextServices
        {
            get
            {
                return this._ifonts;
            }
        }

        public override RootGraphic CreateNewOne(int w, int h)
        {
            return new MyRootGraphic(w, h, this._ifonts);
        }
        public ITopWindowEventRoot TopWinEventPortal
        {
            get { return this._topWindowEventRoot; }
        }
        public override void TopDownRecalculateContent()
        {
            _topWindowRenderBox.TopDownReCalculateContentSize();
        }
        public override void InvalidateRootArea(Rectangle r)
        {
            InvalidateGraphicArea(_topWindowRenderBox, ref r);

        }
        public override void InvalidateRootGraphicArea(ref Rectangle elemClientRect, bool passSourceElem = false)
        {
            base.InvalidateGraphicArea(_topWindowRenderBox, ref elemClientRect, passSourceElem);
        }
        public override bool GfxTimerEnabled
        {
            get
            {
                return this._graphicTimerTaskMan.Enabled;
            }
            set
            {
                this._graphicTimerTaskMan.Enabled = value;
            }
        }

        public override IRenderElement TopWindowRenderBox => _topWindowRenderBox;

        public override void PrepareRender()
        {
            //clear layout queue before render*** 
            this.LayoutQueueClearing = true;
            InvokeClearingBeforeRender();
            this.LayoutQueueClearing = false;
            this.ClearRenderRequests();
            ClearNotificationSizeChangeList();
        }
        void ClearNotificationSizeChangeList()
        {
            LayoutFarm.EventQueueSystem.CentralEventQueue.InvokeEventQueue();

        }

        public override RequestFont DefaultTextEditFontInfo
        {
            get
            {
                return _defaultTextEditFont;
            }
        }
        public override void ClearRenderRequests()
        {
            if (this.VisualRequestCount > 0)
            {
                this.ClearVisualRequests();
            }
        }

        public override void CloseWinRoot()
        {
            if (_graphicTimerTaskMan != null)
            {
                this._graphicTimerTaskMan.CloseAllWorkers();
                this._graphicTimerTaskMan = null;
            }
        }

        public override void CaretStartBlink()
        {
            _graphicTimerTaskMan.StartCaretBlinkTask();
        }
        public override void CaretStopBlink()
        {
            _graphicTimerTaskMan.StopCaretBlinkTask();
        }

        ~MyRootGraphic()
        {
            if (_graphicTimerTaskMan != null)
            {
                this._graphicTimerTaskMan.CloseAllWorkers();
                this._graphicTimerTaskMan = null;
            }


#if DEBUG
            dbugHitTracker.Close();
#endif
        }

        //-------------------------------------------------------------------------------
        public override GraphicsTimerTask SubscribeGraphicsIntervalTask(
            object uniqueName,
            TaskIntervalPlan planName,
            int intervalMs,
            EventHandler<GraphicsTimerTaskEventArgs> tickhandler)
        {
            return this._graphicTimerTaskMan.SubscribeGraphicsTimerTask(uniqueName, planName, intervalMs, tickhandler);
        }
        public override void RemoveIntervalTask(object uniqueName)
        {
            this._graphicTimerTaskMan.UnsubscribeTimerTask(uniqueName);
        }
        //-------------------------------------------------------------------------------
        int VisualRequestCount
        {
            get
            {
                return _renderRequestList.Count;
            }
        }

        public override void AddChild(RenderElement renderE)
        {
            _primaryContainerElement.AddChild(renderE); 
        }

        public override void SetPrimaryContainerElement(RenderBoxBase renderBox)
        {
            if (renderBox == null)
            {
                //reset to default
                _primaryContainerElement = _topWindowRenderBox;
            }
            else
            {
                _primaryContainerElement = renderBox;
            }
        }
        void ClearVisualRequests()
        {
            int j = _renderRequestList.Count;
            for (int i = 0; i < j; ++i)
            {
                RenderElementRequest req = _renderRequestList[i];
                switch (req.req)
                {
                    case RequestCommand.AddToWindowRoot:
                        {
                            AddChild(req.ve);
                        }
                        break;
                    case RequestCommand.DoFocus:
                        {
                            //RenderElement ve = req.ve;
                            //wintop.CurrentKeyboardFocusedElement = ve;
                            //ve.InvalidateGraphic();

                        }
                        break;
                    case RequestCommand.InvalidateArea:
                        {
                            Rectangle r = (Rectangle)req.parameters;
                            this.InvalidateGraphicArea(req.ve, ref r);
                        }
                        break;
                }
            }
            _renderRequestList.Clear();
        }
        public override void SetCurrentKeyboardFocus(RenderElement renderElement)
        {
            if (renderElement == null)
            {
                this._topWindowEventRoot.CurrentKeyboardFocusedElement = null;
                return;
            }

            var owner = renderElement.GetController() as IUIEventListener;
            if (owner != null)
            {
                this._topWindowEventRoot.CurrentKeyboardFocusedElement = owner;
            }
        }



#if DEBUG

        static void dbug_WriteInfo(dbugVisualLayoutTracer debugVisualLay, dbugVisitorMessage msg, RenderElement ve)
        {
            if (debugVisualLay != null)
            {
                debugVisualLay.WriteInfo(msg.text, ve);
            }
        }
        static void dbug_BeginNewContext(dbugVisualLayoutTracer debugVisualLay, dbugVisitorMessage msg, RenderElement ve)
        {
            if (debugVisualLay != null)
            {
                debugVisualLay.BeginNewContext(); debugVisualLay.WriteInfo(msg.text, ve);
            }
        }
        static void dbug_EndCurrentContext(dbugVisualLayoutTracer debugVisualLay, dbugVisitorMessage msg, RenderElement ve)
        {
            if (debugVisualLay != null)
            {
                debugVisualLay.WriteInfo(msg.text, ve);
                debugVisualLay.EndCurrentContext();
            }
        }

        void dbug_DumpAllVisualElementProps(dbugLayoutMsgWriter writer)
        {
            //this.dbug_DumpVisualProps(writer);
            _topWindowRenderBox.dbug_DumpVisualProps(writer);
            writer.Add(new dbugLayoutMsg(this.TopWindowRenderBox, "FINISH"));
        }
        public void dbugShowRenderPart(DrawBoard canvasPage, Rectangle updateArea)
        {
            RootGraphic visualroot = this;
            if (visualroot.dbug_ShowRootUpdateArea)
            {
                canvasPage.FillRectangle(Color.FromArgb(50, Color.Black),
                     updateArea.Left, updateArea.Top,
                        updateArea.Width - 1, updateArea.Height - 1);
                canvasPage.FillRectangle(Color.White,
                     updateArea.Left, updateArea.Top, 5, 5);
                canvasPage.DrawRectangle(Color.Yellow,
                        updateArea.Left, updateArea.Top,
                        updateArea.Width - 1, updateArea.Height - 1);
                Color c_color = canvasPage.CurrentTextColor;
                canvasPage.CurrentTextColor = Color.White;
                canvasPage.DrawText(visualroot.dbug_RootUpdateCounter.ToString().ToCharArray(), updateArea.Left, updateArea.Top);
                if (updateArea.Height > 25)
                {
                    canvasPage.DrawText(visualroot.dbug_RootUpdateCounter.ToString().ToCharArray(), updateArea.Left, updateArea.Top + (updateArea.Height - 20));
                }
                canvasPage.CurrentTextColor = c_color;
                visualroot.dbug_RootUpdateCounter++;
            }
        }

#endif

        ITopWindowEventRoot ITopWindowEventRootProvider.EventRoot
        {
            get { return this._topWindowEventRoot; }
        }


    }


}