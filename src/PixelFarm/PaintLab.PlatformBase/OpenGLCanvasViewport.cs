
using System.Collections.Generic;
using PixelFarm.Drawing;
namespace LayoutFarm.UI.OpenGL
{


    public partial class OpenGLCanvasViewport : CanvasViewport
    {
        DrawBoard _drawboard;
        bool _isClosed;

        GfxUpdatePlan _gfxUpdatePlan;
        public OpenGLCanvasViewport(RootGraphic root, Size viewportSize)
            : base(root, viewportSize)
        {
            _gfxUpdatePlan = new GfxUpdatePlan(root);
        }
        //----------------
#if DEBUG
        static void dbugDrawDebugRedBoxes(DrawBoard mycanvas)
        {
            return;
            //### DRAW_DEBUG_RED_BOXES***
            //our OpenGLCanvasViewport use Html5Canvas model
            //Html5Canvas coordinate (0,0) is on Upper-Left
            //so this red rects should run from upper-left to lower-right

            for (int i = 0; i < 100; ++i)
            {
                mycanvas.FillRectangle(Color.Red, i * 5, i * 5, 5, 5);
            }
        }
        //----------------
#endif
        protected override void OnClosing()
        {
            _isClosed = true;
            if (_drawboard != null)
            {
                _drawboard.CloseCanvas();
                _drawboard = null;
            }
        }
        public override void CanvasInvalidateArea(Rectangle r)
        {
        }
        internal void NotifyWindowControlBinding()
        {
        }

        public void SetDrawboard(DrawBoard d)
        {
            _drawboard = d;

        }
        //----------
        //for test
#if DEBUG
        //void dbugTest01()
        //{
        //    //canvas.Orientation = CanvasOrientation.LeftTop;
        //    canvas.ClearSurface(Color.White);

        //    canvas.FillRectangle(Color.Red, 50, 50, 100, 100);

        //    dbugGLOffsetCanvasOrigin(50, 50);
        //    //simulate draw content 
        //    canvas.FillRectangle(Color.Gray, 10, 10, 80, 80);
        //    dbugGLOffsetCanvasOrigin(-50, -50);
        //}
        //void dbugGLSetCanvasOrigin(int x, int y)
        //{
        //    canvas.SetCanvasOrigin(x, y);
        //    //int properW = Math.Min(canvas.Width, canvas.Height);
        //    ////int max = 600;
        //    ////init             
        //    ////---------------------------------
        //    ////-1 temp fix split scanline in some screen
        //    ////OpenTK.Graphics.OpenGL.GL.Viewport(x, y, properW, properW - 1);
        //    //////--------------------------------- 
        //    //OpenTK.Graphics.OpenGL.GL.MatrixMode(OpenTK.Graphics.OpenGL.MatrixMode.Projection);
        //    //OpenTK.Graphics.OpenGL.GL.LoadIdentity();
        //    //OpenTK.Graphics.OpenGL.GL.Ortho(0, properW, properW, 0, 0.0, 100);

        //    ////switch (this.orientation)
        //    ////{
        //    ////    case Drawing.CanvasOrientation.LeftTop:
        //    ////        {
        //    ////            OpenTK.Graphics.OpenGL.GL.Ortho(0, properW, properW, 0, 0.0, 100);
        //    ////        } break;
        //    ////    default:
        //    ////        {
        //    ////            OpenTK.Graphics.OpenGL.GL.Ortho(0, properW, 0, properW, 0.0, 100);
        //    ////        } break;
        //    ////}
        //    //OpenTK.Graphics.OpenGL.GL.MatrixMode(OpenTK.Graphics.OpenGL.MatrixMode.Modelview);
        //    //OpenTK.Graphics.OpenGL.GL.LoadIdentity();
        //    //OpenTK.Graphics.OpenGL.GL.Translate(x, y, 0);
        //}
        //void dbugGLOffsetCanvasOrigin(int dx, int dy)
        //{
        //    dbugGLSetCanvasOrigin(canvas.CanvasOriginX + dx, canvas.CanvasOriginY + dy);
        //}
#endif
        //-------

        static Stack<UpdateArea> _updateAreaPool = new Stack<UpdateArea>();

        static UpdateArea GetFreeUpdateArea() => (_updateAreaPool.Count == 0) ? new UpdateArea() : _updateAreaPool.Pop();

        static void ReleaseUpdateArea(UpdateArea u)
        {
            u.Reset();
            _updateAreaPool.Push(u);
        }

        //-------

        public void PaintMe()
        {

            if (_isClosed || _drawboard == null)
            {
                return;
            }

            RootGraphic backup = GlobalRootGraphic.CurrentRootGfx;
            GlobalRootGraphic.CurrentRootGfx = _rootgfx;
            _rootgfx.PrepareRender();
            //---------------
            _rootgfx.BeginRenderPhase();
#if DEBUG
            _rootgfx.dbug_rootDrawingMsg.Clear();
            _rootgfx.dbug_drawLevel = 0;
#endif

            if (_rootgfx.HasAccumInvalidateRect)
            {

                //evaluate  
                _gfxUpdatePlan.SetUpdatePlanForFlushAccum();
                int j = _gfxUpdatePlan.UpdateListCount;

                if (j > 0)
                {
                    //special mode
                    UpdateArea u = GetFreeUpdateArea();
                    for (int i = 0; i < j; ++i)
                    {

                        _gfxUpdatePlan.SetCurrentUpdate(i);

                        u.CurrentRect = _gfxUpdatePlan.AccumUpdateArea;

                        _drawboard.SetClipRect(u.CurrentRect); //**

                        UpdateInvalidateArea(_drawboard, _topWindowBox, u);

                        _gfxUpdatePlan.ClearCurrentUpdate();


#if DEBUG
                        if (RenderElement.WaitForStartRenderElement)
                        {
                            //should not occur
                            throw new System.NotSupportedException();
                        }
#endif

                    }

                    _gfxUpdatePlan.ResetUpdatePlan();
                    ReleaseUpdateArea(u);
                }
                else
                {

                    //default mode
                    _drawboard.SetClipRect(_rootgfx.AccumInvalidateRect);
                    UpdateArea u = GetFreeUpdateArea();

                    //TODO: review clear bg again
                    _drawboard.Clear(Color.White);
                    u.CurrentRect = _rootgfx.AccumInvalidateRect;

                    UpdateInvalidateArea(_drawboard, _topWindowBox, u);

                    _gfxUpdatePlan.ResetUpdatePlan();
                    ReleaseUpdateArea(u);
                }
                //-----------
            }
            _rootgfx.EndRenderPhase();
            GlobalRootGraphic.CurrentRootGfx = backup;
#if DEBUG

            RootGraphic visualroot = RootGraphic.dbugCurrentGlobalVRoot;
            if (visualroot.dbug_RecordDrawingChain)
            {
                List<dbugLayoutMsg> outputMsgs = dbugOutputWindow.dbug_rootDocDebugMsgs;
                outputMsgs.Clear();
                outputMsgs.Add(new dbugLayoutMsg(null as RenderElement, "[" + debug_render_to_output_count + "]"));
                visualroot.dbug_DumpRootDrawingMsg(outputMsgs);
                dbugOutputWindow.dbug_InvokeVisualRootDrawMsg();
                debug_render_to_output_count++;
            }
            if (dbugHelper01.dbugVE_HighlightMe != null)
            {
                dbugOutputWindow.dbug_HighlightMeNow(dbugHelper01.dbugVE_HighlightMe.dbugGetGlobalRect());

            }
#endif

        }

        static void UpdateInvalidateArea(DrawBoard d, IRenderElement topWindowRenderBox, UpdateArea updateArea)
        {
            int enter_canvas_x = d.OriginX;
            int enter_canvas_y = d.OriginY;

            d.SetCanvasOrigin(enter_canvas_x - d.Left, enter_canvas_y - d.Top);
            topWindowRenderBox.Render(d, updateArea);

            //Rectangle rect = mycanvas.Rect;
            //topWindowRenderBox.DrawToThisCanvas(mycanvas, rect);
#if DEBUG
            dbugDrawDebugRedBoxes(d);
#endif
            d.SetCanvasOrigin(enter_canvas_x, enter_canvas_y);//restore
        }
    }

}
