//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.RenderBoxes;
namespace LayoutFarm.UI
{
    public static class MyFontSettings
    {
        public static RequestFont DefaultRootGraphicsFont = new RequestFont("Source Sans Pro", 10);
    }

    public sealed class MyRootGraphic : RootGraphic, ITopWindowEventRootProvider, IDisposable
    {
        List<RenderElementRequest> _renderRequestList = new List<RenderElementRequest>();
        GraphicsTimerTaskManager _gfxTimerTaskMx;
        static object _normalUpdateTask = new object();
        readonly TopWindowEventRoot _topWindowEventRoot;
        readonly TopWindowRenderBox _topWindowRenderBox;

        RenderBoxBase _primaryContainerElement;

        RequestFont _defaultTextEditFont; //TODO: review here
        ITextService _textService;
        GraphicsTimerTask _gfxTimerTask;

        Func<PixelFarm.Drawing.GLES2.MyGLDrawBoard> _getDrawboard; //

        public MyRootGraphic(
            int width, int height,
            ITextService textService)
            : base(width, height)
        {
            _textService = textService;
            _gfxTimerTaskMx = new GraphicsTimerTaskManager(this);
            _defaultTextEditFont = MyFontSettings.DefaultRootGraphicsFont;

            if (textService != null)
            {
                //precalculate whitespace face for a default font
                textService.MeasureWhitespace(_defaultTextEditFont);
            }
#if DEBUG
            dbugCurrentGlobalVRoot = this;
            dbug_Init(null, null, null);
#endif

            //create default render box***
            _topWindowRenderBox = new TopWindowRenderBox(this, width, height);
            _topWindowEventRoot = new TopWindowEventRoot(this, _topWindowRenderBox);
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

        public void Dispose()
        {
            if (_gfxTimerTaskMx != null)
            {
                _gfxTimerTaskMx.CloseAllWorkers();
                _gfxTimerTaskMx = null;
            }
#if DEBUG
            dbugHitTracker.Close();
#endif
        }

        public void SetDrawboardReqDelegate(Func<PixelFarm.Drawing.GLES2.MyGLDrawBoard> getDrawboard)
        {
            _getDrawboard = getDrawboard;
        }

        List<RenderElementRequest> _fmtStrRenderReqList = new List<RenderElementRequest>();
        List<PixelFarm.DrawingGL.GLRenderVxFormattedString> _fmtList = new List<PixelFarm.DrawingGL.GLRenderVxFormattedString>();

        public override void EnqueueRenderRequest(RenderElementRequest renderReq)
        {
            if (renderReq.req == RequestCommand.ProcessFormattedString)
            {
                if (renderReq.parameters is PixelFarm.DrawingGL.GLRenderVxFormattedString fmtStr &&
                    fmtStr.State == RenderVxFormattedString.VxState.NoStrip)
                {
                    _fmtStrRenderReqList.Add(renderReq);
                    _fmtList.Add(fmtStr);
                    fmtStr.State = RenderVxFormattedString.VxState.Waiting;
                }
            }
            else
            {
                _renderRequestList.Add(renderReq);
            }
        }
        public override void CloseWinRoot()
        {
            if (_gfxTimerTask != null)
            {
                _gfxTimerTask.RemoveSelf();
                _gfxTimerTask = null;
            }

            if (_gfxTimerTaskMx != null)
            {
                _gfxTimerTaskMx.CloseAllWorkers();
                _gfxTimerTaskMx = null;
            }

        }
       

        public ITopWindowEventRoot TopWinEventPortal => _topWindowEventRoot;

        public override bool GfxTimerEnabled
        {
            get => _gfxTimerTaskMx.Enabled;
            set => _gfxTimerTaskMx.Enabled = value;
        }
        //
        public override IRenderElement TopWindowRenderBox => _topWindowRenderBox;
        //

        public override void PrepareRender()
        {
            //eg. clear waiting layout 
            InvokePreRenderEvent();

            ManageRenderElementRequests(); //eg. add some waiting render element

            //other event after manage render element request
            EventQueueSystem.CentralEventQueue.InvokeEventQueue();
        }
        public override RequestFont DefaultTextEditFontInfo => _defaultTextEditFont;
        // 


        public override void ManageRenderElementRequests()
        {
            int j = _renderRequestList.Count;
            //------------------------------------
            if (j > 0)
            {
                for (int i = 0; i < j; ++i)
                {
                    RenderElementRequest req = _renderRequestList[i];
                    switch (req.req)
                    {
                        case RequestCommand.AddToWindowRoot:
                            {
                                AddChild(req.renderElem);
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
                                //InvalidateGfxArgs args = GetInvalidateGfxArgs();
                                //args.SetReason_UpdateLocalArea(req.renderElem, (Rectangle)req.parameters);
                                //InternalBubbleup(args);

                                req.renderElem.InvalidateGraphics((Rectangle)req.parameters);
                            }
                            break;
                    }
                }
                _renderRequestList.Clear();
            }
            //---------------------------------------------
            //generated formated string

            if ((j = _fmtList.Count) > 0)
            {
                //a root dose not have default drawboard***
                //so we ask for some drawboard to handle these requests 

                PixelFarm.Drawing.GLES2.MyGLDrawBoard drawboard = _getDrawboard();
                 
                drawboard.PrepareWordStrips(_fmtList);

                _fmtList.Clear();


                //all should be ready
                //each render element must be update again

                j = _fmtStrRenderReqList.Count;
                for (int i = 0; i < j; ++i)
                {
                    RenderElement re = _fmtStrRenderReqList[i].renderElem;
                    if (re != null)
                    {
                        if (re.NeedPreRenderEval)
                        {
                            RenderElement.InvokePreRenderEvaluation(re);
                        }
                        re.InvalidateGraphics();
                    }
                    else
                    {

                    }
                }
                _fmtStrRenderReqList.Clear();
            }
        }

        public override void CaretStartBlink()
        {
            _gfxTimerTaskMx.StartCaretBlinkTask();
        }
        public override void CaretStopBlink()
        {
            _gfxTimerTaskMx.StopCaretBlinkTask();

        }


        //-------------------------------------------------------------------------------
        public override GraphicsTimerTask SubscribeGraphicsIntervalTask(
            object uniqueName,
            TaskIntervalPlan planName,
            int intervalMs,
            EventHandler<GraphicsTimerTaskEventArgs> tickhandler)
        {
            return _gfxTimerTaskMx.SubscribeGraphicsTimerTask(uniqueName, planName, intervalMs, tickhandler);
        }
        public override void RemoveIntervalTask(object uniqueName)
        {
            _gfxTimerTaskMx.UnsubscribeTimerTask(uniqueName);
        }
        //-------------------------------------------------------------------------------
        //
        //int VisualRequestCount => _renderRequestList.Count;
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

        public override void SetCurrentKeyboardFocus(RenderElement renderElement)
        {
            if (renderElement == null)
            {
                _topWindowEventRoot.CurrentKeyboardFocusedElement = null;
                return;
            }

            if (renderElement.GetController() is IUIEventListener owner)
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
                debugVisualLay.BeginNewContext();
                debugVisualLay.WriteInfo(msg.text, ve);
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
        public void dbugShowRenderPart(DrawBoard d, Rectangle updateArea)
        {
            RootGraphic visualroot = this;
            if (visualroot.dbug_ShowRootUpdateArea)
            {
                d.FillRectangle(Color.FromArgb(50, Color.Black),
                     updateArea.Left, updateArea.Top,
                        updateArea.Width - 1, updateArea.Height - 1);
                d.FillRectangle(Color.White,
                     updateArea.Left, updateArea.Top, 5, 5);
                d.DrawRectangle(Color.Yellow,
                        updateArea.Left, updateArea.Top,
                        updateArea.Width - 1, updateArea.Height - 1);
                Color c_color = d.CurrentTextColor;
                d.CurrentTextColor = Color.White;
                d.DrawText(visualroot.dbug_RootUpdateCounter.ToString().ToCharArray(), updateArea.Left, updateArea.Top);
                if (updateArea.Height > 25)
                {
                    d.DrawText(visualroot.dbug_RootUpdateCounter.ToString().ToCharArray(), updateArea.Left, updateArea.Top + (updateArea.Height - 20));
                }
                d.CurrentTextColor = c_color;
                visualroot.dbug_RootUpdateCounter++;
            }
        }

#endif
        //


    }


}