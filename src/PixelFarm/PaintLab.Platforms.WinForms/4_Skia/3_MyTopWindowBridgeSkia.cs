//Apache2, 2014-present, WinterDev

using System;
using System.Windows.Forms;
using PixelFarm.Drawing;
#if __SKIA__
namespace LayoutFarm.UI.Skia
{
    class MyTopWindowBridgeSkia : TopWindowBridgeWinForm
    {
        Control _windowControl;
        SkiaCanvasViewport _canvasViewport;
        public MyTopWindowBridgeSkia(RootGraphic root, ITopWindowEventRoot topWinEventRoot)
            : base(root, topWinEventRoot)
        {
        }
        public override void PaintToOutputWindow(Rectangle invalidateArea)
        {
            throw new NotImplementedException();
        }
        public override void BindWindowControl(Control windowControl)
        {
            //bind to anycontrol GDI control  
            this._windowControl = windowControl;
            this.SetBaseCanvasViewport(this._canvasViewport = new SkiaCanvasViewport(this.RootGfx,
                this.Size.ToSize()));

            this.RootGfx.SetPaintDelegates(
                    this._canvasViewport.CanvasInvalidateArea,
                    this.PaintToOutputWindow);
#if DEBUG
            this.dbugWinControl = windowControl;
            this._canvasViewport.dbugOutputWindow = this;
#endif
            this.EvaluateScrollbar();
        }
        System.Drawing.Size Size
        {
            get { return this._windowControl.Size; }
        }
        public override void InvalidateRootArea(Rectangle r)
        {
#if DEBUG
            Rectangle rect = r;
#endif
            this.RootGfx.InvalidateRootGraphicArea(ref r);
        }
        public override void PaintToOutputWindow()
        {
            //*** force paint to output viewdow
            IntPtr hdc = GetDC(this._windowControl.Handle);
            this._canvasViewport.PaintMe(hdc);
            ReleaseDC(this._windowControl.Handle, hdc);
        }
        public override void CopyOutputPixelBuffer(int x, int y, int w, int h, IntPtr outputBuffer)
        {
            throw new NotImplementedException();
        }
        protected override void ChangeCursorStyle(MouseCursorStyle cursorStyle)
        {
            switch (cursorStyle)
            {
                case MouseCursorStyle.Pointer:
                    {
                        _windowControl.Cursor = Cursors.Hand;
                    }
                    break;
                case MouseCursorStyle.IBeam:
                    {
                        _windowControl.Cursor = Cursors.IBeam;
                    }
                    break;
                default:
                    {
                        _windowControl.Cursor = Cursors.Default;
                    }
                    break;
            }
        }
    }
}

#endif