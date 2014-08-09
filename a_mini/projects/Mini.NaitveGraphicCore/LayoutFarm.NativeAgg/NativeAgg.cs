using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;

using LayoutFarm.NativeInterop;
namespace LayoutFarm.NativeAgg
{

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate int LibGetVersion();


    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate int RegisterManagedCallBack(IntPtr funcPtr, int callbackKind);


    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate void ManagedListenerDel(int mIndex, int methodName);


    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate int TestCallBack();


    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate int CallServices(int serviceNumber);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate IntPtr AggCreateCanvas();


    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate int AggApp_Paint(IntPtr nativeAggApp, IntPtr hdc);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate void AggApp_Move(IntPtr nativeAggApp, float x, float y);


    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate IntPtr CreateSprite();


    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate void CanvasClearBackground(IntPtr canvas);


    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate void SpriteDraw(IntPtr sprite, IntPtr canvas);


    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate void SpriteMove(IntPtr sprite, double x, double y);


    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    delegate IntPtr SpriteGetInternalPathStore(IntPtr sprite);

    static class NativeAggInterOp
    {
        static NativeModuleLoader nativeModuleLoader;
        //------------------------------------------------- 
        static IntPtr hModule;
        //-------------------------------------------------
        static ManagedListenerDel managedListener;
        static bool initOnce;
        static object syncRoot = new object();
        //-------------------------------------------------
        static IntPtr unmangedCallBack;

        [NativeFunc]
        static CallServices callServices;
        [NativeFunc]
        internal static AggCreateCanvas aggCreateCanvas;
        [NativeFunc]
        static AggApp_Paint aggCanvasPaint;
        [NativeFunc]
        static AggApp_Move aggMove;
        [NativeFunc]
        static CreateSprite createSprite;
        [NativeFunc]
        static RegisterManagedCallBack registerMxCallBack;
        [NativeFunc]
        internal static SpriteMove spriteMove;
        [NativeFunc]
        internal static SpriteDraw spriteDraw;
        [NativeFunc]
        internal static SpriteGetInternalPathStore spriteGetInternalPathStore;
        [NativeFunc]
        internal static CanvasClearBackground canvasClearBackground;
        [NativeFunc]
        internal static LibGetVersion libGetVersion;
        [NativeFunc]
        internal static TestCallBack testCallBack;
        //-------------------------------------------------

        /// <summary>
        /// load sqlite3 library
        /// </summary>
        public static void LoadLib()
        {
            lock (syncRoot)
            {
                if (initOnce)
                {
                    return;
                }

                //--------------------------
                //change location of dll ...
                //or embeded as resource file
                //-------------------------- 
                IntPtr nativeModule = UnsafeMethods.LoadLibrary(
                    @"..\..\..\NativeAgg\myagg\examples\win32_api\lion\Debug\lion.dll");
                hModule = nativeModule;

                if (nativeModule == IntPtr.Zero)
                {
                    return;
                }

                nativeModuleLoader = new NativeModuleLoader("libagg", nativeModule);
                nativeModuleLoader.LoadRequestProcs(typeof(NativeAggInterOp));
                 
                //1. get version
                int version = libGetVersion(); 
                //2. callback for pixellib  
                managedListener = new ManagedListenerDel(HandleCallFromNativePixelLib);
                unmangedCallBack = System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(managedListener);
                int regResult = registerMxCallBack(unmangedCallBack, 0);
                //3. test call back 
                var result2 = testCallBack(); 
                initOnce = true;
            }

        }
        static void ExecHandler(IntPtr notuse, int argCount, IntPtr char1, IntPtr char2)
        {

        }
        static void HandleCallFromNativePixelLib(int oindex, int name)
        {

        }


       

        public static int CallServerService(ServerServiceName serviceName)
        {
            return callServices((int)serviceName);
        }
        public static void AggPaint(IntPtr nativePtr, IntPtr hdc)
        {
            //paint svg content
            aggCanvasPaint(nativePtr, hdc);
        }



        public static void AggMove(IntPtr nativePtr, float x, float y)
        {
            aggMove(nativePtr, x, y);
        }
        public static IntPtr AggCreateSprite()
        {
            return createSprite();
        }

    }
    public class AggSprite
    {
        IntPtr nativePtr;
        double x;
        double y;
        private AggSprite(IntPtr nativePtr)
        {
            this.nativePtr = nativePtr;
        }
        public virtual void Draw(AppCanvas canvas)
        {
            NativeAggInterOp.spriteDraw(this.nativePtr, canvas.NaitveHandle);
        }
        public void SetLocation(double x, double y)
        {
            this.x = x;
            this.y = y;
            //move native data if exist
            NativeAggInterOp.spriteMove(this.nativePtr, x, y);
        }
        public double X
        {
            get
            {
                return this.x;
            }
        }
        public double Y
        {
            get
            {
                return this.y;
            }
        }
        public static AggSprite CreateNewSprite()
        {
            return new AggSprite(NativeAggInterOp.AggCreateSprite());
        }
        internal IntPtr NaitveHandle
        {
            get
            {
                return this.nativePtr;
            }
        }

    }
    public class AppCanvas
    {
        IntPtr nativePtr;
        private AppCanvas(IntPtr nativePtr)
        {
            this.nativePtr = nativePtr;
        }
        public void RenderTo(IntPtr hdc)
        {
            NativeAggInterOp.AggPaint(this.nativePtr, hdc);
        }
        public void ClearBackground()
        {
            NativeAggInterOp.canvasClearBackground(this.nativePtr);
        }
        public static AppCanvas CreateAppController()
        {
            return new AppCanvas(NativeAggInterOp.aggCreateCanvas());
        }
        public void Resize(float x, float y)
        {
            NativeAggInterOp.AggMove(this.nativePtr, x, y);
        }
        internal IntPtr NaitveHandle
        {
            get
            {
                return this.nativePtr;
            }
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