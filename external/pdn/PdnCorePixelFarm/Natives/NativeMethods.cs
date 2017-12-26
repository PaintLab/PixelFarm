/////////////////////////////////////////////////////////////////////////////////
// Paint.NET                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, Tom Jackson, and contributors.     //
// Portions Copyright (C) Microsoft Corporation. All Rights Reserved.          //
// See src/Resources/Files/License.txt for full licensing and attribution      //
// details.                                                                    //
// .                                                                           //
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace PaintDotNet.SystemLayer
{
    internal static class NativeMethods
    {

        internal static void ThrowOnWin32Error(string message)
        {
            int lastWin32Error = Marshal.GetLastWin32Error();
            ThrowOnWin32Error(message, lastWin32Error);
        }

        internal static void ThrowOnWin32Error(string message, NativeErrors lastWin32Error)
        {
            ThrowOnWin32Error(message, (int)lastWin32Error);
        }

        internal static void ThrowOnWin32Error(string message, int lastWin32Error)
        {
            if (lastWin32Error != NativeConstants.ERROR_SUCCESS)
            {
                string exMessageFormat = "{0} ({1}, {2})";
                string exMessage = string.Format(exMessageFormat, message, lastWin32Error, ((NativeErrors)lastWin32Error).ToString());

                throw new Win32Exception(lastWin32Error, exMessage);
            }
        }
    }
}
