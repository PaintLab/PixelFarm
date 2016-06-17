using System;
using System.Runtime.InteropServices;

namespace Pencil.Gaming.Scripting {
	public static unsafe partial class Lua {
		public const string VersionMajor = "5";
		public const string VersionMinor = "2";
		public const int VersionNum = 502;
		public const string VersionRelease = "1";
		public const string VersionString = "Lua 5.2";
		public const string Release = "Lua 5.2  Copyright (c) 1994-2012 Lua.org, PUC-Rio";
		public const string Authors = "R. Ierusalimschy, L. H. de Figueiredo, W. Celes";
		public const int MultRet = -1;
		private static int MaxStack {
			get {
				if (IntPtr.Size == 8) {
					return 1000000;
				}
				return 15000;
			}
		}
		public static int RegistryIndex {
			get {
				return -MaxStack - 1000;
			}
		}
		public const int MinStack = 20;
		public const int RIdxMainThread = 1;
		public const int RIdxGlobals = 2;


		public static LuaStatePtr NewState(LuaAlloc f, IntPtr ud) {
			System.GC.KeepAlive(f);
			return LuaDelegates.lua_newstate(f, ud);
		}
		public static void Close(LuaStatePtr l) {
			LuaDelegates.lua_close(l);
		}
		public static LuaStatePtr NewThread(LuaStatePtr l) {
			return LuaDelegates.lua_newthread(l);
		}
		public static LuaCFunction AtPanic(LuaStatePtr l, LuaCFunction panicf) {
			System.GC.KeepAlive(panicf);
			return LuaDelegates.lua_atpanic(l, panicf);
		}
		public static double[] Version(LuaStatePtr l) {
			return LuaDelegates.lua_version(l);
		}
		public static int AbsIndex(LuaStatePtr l, int idx) {
			return LuaDelegates.lua_absindex(l, idx);
		}
		public static int GetTop(LuaStatePtr l) {
			return LuaDelegates.lua_gettop(l);
		}
		public static void SetTop(LuaStatePtr l, int idx) {
			LuaDelegates.lua_settop(l, idx);
		}
		public static void PushValue(LuaStatePtr l, int idx) {
			LuaDelegates.lua_pushvalue(l, idx);
		}
		public static void Remove(LuaStatePtr l, int idx) {
			LuaDelegates.lua_remove(l, idx);
		}
		public static void Insert(LuaStatePtr l, int idx) {
			LuaDelegates.lua_insert(l, idx);
		}
		public static void Replace(LuaStatePtr l, int idx) {
			LuaDelegates.lua_replace(l, idx);
		}
		public static void Copy(LuaStatePtr l, int fromidx, int toidx) {
			LuaDelegates.lua_copy(l, fromidx, toidx);
		}
		public static int CheckStack(LuaStatePtr l, int sz) {
			return LuaDelegates.lua_checkstack(l, sz);
		}
		public static void XMove(LuaStatePtr @from, LuaStatePtr to, int n) {
			LuaDelegates.lua_xmove(@from, to, n);
		}
		public static bool IsNumber(LuaStatePtr l, int idx) {
			return LuaDelegates.lua_isnumber(l, idx) == 1;
		}
		public static bool IsString(LuaStatePtr l, int idx) {
			return LuaDelegates.lua_isstring(l, idx) == 1;
		}
		public static bool IsCFunction(LuaStatePtr l, int idx) {
			return LuaDelegates.lua_iscfunction(l, idx) == 1;
		}
		public static bool IsUserData(LuaStatePtr l, int idx) {
			return LuaDelegates.lua_isuserdata(l, idx) == 1;
		}
		public static BasicType Type(LuaStatePtr l, int idx) {
			return (BasicType)LuaDelegates.lua_type(l, idx);
		}
		public static string TypeName(LuaStatePtr l, BasicType tp) {
			return LuaDelegates.lua_typename(l, (int)tp);
		}
		public static double ToNumberx(LuaStatePtr l, int idx, out bool isnum) {
			int * i = stackalloc int[1];
			double result = LuaDelegates.lua_tonumberx(l, idx, i);
			isnum = (*i == 1);
			return result;
		}
		public static int ToIntegerx(LuaStatePtr l, int idx, out bool isnum) {
			int * i = stackalloc int[1];
			int result = LuaDelegates.lua_tointegerx(l, idx, i);
			isnum = (*i == 1);
			return result;
		}
		public static uint ToUnsignedx(LuaStatePtr l, int idx, out bool isnum) {
			int * i = stackalloc int[1];
			uint result = LuaDelegates.lua_tounsignedx(l, idx, i);
			isnum = (*i == 1);
			return result;
		}
		public static bool ToBoolean(LuaStatePtr l, int idx) {
			return LuaDelegates.lua_toboolean(l, idx) == 1;
		}
		public static string ToLString(LuaStatePtr l, int idx, out int len) {
			fixed(int * i = &len) {
				sbyte * ptr = LuaDelegates.lua_tolstring(l, idx, i);
				char[] arr = new char[*i];
				for (int j = 0; j < *i; ++j) {
					arr[j] = (char)ptr[j];
				}
				return new string(arr);
			}
		}
		public static int RawLen(LuaStatePtr l, int idx) {
			return LuaDelegates.lua_rawlen(l, idx);
		}
		public static LuaCFunction ToCFunction(LuaStatePtr l, int idx) {
			return LuaDelegates.lua_tocfunction(l, idx);
		}
		public static T ToUserData<T>(LuaStatePtr l, int idx) {
			return (T)Marshal.PtrToStructure(LuaDelegates.lua_touserdata(l, idx), typeof(T));
		}
		public static LuaStatePtr ToThread(LuaStatePtr l, int idx) {
			return LuaDelegates.lua_tothread(l, idx);
		}
		public static IntPtr ToPointer(LuaStatePtr l, int idx) {
			return LuaDelegates.lua_topointer(l, idx);
		}
		public static void Arith(LuaStatePtr l, ArithmeticOp op) {
			LuaDelegates.lua_arith(l, (int)op);
		}
		public static int RawEqual(LuaStatePtr l, int idx1, int idx2) {
			return LuaDelegates.lua_rawequal(l, idx1, idx2);
		}
		public static int Compare(LuaStatePtr l, int idx1, int idx2, CompareOp op) {
			return LuaDelegates.lua_compare(l, idx1, idx2, (int)op);
		}
		public static void PushNil(LuaStatePtr l) {
			LuaDelegates.lua_pushnil(l);
		}
		public static void PushNumber(LuaStatePtr l, double n) {
			LuaDelegates.lua_pushnumber(l, n);
		}
		public static void PushInteger(LuaStatePtr l, int n) {
			LuaDelegates.lua_pushinteger(l, n);
		}
		public static void PushUnsigned(LuaStatePtr l, uint n) {
			LuaDelegates.lua_pushunsigned(l, n);
		}
		public static string PushLString(LuaStatePtr l, string s) {
			return new string(LuaDelegates.lua_pushlstring(l, s, s.Length));
		}
		public static void PushCClosure(LuaStatePtr l, LuaCFunction fn, int n) {
			System.GC.KeepAlive(fn);
			LuaDelegates.lua_pushcclosure(l, fn, n);
		}
		public static void PushBoolean(LuaStatePtr l, bool b) {
			LuaDelegates.lua_pushboolean(l, b ? 1 : 0);
		}
		public static void PushLightUserData<T>(LuaStatePtr l, T p) {
			IntPtr ptr = new IntPtr();
			Marshal.StructureToPtr(p, ptr, false);
			LuaDelegates.lua_pushlightuserdata(l, ptr);
		}
		public static int PushThread(LuaStatePtr l) {
			return LuaDelegates.lua_pushthread(l);
		}
		public static void GetGlobal(LuaStatePtr l, string var) {
			LuaDelegates.lua_getglobal(l, var);
		}
		public static void GetTable(LuaStatePtr l, int idx) {
			LuaDelegates.lua_gettable(l, idx);
		}
		public static void GetField(LuaStatePtr l, int idx, string k) {
			LuaDelegates.lua_getfield(l, idx, k);
		}
		public static void RawGet(LuaStatePtr l, int idx) {
			LuaDelegates.lua_rawget(l, idx);
		}
		public static void RawGeti(LuaStatePtr l, int idx, int n) {
			LuaDelegates.lua_rawgeti(l, idx, n);
		}
		public static void RawGetp(LuaStatePtr l, int idx, IntPtr p) {
			LuaDelegates.lua_rawgetp(l, idx, p);
		}
		public static void CreateTable(LuaStatePtr l, int narr, int nrec) {
			LuaDelegates.lua_createtable(l, narr, nrec);
		}
		public static T NewUserData<T>(LuaStatePtr l, int sz) {
			return (T)Marshal.PtrToStructure(LuaDelegates.lua_newuserdata(l, sz), typeof(T));
		}
		public static int GetMetaTable(LuaStatePtr l, int objindex) {
			return LuaDelegates.lua_getmetatable(l, objindex);
		}
		public static void GetUserValue(LuaStatePtr l, int idx) {
			LuaDelegates.lua_getuservalue(l, idx);
		}
		public static void SetGlobal(LuaStatePtr l, string var) {
			LuaDelegates.lua_setglobal(l, var);
		}
		public static void SetTable(LuaStatePtr l, int idx) {
			LuaDelegates.lua_settable(l, idx);
		}
		public static void SetField(LuaStatePtr l, int idx, string k) {
			LuaDelegates.lua_setfield(l, idx, k);
		}
		public static void RawSet(LuaStatePtr l, int idx) {
			LuaDelegates.lua_rawset(l, idx);
		}
		public static void RawSeti(LuaStatePtr l, int idx, int n) {
			LuaDelegates.lua_rawseti(l, idx, n);
		}
		public static void RawSetp(LuaStatePtr l, int idx, IntPtr p) {
			LuaDelegates.lua_rawsetp(l, idx, p);
		}
		public static int SetMetaTable(LuaStatePtr l, int objindex) {
			return LuaDelegates.lua_setmetatable(l, objindex);
		}
		public static void SetUserValue(LuaStatePtr l, int idx) {
			LuaDelegates.lua_setuservalue(l, idx);
		}
		public static void Callk(LuaStatePtr l, int nargs, int nresults, int ctx, LuaCFunction k) {
			System.GC.KeepAlive(k);
			LuaDelegates.lua_callk(l, nargs, nresults, ctx, k);
		}
		public static int GetCtx(LuaStatePtr l, out int ctx) {
			int * i = stackalloc int[1];
			int result = LuaDelegates.lua_getctx(l, i);
			ctx = *i;
			return result;
		}
		public static int PCallk(LuaStatePtr l, int nargs, int nresults, int errfunc, int ctx, LuaCFunction k) {
			System.GC.KeepAlive(k);
			return LuaDelegates.lua_pcallk(l, nargs, nresults, errfunc, ctx, k);
		}
		public static int Load(LuaStatePtr l, LuaReader reader, IntPtr dt, string chunkname, string mode) {
			return LuaDelegates.lua_load(l, reader, dt, chunkname, mode);
		}
		public static int Dump(LuaStatePtr l, LuaWriter writer, IntPtr data) {
			return LuaDelegates.lua_dump(l, writer, data);
		}
		public static int Yieldk(LuaStatePtr l, int nresults, int ctx, LuaCFunction k) {
			System.GC.KeepAlive(k);
			return LuaDelegates.lua_yieldk(l, nresults, ctx, k);
		}
		public static int Resume(LuaStatePtr l, LuaStatePtr from, int narg) {
			return LuaDelegates.lua_resume(l, from, narg);
		}
		public static int Status(LuaStatePtr l) {
			return LuaDelegates.lua_status(l);
		}
		public static int GC(LuaStatePtr l, GCOption what, int data) {
			return LuaDelegates.lua_gc(l, (int)what, data);
		}
		public static int Error(LuaStatePtr l) {
			return LuaDelegates.lua_error(l);
		}
		public static int Next(LuaStatePtr l, int idx) {
			return LuaDelegates.lua_next(l, idx);
		}
		public static void Concat(LuaStatePtr l, int n) {
			LuaDelegates.lua_concat(l, n);
		}
		public static void Len(LuaStatePtr l, int idx) {
			LuaDelegates.lua_len(l, idx);
		}
		public static LuaAlloc GetAllocf(LuaStatePtr l, out IntPtr ud) {
			return LuaDelegates.lua_getallocf(l, out ud);
		}
		public static void SetAllocf(LuaStatePtr l, LuaAlloc f, IntPtr ud) {
			System.GC.KeepAlive(f);
			LuaDelegates.lua_setallocf(l, f, ud);
		}
		public static int GetStack(LuaStatePtr l, int level, LuaDebugPtr ar) {
			return LuaDelegates.lua_getstack(l, level, ar);
		}
		public static int GetInfo(LuaStatePtr l, string what, LuaDebugPtr ar) {
			return LuaDelegates.lua_getinfo(l, what, ar);
		}
		public static string GetLocal(LuaStatePtr l, LuaDebugPtr ar, int n) {
			return new string(LuaDelegates.lua_getlocal(l, ar, n));
		}
		public static string SetLocal(LuaStatePtr l, LuaDebugPtr ar, int n) {
			return new string(LuaDelegates.lua_setlocal(l, ar, n));
		}
		public static string GetUpValue(LuaStatePtr l, int funcindex, int n) {
			return new string(LuaDelegates.lua_getupvalue(l, funcindex, n));
		}
		public static string SetUpValue(LuaStatePtr l, int funcindex, int n) {
			return new string(LuaDelegates.lua_setupvalue(l, funcindex, n));
		}
		public static IntPtr UpValueID(LuaStatePtr l, int fidx, int n) {
			return LuaDelegates.lua_upvalueid(l, fidx, n);
		}
		public static void UpValueJoin(LuaStatePtr l, int fidx1, int n1, int fidx2, int n2) {
			LuaDelegates.lua_upvaluejoin(l, fidx1, n1, fidx2, n2);
		}
		public static int SetHook(LuaStatePtr l, LuaHook func, int mask, int count) {
			return LuaDelegates.lua_sethook(l, func, mask, count);
		}
		public static LuaHook GetHook(LuaStatePtr l) {
			return LuaDelegates.lua_gethook(l);
		}
		public static int GetHookMask(LuaStatePtr l) {
			return LuaDelegates.lua_gethookmask(l);
		}
		public static int GetHookCount(LuaStatePtr l) {
			return LuaDelegates.lua_gethookcount(l);
		}
	}
}
