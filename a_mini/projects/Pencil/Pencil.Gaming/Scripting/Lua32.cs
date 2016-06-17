using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Pencil.Gaming.Scripting {
	internal static unsafe class Lua32 {
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern LuaStatePtr lua_newstate(LuaAlloc f, IntPtr ud);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void lua_close(LuaStatePtr l);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern LuaStatePtr lua_newthread(LuaStatePtr l);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern LuaCFunction lua_atpanic(LuaStatePtr l, LuaCFunction panicf);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		[return: MarshalAs(UnmanagedType.LPArray)]
		internal static extern double[] lua_version(LuaStatePtr l);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern int lua_absindex(LuaStatePtr l, int idx);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern int lua_gettop(LuaStatePtr l);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void lua_settop(LuaStatePtr l, int idx);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void lua_pushvalue(LuaStatePtr l, int idx);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void lua_remove(LuaStatePtr l, int idx);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void lua_insert(LuaStatePtr l, int idx);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void lua_replace(LuaStatePtr l, int idx);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void lua_copy(LuaStatePtr l, int fromidx, int toidx);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern int lua_checkstack(LuaStatePtr l, int sz);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void lua_xmove(LuaStatePtr from, LuaStatePtr to, int n);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern int lua_isnumber(LuaStatePtr l, int idx);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern int lua_isstring(LuaStatePtr l, int idx);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern int lua_iscfunction(LuaStatePtr l, int idx);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern int lua_isuserdata(LuaStatePtr l, int idx);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern int lua_type(LuaStatePtr l, int idx);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern string lua_typename(LuaStatePtr l, int tp);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern double lua_tonumberx(LuaStatePtr l, int idx, int * isnum);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern int lua_tointegerx(LuaStatePtr l, int idx, int * isnum);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern uint lua_tounsignedx(LuaStatePtr l, int idx, int * isnum);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern int lua_toboolean(LuaStatePtr l, int idx);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern sbyte * lua_tolstring(LuaStatePtr l, int idx, int * len);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern int lua_rawlen(LuaStatePtr l, int idx);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern LuaCFunction lua_tocfunction(LuaStatePtr l, int idx);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern IntPtr lua_touserdata(LuaStatePtr l, int idx);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern LuaStatePtr lua_tothread(LuaStatePtr l, int idx);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern IntPtr lua_topointer(LuaStatePtr l, int idx);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void lua_arith(LuaStatePtr l, int op);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern int lua_rawequal(LuaStatePtr l, int idx1, int idx2);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern int lua_compare(LuaStatePtr l, int idx1, int idx2, int op);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void lua_pushnil(LuaStatePtr l);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void lua_pushnumber(LuaStatePtr l, double n);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void lua_pushinteger(LuaStatePtr l, int n);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void lua_pushunsigned(LuaStatePtr l, uint n);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern sbyte * lua_pushlstring(LuaStatePtr l, [MarshalAs(UnmanagedType.LPStr)] string s, int len);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern sbyte * lua_pushstring(LuaStatePtr l, [MarshalAs(UnmanagedType.LPStr)] string s);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void lua_pushcclosure(LuaStatePtr l, LuaCFunction fn, int n);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void lua_pushboolean(LuaStatePtr l, int b);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void lua_pushlightuserdata(LuaStatePtr l, IntPtr p);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern int lua_pushthread(LuaStatePtr l);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void lua_getglobal(LuaStatePtr l, [MarshalAs(UnmanagedType.LPStr)] string var);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void lua_gettable(LuaStatePtr l, int idx);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void lua_getfield(LuaStatePtr l, int idx, [MarshalAs(UnmanagedType.LPStr)] string k);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void lua_rawget(LuaStatePtr l, int idx);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void lua_rawgeti(LuaStatePtr l, int idx, int n);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void lua_rawgetp(LuaStatePtr l, int idx, IntPtr p);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void lua_createtable(LuaStatePtr l, int narr, int nrec);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern IntPtr lua_newuserdata(LuaStatePtr l, int sz);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern int lua_getmetatable(LuaStatePtr l, int objindex);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void lua_getuservalue(LuaStatePtr l, int idx);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void lua_setglobal(LuaStatePtr l, [MarshalAs(UnmanagedType.LPStr)] string var);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void lua_settable(LuaStatePtr l, int idx);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void lua_setfield(LuaStatePtr l, int idx, [MarshalAs(UnmanagedType.LPStr)] string k);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void lua_rawset(LuaStatePtr l, int idx);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void lua_rawseti(LuaStatePtr l, int idx, int n);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void lua_rawsetp(LuaStatePtr l, int idx, IntPtr p);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern int lua_setmetatable(LuaStatePtr l, int objindex);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void lua_setuservalue(LuaStatePtr l, int idx);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void lua_callk(LuaStatePtr l, int nargs, int nresults, int ctx, LuaCFunction k);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern int lua_getctx(LuaStatePtr l, int * ctx);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern int lua_pcallk(LuaStatePtr l, int nargs, int nresults, int errfunc, int ctx, LuaCFunction k);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern int lua_load(LuaStatePtr l, LuaReader reader, IntPtr dt, [MarshalAs(UnmanagedType.LPStr)] string chunkname, [MarshalAs(UnmanagedType.LPStr)] string mode);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern int lua_dump(LuaStatePtr l, LuaWriter writer, IntPtr data);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern int lua_yieldk(LuaStatePtr l, int nresults, int ctx, LuaCFunction k);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern int lua_resume(LuaStatePtr l, LuaStatePtr from, int narg);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern int lua_status(LuaStatePtr l);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern int lua_gc(LuaStatePtr l, int what, int data);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern int lua_error(LuaStatePtr l);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern int lua_next(LuaStatePtr l, int idx);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void lua_concat(LuaStatePtr l, int n);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void lua_len(LuaStatePtr l, int idx);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern LuaAlloc lua_getallocf(LuaStatePtr l, out IntPtr ud);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void lua_setallocf(LuaStatePtr l, LuaAlloc f, IntPtr ud);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern int lua_getstack(LuaStatePtr l, int level, LuaDebugPtr ar);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern int lua_getinfo(LuaStatePtr l, [MarshalAs(UnmanagedType.LPStr)] string what, LuaDebugPtr ar);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern sbyte * lua_getlocal(LuaStatePtr l, LuaDebugPtr ar, int n);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern sbyte * lua_setlocal(LuaStatePtr l, LuaDebugPtr ar, int n);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern sbyte * lua_getupvalue(LuaStatePtr l, int funcindex, int n);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern sbyte * lua_setupvalue(LuaStatePtr l, int funcindex, int n);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern IntPtr lua_upvalueid(LuaStatePtr l, int fidx, int n);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern void lua_upvaluejoin(LuaStatePtr l, int fidx1, int n1, int fidx2, int n2);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern int lua_sethook(LuaStatePtr l, LuaHook func, int mask, int count);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern LuaHook lua_gethook(LuaStatePtr l);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern int lua_gethookmask(LuaStatePtr l);
		[DllImport("natives32/lua.dll"), SuppressUnmanagedCodeSecurity]
		internal static extern int lua_gethookcount(LuaStatePtr l);
	}
}