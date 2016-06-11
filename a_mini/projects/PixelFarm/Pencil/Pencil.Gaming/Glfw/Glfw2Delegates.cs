#region License
// Copyright (c) 2013 Antonie Blom
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

#if USE_GLFW2
using System;
using System.Security;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Pencil.Gaming {
	internal static class GlfwDelegates {
		static GlfwDelegates() {
#if DEBUG
			Stopwatch sw = new Stopwatch();
			sw.Start();
#endif
			Type glfwInterop = (IntPtr.Size == 8) ? typeof(Glfw64) : typeof(Glfw32);
#if DEBUG
			Console.WriteLine("GLFW interop: {0}", glfwInterop.Name);
#endif
			FieldInfo[] fields = typeof(GlfwDelegates).GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
			foreach (FieldInfo fi in fields) {
				MethodInfo mi = glfwInterop.GetMethod(fi.Name, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
				Delegate function = Delegate.CreateDelegate(fi.FieldType, mi);
				fi.SetValue(null, function);
			}
#if DEBUG
			sw.Stop();
			Console.WriteLine("Copying GLFW delegates took {0} milliseconds.", sw.ElapsedMilliseconds);
#endif
		}

#pragma warning disable 0649

		[SuppressUnmanagedCodeSecurity]
		internal delegate int Init();
		internal static Init glfwInit;
		[SuppressUnmanagedCodeSecurity]
		internal delegate void Terminate();
		internal static Terminate glfwTerminate;
		[SuppressUnmanagedCodeSecurity]
		internal delegate void GetVersion(out int major,out int minor,out int rev);
		internal static GetVersion glfwGetVersion;

		[SuppressUnmanagedCodeSecurity]
		internal delegate int OpenWindow(int width,int height,int redbits,int greenbits,int bluebits,int alphabits,int depthbits,int stencilbits,int mode);
		internal static OpenWindow glfwOpenWindow;
		[SuppressUnmanagedCodeSecurity]
		internal delegate void OpenWindowHint(int target,int hint);
		internal static OpenWindowHint glfwOpenWindowHint;
		[SuppressUnmanagedCodeSecurity]
		internal delegate void CloseWindow();
		internal static CloseWindow glfwCloseWindow;
		[SuppressUnmanagedCodeSecurity]
		internal delegate void SetWindowTitle([MarshalAs(UnmanagedType.LPStr)] string title);
		internal static SetWindowTitle glfwSetWindowTitle;
		[SuppressUnmanagedCodeSecurity]
		internal delegate void GetWindowSize(out int width,out int height);
		internal static GetWindowSize glfwGetWindowSize;
		[SuppressUnmanagedCodeSecurity]
		internal delegate void SetWindowSize(int width,int height);
		internal static SetWindowSize glfwSetWindowSize;
		[SuppressUnmanagedCodeSecurity]
		internal delegate void SetWindowPos(int x,int y);
		internal static SetWindowPos glfwSetWindowPos;
		[SuppressUnmanagedCodeSecurity]
		internal delegate void IconifyWindow();
		internal static IconifyWindow glfwIconifyWindow;
		[SuppressUnmanagedCodeSecurity]
		internal delegate void RestoreWindow();
		internal static RestoreWindow glfwRestoreWindow;
		[SuppressUnmanagedCodeSecurity]
		internal delegate void SwapBuffers();
		internal static SwapBuffers glfwSwapBuffers;
		[SuppressUnmanagedCodeSecurity]
		internal delegate void SwapInterval(int interval);
		internal static SwapInterval glfwSwapInterval;
		[SuppressUnmanagedCodeSecurity]
		internal delegate int GetWindowParam(int param);
		internal static GetWindowParam glfwGetWindowParam;
		[SuppressUnmanagedCodeSecurity]
		internal delegate void SetWindowSizeCallback(GlfwWindowSizeFun cbfun);
		internal static SetWindowSizeCallback glfwSetWindowSizeCallback;
		[SuppressUnmanagedCodeSecurity]
		internal delegate void SetWindowCloseCallback(GlfwWindowCloseFun cbfun);
		internal static SetWindowCloseCallback glfwSetWindowCloseCallback;
		[SuppressUnmanagedCodeSecurity]
		internal delegate void SetWindowRefreshCallback(GlfwWindowRefreshFun cbfun);
		internal static SetWindowRefreshCallback glfwSetWindowRefreshCallback;

		[SuppressUnmanagedCodeSecurity]
		internal delegate int GetVideoModes([MarshalAs(UnmanagedType.LPArray)] GlfwVidMode[] list,int maxcount);
		internal static GetVideoModes glfwGetVideoModes;
		[SuppressUnmanagedCodeSecurity]
		internal delegate void GetDesktopMode(out GlfwVidMode mode);
		internal static GetDesktopMode glfwGetDesktopMode;

		[SuppressUnmanagedCodeSecurity]
		internal delegate void PollEvents();
		internal static PollEvents glfwPollEvents;
		[SuppressUnmanagedCodeSecurity]
		internal delegate void WaitEvents();
		internal static WaitEvents glfwWaitEvents;
		[SuppressUnmanagedCodeSecurity]
		internal delegate int GetKey(int key);
		internal static GetKey glfwGetKey;
		[SuppressUnmanagedCodeSecurity]
		internal delegate int GetMouseButton(int button);
		internal static GetMouseButton glfwGetMouseButton;
		[SuppressUnmanagedCodeSecurity]
		internal delegate void GetMousePos(out int xpos,out int ypos);
		internal static GetMousePos glfwGetMousePos;
		[SuppressUnmanagedCodeSecurity]
		internal delegate void SetMousePos(int xpos,int ypos);
		internal static SetMousePos glfwSetMousePos;
		[SuppressUnmanagedCodeSecurity]
		internal delegate int GetMouseWheel();
		internal static GetMouseWheel glfwGetMouseWheel;
		[SuppressUnmanagedCodeSecurity]
		internal delegate void SetMouseWheel(int pos);
		internal static SetMouseWheel glfwSetMouseWheel;
		[SuppressUnmanagedCodeSecurity]
		internal delegate void SetKeyCallback(GlfwKeyFun cbfun);
		internal static SetKeyCallback glfwSetKeyCallback;
		[SuppressUnmanagedCodeSecurity]
		internal delegate void SetCharCallback(GlfwCharFun cbfun);
		internal static SetCharCallback glfwSetCharCallback;
		[SuppressUnmanagedCodeSecurity]
		internal delegate void SetMouseButtonCallback(GlfwMouseButtonFun cbfun);
		internal static SetMouseButtonCallback glfwSetMouseButtonCallback;
		[SuppressUnmanagedCodeSecurity]
		internal delegate void SetMousePosCallback(GlfwMousePosFun cbfun);
		internal static SetMousePosCallback glfwSetMousePosCallback;
		[SuppressUnmanagedCodeSecurity]
		internal delegate void SetMouseWheelCallback(GlfwMouseWheelFun cbfun);
		internal static SetMouseWheelCallback glfwSetMouseWheelCallback;

		[SuppressUnmanagedCodeSecurity]
		internal delegate int GetJoystickParam(int joy,int param);
		internal static GetJoystickParam glfwGetJoystickParam;
		[SuppressUnmanagedCodeSecurity]
		internal delegate int GetJoystickPos(int joy,[MarshalAs(UnmanagedType.LPArray)] float[] pos,int numaxes);
		internal static GetJoystickPos glfwGetJoystickPos;
		[SuppressUnmanagedCodeSecurity]
		internal delegate int GetJoystickButtons(int joy,[MarshalAs(UnmanagedType.LPArray)] byte[] buttons,int numbuttons);
		internal static GetJoystickButtons glfwGetJoystickButtons;

		[SuppressUnmanagedCodeSecurity]
		internal delegate double GetTime();
		internal static GetTime glfwGetTime;
		[SuppressUnmanagedCodeSecurity]
		internal delegate void SetTime(double time);
		internal static SetTime glfwSetTime;

		[SuppressUnmanagedCodeSecurity]
		internal delegate int ExtensionSupported([MarshalAs(UnmanagedType.LPStr)] string extension);
		internal static ExtensionSupported glfwExtensionSupported;
		[SuppressUnmanagedCodeSecurity]
		internal delegate IntPtr GetProcAddress([MarshalAs(UnmanagedType.LPStr)] string procname);
		internal static GetProcAddress glfwGetProcAddress;
		[SuppressUnmanagedCodeSecurity]
		internal delegate void GetGLVersion(out int major,out int minor,out int rev);
		internal static GetGLVersion glfwGetGLVersion;

		[SuppressUnmanagedCodeSecurity]
		internal delegate void Enable(int token);
		internal static Enable glfwEnable;
		[SuppressUnmanagedCodeSecurity]
		internal delegate void Disable(int token);
		internal static Disable glfwDisable;

		[SuppressUnmanagedCodeSecurity]
		internal delegate int ReadImage([MarshalAs(UnmanagedType.LPStr)] string name,out GlfwImage img,int flags);
		internal static ReadImage glfwReadImage;
		[SuppressUnmanagedCodeSecurity]
		internal delegate int ReadMemoryImage(IntPtr data,long size,ref GlfwImage img,int flags);
		internal static ReadMemoryImage glfwReadMemoryImage;
		[SuppressUnmanagedCodeSecurity]
		internal delegate void FreeImage(ref GlfwImage img);
		internal static FreeImage glfwFreeImage;
		[SuppressUnmanagedCodeSecurity]
		internal delegate int LoadTexture2D([MarshalAs(UnmanagedType.LPStr)] string name,int flags);
		internal static LoadTexture2D glfwLoadTexture2D;
		[SuppressUnmanagedCodeSecurity]
		internal delegate int LoadMemoryTexture2D(IntPtr data,long size,int flags);
		internal static LoadMemoryTexture2D glfwLoadMemoryTexture2D;
		[SuppressUnmanagedCodeSecurity]
		internal delegate int LoadTextureImage2D(ref GlfwImage img,int flags);
		internal static LoadTextureImage2D glfwLoadTextureImage2D;
	}
}

#endif