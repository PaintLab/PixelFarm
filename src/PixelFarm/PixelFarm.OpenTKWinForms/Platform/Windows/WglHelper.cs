/* Copyright (c) 2006, 2007 Stefanos Apostolopoulos
 * See license.txt for license info
 *
 * Date: 12/8/2007
 * Time: 6:43 ��
 */

using System;
using System.Runtime.InteropServices;
using System.Reflection;
namespace OpenTK.Platform.Windows
{
    internal partial class Wgl
    {

        static Wgl()
        {

            //version 1
            //assembly = Assembly.GetExecutingAssembly();
            //wglClass = assembly.GetType("OpenTK.Platform.Windows.Wgl");
            //delegatesClass = wglClass.GetNestedType("Delegates", BindingFlags.Static | BindingFlags.NonPublic);
            //importsClass = wglClass.GetNestedType("Imports", BindingFlags.Static | BindingFlags.NonPublic);

            //version 2:
            wglClass = typeof(Wgl);
            assembly = wglClass.Assembly;
            delegatesClass = typeof(Wgl.Delegates);
            importsClass = typeof(Wgl.Imports);
            // Ensure core entry points are ready prior to accessing any method.
            // Resolves bug [#993]: "Possible bug in GraphicsContext.CreateDummyContext()" 
            LoadAll();
        }



        internal const string Library = "OPENGL32.DLL";
        private static Assembly assembly;
        private static Type wglClass;
        private static Type delegatesClass;
        private static Type importsClass;
        private static bool rebuildExtensionList = true;


        /// <summary>
        /// Creates a System.Delegate that can be used to call an OpenGL function, core or extension.
        /// </summary>
        /// <param name="name">The name of the Wgl function (eg. "wglNewList")</param>
        /// <param name="signature">The signature of the OpenGL function.</param>
        /// <returns>
        /// A System.Delegate that can be used to call this OpenGL function, or null if the specified
        /// function name did not correspond to an OpenGL function.
        /// </returns>
        static Delegate LoadDelegate(string name, Type signature)
        {
            Delegate d;
            string realName = name.StartsWith("wgl") ? name.Substring(3) : name;
            if (importsClass.GetMethod(realName,
                BindingFlags.NonPublic | BindingFlags.Static) != null)
                d = GetExtensionDelegate(name, signature) ??
                    Delegate.CreateDelegate(signature, typeof(Imports), realName);
            else
                d = GetExtensionDelegate(name, signature);
            return d;
        }



        /// <summary>
        /// Creates a System.Delegate that can be used to call a dynamically exported OpenGL function.
        /// </summary>
        /// <param name="name">The name of the OpenGL function (eg. "glNewList")</param>
        /// <param name="signature">The signature of the OpenGL function.</param>
        /// <returns>
        /// A System.Delegate that can be used to call this OpenGL function or null
        /// if the function is not available in the current OpenGL context.
        /// </returns>
        private static Delegate GetExtensionDelegate(string name, Type signature)
        {
            IntPtr address = Imports.GetProcAddress(name);
            if (address == IntPtr.Zero ||
                address == new IntPtr(1) ||     // Workaround for buggy nvidia drivers which return
                address == new IntPtr(2))       // 1 or 2 instead of IntPtr.Zero for some extensions.
            {
                return null;
            }
            else
            {
                return Marshal.GetDelegateForFunctionPointer(address, signature);
            }
        }


        static WglExtensionLoader wglExtensionLoader;
        /// <summary>
        /// Loads all Wgl entry points, core and extensions.
        /// </summary>
        public static GLExtensionLoader LoadAll()
        {
            if (wglExtensionLoader == null)
            {
                wglExtensionLoader = new WglExtensionLoader();
                OpenTK.Platform.Utilities.LoadExtensions(wglExtensionLoader);
            }
            return wglExtensionLoader;
        }
        public static void ClearExtensionLoader()
        {
            wglExtensionLoader = null;
        }



        class WglExtensionLoader : GLExtensionLoader
        {

            System.Collections.Generic.Dictionary<string, bool> loadedExtNames;
            public WglExtensionLoader()
            {
            }
            public override bool SupportFuncName(string funcName)
            {
                if (loadedExtNames != null)
                {
                    return loadedExtNames.ContainsKey(funcName);
                }
                return false;
            }
            public override int LoadDelegates()
            {
                loadedExtNames = new System.Collections.Generic.Dictionary<string, bool>();
                int supported = 0;
                Type extensions_class = typeof(Wgl.Delegates);
                //get all fields
                FieldInfo[] delegates = extensions_class.GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                //
                if (delegates == null)
                    throw new InvalidOperationException("The specified type does not have any loadable extensions.");
                //
                foreach (FieldInfo f in delegates)
                {
                    //so... this field name must be preserved!
                    Delegate d = Wgl.LoadDelegate(f.Name, f.FieldType);
                    if (d != null)
                    {
                        loadedExtNames.Add(f.Name, true);
                        ++supported;
                    }
                    f.SetValue(null, d);
                }
                Wgl.rebuildExtensionList = true;

                return supported;
            }
        }


        ///// <summary>
        ///// Loads the given Wgl entry point.
        ///// </summary>
        ///// <param name="functionName">The name of the function to load.</param>
        ///// <returns></returns>
        //public static bool Load(string functionName)
        //{
        //    return OpenTK.Platform.Utilities.TryLoadExtension(typeof(Wgl), functionName);
        //}



        /// <summary>Contains ARB extensions for WGL.</summary>
        public static partial class Arb
        {
            /// <summary>
            /// Checks if a Wgl extension is supported by the given context.
            /// </summary>
            /// <param name="context">The device context.</param>
            /// <param name="ext">The extension to check.</param>
            /// <returns>True if the extension is supported by the given context, false otherwise</returns>
            public static bool SupportsExtension(WinGLContext context, string ext)
            {
                // We cache this locally, as another thread might create a context which doesn't support  this method.
                // The design is far from ideal, but there's no good solution to this issue as long as we are using
                // static WGL/GL classes. Fortunately, this issue is extremely unlikely to arise in practice, as you'd
                // have to create one accelerated and one non-accelerated context in the same application, with the
                // non-accelerated context coming second.
                Wgl.Delegates.GetExtensionsStringARB get = Wgl.Delegates.wglGetExtensionsStringARB;
                if (get != null)
                {
                    string[] extensions = null;
                    unsafe
                    {
                        extensions = new string((sbyte*)get(context.DeviceContext))
                            .Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    }
                    if (extensions == null || extensions.Length == 0)
                        return false;
                    foreach (string s in extensions)
                        if (s == ext)
                            return true;
                }
                return false;
            }
        }



        /// <summary>Contains EXT extensions for WGL.</summary>
        public static partial class Ext
        {
            private static string[] extensions;
            /// <summary>
            /// Checks if a Wgl extension is supported by the given context.
            /// </summary>
            /// <param name="ext">The extension to check.</param>
            /// <returns>True if the extension is supported by the given context, false otherwise</returns>
            public static bool SupportsExtension(string ext)
            {
                if (Wgl.Delegates.wglGetExtensionsStringEXT != null)
                {
                    if (extensions == null || rebuildExtensionList)
                    {
                        extensions = Wgl.Ext.GetExtensionsString().Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        Array.Sort(extensions);
                        rebuildExtensionList = false;
                    }

                    return Array.BinarySearch(extensions, ext) != -1;
                }
                return false;
            }
        }

    }
}
