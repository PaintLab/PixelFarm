//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using PixelFarm.Drawing.WinGdi;
namespace LayoutFarm.UI.GdiPlus
{
    class GdiPlusCanvasViewport : CanvasViewport, IDisposable
    {

        GdiPlusDrawBoard _drawBoard;
        public GdiPlusCanvasViewport(RootGraphic rootgfx, Size viewportSize) :
            base(rootgfx, viewportSize)
        {
            _drawBoard = new GdiPlusDrawBoard(0, 0, viewportSize.Width, viewportSize.Height);
            this.CalculateCanvasPages();
        }
        public void Dispose()
        {
            if (_drawBoard != null)
            {
                _drawBoard.Dispose();
                _drawBoard = null;
            }
        }       

#if DEBUG
        //int dbugCount;
#endif
        public override void CanvasInvalidateArea(Rectangle r)
        {
            if (_drawBoard != null && _drawBoard.IntersectsWith(r))
            {
                _drawBoard.Invalidate(r);
            }
#if DEBUG
            //Console.WriteLine("CanvasInvalidateArea:" + (dbugCount++).ToString() + " " + r.ToString());
#endif
        }
        protected override void ResetQuadPages(int viewportWidth, int viewportHeight)
        {
            ResizeAllPages(viewportWidth, viewportHeight);
        }
        protected override void CalculateCanvasPages()
        {

            this.FullMode = true;
        }
        public void PaintMe2(IntPtr hdc, Rectangle invalidateArea)
        {
            if (this.IsClosed) { return; }
            //------------------------------------ 

            this._rootGraphics.PrepareRender();
            //---------------
            this._rootGraphics.IsInRenderPhase = true;
#if DEBUG
            this._rootGraphics.dbug_rootDrawingMsg.Clear();
            this._rootGraphics.dbug_drawLevel = 0;
#endif
            if (this.FullMode)
            {
                RenderToOutputWindowFullMode(
                    _rootGraphics.TopWindowRenderBox, hdc,
                    this.ViewportX, this.ViewportY, this.ViewportWidth, this.ViewportHeight);
            }
            else
            {
                //temp to full mode
                //quadPages.RenderToOutputWindowFullMode(rootGraphics.TopWindowRenderBox, hdc, this.ViewportX, this.ViewportY, this.ViewportWidth, this.ViewportHeight);
                RenderToOutputWindowPartialMode2(
                        _rootGraphics.TopWindowRenderBox, hdc,
                        this.ViewportX, this.ViewportY, this.ViewportWidth, this.ViewportHeight, invalidateArea);
            }
            this._rootGraphics.IsInRenderPhase = false;
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

        public void PaintMe(IntPtr hdc)
        {
            //paint the content to target hdc

            if (this.IsClosed) { return; }
            //------------------------------------ 

            this._rootGraphics.PrepareRender();
            //---------------
            this._rootGraphics.IsInRenderPhase = true;
#if DEBUG
            this._rootGraphics.dbug_rootDrawingMsg.Clear();
            this._rootGraphics.dbug_drawLevel = 0;
#endif
            if (this.FullMode)
            {
                RenderToOutputWindowFullMode(
                    _rootGraphics.TopWindowRenderBox, hdc,
                    this.ViewportX, this.ViewportY, this.ViewportWidth, this.ViewportHeight);
            }
            else
            {
                //temp to full mode
                //quadPages.RenderToOutputWindowFullMode(rootGraphics.TopWindowRenderBox, hdc, this.ViewportX, this.ViewportY, this.ViewportWidth, this.ViewportHeight);
                RenderToOutputWindowPartialMode(
                     _rootGraphics.TopWindowRenderBox, hdc,
                     this.ViewportX, this.ViewportY, this.ViewportWidth, this.ViewportHeight);
            }
            this._rootGraphics.IsInRenderPhase = false;
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


        public void PaintMe(GdiPlusDrawBoard mycanvas)
        {
            if (this.IsClosed) { return; }
            //------------------------------------ 

            this._rootGraphics.PrepareRender();
            //---------------
            this._rootGraphics.IsInRenderPhase = true;
#if DEBUG
            this._rootGraphics.dbug_rootDrawingMsg.Clear();
            this._rootGraphics.dbug_drawLevel = 0;
#endif

            mycanvas.OffsetCanvasOrigin(-mycanvas.Left, -mycanvas.Top);
            Rectangle rect = mycanvas.Rect;
            this._rootGraphics.TopWindowRenderBox.DrawToThisCanvas(mycanvas, rect);
#if DEBUG
            this._rootGraphics.TopWindowRenderBox.dbugShowRenderPart(mycanvas, rect);
#endif

            mycanvas.IsContentReady = true;
            mycanvas.OffsetCanvasOrigin(mycanvas.Left, mycanvas.Top);
            //if (this.FullMode)
            //{
            //    quadPages.RenderToOutputWindowFullMode(rootGraphics.TopWindowRenderBox, hdc,
            //        this.ViewportX, this.ViewportY, this.ViewportWidth, this.ViewportHeight);
            //}
            //else
            //{
            //    //temp to full mode
            //    //quadPages.RenderToOutputWindowFullMode(rootGraphics.TopWindowRenderBox, hdc, this.ViewportX, this.ViewportY, this.ViewportWidth, this.ViewportHeight);
            //    quadPages.RenderToOutputWindowPartialMode(rootGraphics.TopWindowRenderBox, hdc,
            //        this.ViewportX, this.ViewportY, this.ViewportWidth, this.ViewportHeight);

            //}
            this._rootGraphics.IsInRenderPhase = false;
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


        public void RenderToOutputWindowFullMode(
            IRenderElement topWindowRenderBox,
            IntPtr destOutputHdc,
            int viewportX, int viewportY, int viewportWidth, int viewportHeight)
        {

            if (_drawBoard != null && !_drawBoard.IsContentReady)
            {
                UpdateAllArea(_drawBoard, topWindowRenderBox);
            }
            _drawBoard.RenderTo(destOutputHdc, viewportX - _drawBoard.Left,
                          viewportY - _drawBoard.Top,
                          new Rectangle(0, 0,
                          viewportWidth,
                          viewportHeight));

        }
        static void UpdateAllArea(GdiPlusDrawBoard mycanvas, IRenderElement topWindowRenderBox)
        {
            mycanvas.OffsetCanvasOrigin(-mycanvas.Left, -mycanvas.Top);
            Rectangle rect = mycanvas.Rect;
            topWindowRenderBox.DrawToThisCanvas(mycanvas, rect);
#if DEBUG
            topWindowRenderBox.dbugShowRenderPart(mycanvas, rect);
#endif

            mycanvas.IsContentReady = true;
            mycanvas.OffsetCanvasOrigin(mycanvas.Left, mycanvas.Top);
        }

        static void UpdateInvalidArea(GdiPlusDrawBoard mycanvas, IRenderElement rootElement)
        {
            mycanvas.OffsetCanvasOrigin(-mycanvas.Left, -mycanvas.Top);
            Rectangle rect = mycanvas.InvalidateArea;

            if (rect.Width > 0 && rect.Height > 0)
            {
                rootElement.DrawToThisCanvas(mycanvas, rect);
#if DEBUG
                rootElement.dbugShowRenderPart(mycanvas, rect);
#endif
            }
            else
            {

            }


            mycanvas.IsContentReady = true;
            mycanvas.OffsetCanvasOrigin(mycanvas.Left, mycanvas.Top);
        }

        public void RenderToOutputWindowPartialMode(
            IRenderElement renderE,
            IntPtr destOutputHdc,
            int viewportX, int viewportY,
            int viewportWidth, int viewportHeight)
        {
            if (!_drawBoard.IsContentReady)
            {
                UpdateInvalidArea(_drawBoard, renderE);
            }

            Rectangle invalidateArea = _drawBoard.InvalidateArea;

            _drawBoard.RenderTo(destOutputHdc, invalidateArea.Left - _drawBoard.Left, invalidateArea.Top - _drawBoard.Top,
                new Rectangle(invalidateArea.Left -
                    viewportX, invalidateArea.Top - viewportY,
                    invalidateArea.Width, invalidateArea.Height));
            _drawBoard.ResetInvalidateArea();
        }
        public void RenderToOutputWindowPartialMode2(
            IRenderElement renderE,
            IntPtr destOutputHdc,
            int viewportX, int viewportY,
            int viewportWidth, int viewportHeight,
            Rectangle windowMsgInvalidateArea)
        {
            if (!_drawBoard.IsContentReady)
            {
                UpdateInvalidArea(_drawBoard, renderE);
            }

            Rectangle invalidateArea = _drawBoard.InvalidateArea;
            if (invalidateArea.Width == 0 || invalidateArea.Height == 0)
            {
                invalidateArea = windowMsgInvalidateArea;// new Rectangle(0, 0, _pageA.Width, _pageA.Height);
            }

            _drawBoard.RenderTo(destOutputHdc, invalidateArea.Left - _drawBoard.Left, invalidateArea.Top - _drawBoard.Top,
                new Rectangle(invalidateArea.Left -
                    viewportX, invalidateArea.Top - viewportY,
                    invalidateArea.Width, invalidateArea.Height));
            _drawBoard.ResetInvalidateArea();
        }
        public void ResizeAllPages(int newWidth, int newHeight)
        {
            //physicalCanvasCollection.Dispose();
            //physicalCanvasCollection.ResizeAllPages(newWidth, newHeight);
            //if (_pageA != null)
            //{
            //    _pageA.IsUnused = true;
            //    _pageA = null;
            //}
            if (_drawBoard != null)
            {
                if (_drawBoard.Height < newHeight || _drawBoard.Width < newWidth)
                {

                    _drawBoard.Dispose();
                    _drawBoard = null;
                }
                else
                {
                    return;
                }
            }

            _drawBoard = new GdiPlusDrawBoard(0, 0, newWidth, newHeight);
        }
        public bool IsValid
        {
            get
            {
                if (_drawBoard != null)
                {
                    if (!_drawBoard.IsContentReady)
                    {
                        return false;
                    }
                }
                return true;
            }
        }
    }
}