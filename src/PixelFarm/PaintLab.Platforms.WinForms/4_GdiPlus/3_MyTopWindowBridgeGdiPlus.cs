//Apache2, 2014-present, WinterDev

using System;
using System.Windows.Forms;
using PixelFarm.Drawing;
namespace LayoutFarm.UI.GdiPlus
{
    class MyTopWindowBridgeGdiPlus : TopWindowBridgeWinForm
    {
        Control windowControl;
        GdiPlusCanvasViewport gdiPlusViewport;
        public MyTopWindowBridgeGdiPlus(RootGraphic root, ITopWindowEventRoot topWinEventRoot)
            : base(root, topWinEventRoot)
        {
        }

        public override void BindWindowControl(Control windowControl)
        {
            //bind to anycontrol GDI control  
            this.windowControl = windowControl;
            this.SetBaseCanvasViewport(this.gdiPlusViewport = new GdiPlusCanvasViewport(this.RootGfx, this.Size.ToSize(), 4));
            this.RootGfx.SetPaintDelegates(
                    this.gdiPlusViewport.CanvasInvlidateArea,
                    this.PaintToOutputWindow);
#if DEBUG
            this.dbugWinControl = windowControl;
            this.gdiPlusViewport.dbugOutputWindow = this;
#endif
            this.EvaluateScrollbar();
        }
        System.Drawing.Size Size
        {
            get { return this.windowControl.Size; }
        }
        public override void InvalidateRootArea(Rectangle r)
        {
            Rectangle rect = r;
            this.RootGfx.InvalidateGraphicArea(
                RootGfx.TopWindowRenderBox,
                ref rect);
        }
        public override void PaintToOutputWindow()
        {
            IntPtr hdc = GetDC(this.windowControl.Handle);
            this.gdiPlusViewport.PaintMe(hdc);
            ReleaseDC(this.windowControl.Handle, hdc);
        }
        public override void CopyOutputPixelBuffer(int x, int y, int w, int h, IntPtr outputBuffer)
        {
            //1. This version support on Win32 only
            //2. this is an example, to draw directly into the memDC, not need to create control            
            //3. x and y set to 0
            //4. w and h must be the width and height of the viewport


            unsafe
            {
                //create new memdc
                Win32.NativeWin32MemoryDc memDc = new Win32.NativeWin32MemoryDc(w, h);
                memDc.PatBlt(Win32.NativeWin32MemoryDc.PatBltColor.White);
                //TODO: check if we need to set init font/brush/pen for the new DC or not
                gdiPlusViewport.FullMode = true;
                //pain to the destination dc
                this.gdiPlusViewport.PaintMe(memDc.DC);
                IntPtr outputBits = memDc.PPVBits;
                //Win32.MyWin32.memcpy((byte*)outputBuffer, (byte*)memDc.PPVBits, w * 4 * h);
                memDc.CopyPixelBitsToOutput((byte*)outputBuffer);
                memDc.Dispose();
            }

        }

        public void PrintToCanvas(PixelFarm.Drawing.WinGdi.GdiPlusDrawBoard canvas)
        {
            this.gdiPlusViewport.PaintMe(canvas);
        }
        protected override void ChangeCursorStyle(MouseCursorStyle cursorStyle)
        {
            switch (cursorStyle)
            {
                case MouseCursorStyle.Pointer:
                    {
                        windowControl.Cursor = Cursors.Hand;
                    }
                    break;
                case MouseCursorStyle.IBeam:
                    {
                        windowControl.Cursor = Cursors.IBeam;
                    }
                    break;
                default:
                    {
                        windowControl.Cursor = Cursors.Default;
                    }
                    break;
            }
        }
    }
}