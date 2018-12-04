//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;


#if __SKIA__
namespace LayoutFarm.UI.Skia
{
    class SkiaCanvasViewport : CanvasViewport
    {

        PixelFarm.Drawing.Skia.MySkiaDrawBoard mySkCanvas;
        //TODO: review here again
        int internalSizeW = 800;
        int internalSizwH = 600;

        Win32.NativeWin32MemoryDC memdc;
        public SkiaCanvasViewport(RootGraphic rootgfx, Size viewportSize)
            : base(rootgfx, viewportSize)
        {

            this.CalculateCanvasPages();
            mySkCanvas = new PixelFarm.Drawing.Skia.MySkiaDrawBoard(0, 0, 0, 0, internalSizeW, internalSizwH);
            memdc = new Win32.NativeWin32MemoryDC(internalSizeW, internalSizwH);

        }
        protected override void OnClosing()
        {

            memdc.Dispose();
            base.OnClosing();
        }
        public override void CanvasInvalidateArea(Rectangle r)
        {
            //quadPages.CanvasInvalidate(r);
            //Console.WriteLine((dbugCount++).ToString() + " " + r.ToString());
        }
        
        protected override void ResetViewSize(int viewportWidth, int viewportHeight)
        {
            //  quadPages.ResizeAllPages(viewportWidth, viewportHeight);
        }
        protected override void CalculateCanvasPages()
        {
            //quadPages.CalculateCanvasPages(this.ViewportX, this.ViewportY, this.ViewportWidth, this.ViewportHeight);
            this.FullMode = true;
        }

        static void UpdateAllArea(PixelFarm.Drawing.Skia.MySkiaDrawBoard mycanvas, IRenderElement topWindowRenderBox)
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
        public void PaintMe(IntPtr hdc)
        {
            if (this.IsClosed) { return; }
            _rootGraphics.PrepareRender();
            //---------------
            _rootGraphics.IsInRenderPhase = true;
#if DEBUG
            _rootGraphics.dbug_rootDrawingMsg.Clear();
            _rootGraphics.dbug_drawLevel = 0;
#endif

            //1. clear sk surface
            mySkCanvas.Clear(PixelFarm.Drawing.Color.White);
            //2. render to the surface
            UpdateAllArea(mySkCanvas, _rootGraphics.TopWindowRenderBox);
            //3. copy bitmap buffer from the surface and render to final hdc

            //-----------------------------------------------
            //TODO: review performance here 
            //we should copy from unmanaged (skia bitmap) 
            //and write to unmanaged (hdc'bitmap)

            SkiaSharp.SKBitmap backBmp = mySkCanvas.BackBmp;
            backBmp.LockPixels();
            IntPtr h = backBmp.GetPixels();
            int w1 = backBmp.Width;
            int h1 = backBmp.Height;
            int stride = backBmp.RowBytes;
            //PixelFarm.Agg.AggMemMx.memcpy()
            //System.Runtime.InteropServices.Marshal.Copy(
            //    h, tmpBuffer, 0, tmpBuffer.Length
            //    );
            //copy skia pixels to our dc
            unsafe
            {
                //Win32.MyWin32.memcpy((byte*)memdc.PPVBits, (byte*)h, internalSizeW * 4 * internalSizwH);
                memdc.CopyPixelBitsToOutput((byte*)h);
            }
            backBmp.UnlockPixels();

            //bitblt to target             
            memdc.BitBltTo(hdc);
            //var bmpdata = tmpBmp.LockBits(new System.Drawing.Rectangle(0, 0, w1, h1),
            //    System.Drawing.Imaging.ImageLockMode.ReadWrite,
            //    System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            //System.Runtime.InteropServices.Marshal.Copy(tmpBuffer, 0, bmpdata.Scan0, tmpBuffer.Length);
            //tmpBmp.UnlockBits(bmpdata); 
            //using (System.Drawing.Graphics g2 = System.Drawing.Graphics.FromHdc(hdc))
            //{
            //    g2.DrawImage(tmpBmp, 0, 0);
            //}
            //-----------------------------------------------------------------------------
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



    }
}

#endif