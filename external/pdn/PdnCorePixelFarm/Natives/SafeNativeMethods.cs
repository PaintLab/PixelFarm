/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;

namespace PaintDotNet.SystemLayer
{
    [SuppressUnmanagedCodeSecurity]
    internal static class SafeNativeMethods
    {


        [DllImport("gdi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr CreateFontW(
            int nHeight,
            int nWidth,
            int nEscapement,
            int nOrientation,
            int fnWeight,
            uint fdwItalic,
            uint fdwUnderline,
            uint fdwStrikeOut,
            uint fdwCharSet,
            uint fdwOutputPrecision,
            uint fdwClipPrecision,
            uint fdwQuality,
            uint fdwPitchAndFamily,
            string lpszFace);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int DrawTextW(
            IntPtr hdc,
            string lpString,
            int nCount,
            ref NativeStructs.RECT lpRect,
            uint uFormat);

        [DllImport("gdi32.dll", SetLastError = true)]
        internal static extern IntPtr CreateDIBSection(
            IntPtr hdc,
            ref NativeStructs.BITMAPINFO pbmi,
            uint iUsage,
            out IntPtr ppvBits,
            IntPtr hSection,
            uint dwOffset);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern uint WaitForSingleObject(
            IntPtr hHandle,
            uint dwMilliseconds);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern uint WaitForMultipleObjects(
            uint nCount,
            IntPtr[] lpHandles,
            [MarshalAs(UnmanagedType.Bool)] bool bWaitAll,
            uint dwMilliseconds);

        internal static uint WaitForMultipleObjects(IntPtr[] lpHandles, bool bWaitAll, uint dwMilliseconds)
        {
            return WaitForMultipleObjects((uint)lpHandles.Length, lpHandles, bWaitAll, dwMilliseconds);
        }


        [DllImport("Gdi32.dll", SetLastError = true)]
        internal unsafe static extern uint GetRegionData(
            IntPtr hRgn,
            uint dwCount,
            NativeStructs.RGNDATA* lpRgnData);

        [DllImport("Gdi32.dll", SetLastError = true)]
        internal unsafe static extern IntPtr CreateRectRgn(
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect);

        [DllImport("Gdi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal extern static bool MoveToEx(
            IntPtr hdc,
            int X,
            int Y,
            out NativeStructs.POINT lpPoint);

        [DllImport("Gdi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal extern static bool LineTo(
            IntPtr hdc,
            int nXEnd,
            int nYEnd);

        [DllImport("User32.dll", SetLastError = true)]
        internal extern static int FillRect(
            IntPtr hDC,
            ref NativeStructs.RECT lprc,
            IntPtr hbr);

        [DllImport("Gdi32.dll", SetLastError = true)]
        internal extern static IntPtr CreatePen(
            int fnPenStyle,
            int nWidth,
            uint crColor);

        [DllImport("Gdi32.dll", SetLastError = true)]
        internal extern static IntPtr CreateSolidBrush(uint crColor);

        [DllImport("Gdi32.dll", SetLastError = false)]
        internal extern static IntPtr SelectObject(
            IntPtr hdc,
            IntPtr hgdiobj);

        [DllImport("Gdi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal extern static bool DeleteObject(IntPtr hObject);

        [DllImport("Gdi32.dll", SetLastError = true)]
        internal extern static uint DeleteDC(IntPtr hdc);

        [DllImport("Gdi32.Dll", SetLastError = true)]
        internal extern static IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("Gdi32.Dll", SetLastError = true)]
        internal extern static uint BitBlt(
            IntPtr hdcDest,
            int nXDest,
            int nYDest,
            int nWidth,
            int nHeight,
            IntPtr hdcSrc,
            int nXSrc,
            int nYSrc,
            uint dwRop);


    }
}
