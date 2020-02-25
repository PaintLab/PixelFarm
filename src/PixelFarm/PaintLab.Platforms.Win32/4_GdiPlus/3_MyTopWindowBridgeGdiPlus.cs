//Apache2, 2014-present, WinterDev

using System;
using PixelFarm.Drawing;

namespace LayoutFarm.UI.GdiPlus
{
    using LayoutFarm.UI.InputBridge;
    class MyTopWindowBridgeGdiPlus : AbstractTopWindowBridge
    {
        IGpuOpenGLSurfaceView _windowControl;
        GdiPlusCanvasViewport _gdiPlusViewport;

#if DEBUG
        static int s_totalDebugId;
        public readonly int dbugId = s_totalDebugId++;
        int dbugPaintToOutputWin;
#endif


        public MyTopWindowBridgeGdiPlus(RootGraphic root, ITopWindowEventRoot topWinEventRoot)
            : base(root, topWinEventRoot)
        {

        }
        protected override void ChangeCursor(ImageBinder imgbinder)
        {
            //use custom cursor 
            //if not support then just ignore
            return;
        }
        public override void BindWindowControl(IGpuOpenGLSurfaceView windowControl)
        {
            //bind to anycontrol GDI control  
            _windowControl = windowControl;
            this.SetBaseCanvasViewport(_gdiPlusViewport = new GdiPlusCanvasViewport(this.RootGfx, this.Size.ToSize()));
            this.RootGfx.SetPaintDelegates(
                    _gdiPlusViewport.CanvasInvalidateArea,
                    this.PaintToOutputWindow);
#if DEBUG
            //this.dbugWinControl = (Control)windowControl;
            _gdiPlusViewport.dbugOutputWindow = this;
#endif
            this.EvaluateScrollbar();
        }

        System.Drawing.Size Size => _windowControl.GetSize().ToSize();

 
        public override void PaintToOutputWindow()
        {
            IntPtr winHandle = _windowControl.NativeWindowHwnd;
            IntPtr hdc = Win32.MyWin32.GetDC(winHandle);
            _gdiPlusViewport.PaintMe(hdc);
            Win32.MyWin32.ReleaseDC(winHandle, hdc);
#if DEBUG
            //Console.WriteLine("p->w  " + dbugId + " " + dbugPaintToOutputWin++);
#endif
        }
        public override void PaintToOutputWindow(Rectangle invalidateArea)
        {
            IntPtr winHandle = _windowControl.NativeWindowHwnd;
            IntPtr hdc = Win32.MyWin32.GetDC(winHandle);
            _gdiPlusViewport.PaintMe(hdc, invalidateArea);
            Win32.MyWin32.ReleaseDC(winHandle, hdc);
#if DEBUG
            //System.Diagnostics.Debug.WriteLine("p->w2 " + dbugId + " " + dbugPaintToOutputWin++ + " " + invalidateArea.ToString());
#endif
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
                Win32.NativeWin32MemoryDC memDc = new Win32.NativeWin32MemoryDC(w, h);
                memDc.PatBlt(Win32.NativeWin32MemoryDC.PatBltColor.White);
                //TODO: check if we need to set init font/brush/pen for the new DC or not
                _gdiPlusViewport.FullMode = true;
                //pain to the destination dc
                _gdiPlusViewport.PaintMe(memDc.DC);
                IntPtr outputBits = memDc.PPVBits;
                //Win32.MyWin32.memcpy((byte*)outputBuffer, (byte*)memDc.PPVBits, w * 4 * h);
                memDc.CopyPixelBitsToOutput((byte*)outputBuffer);
                memDc.Dispose();
            }
        }

        protected override void ChangeCursor(MouseCursorStyle cursorStyle)
        {
            //switch (cursorStyle)
            //{
            //    case MouseCursorStyle.Pointer:
            //        {
            //            _windowControl.Cursor = Cursors.Hand;
            //        }
            //        break;
            //    case MouseCursorStyle.IBeam:
            //        {
            //            _windowControl.Cursor = Cursors.IBeam;
            //        }
            //        break;
            //    default:
            //        {
            //            _windowControl.Cursor = Cursors.Default;
            //        }
            //        break;
            //}
        }
    }



   public class MyTopWindowBridgeAgg : AbstractTopWindowBridge
    {
        IGpuOpenGLSurfaceView _windowControl;
        IntPtr _nativeHwnd;
        GdiPlusCanvasViewport _gdiPlusViewport;
#if DEBUG
        static int s_totalDebugId;
        public readonly int dbugId = s_totalDebugId++;
        int dbugPaintToOutputWin;
#endif
        public MyTopWindowBridgeAgg(RootGraphic root, ITopWindowEventRoot topWinEventRoot)
            : base(root, topWinEventRoot)
        {

        }

        public override void BindWindowControl(IGpuOpenGLSurfaceView windowControl)
        {
            //bind to anycontrol GDI control  
            _windowControl = windowControl;
            _nativeHwnd = windowControl.NativeWindowHwnd;
            //_winControl = (Control)windowControl;
            this.SetBaseCanvasViewport(_gdiPlusViewport = new GdiPlusCanvasViewport(this.RootGfx, this.Size.ToSize()));
            this.RootGfx.SetPaintDelegates(
                    _gdiPlusViewport.CanvasInvalidateArea,
                    this.PaintToOutputWindow);
#if DEBUG
            //this.dbugWinControl = (Control)windowControl;
            _gdiPlusViewport.dbugOutputWindow = this;
#endif
            this.EvaluateScrollbar();
        }
        //
        System.Drawing.Size Size => _windowControl.GetSize().ToSize();
       

        public override void PaintToOutputWindow()
        {

            IntPtr hdc = Win32.MyWin32.GetDC(_nativeHwnd);
            _gdiPlusViewport.PaintMe(hdc);
            Win32.MyWin32.ReleaseDC(_nativeHwnd, hdc);
#if DEBUG
            //Console.WriteLine("p->w  " + dbugId + " " + dbugPaintToOutputWin++);
#endif
        }
        public override void PaintToOutputWindow(Rectangle invalidateArea)
        {

            IntPtr hdc = Win32.MyWin32.GetDC(_nativeHwnd);
            _gdiPlusViewport.PaintMe(hdc, invalidateArea);
            Win32.MyWin32.ReleaseDC(_nativeHwnd, hdc);
#if DEBUG
            //System.Diagnostics.Debug.WriteLine("p->w2 " + dbugId + " " + dbugPaintToOutputWin++ + " " + invalidateArea.ToString());
#endif
        }
        public override void CopyOutputPixelBuffer(int x, int y, int w, int h, IntPtr outputBuffer)
        {
            //1. This version support on Win32 only
            //2. this is an example, to draw directly into the memDC, not need to create control            
            //3. x and y set to 0
            //4. w and h must be the width and height of the viewport 
            //create new memdc
            using (var memDc = new Win32.NativeWin32MemoryDC(w, h))
            {
                //clear background with white color
                memDc.PatBlt(Win32.NativeWin32MemoryDC.PatBltColor.White);

                //TODO: check if we need to set init font/brush/pen for the new DC or not
                _gdiPlusViewport.FullMode = true;

                //pain to the destination dc 
                _gdiPlusViewport.PaintMe(memDc.DC);

                unsafe
                {
                    memDc.CopyPixelBitsToOutput((byte*)memDc.PPVBits);
                }
            }
        }
        protected override void ChangeCursor(ImageBinder imgbinder)
        {
            //use custom cursor 
            //if not support then just ignore
            return;
        }
        protected override void ChangeCursor(MouseCursorStyle cursorStyle)
        {
            //switch (cursorStyle)
            //{
            //    case MouseCursorStyle.Pointer:
            //        {
            //            _windowControl.Cursor = Cursors.Hand;
            //        }
            //        break;
            //    case MouseCursorStyle.IBeam:
            //        {
            //            _windowControl.Cursor = Cursors.IBeam;
            //        }
            //        break;
            //    default:
            //        {
            //            _windowControl.Cursor = Cursors.Default;
            //        }
            //        break;
            //}
        }
    }
}