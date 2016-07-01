using System;
using System.Runtime.InteropServices;

namespace Pencil.Gaming.Scripting {
	public delegate int LuaCFunction(LuaStatePtr l);
	[return: MarshalAs(UnmanagedType.LPStr)]
	public delegate string LuaReader(LuaStatePtr l,IntPtr ud,out int sz);
	public delegate int LuaWriter(LuaStatePtr l,IntPtr p,int sz,IntPtr ud);
	public delegate IntPtr LuaAlloc(IntPtr ud,IntPtr ptr,int osize,int nsize);
	public delegate void LuaHook(LuaStatePtr l,LuaDebugPtr ar);
}

