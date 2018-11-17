//Apache2, 2014-present, WinterDev

using System;
using PixelFarm.Forms;
using PixelFarm.Drawing;
namespace LayoutFarm.UI.Skia
{
    class MyTopWindowBridgeSkia : TopWindowBridgeWinNeutral
    {
        Control _windowControl;
        SkiaCanvasViewport _canvasViewport;
        public MyTopWindowBridgeSkia(RootGraphic root, ITopWindowEventRoot topWinEventRoot)
            : base(root, topWinEventRoot)
        {
        }
        public override void BindWindowControl(Control windowControl)
        {
            //bind to anycontrol GDI control  
            this._windowControl = windowControl;
            this.SetBaseCanvasViewport(this._canvasViewport = new SkiaCanvasViewport(this.RootGfx,
                new Size(windowControl.Width, windowControl.Height)));

            this.RootGfx.SetPaintDelegates(
                    this._canvasViewport.CanvasInvalidateArea,
                    this.PaintToOutputWindow);
#if DEBUG
            this.dbugWinControl = windowControl;
            this._canvasViewport.dbugOutputWindow = this;
#endif
            this.EvaluateScrollbar();
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
            //TODO: review here
            throw new NotSupportedException();
            ////*** force paint to output viewdow
            //IntPtr hdc = Win32.MyWin32.GetDC(this.windowControl.Handle);
            //this.canvasViewport.PaintMe(hdc);
            //Win32.MyWin32.ReleaseDC(this.windowControl.Handle, hdc);
        }
        //public void PrintToCanvas(PixelFarm.Drawing.WinGdi.MyGdiPlusCanvas canvas)
        //{
        //    this.canvasViewport.PaintMe(canvas);
        //}
        protected override void ChangeCursorStyle(MouseCursorStyle cursorStyle)
        {
            //implement change cursor style 
            //switch (cursorStyle)
            //{
            //    case MouseCursorStyle.Pointer:
            //        {
            //            windowControl.Cursor = Cursors.Hand;
            //        }
            //        break;
            //    case MouseCursorStyle.IBeam:
            //        {
            //            windowControl.Cursor = Cursors.IBeam;
            //        }
            //        break;
            //    default:
            //        {
            //            windowControl.Cursor = Cursors.Default;
            //        }
            //        break;
            //}
        }
    }
}