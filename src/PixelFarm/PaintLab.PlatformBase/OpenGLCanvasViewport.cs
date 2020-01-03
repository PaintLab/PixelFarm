
using System.Collections.Generic;
using PixelFarm.Drawing;
namespace LayoutFarm.UI.OpenGL
{


    public partial class OpenGLCanvasViewport : CanvasViewport
    {
        DrawBoard _canvas;
        bool _isClosed;
        public OpenGLCanvasViewport(RootGraphic root, Size viewportSize)
            : base(root, viewportSize)
        {
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
            if (_canvas != null)
            {
                _canvas.CloseCanvas();
                _canvas = null;
            }
        }
        public override void CanvasInvalidateArea(Rectangle r)
        {
        }
        internal void NotifyWindowControlBinding()
        {
        }

        public void SetCanvas(DrawBoard canvas)
        {
            _canvas = canvas;

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
        public void PaintMe()
        {
            //similar to PaintMe()
            if (_isClosed || _canvas == null)
            {
                return;
            }
            //---------------------------------------------

            //canvas.Orientation = CanvasOrientation.LeftTop;
            //Test01(); 
            //return;
            //Test01();
            //return;
            //canvas.ClearSurface(Color.White);
            //canvas.FillRectangle(Color.Red, 20, 20, 200, 400);
            // return;
            //----------------------------------
            //gl paint here
            //if (_canvas == null)
            //{
            //    return;
            //}
            //////test draw rect
            ////canvas.StrokeColor = PixelFarm.Drawing.Color.Blue;
            ////canvas.DrawRectangle(Color.Blue, 20, 20, 200, 200);
            //////------------------------ 

            //if (this.IsClosed)
            //{
            //    return;
            //}
            //------------------------------------ 


            _rootGraphics.PrepareRender();
            //---------------
            _rootGraphics.IsInRenderPhase = true;
#if DEBUG
            _rootGraphics.dbug_rootDrawingMsg.Clear();
            _rootGraphics.dbug_drawLevel = 0;
#endif

            //old             
            //_canvas.Clear(Color.White);
            //UpdateAllArea(_canvas, _topWindowBox);
            //------------------
            //test
            //if (!_rootGraphics.HasAccumInvalidateRect)
            //{ 
            //    //_canvas.SetClipRect(_rootGraphics.AccumInvalidateRect);
            //    //_canvas.Clear(Color.White); 
            //    //UpdateInvalidateArea(_canvas, _topWindowBox,
            //    //    new Rectangle(0, 0, _rootGraphics.Width, _rootGraphics.Height));
            //}
            //else
            //{ 
            //    _canvas.SetClipRect(_rootGraphics.AccumInvalidateRect);
            //    _canvas.Clear(Color.White);
            //    UpdateInvalidateArea(_canvas, _topWindowBox, _rootGraphics.AccumInvalidateRect);
            //}

            if (_rootGraphics.HasAccumInvalidateRect)
            {
                //set clip before clear
                _canvas.SetClipRect(_rootGraphics.AccumInvalidateRect);
                _canvas.Clear(Color.White);
                UpdateInvalidateArea(_canvas, _topWindowBox, _rootGraphics.AccumInvalidateRect);

            }


            _rootGraphics.IsInRenderPhase = false;
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

        static void UpdateInvalidateArea(DrawBoard mycanvas, IRenderElement topWindowRenderBox, Rectangle updateArea)
        {
            int enter_canvas_x = mycanvas.OriginX;
            int enter_canvas_y = mycanvas.OriginY;

            mycanvas.SetCanvasOrigin(enter_canvas_x - mycanvas.Left, enter_canvas_y - mycanvas.Top);
            topWindowRenderBox.DrawToThisCanvas(mycanvas, updateArea);
            //Rectangle rect = mycanvas.Rect;
            //topWindowRenderBox.DrawToThisCanvas(mycanvas, rect);
#if DEBUG 
            dbugDrawDebugRedBoxes(mycanvas);
#endif
            mycanvas.SetCanvasOrigin(enter_canvas_x, enter_canvas_y);//restore
        }
        static void UpdateAllArea(DrawBoard mycanvas, IRenderElement topWindowRenderBox)
        {
            int enter_canvas_x = mycanvas.OriginX;
            int enter_canvas_y = mycanvas.OriginY;

            mycanvas.SetCanvasOrigin(enter_canvas_x - mycanvas.Left, enter_canvas_y - mycanvas.Top);

            Rectangle rect = mycanvas.Rect;
            topWindowRenderBox.DrawToThisCanvas(mycanvas, rect);
#if DEBUG 
            dbugDrawDebugRedBoxes(mycanvas);
#endif
            mycanvas.SetCanvasOrigin(enter_canvas_x, enter_canvas_y);//restore
        }

    }

}
