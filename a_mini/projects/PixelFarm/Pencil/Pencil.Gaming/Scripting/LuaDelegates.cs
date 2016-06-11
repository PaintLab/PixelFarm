using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Security;

namespace Pencil.Gaming.Scripting {
	internal static unsafe class LuaDelegates {
		static LuaDelegates() {
#if DEBUG
			Stopwatch sw = new Stopwatch();
			sw.Start();
#endif
			Type luaInterop = (IntPtr.Size == 8) ? typeof(Lua64) : typeof(Lua32);
#if DEBUG
			Console.WriteLine("Lua interop: {0}", luaInterop.Name);
#endif

			FieldInfo[] fields = typeof(LuaDelegates).GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
			foreach (FieldInfo fi in fields) {
				try {
					MethodInfo mi = luaInterop.GetMethod(fi.Name, BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
					Delegate function = Delegate.CreateDelegate(fi.FieldType, mi);
					fi.SetValue(null, function);
				} catch {
					Console.WriteLine(fi.Name);
				}
			}
#if DEBUG
			sw.Stop();
			Console.WriteLine("Copying Lua delegates took {0} milliseconds.", sw.ElapsedMilliseconds);
#endif
		}

		[SuppressUnmanagedCodeSecurity]
		internal delegate LuaStatePtr newstate(LuaAlloc f,IntPtr ud);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void close(LuaStatePtr l);
		[SuppressUnmanagedCodeSecurity]
		internal delegate LuaStatePtr newthread(LuaStatePtr l);
		[SuppressUnmanagedCodeSecurity]
		internal delegate LuaCFunction atpanic(LuaStatePtr l,LuaCFunction panicf);
		[return: MarshalAs(UnmanagedType.LPArray)]
		[SuppressUnmanagedCodeSecurity]
		internal delegate double[] version(LuaStatePtr l);
		[SuppressUnmanagedCodeSecurity]
		internal delegate int absindex(LuaStatePtr l,int idx);
		[SuppressUnmanagedCodeSecurity]
		internal delegate int gettop(LuaStatePtr l);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void settop(LuaStatePtr l,int idx);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void pushvalue(LuaStatePtr l,int idx);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void @remove(LuaStatePtr l,int idx);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void insert(LuaStatePtr l,int idx);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void replace(LuaStatePtr l,int idx);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void copy(LuaStatePtr l,int fromidx,int toidx);
		[SuppressUnmanagedCodeSecurity]
		internal delegate int checkstack(LuaStatePtr l,int sz);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void xmove(LuaStatePtr from,LuaStatePtr to,int n);
		[SuppressUnmanagedCodeSecurity]
		internal delegate int isnumber(LuaStatePtr l,int idx);
		[SuppressUnmanagedCodeSecurity]
		internal delegate int isstring(LuaStatePtr l,int idx);
		[SuppressUnmanagedCodeSecurity]
		internal delegate int iscfunction(LuaStatePtr l,int idx);
		[SuppressUnmanagedCodeSecurity]
		internal delegate int isuserdata(LuaStatePtr l,int idx);
		[SuppressUnmanagedCodeSecurity]
		internal delegate int type(LuaStatePtr l,int idx);
		[SuppressUnmanagedCodeSecurity]
		internal delegate string typename(LuaStatePtr l,int tp);
		[SuppressUnmanagedCodeSecurity]
		internal delegate double tonumberx(LuaStatePtr l,int idx,int * isnum);
		[SuppressUnmanagedCodeSecurity]
		internal delegate int tointegerx(LuaStatePtr l,int idx,int * isnum);
		[SuppressUnmanagedCodeSecurity]
		internal delegate uint tounsignedx(LuaStatePtr l,int idx,int * isnum);
		[SuppressUnmanagedCodeSecurity]
		internal delegate int toboolean(LuaStatePtr l,int idx);
		[SuppressUnmanagedCodeSecurity]
		internal delegate sbyte *tolstring(LuaStatePtr l,int idx,int * len);
		[SuppressUnmanagedCodeSecurity]
		internal delegate int rawlen(LuaStatePtr l,int idx);
		[SuppressUnmanagedCodeSecurity]
		internal delegate LuaCFunction tocfunction(LuaStatePtr l,int idx);
		[SuppressUnmanagedCodeSecurity]
		internal delegate IntPtr touserdata(LuaStatePtr l,int idx);
		[SuppressUnmanagedCodeSecurity]
		internal delegate LuaStatePtr tothread(LuaStatePtr l,int idx);
		[SuppressUnmanagedCodeSecurity]
		internal delegate IntPtr topointer(LuaStatePtr l,int idx);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void arith(LuaStatePtr l,int op);
		[SuppressUnmanagedCodeSecurity]
		internal delegate int rawequal(LuaStatePtr l,int idx1,int idx2);
		[SuppressUnmanagedCodeSecurity]
		internal delegate int compare(LuaStatePtr l,int idx1,int idx2,int op);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void pushnil(LuaStatePtr l);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void pushnumber(LuaStatePtr l,double n);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void pushinteger(LuaStatePtr l,int n);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void pushunsigned(LuaStatePtr l,uint n);
		[SuppressUnmanagedCodeSecurity]
		internal delegate sbyte * pushlstring(LuaStatePtr l,[MarshalAs(UnmanagedType.LPStr)] string s,int len);
		[SuppressUnmanagedCodeSecurity]
		internal delegate sbyte * pushstring(LuaStatePtr l,[MarshalAs(UnmanagedType.LPStr)] string s);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void pushcclosure(LuaStatePtr l,LuaCFunction fn,int n);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void pushboolean(LuaStatePtr l,int b);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void pushlightuserdata(LuaStatePtr l,IntPtr p);
		[SuppressUnmanagedCodeSecurity]
		internal delegate int pushthread(LuaStatePtr l);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void getglobal(LuaStatePtr l,[MarshalAs(UnmanagedType.LPStr)] string var);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void gettable(LuaStatePtr l,int idx);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void getfield(LuaStatePtr l,int idx,[MarshalAs(UnmanagedType.LPStr)] string k);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void rawget(LuaStatePtr l,int idx);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void rawgeti(LuaStatePtr l,int idx,int n);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void rawgetp(LuaStatePtr l,int idx,IntPtr p);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void createtable(LuaStatePtr l,int narr,int nrec);
		[SuppressUnmanagedCodeSecurity]
		internal delegate IntPtr newuserdata(LuaStatePtr l,int sz);
		[SuppressUnmanagedCodeSecurity]
		internal delegate int getmetatable(LuaStatePtr l,int objindex);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void getuservalue(LuaStatePtr l,int idx);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void setglobal(LuaStatePtr l,[MarshalAs(UnmanagedType.LPStr)] string var);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void settable(LuaStatePtr l,int idx);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void setfield(LuaStatePtr l,int idx,[MarshalAs(UnmanagedType.LPStr)] string k);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void rawset(LuaStatePtr l,int idx);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void rawseti(LuaStatePtr l,int idx,int n);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void rawsetp(LuaStatePtr l,int idx,IntPtr p);
		[SuppressUnmanagedCodeSecurity]
		internal delegate int setmetatable(LuaStatePtr l,int objindex);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void setuservalue(LuaStatePtr l,int idx);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void callk(LuaStatePtr l,int nargs,int nresults,int ctx,LuaCFunction k);
		[SuppressUnmanagedCodeSecurity]
		internal delegate int getctx(LuaStatePtr l,int * ctx);
		[SuppressUnmanagedCodeSecurity]
		internal delegate int pcallk(LuaStatePtr l,int nargs,int nresults,int errfunc,int ctx,LuaCFunction k);
		[SuppressUnmanagedCodeSecurity]
		internal delegate int load(LuaStatePtr l,LuaReader reader,IntPtr dt,[MarshalAs(UnmanagedType.LPStr)] string chunkname,[MarshalAs(UnmanagedType.LPStr)] string mode);
		[SuppressUnmanagedCodeSecurity]
		internal delegate int dump(LuaStatePtr l,LuaWriter writer,IntPtr data);
		[SuppressUnmanagedCodeSecurity]
		internal delegate int yieldk(LuaStatePtr l,int nresults,int ctx,LuaCFunction k);
		[SuppressUnmanagedCodeSecurity]
		internal delegate int resume(LuaStatePtr l,LuaStatePtr from,int narg);
		[SuppressUnmanagedCodeSecurity]
		internal delegate int status(LuaStatePtr l);
		[SuppressUnmanagedCodeSecurity]
		internal delegate int gc(LuaStatePtr l,int what,int data);
		[SuppressUnmanagedCodeSecurity]
		internal delegate int error(LuaStatePtr l);
		[SuppressUnmanagedCodeSecurity]
		internal delegate int next(LuaStatePtr l,int idx);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void concat(LuaStatePtr l,int n);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void len(LuaStatePtr l,int idx);
		[SuppressUnmanagedCodeSecurity]
		internal delegate LuaAlloc getallocf(LuaStatePtr l,out IntPtr ud);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void setallocf(LuaStatePtr l,LuaAlloc f,IntPtr ud);
		[SuppressUnmanagedCodeSecurity]
		internal delegate int getstack(LuaStatePtr l,int level,LuaDebugPtr ar);
		[SuppressUnmanagedCodeSecurity]
		internal delegate int getinfo(LuaStatePtr l,[MarshalAs(UnmanagedType.LPStr)] string what,LuaDebugPtr ar);
		[SuppressUnmanagedCodeSecurity]
		internal delegate sbyte * getlocal(LuaStatePtr l,LuaDebugPtr ar,int n);
		[SuppressUnmanagedCodeSecurity]
		internal delegate sbyte * setlocal(LuaStatePtr l,LuaDebugPtr ar,int n);
		[SuppressUnmanagedCodeSecurity]
		internal delegate sbyte * getupvalue(LuaStatePtr l,int funcindex,int n);
		[SuppressUnmanagedCodeSecurity]
		internal delegate sbyte * setupvalue(LuaStatePtr l,int funcindex,int n);
		[SuppressUnmanagedCodeSecurity]
		internal delegate IntPtr upvalueid(LuaStatePtr l,int fidx,int n);
		[SuppressUnmanagedCodeSecurity]
		internal delegate void upvaluejoin(LuaStatePtr l,int fidx1,int n1,int fidx2,int n2);
		[SuppressUnmanagedCodeSecurity]
		internal delegate int sethook(LuaStatePtr l,LuaHook func,int mask,int count);
		[SuppressUnmanagedCodeSecurity]
		internal delegate LuaHook gethook(LuaStatePtr l);
		[SuppressUnmanagedCodeSecurity]
		internal delegate int gethookmask(LuaStatePtr l);
		[SuppressUnmanagedCodeSecurity]
		internal delegate int gethookcount(LuaStatePtr l);

#pragma warning disable 0649

		internal static newstate lua_newstate;
		internal static close lua_close;
		internal static newthread lua_newthread;
		internal static atpanic lua_atpanic;
		internal static version lua_version;
		internal static absindex lua_absindex;
		internal static gettop lua_gettop;
		internal static settop lua_settop;
		internal static pushvalue lua_pushvalue;
		internal static @remove lua_remove;
		internal static insert lua_insert;
		internal static replace lua_replace;
		internal static copy lua_copy;
		internal static checkstack lua_checkstack;
		internal static xmove lua_xmove;
		internal static isnumber lua_isnumber;
		internal static isstring lua_isstring;
		internal static iscfunction lua_iscfunction;
		internal static isuserdata lua_isuserdata;
		internal static type lua_type;
		internal static typename lua_typename;
		internal static tonumberx lua_tonumberx;
		internal static tointegerx lua_tointegerx;
		internal static tounsignedx lua_tounsignedx;
		internal static toboolean lua_toboolean;
		internal static tolstring lua_tolstring;
		internal static rawlen lua_rawlen;
		internal static tocfunction lua_tocfunction;
		internal static touserdata lua_touserdata;
		internal static tothread lua_tothread;
		internal static topointer lua_topointer;
		internal static arith lua_arith;
		internal static rawequal lua_rawequal;
		internal static compare lua_compare;
		internal static pushnil lua_pushnil;
		internal static pushnumber lua_pushnumber;
		internal static pushinteger lua_pushinteger;
		internal static pushunsigned lua_pushunsigned;
		internal static pushlstring lua_pushlstring;
		internal static pushstring lua_pushstring;
		internal static pushcclosure lua_pushcclosure;
		internal static pushboolean lua_pushboolean;
		internal static pushlightuserdata lua_pushlightuserdata;
		internal static pushthread lua_pushthread;
		internal static getglobal lua_getglobal;
		internal static gettable lua_gettable;
		internal static getfield lua_getfield;
		internal static rawget lua_rawget;
		internal static rawgeti lua_rawgeti;
		internal static rawgetp lua_rawgetp;
		internal static createtable lua_createtable;
		internal static newuserdata lua_newuserdata;
		internal static getmetatable lua_getmetatable;
		internal static getuservalue lua_getuservalue;
		internal static setglobal lua_setglobal;
		internal static settable lua_settable;
		internal static setfield lua_setfield;
		internal static rawset lua_rawset;
		internal static rawseti lua_rawseti;
		internal static rawsetp lua_rawsetp;
		internal static setmetatable lua_setmetatable;
		internal static setuservalue lua_setuservalue;
		internal static callk lua_callk;
		internal static getctx lua_getctx;
		internal static pcallk lua_pcallk;
		internal static load lua_load;
		internal static dump lua_dump;
		internal static yieldk lua_yieldk;
		internal static resume lua_resume;
		internal static status lua_status;
		internal static gc lua_gc;
		internal static error lua_error;
		internal static next lua_next;
		internal static concat lua_concat;
		internal static len lua_len;
		internal static getallocf lua_getallocf;
		internal static setallocf lua_setallocf;
		internal static getstack lua_getstack;
		internal static getinfo lua_getinfo;
		internal static getlocal lua_getlocal;
		internal static setlocal lua_setlocal;
		internal static getupvalue lua_getupvalue;
		internal static setupvalue lua_setupvalue;
		internal static upvalueid lua_upvalueid;
		internal static upvaluejoin lua_upvaluejoin;
		internal static sethook lua_sethook;
		internal static gethook lua_gethook;
		internal static gethookmask lua_gethookmask;
		internal static gethookcount lua_gethookcount;

#pragma warning restore 0649
	}
}
