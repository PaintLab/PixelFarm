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
using System.Runtime.InteropServices;

namespace Pencil.Gaming {
	internal static class Glfw64 {
		private const string lib = "natives64/glfw.dll";

		[DllImport(Glfw64.lib), SuppressUnmanagedCodeSecurity]
		internal static extern int glfwInit();
		[DllImport(Glfw64.lib), SuppressUnmanagedCodeSecurity]
		internal static extern void glfwTerminate();
		[DllImport(Glfw64.lib), SuppressUnmanagedCodeSecurity]
		internal static extern void glfwGetVersion(out int major, out int minor, out int rev);

		[DllImport(Glfw64.lib), SuppressUnmanagedCodeSecurity]
		internal static extern int glfwOpenWindow(int width, int height, int redbits, int greenbits, int bluebits, int alphabits, int depthbits, int stencilbits, int mode);
		[DllImport(Glfw64.lib), SuppressUnmanagedCodeSecurity]
		internal static extern void glfwOpenWindowHint(int target, int hint);
		[DllImport(Glfw64.lib), SuppressUnmanagedCodeSecurity]
		internal static extern void glfwCloseWindow();
		[DllImport(Glfw64.lib), SuppressUnmanagedCodeSecurity]
		internal static extern void glfwSetWindowTitle([MarshalAs(UnmanagedType.LPStr)] string title);
		[DllImport(Glfw64.lib), SuppressUnmanagedCodeSecurity]
		internal static extern void glfwGetWindowSize(out int width, out int height);
		[DllImport(Glfw64.lib), SuppressUnmanagedCodeSecurity]
		internal static extern void glfwSetWindowSize(int width, int height);
		[DllImport(Glfw64.lib), SuppressUnmanagedCodeSecurity]
		internal static extern void glfwSetWindowPos(int x, int y);
		[DllImport(Glfw64.lib), SuppressUnmanagedCodeSecurity]
		internal static extern void glfwIconifyWindow();
		[DllImport(Glfw64.lib), SuppressUnmanagedCodeSecurity]
		internal static extern void glfwRestoreWindow();
		[DllImport(Glfw64.lib), SuppressUnmanagedCodeSecurity]
		internal static extern void glfwSwapBuffers();
		[DllImport(Glfw64.lib), SuppressUnmanagedCodeSecurity]
		internal static extern void glfwSwapInterval(int interval);
		[DllImport(Glfw64.lib), SuppressUnmanagedCodeSecurity]
		internal static extern int glfwGetWindowParam(int param);
		[DllImport(Glfw64.lib), SuppressUnmanagedCodeSecurity]
		internal static extern void glfwSetWindowSizeCallback(GlfwWindowSizeFun cbfun);
		[DllImport(Glfw64.lib), SuppressUnmanagedCodeSecurity]
		internal static extern void glfwSetWindowCloseCallback(GlfwWindowCloseFun cbfun);
		[DllImport(Glfw64.lib), SuppressUnmanagedCodeSecurity]
		internal static extern void glfwSetWindowRefreshCallback(GlfwWindowRefreshFun cbfun);

		[DllImport(Glfw64.lib), SuppressUnmanagedCodeSecurity]
		internal static extern int glfwGetVideoModes([MarshalAs(UnmanagedType.LPArray)] GlfwVidMode[] list, int maxcount);
		[DllImport(Glfw64.lib), SuppressUnmanagedCodeSecurity]
		internal static extern void glfwGetDesktopMode(out GlfwVidMode mode);

		[DllImport(Glfw64.lib), SuppressUnmanagedCodeSecurity]
		internal static extern void glfwPollEvents();
		[DllImport(Glfw64.lib), SuppressUnmanagedCodeSecurity]
		internal static extern void glfwWaitEvents();
		[DllImport(Glfw64.lib), SuppressUnmanagedCodeSecurity]
		internal static extern int glfwGetKey(int key);
		[DllImport(Glfw64.lib), SuppressUnmanagedCodeSecurity]
		internal static extern int glfwGetMouseButton(int button);
		[DllImport(Glfw64.lib), SuppressUnmanagedCodeSecurity]
		internal static extern void glfwGetMousePos(out int xpos, out int ypos);
		[DllImport(Glfw64.lib), SuppressUnmanagedCodeSecurity]
		internal static extern void glfwSetMousePos(int xpos, int ypos);
		[DllImport(Glfw64.lib), SuppressUnmanagedCodeSecurity]
		internal static extern int glfwGetMouseWheel();
		[DllImport(Glfw64.lib), SuppressUnmanagedCodeSecurity]
		internal static extern void glfwSetMouseWheel(int pos);
		[DllImport(Glfw64.lib), SuppressUnmanagedCodeSecurity]
		internal static extern void glfwSetKeyCallback(GlfwKeyFun cbfun);
		[DllImport(Glfw64.lib), SuppressUnmanagedCodeSecurity]
		internal static extern void glfwSetCharCallback(GlfwCharFun cbfun);
		[DllImport(Glfw64.lib), SuppressUnmanagedCodeSecurity]
		internal static extern void glfwSetMouseButtonCallback(GlfwMouseButtonFun cbfun);
		[DllImport(Glfw64.lib), SuppressUnmanagedCodeSecurity]
		internal static extern void glfwSetMousePosCallback(GlfwMousePosFun cbfun);
		[DllImport(Glfw64.lib), SuppressUnmanagedCodeSecurity]
		internal static extern void glfwSetMouseWheelCallback(GlfwMouseWheelFun cbfun);

		[DllImport(Glfw64.lib), SuppressUnmanagedCodeSecurity]
		internal static extern int glfwGetJoystickParam(int joy, int param);
		[DllImport(Glfw64.lib), SuppressUnmanagedCodeSecurity]
		internal static extern int glfwGetJoystickPos(int joy, [MarshalAs(UnmanagedType.LPArray)] float[] pos, int numaxes);
		[DllImport(Glfw64.lib), SuppressUnmanagedCodeSecurity]
		internal static extern int glfwGetJoystickButtons(int joy, [MarshalAs(UnmanagedType.LPArray)] byte[] buttons, int numbuttons);

		[DllImport(Glfw64.lib), SuppressUnmanagedCodeSecurity]
		internal static extern double glfwGetTime();
		[DllImport(Glfw64.lib), SuppressUnmanagedCodeSecurity]
		internal static extern void glfwSetTime(double time);

		[DllImport(Glfw64.lib), SuppressUnmanagedCodeSecurity]
		internal static extern int glfwExtensionSupported([MarshalAs(UnmanagedType.LPStr)] string extension);
		[DllImport(Glfw64.lib), SuppressUnmanagedCodeSecurity]
		internal static extern IntPtr glfwGetProcAddress([MarshalAs(UnmanagedType.LPStr)] string procname);
		[DllImport(Glfw64.lib), SuppressUnmanagedCodeSecurity]
		internal static extern void glfwGetGLVersion(out int major, out int minor, out int rev);

		[DllImport(Glfw64.lib), SuppressUnmanagedCodeSecurity]
		internal static extern void glfwEnable(int token);
		[DllImport(Glfw64.lib), SuppressUnmanagedCodeSecurity]
		internal static extern void glfwDisable(int token);

		[DllImport(Glfw64.lib), SuppressUnmanagedCodeSecurity]
		internal static extern int glfwReadImage([MarshalAs(UnmanagedType.LPStr)] string name, out GlfwImage img, int flags);
		[DllImport(Glfw64.lib), SuppressUnmanagedCodeSecurity]
		internal static extern int glfwReadMemoryImage(IntPtr data, long size, ref GlfwImage img, int flags);
		[DllImport(Glfw64.lib), SuppressUnmanagedCodeSecurity]
		internal static extern void glfwFreeImage(ref GlfwImage img);
		[DllImport(Glfw64.lib), SuppressUnmanagedCodeSecurity]
		internal static extern int glfwLoadTexture2D([MarshalAs(UnmanagedType.LPStr)] string name, int flags);
		[DllImport(Glfw64.lib), SuppressUnmanagedCodeSecurity]
		internal static extern int glfwLoadMemoryTexture2D(IntPtr data, long size, int flags);
		[DllImport(Glfw64.lib), SuppressUnmanagedCodeSecurity]
		internal static extern int glfwLoadTextureImage2D(ref GlfwImage img, int flags);
	}
}

#endif