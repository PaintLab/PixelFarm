//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using PixelFarm.Drawing.WinGdi;
namespace LayoutFarm.UI.GdiPlus
{
    class GdiPlusCanvasViewport : CanvasViewport, System.IDisposable
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
        public override bool IsQuadPageValid
        {
            get
            {
                return this._quadPages.IsValid;
            }
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
}