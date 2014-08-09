using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

using LayoutFarm.NativeInterop;
namespace LayoutFarm.NativePixelLib
{

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate int LibGetVersion();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate int RegisterManagedCallBack(IntPtr funcPtr, int callbackKind);//int oindex, [MarshalAs(UnmanagedType.LPWStr)]string methodName);


    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate void ManagedListenerDel(int mIndex, int methodName);


    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate void PixelLibCallbackHandler(IntPtr notuse, int argCount, IntPtr char1, IntPtr char2);


    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate int TestCallBack();


    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate int CallServices(IntPtr nativeWinVideo, int serviceNumber);


    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate IntPtr SetupMainWindow(IntPtr hWnd);


    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate void DrawImage(IntPtr nativeWinVideo, IntPtr bmp, int x1, int y1, int x2, int y2);


    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate void DrawImage2(IntPtr nativeWinVideo, IntPtr bmp, int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4);


    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate IntPtr MakeBitmapWrapper(int bmpW, int bmpH, int stride, int bpp, IntPtr pixels);


    static class NativePixelLibInterOp
    {
        //-------------------------------------------------
        static IntPtr hModule;
        static NativeModuleLoader nativeModuleLoader;
        //------------------------------------------------- 
        [NativeFunc]
        internal static SetupMainWindow setupMainWindow;
        [NativeFunc]
        static CallServices callServices;
        [NativeFunc]
        internal static DrawImage drawImage;
        [NativeFunc]
        internal static DrawImage2 drawImage2;
        [NativeFunc]
        internal static MakeBitmapWrapper makeBitmapWrapper;
        [NativeFunc]
        internal static LibGetVersion libGetVersion;
        [NativeFunc]
        static ManagedListenerDel managedListener;
        [NativeFunc]
        static RegisterManagedCallBack registerMxCallBack;
        [NativeFunc]
        static TestCallBack testCallBack;


        static IntPtr myCallBackDelegate;

        static bool initOnce;
        static object syncRoot = new object();


        public static void LoadLib()
        {
            lock (syncRoot)
            {
                if (initOnce)
                {
                    return;
                }

                IntPtr nativeModule = UnsafeMethods.LoadLibrary(@"pixellib01.dll");
                hModule = nativeModule;
                if (nativeModule == IntPtr.Zero)
                {
                    return;
                }

                nativeModuleLoader = new NativeModuleLoader("pixellib01", hModule);
                nativeModuleLoader.LoadRequestProcs(typeof(NativePixelLibInterOp));
                //-------------------------------
                //1. get version                 
                int version = libGetVersion();
                //2. callback for pixellib                  
                managedListener = new ManagedListenerDel(HandleCallFromNativePixelLib);
                myCallBackDelegate = System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(managedListener);
                int regResult = registerMxCallBack(myCallBackDelegate, 0);
                //3. test call back 
                var result2 = testCallBack();
                //-------------------------------
                //ok
                //start graphic surface
                //------------------------------- 
                initOnce = true;
            }

        }
        static void ExecHandler(IntPtr notuse, int argCount, IntPtr char1, IntPtr char2)
        {

        }
        static void HandleCallFromNativePixelLib(int oindex, int name)
        {

        }


        public static int CallServerService(IntPtr nativeWinVideo, ServerServiceName serviceName)
        {
            return callServices(nativeWinVideo, (int)serviceName);
        }
        public static void DrawImage(IntPtr canvasPtr, NativeBmp nativeBmp, int x, int y, int w, int h)
        {
            int height = 600;
            int x1 = x;
            int y1 = y;
            int x2 = x1 + w;
            int y2 = y1 + h;
            //flip coord
            NativePixelLibInterOp.drawImage(canvasPtr,
                nativeBmp.Handle, x1, height - y1, x2, height - y2);
        }
    }

    public class NativeCanvas
    {
        IntPtr nativeCanvasPtr;
        int w;
        int h;
        private NativeCanvas(IntPtr nativeCanvasPtr)
        {
            this.nativeCanvasPtr = nativeCanvasPtr;
        }
        public static NativeCanvas CreateNativeCanvas(IntPtr windowPtr, int w, int h)
        {
            IntPtr ptr = NativePixelLibInterOp.setupMainWindow(windowPtr);
            NativeCanvas nativeCanvas = new NativeCanvas(ptr);
            nativeCanvas.w = w;
            nativeCanvas.h = h;

            if (NativePixelLibInterOp.CallServerService(ptr, ServerServiceName.Init) == 0)
            {
                //ok
            }
            return nativeCanvas;

        }
        public int Width { get { return this.w; } }
        public int Height { get { return this.h; } }
        public void ClearBackground()
        {
            NativePixelLibInterOp.CallServerService(nativeCanvasPtr, ServerServiceName.Draw4);
        }
        public void DrawImage(NativeBmp bmp, int x, int y)
        {
            NativePixelLibInterOp.DrawImage(
                       this.nativeCanvasPtr, bmp,
                       x, y, bmp.Width, bmp.Height);
        }
        public void DrawImage(NativeBmp bmp, int x, int y, int w, int h)
        {
            NativePixelLibInterOp.DrawImage(
                       this.nativeCanvasPtr, bmp,
                       x, y, w, h);
        }
        public void DrawImage(NativeBmp bmp, Point[] fourCorners)
        {

        }
        public void Render()
        {
            NativePixelLibInterOp.CallServerService(nativeCanvasPtr, ServerServiceName.RefreshScreen);
        }
        public void Dispose()
        {
            NativePixelLibInterOp.CallServerService(nativeCanvasPtr, ServerServiceName.Shutdown);
        }
    }
    public class NativeBmp
    {
        IntPtr nativeBmp;
        readonly int w;
        readonly int h;
        private NativeBmp(IntPtr nativeBmp, int w, int h)
        {
            this.nativeBmp = nativeBmp;
            this.w = w;
            this.h = h;
        }
        internal IntPtr Handle
        {
            get { return this.nativeBmp; }
        }
        public int Width { get { return this.w; } }
        public int Height { get { return this.h; } }

        public static NativeBmp CreateNativeWrapper(int bmpW, int bmpH, int stride, int bpp, IntPtr pixels)
        {
            IntPtr nbmp = NativePixelLibInterOp.makeBitmapWrapper(bmpW, bmpH, stride, bpp, pixels);
            return new NativeBmp(nbmp, bmpW, bmpH);
        }
        public static NativeBmp CreateNativeWrapper(System.Drawing.Bitmap bmp)
        {

            var bmpdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);

            int bpp = 24;
            switch (bmpdata.PixelFormat)
            {
                case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                    {
                        bpp = 24;
                    } break;
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                    {
                        bpp = 32;
                    } break;
                default:
                    {
                        throw new NotSupportedException();
                    }

            }
            NativeBmp nativeBmp = CreateNativeWrapper(bmp.Width, bmp.Height, bmpdata.Stride, bpp, bmpdata.Scan0);
            bmp.UnlockBits(bmpdata);
            return nativeBmp;
        }
    }

    public enum ServerServiceName
    {
        Unknown,
        Init,//1
        RefreshScreen,//2
        Shutdown,//3
        Draw2,//4
        Draw4,//5

    }

}