//MIT 2014, WinterDev
using System;
using System.IO;
using System.Runtime.InteropServices;
namespace MatterHackers.Agg
{

    public class NativeBufferMan
    {
        byte[] mmm; 
        int totalAllocSize;
        int currentIndex; 
        public NativeBufferMan(int initSize)
        {
            this.mmm = new byte[initSize];
            this.totalAllocSize = initSize;
        }
        public void Clear()
        {
            //clear with zero

            MemSet(0, 0, totalAllocSize);
        }
        public void Clear(byte b)
        {
            //clear with zero
            MemSet(0, b, totalAllocSize);
        }
        public void MemSet(int startAt, byte value, int count)
        {
            unsafe
            {
                fixed (byte* head = &mmm[0])
                {
                    memset(head, 0, 100);
                }
            }
        }
        public void MemCopy(int startAt, int len, byte[] outputBuffer, int outputStartAt)
        {
            unsafe
            {
                fixed (byte* head_dest = &outputBuffer[outputStartAt])
                fixed (byte* head_src = &mmm[startAt])
                {
                    memcpy(head_dest, head_src, len);
                }
            }

        }
        public int TotalAllocSize
        {
            get
            {
                return this.totalAllocSize;
            }
        }

        [DllImport("msvcrt.dll", EntryPoint = "memset", CallingConvention = CallingConvention.Cdecl)]
        static unsafe extern void memset(byte* dest, byte c, int byteCount);
        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl)]
        static unsafe extern void memcpy(byte* dest, byte* src, int byteCount);

        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl)]
        static unsafe extern int memcmp(byte* dest, byte* src, int byteCount);

    }

}