using System;

namespace Pencil.Gaming.Scripting {
	public static unsafe partial class Lua {
		public static int UpValueIndex(int i) {
			return Lua.RegistryIndex - i;
		}

		public static void Call(LuaStatePtr l, int n, int r) {
			Lua.Callk(l, n, r, 0, null);
		}

		public static int PCall(LuaStatePtr l, int n, int r, int f) {
			return Lua.PCallk(l, n, r, f, 0, null);
		}

		public static double ToNumber(LuaStatePtr l, int i) {
			return LuaDelegates.lua_tonumberx(l, i, null);
		}

		public static int ToInteger(LuaStatePtr l, int i) {
			return LuaDelegates.lua_tointegerx(l, i, null);
		}

		public static uint ToUnsigned(LuaStatePtr l, int i) {
			return LuaDelegates.lua_tounsignedx(l, i, null);
		}

		public static void Pop(LuaStatePtr l, int n) {
			Lua.SetTop(l, -n - 1);
		}

		public static void NewTable(LuaStatePtr l) {
			Lua.CreateTable(l, 0, 0);
		}

		public static void Register(LuaStatePtr l, string n, LuaCFunction f) {
			Lua.PushCFunction(l, f);
			Lua.SetGlobal(l, n);
		}

		public static void PushCFunction(LuaStatePtr l, LuaCFunction f) {
			Lua.PushCClosure(l, f, 0);
		}

		public static bool IsFunction(LuaStatePtr l, int n) {
			return (Lua.Type(l, n) == BasicType.Function);
		}

		public static bool IsTable(LuaStatePtr l, int n) {
			return (Lua.Type(l, n) == BasicType.Table);
		}

		public static bool IsLightUserData(LuaStatePtr l, int n) {
			return (Lua.Type(l, n) == BasicType.LightUserData);
		}

		public static bool IsNil(LuaStatePtr l, int n) {
			return (Lua.Type(l, n) == BasicType.Nil);
		}

		public static bool IsBoolean(LuaStatePtr l, int n) {
			return (Lua.Type(l, n) == BasicType.Boolean);
		}

		public static bool IsThread(LuaStatePtr l, int n) {
			return (Lua.Type(l, n) == BasicType.Thread);
		}

		public static bool IsNone(LuaStatePtr l, int n) {
			return (Lua.Type(l, n) == BasicType.None);
		}

		public static bool IsNoneOrNil(LuaStatePtr l, int n) {
			return (Lua.Type(l, n) <= 0);
		}

		public static void PushGlobalTable(LuaStatePtr l) {
			Lua.RawGeti(l, Lua.RegistryIndex, Lua.RIdxGlobals);
		}

		public static string ToString(LuaStatePtr l, int i) {
			return new string(LuaDelegates.lua_tolstring(l, i, null));
		}
	}
}

