using System;
using System.Runtime.InteropServices;

namespace Pencil.Gaming.Scripting {
	public static unsafe partial class LuaL {
		public static int LoadFile(LuaStatePtr l, string f) {
			return LuaL.LoadFilex(l, f, null);
		}

		public static void NewLibTable(LuaStatePtr l, string l_) {
			Lua.CreateTable(l, 0, l_.Length);
		}

		public static void NewLib(LuaStatePtr l, string l_) {
			LuaL.NewLibTable(l, l_);
			fixed (char * chptr = &l_.ToCharArray()[0]) {
				LuaL.SetFuncs(l, new IntPtr(chptr), 0);
			}
		}

//		public static bool ArgCheck(LuaStatePtr l, bool cond, int numarg, string extramsg) {
//			// WHAT DO I PUT HERE?
//		}

		public static string CheckString(LuaStatePtr l, int n) {
			return new string(LuaLDelegates.luaL_checklstring(l, n, null));
		}

		public static string OptString(LuaStatePtr l, int n, string d) {
			return new string(LuaLDelegates.luaL_optlstring(l, n, d, null));
		}

		// Skipped unneeded methods luaL_checkint etc.

		public static string TypeName(LuaStatePtr l, int i) {
			return Lua.TypeName(l, Lua.Type(l, i));
		}

		public static void DoFile(LuaStatePtr l, string fn) {
			LuaL.LoadFile(l, fn);
			Lua.PCall(l, 0, Lua.MultRet, 0);
		}

		public static void DoString(LuaStatePtr l, string s) {
			LuaL.LoadString(l, s);
			Lua.PCall(l, 0, Lua.MultRet, 0);
		}

		public static void GetMetaTable(LuaStatePtr l, string n) {
			Lua.GetField(l, Lua.RegistryIndex, n);
		}

		public static int Opt(LuaStatePtr l, Delegate f, int n, int d) {
			return Lua.IsNoneOrNil(l, n) ? d : (int)f.DynamicInvoke(l, n);
		}

		public static LuaBufferPtr LoadBuffer(LuaStatePtr l, string s, int sz, string n) {
			return LuaL.LoadBufferx(l, s, sz, n, null);
		}
	}
}

