//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.RenderBoxes;
using LayoutFarm.UI.InputBridge;

namespace LayoutFarm.UI
{
    public static class MyFontSettings
    {
        public static RequestFont DefaultRootGraphicsFont = new RequestFont("Source Sans Pro", 10);
    }

    public sealed class MyRootGraphic : RootGraphic, ITopWindowEventRootProvider
    {
        List<RenderElementRequest> _renderRequestList = new List<RenderElementRequest>();
        GraphicsTimerTaskManager _graphicTimerTaskMan;
        static object _normalUpdateTask = new object();
        readonly TopWindowEventRoot _topWindowEventRoot;
        readonly RenderBoxBase _topWindowRenderBox;

        RenderBoxBase _primaryContainerElement;

        RequestFont _defaultTextEditFont; //TODO: review here
        ITextService _textService;
        GraphicsTimerTask _gfxTimerTask;
        public MyRootGraphic(
            int width, int height,
            ITextService textService)
            : base(width, height)
        {
            _textService = textService;
            _graphicTimerTaskMan = new GraphicsTimerTaskManager(this);
            _defaultTextEditFont = MyFontSettings.DefaultRootGraphicsFont;

            if (textService != null)
            {
                textService.MeasureWhitespace(_defaultTextEditFont);
            }
#if DEBUG
            dbugCurrentGlobalVRoot = this;
            dbug_Init(null, null, null);
#endif

            //create default render box***
            _topWindowRenderBox = new TopWindowRenderBox(this, width, height);
            _topWindowEventRoot = new TopWindowEventRoot(_topWindowRenderBox);
            _gfxTimerTask = this.SubscribeGraphicsIntervalTask(_normalUpdateTask,
                TaskIntervalPlan.Animation,
                20,
                (s, e) =>
                {
                    this.PrepareRender();
                    this.FlushAccumGraphics();
                });

            _primaryContainerElement = _topWindowRenderBox;
        }
        public override void CloseWinRoot()
        {
            if (_gfxTimerTask != null)
            {
                _gfxTimerTask.RemoveSelf();
                _gfxTimerTask = null;
            }


            if (_graphicTimerTaskMan != null)
            {
                _graphicTimerTaskMan.CloseAllWorkers();
                _graphicTimerTaskMan = null;
            }

        }
        public override ITextService TextServices => _textService;

        public ITopWindowEventRoot TopWinEventPortal => _topWindowEventRoot;
        //
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
            get => _graphicTimerTaskMan.Enabled;
            set => _graphicTimerTaskMan.Enabled = value;
        }
        //
        public override IRenderElement TopWindowRenderBox => _topWindowRenderBox;
        //
        public override void PrepareRender()
        {
            //clear layout queue before render*** 
            //1. layout requests
            LayoutQueueClearing = true;
            InvokeClearingBeforeRender();
            LayoutQueueClearing = false;

            //----------------------------
            //2. render requests
            ClearRenderRequests();            
            ClearNotificationSizeChangeList();
        }
        void ClearNotificationSizeChangeList()
        {
            LayoutFarm.EventQueueSystem.CentralEventQueue.InvokeEventQueue();
        }
        //
        public override RequestFont DefaultTextEditFontInfo => _defaultTextEditFont;
        //
        public override void ClearRenderRequests()
        {
            if (this.VisualRequestCount > 0)
            {
                this.ClearVisualRequests();
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
                _graphicTimerTaskMan.CloseAllWorkers();
                _graphicTimerTaskMan = null;
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
            return _graphicTimerTaskMan.SubscribeGraphicsTimerTask(uniqueName, planName, intervalMs, tickhandler);
        }
        public override void RemoveIntervalTask(object uniqueName)
        {
            _graphicTimerTaskMan.UnsubscribeTimerTask(uniqueName);
        }
        //-------------------------------------------------------------------------------
        //
        int VisualRequestCount => _renderRequestList.Count;
        //
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
                _topWindowEventRoot.CurrentKeyboardFocusedElement = null;
                return;
            }

            var owner = renderElement.GetController() as IUIEventListener;
            if (owner != null)
            {
                _topWindowEventRoot.CurrentKeyboardFocusedElement = owner;
            }
        }

        //
        ITopWindowEventRoot ITopWindowEventRootProvider.EventRoot => _topWindowEventRoot;
        //
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
        //


    }


}