using System;

namespace Pencil.Gaming.Scripting {
	public static unsafe partial class LuaL {
		public const int NoRef = -2;
		public const int RefNil = -1;

		public static void CheckVersion(LuaStatePtr l, double ver) {
			LuaLDelegates.luaL_checkversion_(l, ver);
		}
		public static int GetMetaField(LuaStatePtr l, int obj, string e) {
			return LuaLDelegates.luaL_getmetafield(l, obj, e);
		}
		public static int CallMeta(LuaStatePtr l, int obj, string e) {
			return LuaLDelegates.luaL_callmeta(l, obj, e);
		}
		public static string ToLString(LuaStatePtr l, int idx, out int len) {
			fixed(int * i = &len) {
				return new string(LuaLDelegates.luaL_tolstring(l, idx, i));
			}
		}
		public static int ArgError(LuaStatePtr l, int numarg, string extramsg) {
			return LuaLDelegates.luaL_argerror(l, numarg, extramsg);
		}
		public static string CheckLString(LuaStatePtr l, int numArg, out int l_) {
			fixed (int * i = &l_) {
				return new string(LuaLDelegates.luaL_checklstring(l, numArg, i));
			}
		}
		public static string OptLString(LuaStatePtr l, int numArg, string def, out int l_) {
			fixed (int * i = &l_) {
				return new string(LuaLDelegates.luaL_optlstring(l, numArg, def, i));
			}
		}
		public static double CheckNumber(LuaStatePtr l, int numArg) {
			return LuaLDelegates.luaL_checknumber(l, numArg);
		}
		public static double OptNumber(LuaStatePtr l, int nArg, double def) {
			return LuaLDelegates.luaL_optnumber(l, nArg, def);
		}
		public static int CheckInteger(LuaStatePtr l, int numArg) {
			return LuaLDelegates.luaL_checkinteger(l, numArg);
		}
		public static int OptInteger(LuaStatePtr l, int nArg, int def) {
			return LuaLDelegates.luaL_optinteger(l, nArg, def);
		}
		public static uint CheckUnsigned(LuaStatePtr l, int numArg) {
			return LuaLDelegates.luaL_checkunsigned(l, numArg);
		}
		public static uint OptUnsigned(LuaStatePtr l, int numArg, uint def) {
			return LuaLDelegates.luaL_optunsigned(l, numArg, def);
		}
		public static void CheckStack(LuaStatePtr l, int sz, string msg) {
			LuaLDelegates.luaL_checkstack(l, sz, msg);
		}
		public static void CheckType(LuaStatePtr l, int narg, int t) {
			LuaLDelegates.luaL_checktype(l, narg, t);
		}
		public static void CheckAny(LuaStatePtr l, int narg) {
			LuaLDelegates.luaL_checkany(l, narg);
		}
		public static int NewMetaTable(LuaStatePtr l, string tname) {
			return LuaLDelegates.luaL_newmetatable(l, tname);
		}
		public static void SetMetaTable(LuaStatePtr l, string tname) {
			LuaLDelegates.luaL_setmetatable(l, tname);
		}
		public static IntPtr TestUData(LuaStatePtr l, int ud, string tname) {
			return LuaLDelegates.luaL_testudata(l, ud, tname);
		}
		public static IntPtr CheckUData(LuaStatePtr l, int ud, string tname) {
			return LuaLDelegates.luaL_checkudata(l, ud, tname);
		}
		public static void Where(LuaStatePtr l, int lvl) {
			LuaLDelegates.luaL_where(l, lvl);
		}
		public static int CheckOption(LuaStatePtr l, int narg, string def, string[] lst) {
			sbyte *[] lst_ = new sbyte*[lst.Length];
			for (int i = 0; i < lst.Length; ++i) {
				fixed(char * c = &lst[i].ToCharArray()[0]) {
					sbyte * b = (sbyte *)c;
					lst_[i] = b;
				}
			}
			return LuaLDelegates.luaL_checkoption(l, narg, def, lst_);
		}
		public static int FileResult(LuaStatePtr l, int stat, string fname) {
			return LuaLDelegates.luaL_fileresult(l, stat, fname);
		}
		public static int ExecResult(LuaStatePtr l, int stat) {
			return LuaLDelegates.luaL_execresult(l, stat);
		}
		public static int Ref(LuaStatePtr l, int t) {
			return LuaLDelegates.luaL_ref(l, t);
		}
		public static void UnRef(LuaStatePtr l, int t, int @ref) {
			LuaLDelegates.luaL_unref(l, t,  @ref);
		}
		public static int LoadFilex(LuaStatePtr l, string filename, string mode) {
			return LuaLDelegates.luaL_loadfilex(l, filename, mode);
		}
		public static LuaBufferPtr LoadBufferx(LuaStatePtr l, string buff, int sz, string name, string mode) {
			return new LuaBufferPtr(new IntPtr(LuaLDelegates.luaL_loadbufferx(l, buff, sz, name, mode)));
		}
		public static int LoadString(LuaStatePtr l, string s) {
			return LuaLDelegates.luaL_loadstring(l, s);
		}
		public static LuaStatePtr NewState() {
			return LuaLDelegates.luaL_newstate();
		}
		public static int Len(LuaStatePtr l, int idx) {
			return LuaLDelegates.luaL_len(l, idx);
		}
		public static string GSub(LuaStatePtr l, string s, string p, string r) {
			return new string(LuaLDelegates.luaL_gsub(l, s, p, r));
		}
		public static void SetFuncs(LuaStatePtr l, IntPtr l_, int nup) {
			LuaLDelegates.luaL_setfuncs(l, l_, nup);
		}
		public static int GetSubTable(LuaStatePtr l, int idx, string fname) {
			return LuaLDelegates.luaL_getsubtable(l, idx, fname);
		}
		public static void TraceBack(LuaStatePtr l, LuaStatePtr l1, string msg, int level) {
			LuaLDelegates.luaL_traceback(l, l1, msg, level);
		}
		public static void Requiref(LuaStatePtr l, string modname, LuaCFunction openf, int glb) {
			GC.KeepAlive(openf);
			LuaLDelegates.luaL_requiref(l, modname, openf, glb);
		}
		public static void BuffInit(LuaStatePtr l, LuaBufferPtr B) {
			LuaLDelegates.luaL_buffinit(l, B);
		}
		public static sbyte[] PrepBufferSize(LuaBufferPtr B, int sz) {
			sbyte * bytes = LuaLDelegates.luaL_prepbuffsize(B, sz);
			sbyte[] result = new sbyte[sz];
			for (; sz >= 0; --sz) {
				result[sz] = bytes[sz];
			}
			return result;
		}
		public static void AddLString(LuaBufferPtr B, string s, int l) {
			LuaLDelegates.luaL_addlstring(B, s, l);
		}
		public static void AddString(LuaBufferPtr B, string s) {
			LuaLDelegates.luaL_addstring(B, s);
		}
		public static void AddValue(LuaBufferPtr B) {
			LuaLDelegates.luaL_addvalue(B);
		}
		public static void PushResult(LuaBufferPtr B) {
			LuaLDelegates.luaL_pushresult(B);
		}
		public static void PushResultSize(LuaBufferPtr B, int sz) {
			LuaLDelegates.luaL_pushresultsize(B, sz);
		}
		public static sbyte[] BuffInitSize(LuaStatePtr l, LuaBufferPtr B, int sz) {
			sbyte * bytes = LuaLDelegates.luaL_buffinitsize(l, B, sz);
			sbyte[] result = new sbyte[sz];
			for (; sz >= 0; --sz) {
				result[sz] = bytes[sz];
			}
			return result;
		}
		public static void OpenLibs(LuaStatePtr l) {
			LuaLDelegates.luaL_openlibs(l);
		}
	}
}

