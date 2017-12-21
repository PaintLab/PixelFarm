using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;

using LayoutFarm.NativeInterop;

namespace LayoutFarm.NativeWindows
{
    public static class WinNative
    {
        [DllImport("User32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);
        [DllImport("User32.dll")]
        public static extern int ReleaseDC(IntPtr hWnd,IntPtr hdc);
    }
}