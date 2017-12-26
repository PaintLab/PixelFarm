using System;
using System.Runtime.InteropServices;
 
namespace System
{
    [System.Security.SuppressUnmanagedCodeSecurity] //apply this to all native methods in this class
    static class NaitveMemMx
    {
        //check this ....
        //for cross platform code

        //TODO: review here again***
        //this is platform specific ***

        [DllImport("msvcrt.dll", EntryPoint = "memset", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern void memset(byte* dest, byte c, int byteCount);
        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern void memcpy(byte* dest, byte* src, int byteCount);
        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl)]
        public static unsafe extern int memcmp(byte* dest, byte* src, int byteCount);
        //----------

        public static void MemSet(byte[] dest, int startAt, byte value, int count)
        {
            unsafe
            {
                fixed (byte* head = &dest[startAt])
                {
                    memset(head, value, count);
                }
            }
        }
        public static void MemCopy(byte[] dest_buffer, int dest_startAt, byte[] src_buffer, int src_StartAt, int len)
        {
            unsafe
            {
                fixed (byte* head_dest = &dest_buffer[dest_startAt])
                fixed (byte* head_src = &src_buffer[src_StartAt])
                {
                    memcpy(head_dest, head_src, len);
                }
            }
        }
        public static unsafe void MemCopy(byte* head_dest, byte* head_src, int len)
        {
            memcpy(head_dest, head_src, len);
        }
    }
    public abstract class PlatformMemory
    {
        static PlatformMemory platformMem;
        public static void SetPlatformMemImpl(PlatformMemory p)
        {
            platformMem = p;
        }
        //------------------------------------------------------------------------
        protected abstract void ProtectBlockLargeImpl(IntPtr h, ulong n, bool readAccess, bool writeAccess);
        protected abstract unsafe void CopyImpl(void* dstPtr, void* srcPtr, ulong len);
        protected abstract void FreeImpl(IntPtr hmem);

        protected abstract void FreeLargeImpl(IntPtr hmem, ulong len);
        protected abstract void FreeBitmapImpl(IntPtr hbmp, int w, int h);
        protected abstract IntPtr AllocateLargeImpl(ulong bytes);
        protected abstract IntPtr AllocateImpl(ulong bytes);
        protected abstract IntPtr AllocateBitmapImpl(int w, int h, out IntPtr handle);
        protected abstract void SetToZeroImpl(IntPtr ptr, ulong len);
        unsafe protected abstract void SetToZeroImpl(void* ptr, ulong len);
        ////--------------------------------------------------------------------
        //public static void ProtectBlockLarge(IntPtr h, ulong n, bool f1, bool f2)
        //{
        //    platformMem.ProtectBlockLargeImpl(h, n, f1, f2);
        //}
        public static unsafe void Copy(void* dstPtr, void* srcPtr, ulong len)
        {
            platformMem.CopyImpl(dstPtr, srcPtr, len);
        }
        public static void Free(IntPtr hmem)
        {
            platformMem.FreeImpl(hmem);
        }
        //public static void FreeLarge(IntPtr hmem, ulong len)
        //{
        //    platformMem.FreeLargeImpl(hmem, len);
        //}
        //public static void FreeBitmap(IntPtr hbmp, int w, int h)
        //{
        //    platformMem.FreeBitmapImpl(hbmp, w, h);
        //}
        //public static IntPtr AllocateLarge(ulong bytes)
        //{
        //    return platformMem.AllocateLargeImpl(bytes);
        //}
        public static IntPtr Allocate(ulong bytes)
        {
            return platformMem.AllocateImpl(bytes);
        }
        public static IntPtr AllocateBitmap(int w, int h, out IntPtr handle)
        {
            return platformMem.AllocateBitmapImpl(w, h, out handle);
        }
        //public static void SetToZero(IntPtr ptr, ulong len)
        //{
        //    platformMem.SetToZeroImpl(ptr, len);
        //}
        public static unsafe void SetToZero(void* ptr, int len)
        {
            NaitveMemMx.memset((byte*)ptr, 0, (int)len);
            //platformMem.SetToZeroImpl(ptr, len);
        }
        public static unsafe void SetToZero(void* ptr, ulong len)
        {
            NaitveMemMx.memset((byte*)ptr, 0, (int)len);
            //platformMem.SetToZeroImpl(ptr, len);
        }
    }
}