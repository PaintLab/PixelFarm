using System;
using System.Runtime.InteropServices;

namespace Pencil.Gaming.Scripting {
	#pragma warning disable 0169
	#pragma warning disable 0414

	[StructLayout(LayoutKind.Explicit)]
	public struct LuaStatePtr {
		private LuaStatePtr(IntPtr iptr) {
			inner_Ptr = iptr;
		}

		[FieldOffsetAttribute(0)]
		private IntPtr
			inner_Ptr;

		public static readonly LuaStatePtr Null = new LuaStatePtr(IntPtr.Zero);
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct LuaDebug {
		public int Event;
		[MarshalAs(UnmanagedType.LPStr)]
		public string
			Name;
		[MarshalAs(UnmanagedType.LPStr)]
		public string
			NameWhat;
		[MarshalAs(UnmanagedType.LPStr)]
		public string
			What;
		[MarshalAs(UnmanagedType.LPStr)]
		public string
			Source;
		public int CurrentLine;
		public int LineDefined;
		public int LastLineDefined;
		public byte NumberOfUpValues;
		public byte NumberOfParams;
		public sbyte IsVarArg;
		public sbyte IsTailCall;
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 60 * sizeof(char))]
		public char[]
			ShortSrc;
		private IntPtr i_ci;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct LuaDebugPtr {
		private LuaDebugPtr(IntPtr ptr) {
			ar = ptr;
		}

		private IntPtr ar;

		public LuaDebug Deref() {
			return (LuaDebug)Marshal.PtrToStructure(ar, typeof(LuaDebug));
		}

		public static readonly LuaDebugPtr Null = new LuaDebugPtr(IntPtr.Zero);
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct LuaBufferPtr {
		internal LuaBufferPtr(IntPtr iptr) {
			inner_Ptr = iptr;
		}

		[FieldOffsetAttribute(0)]
		private IntPtr
			inner_Ptr;

		public static readonly LuaBufferPtr Null = new LuaBufferPtr(IntPtr.Zero);
	}

	#pragma warning restore 0169
	#pragma warning restore 0414
}

