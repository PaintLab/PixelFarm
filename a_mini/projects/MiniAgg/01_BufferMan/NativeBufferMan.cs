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
        public int TotalAllocSize
        {
            get
            {
                return this.totalAllocSize;
            }
        }
        public void Clear()
        {
            //clear with zero

            MemSet(this.mmm, 0, 0, totalAllocSize);
        }
        public void Clear(byte b)
        {
            //clear with zero
            MemSet(this.mmm, 0, b, totalAllocSize);
        }



        //-------------------------------------------------------------------------
        public static void MemSet(byte[] dest, int startAt, byte value, int count)
        {
            unsafe
            {
                fixed (byte* head = &dest[0])
                {
                    memset(head, 0, 100);
                }
            }
        }
        public static void MemCopy(byte[] dest_buffer, int outputStartAt, byte[] src_buffer, int src_index, int len)
        {
            unsafe
            {
                fixed (byte* head_dest = &dest_buffer[outputStartAt])
                fixed (byte* head_src = &src_buffer[src_index])
                {
                    memcpy(head_dest, head_src, len);
                }
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