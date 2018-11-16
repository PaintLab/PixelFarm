//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using PixelFarm.Drawing.WinGdi;
namespace LayoutFarm.UI.GdiPlus
{
    class GdiPlusCanvasViewport : CanvasViewport, IDisposable
    {
        //TODO: review this again
        //TODO: remove _quadPages

        GdiPlusPaintToOutput _quadPages = null;
        public GdiPlusCanvasViewport(RootGraphic rootgfx, Size viewportSize) :
            base(rootgfx, viewportSize)
        {
            _quadPages = new GdiPlusPaintToOutput(viewportSize.Width, viewportSize.Height);
            this.CalculateCanvasPages();
        }
        public void Dispose()
        {
            if (_quadPages != null)
            {
                _quadPages.Dispose();
            }
        }


        //static int dbugCount = 0;
        protected override void OnClosing()
        {
            if (_quadPages != null)
            {
                _quadPages.Dispose();
                _quadPages = null;
            }
            base.OnClosing();
        }

#if DEBUG
        //int dbugCount;
#endif
        public override void CanvasInvalidateArea(Rectangle r)
        {
            _quadPages.CanvasInvalidate(r);
#if DEBUG
            //Console.WriteLine("CanvasInvalidateArea:" + (dbugCount++).ToString() + " " + r.ToString());
#endif
        }
         
        protected override void ResetQuadPages(int viewportWidth, int viewportHeight)
        {
            _quadPages.ResizeAllPages(viewportWidth, viewportHeight);
        }
        protected override void CalculateCanvasPages()
        {
            _quadPages.CalculateCanvasPages(this.ViewportX, this.ViewportY, this.ViewportWidth, this.ViewportHeight);
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
                _quadPages.RenderToOutputWindowFullMode(
                    _rootGraphics.TopWindowRenderBox, hdc,
                    this.ViewportX, this.ViewportY, this.ViewportWidth, this.ViewportHeight);
            }
            else
            {
                //temp to full mode
                //quadPages.RenderToOutputWindowFullMode(rootGraphics.TopWindowRenderBox, hdc, this.ViewportX, this.ViewportY, this.ViewportWidth, this.ViewportHeight);
                _quadPages.RenderToOutputWindowPartialMode2(
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
                _quadPages.RenderToOutputWindowFullMode(
                    _rootGraphics.TopWindowRenderBox, hdc,
                    this.ViewportX, this.ViewportY, this.ViewportWidth, this.ViewportHeight);
            }
            else
            {
                //temp to full mode
                //quadPages.RenderToOutputWindowFullMode(rootGraphics.TopWindowRenderBox, hdc, this.ViewportX, this.ViewportY, this.ViewportWidth, this.ViewportHeight);
                _quadPages.RenderToOutputWindowPartialMode(
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
    }



    class GdiPlusPaintToOutput : IDisposable
    {
        GdiPlusDrawBoard _pageA;
        public GdiPlusPaintToOutput(
            int eachCachedPageWidth,
            int eachCachedPageHeight)
        {
            _pageA = new GdiPlusDrawBoard(0, 0, eachCachedPageWidth, eachCachedPageHeight);
        }

        public void Dispose()
        {
            if (_pageA != null)
            {
                _pageA.Dispose();
                _pageA = null;
            }
        }
        public void CanvasInvalidate(Rectangle rect)
        {
            Rectangle r = rect;
            if (_pageA != null && _pageA.IntersectsWith(r))
            {
                _pageA.Invalidate(r);
            }
        }
        public bool IsValid
        {
            get
            {
                if (_pageA != null)
                {
                    if (!_pageA.IsContentReady)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public void RenderToOutputWindowFullMode(
            IRenderElement topWindowRenderBox,
            IntPtr destOutputHdc,
            int viewportX, int viewportY, int viewportWidth, int viewportHeight)
        {

            if (_pageA != null && !_pageA.IsContentReady)
            {
                UpdateAllArea(_pageA, topWindowRenderBox);
            }
            _pageA.RenderTo(destOutputHdc, viewportX - _pageA.Left,
                          viewportY - _pageA.Top,
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
            if (!_pageA.IsContentReady)
            {
                UpdateInvalidArea(_pageA, renderE);
            }

            Rectangle invalidateArea = _pageA.InvalidateArea;

            _pageA.RenderTo(destOutputHdc, invalidateArea.Left - _pageA.Left, invalidateArea.Top - _pageA.Top,
                new Rectangle(invalidateArea.Left -
                    viewportX, invalidateArea.Top - viewportY,
                    invalidateArea.Width, invalidateArea.Height));
            _pageA.ResetInvalidateArea();
        }
        public void RenderToOutputWindowPartialMode2(
            IRenderElement renderE,
            IntPtr destOutputHdc,
            int viewportX, int viewportY,
            int viewportWidth, int viewportHeight,
            Rectangle windowMsgInvalidateArea)
        {
            if (!_pageA.IsContentReady)
            {
                UpdateInvalidArea(_pageA, renderE);
            }

            Rectangle invalidateArea = _pageA.InvalidateArea;
            if (invalidateArea.Width == 0 || invalidateArea.Height == 0)
            {
                invalidateArea = windowMsgInvalidateArea;// new Rectangle(0, 0, _pageA.Width, _pageA.Height);
            }

            _pageA.RenderTo(destOutputHdc, invalidateArea.Left - _pageA.Left, invalidateArea.Top - _pageA.Top,
                new Rectangle(invalidateArea.Left -
                    viewportX, invalidateArea.Top - viewportY,
                    invalidateArea.Width, invalidateArea.Height));
            _pageA.ResetInvalidateArea();
        }
        public void CalculateCanvasPages(int viewportX, int viewportY, int viewportWidth, int viewportHeight)
        {
            //int firstVerticalPageNum = viewportY / physicalCanvasCollection.EachPageHeight;
            //int firstHorizontalPageNum = viewportX / physicalCanvasCollection.EachPageWidth;
            ////render_parts = PAGE_A;
            //if (_pageA == null)
            //{
            //    _pageA = physicalCanvasCollection.GetCanvasPage(0, 0);
            //}
            //else
            //{
            //    if (!pageA.IsPageNumber(firstHorizontalPageNum, firstVerticalPageNum))
            //    {
            //        physicalCanvasCollection.ReleasePage(pageA);
            //        pageA = physicalCanvasCollection.GetCanvasPage(firstHorizontalPageNum, firstVerticalPageNum);
            //    }
            //}

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
            if (_pageA != null)
            {
                if (_pageA.Height < newHeight || _pageA.Width < newWidth)
                {

                    _pageA.Dispose();
                    _pageA = null;
                }
                else
                {
                    return;
                }
            }

            _pageA = new GdiPlusDrawBoard(0, 0, newWidth, newHeight);
        }
    }
}