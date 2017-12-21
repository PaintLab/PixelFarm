using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;


namespace LayoutFarm.NativeInterop
{

    public class NativeMethodMap
    {

        string nativeMethodName;
        bool isResolved;
        IntPtr funcPointer;
        Delegate cachedDelegate;

        public NativeMethodMap(string nativeMethodName)
        {
            this.nativeMethodName = nativeMethodName;
        }
        public string NativeMethodName
        {
            get
            {
                return this.nativeMethodName;
            }
        }
        public bool Resolve(IntPtr hModule)
        {
            IntPtr foundProcAddress = UnsafeMethods.GetProcAddress(hModule, this.nativeMethodName);
            if (foundProcAddress == IntPtr.Zero)
            {
                return false;
            }
            this.funcPointer = foundProcAddress;
            return true;
        }
        public bool Resolve(IntPtr hModule, Type targetDelegateType)
        {
            IntPtr foundProcAddress = UnsafeMethods.GetProcAddress(hModule, this.nativeMethodName);
            if (foundProcAddress == IntPtr.Zero)
            {
                return false;
            }
            this.funcPointer = foundProcAddress;
            cachedDelegate = Marshal.GetDelegateForFunctionPointer(funcPointer, targetDelegateType);
            this.isResolved = true;
            return true;
        }
        public bool IsResolved
        {
            get
            {
                return this.isResolved;
            }
            protected set
            {
                this.isResolved = value;
            }
        }

        void TransformFunctionPointer(IntPtr funcPointer)
        {
            this.funcPointer = funcPointer;

        }

        public T GetDelegate<T>()
        {
            if (!isResolved)
            {
                cachedDelegate = Marshal.GetDelegateForFunctionPointer(funcPointer, typeof(T));
                this.isResolved = true;
            }

            return (T)((object)cachedDelegate);
        }
        public Delegate GetCacheDelegate()
        {

            return this.cachedDelegate;
        }
    }

    [System.Runtime.InteropServices.StructLayout(LayoutKind.Sequential)]
    class MethodCallingArgs
    {
        public int numArgs;
    }

    enum ManagedCallbackKind : int
    {
        Listener,
        MethodCall
    }

    public class NativeFunc : Attribute
    {
        public NativeFunc()
        {
            this.NativeName = "";
        }
        public NativeFunc(string nativeName)
        {
            this.NativeName = nativeName;
        }
        public string NativeName { get; private set; }
    }
    static class UnsafeMethods
    {
        //-----------------------------------------------
        [DllImport("Kernel32.dll")]
        public static extern IntPtr LoadLibrary(string libraryName);
        [DllImport("Kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr hModule);
        [DllImport("Kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
        [DllImport("Kernel32.dll")]
        public static extern uint SetErrorMode(int uMode);
        [DllImport("Kernel32.dll")]
        public static extern uint GetLastError();
        //-----------------------------------------------

    }


    public class NativeModuleLoader
    {
        IntPtr nativeModule;
        Dictionary<string, NativeMethodMap> importFuncs = new Dictionary<string, NativeMethodMap>();
        string loadModuleFileName;
        public NativeModuleLoader(string moduleName, IntPtr nativeModule)
        {
            this.nativeModule = nativeModule;
            this.ModuleName = moduleName;
        }
        public NativeModuleLoader(string moduleName, string libFilename)
        {
            this.loadModuleFileName = libFilename;
            this.ModuleName = moduleName;
        }
        public bool LoadRequestProcs(Type holderType)
        {
            //1. load module 
            //2. load procedure
            if (!string.IsNullOrEmpty(this.loadModuleFileName))
            {
                this.nativeModule = UnsafeMethods.LoadLibrary(this.loadModuleFileName);
                if (nativeModule == IntPtr.Zero)
                {
                    return false;
                }
            }
            //---------------------------------------------------


            var allFields = holderType.GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            int j = allFields.Length;
            Type nativeNameAttrType = typeof(NativeFunc);
            Type nativeFuncPointerType = typeof(UnmanagedFunctionPointerAttribute);

            for (int i = 0; i < j; ++i)
            {
                var field = allFields[i];
                var nativeNameAttrs = field.GetCustomAttributes(nativeNameAttrType, false);

                if (nativeNameAttrs.Length > 0)
                {   
                    //only 1
                    NativeFunc natName = (NativeFunc)nativeNameAttrs[0];
                    Type fieldType = field.FieldType;
                    string procName = fieldType.Name;
                    //------------------------------------------------------------
                    if (!string.IsNullOrEmpty(natName.NativeName))
                    {
                        //use field name
                        procName = natName.NativeName;
                    } 
                    //------------------------------------------------------------
                    //find-resolve-assign
                    NativeMethodMap nativeMethodMap = new NativeMethodMap(procName);
                    if (nativeMethodMap.Resolve(nativeModule, fieldType))
                    {
                        importFuncs[procName] = nativeMethodMap;
                        field.SetValue(null, nativeMethodMap.GetCacheDelegate());
                    } 
                }
            }
            return true;
        }
        public string ModuleName { get; private set; }
        public T GetNativeDel<T>(string funcName)
        {
            NativeMethodMap nativeMethod = new NativeMethodMap(funcName);
            nativeMethod.Resolve(nativeModule);
            return (T)nativeMethod.GetDelegate<T>();
        }
    }


}