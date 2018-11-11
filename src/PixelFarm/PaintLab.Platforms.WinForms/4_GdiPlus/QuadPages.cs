//MIT, 2014-present, WinterDev

using System;
using LayoutFarm;
namespace PixelFarm.Drawing.WinGdi
{
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