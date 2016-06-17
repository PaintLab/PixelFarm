using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Pencil.Gaming.Scripting {
	internal static unsafe class LuaL64 {
		[DllImport("natives64/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal extern static void luaL_checkversion_(LuaStatePtr l, double ver);
		[DllImport("natives64/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal extern static int luaL_getmetafield(LuaStatePtr l, int obj, [MarshalAs(UnmanagedType.LPStr)] string e);
		[DllImport("natives64/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal extern static int luaL_callmeta(LuaStatePtr l, int obj, [MarshalAs(UnmanagedType.LPStr)] string e);
		[DllImport("natives64/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal extern static sbyte * luaL_tolstring(LuaStatePtr l, int idx, int *len);
		[DllImport("natives64/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal extern static int luaL_argerror(LuaStatePtr l, int numarg, [MarshalAs(UnmanagedType.LPStr)] string extramsg);
		[DllImport("natives64/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal extern static sbyte * luaL_checklstring(LuaStatePtr l, int numArg, int * l_);
		[DllImport("natives64/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal extern static sbyte * luaL_optlstring(LuaStatePtr l, int numArg, [MarshalAs(UnmanagedType.LPStr)] string def, int * l_);
		[DllImport("natives64/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal extern static double luaL_checknumber(LuaStatePtr l, int numArg);
		[DllImport("natives64/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal extern static double luaL_optnumber(LuaStatePtr l, int nArg, double def);
		[DllImport("natives64/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal extern static int luaL_checkinteger(LuaStatePtr l, int numArg);
		[DllImport("natives64/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal extern static int luaL_optinteger(LuaStatePtr l, int nArg, int def);
		[DllImport("natives64/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal extern static uint luaL_checkunsigned(LuaStatePtr l, int numArg);
		[DllImport("natives64/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal extern static uint luaL_optunsigned(LuaStatePtr l, int numArg, uint def);
		[DllImport("natives64/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal extern static void luaL_checkstack(LuaStatePtr l, int sz, [MarshalAs(UnmanagedType.LPStr)] string msg);
		[DllImport("natives64/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal extern static void luaL_checktype(LuaStatePtr l, int narg, int t);
		[DllImport("natives64/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal extern static void luaL_checkany(LuaStatePtr l, int narg);
		[DllImport("natives64/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal extern static int luaL_newmetatable(LuaStatePtr l, [MarshalAs(UnmanagedType.LPStr)] string tname);
		[DllImport("natives64/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal extern static void luaL_setmetatable(LuaStatePtr l, [MarshalAs(UnmanagedType.LPStr)] string tname);
		[DllImport("natives64/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal extern static IntPtr luaL_testudata(LuaStatePtr l, int ud, [MarshalAs(UnmanagedType.LPStr)] string tname);
		[DllImport("natives64/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal extern static IntPtr luaL_checkudata(LuaStatePtr l, int ud, [MarshalAs(UnmanagedType.LPStr)] string tname);
		[DllImport("natives64/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal extern static void luaL_where(LuaStatePtr l, int lvl);
		[DllImport("natives64/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal extern static int luaL_checkoption(LuaStatePtr l, int narg, [MarshalAs(UnmanagedType.LPStr)] string def, [MarshalAs(UnmanagedType.LPArray)] sbyte *[] lst);
		[DllImport("natives64/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal extern static int luaL_fileresult(LuaStatePtr l, int stat, [MarshalAs(UnmanagedType.LPStr)] string fname);
		[DllImport("natives64/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal extern static int luaL_execresult(LuaStatePtr l, int stat);
		[DllImport("natives64/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal extern static int luaL_ref(LuaStatePtr l, int t);
		[DllImport("natives64/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal extern static void luaL_unref(LuaStatePtr l, int t, int @ref);
		[DllImport("natives64/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal extern static int luaL_loadfilex(LuaStatePtr l, [MarshalAs(UnmanagedType.LPStr)] string filename, [MarshalAs(UnmanagedType.LPStr)] string mode);
		[DllImport("natives64/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal extern static int luaL_loadbufferx(LuaStatePtr l, [MarshalAs(UnmanagedType.LPStr)] string buff, int sz, [MarshalAs(UnmanagedType.LPStr)] string name, [MarshalAs(UnmanagedType.LPStr)] string mode);
		[DllImport("natives64/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal extern static int luaL_loadstring(LuaStatePtr l, [MarshalAs(UnmanagedType.LPStr)] string s);
		[DllImport("natives64/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal extern static LuaStatePtr luaL_newstate();
		[DllImport("natives64/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal extern static int luaL_len(LuaStatePtr l, int idx);
		[DllImport("natives64/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal extern static sbyte * luaL_gsub(LuaStatePtr l, [MarshalAs(UnmanagedType.LPStr)] string s, [MarshalAs(UnmanagedType.LPStr)] string p, [MarshalAs(UnmanagedType.LPStr)] string r);
		[DllImport("natives64/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal extern static void luaL_setfuncs(LuaStatePtr l, IntPtr l_, int nup);
		[DllImport("natives64/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal extern static int luaL_getsubtable(LuaStatePtr l, int idx, [MarshalAs(UnmanagedType.LPStr)] string fname);
		[DllImport("natives64/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal extern static void luaL_traceback(LuaStatePtr l, LuaStatePtr l1, [MarshalAs(UnmanagedType.LPStr)] string msg, int level);
		[DllImport("natives64/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal extern static void luaL_requiref(LuaStatePtr l, [MarshalAs(UnmanagedType.LPStr)] string modname, LuaCFunction openf, int glb);
		[DllImport("natives64/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal extern static void luaL_buffinit(LuaStatePtr l, LuaBufferPtr B);
		[DllImport("natives64/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal extern static sbyte *luaL_prepbuffsize(LuaBufferPtr B, int sz);
		[DllImport("natives64/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal extern static void luaL_addlstring(LuaBufferPtr B, [MarshalAs(UnmanagedType.LPStr)] string s, int l);
		[DllImport("natives64/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal extern static void luaL_addstring(LuaBufferPtr B, [MarshalAs(UnmanagedType.LPStr)] string s);
		[DllImport("natives64/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal extern static void luaL_addvalue(LuaBufferPtr B);
		[DllImport("natives64/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal extern static void luaL_pushresult(LuaBufferPtr B);
		[DllImport("natives64/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal extern static void luaL_pushresultsize(LuaBufferPtr B, int sz);
		[DllImport("natives64/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal extern static sbyte *luaL_buffinitsize(LuaStatePtr l, LuaBufferPtr B, int sz);
		[DllImport("natives64/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal extern static void luaL_openlibs(LuaStatePtr l);
	}
}

