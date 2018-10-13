//MIT, 2018-present, WinterDev
//----------------------------------------------------------------------
//
// The Open Toolkit Library License
//
// Copyright (c) 2006 - 2010 the Open Toolkit library.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Bind.GL2;
using Bind.Structures;

namespace Bind
{

    using Delegate = Bind.Structures.Delegate;
    using Enum = Bind.Structures.Enum;
    using Type = Bind.Structures.Type;


    static class CodeGenSetting
    {
        public const string GLDelegateClass = "Delegates";
    }

    partial class CSharpSpecWriter
    {
        MyEsGenerator _myESGen;
        void InitBindings()
        {
            Bind.GL2.Generator asGL2Gen = Generator as Bind.GL2.Generator;
            if (asGL2Gen != null && asGL2Gen.EnableMyEsGen)
            {
                _myESGen = new MyEsGenerator();
                _myESGen.SetHostCsWriter(this);
                _myESGen.SetOriginalGenerator(asGL2Gen);
            }
            //-----------------
        }

        public class MyEsGenerator
        {

            //for PixelFarm project(https://github.com/PaintLab/PixelFarm)
            //---
            Generator _originalGenerator;
            CSharpSpecWriter _hostCSWriter;
            public void SetHostCsWriter(CSharpSpecWriter csWriter)
            {
                _hostCSWriter = csWriter;
            }

            string _targetOutputDir;
            string _esGlFile;
            string _esEnumFile;
            string _esDelegateFile;

            string _esNamespaceName;


            public void SetOriginalGenerator(Generator originalGenerator)
            {
                _originalGenerator = originalGenerator;
                if (originalGenerator is ES.ES2Generator)
                {
                    _targetOutputDir = @"src\OpenTK\0_MyExtension\ES20mini";
                    //generated files (3 files)
                    _esGlFile = _targetOutputDir + "\\ES20.cs";
                    _esEnumFile = _targetOutputDir + "\\ES20Enum.cs";
                    _esDelegateFile = _targetOutputDir + "\\ES20Delegate.cs";
                    _esNamespaceName = "ES20";

                }
                else if (originalGenerator is ES.ES3Generator)
                {
                    _targetOutputDir = @"src\OpenTK\0_MyExtension\ES30mini";
                    //generated files (3 files)
                    _esGlFile = _targetOutputDir + "\\ES30.cs";
                    _esEnumFile = _targetOutputDir + "\\ES30Enum.cs";
                    _esDelegateFile = _targetOutputDir + "\\ES30Delegate.cs";
                    _esNamespaceName = "ES30";

                }
                else
                {
                    throw new NotSupportedException();
                }

                CreateDirIfNotExist(_targetOutputDir);

            }
            static void CreateDirIfNotExist(string targetDirName)
            {
                if (!Directory.Exists(targetDirName))
                {
                    Directory.CreateDirectory(targetDirName);
                }
            }

            Settings Settings => _hostCSWriter.Settings;

            public void CopyEnums(string orgEnumFile)
            {
                File.Copy(orgEnumFile, _esEnumFile, true);
            }
            public void WriteDelegatesAndDelegateSlots(List<Delegate> outputFuncs)
            {

                using (MemoryStream ms = new MemoryStream())
                using (StreamWriter w = new StreamWriter(ms))
                {
                    w.WriteLine("//autogen " + DateTime.Now.ToString("u"));
                    w.WriteLine("namespace OpenTK.Graphics." + _esNamespaceName + "{");
                    w.WriteLine(" using System;");
                    w.WriteLine(" using System.Text;");
                    w.WriteLine(" using System.Runtime.InteropServices; ");
                    //my experiment

                    //create delegate slot
                    w.WriteLine();
                    w.WriteLine("[System.Security.SuppressUnmanagedCodeSecurity()] //apply to all members");
                    w.WriteLine($"static class {CodeGenSetting.GLDelegateClass} {{");

                    int marker_count = 0;
                    for (int i = 0; i < outputFuncs.Count; ++i)
                    {
                        Delegate d = outputFuncs[i];
                        w.WriteLine();

                        ++marker_count;
                        w.WriteLine("//m* " + marker_count);
#if DEBUG
                        //if (marker_count == 8)
                        //{

                        //}
#endif
                        w.WriteLine();
                        w.WriteLine("[UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Ansi)]");
                        w.WriteLine($"public {_hostCSWriter.GetDeclarationString2(d, true)};");

                        w.WriteLine($"public static {d.Name}  {   Settings.FunctionPrefix + d.Name};");
                        w.WriteLine();
                    }

                    w.WriteLine("}"); //close class GLDelegateClass
                                      //------------

                    //GLDelInit
                    w.WriteLine("static class GLDelInit{");
                    w.WriteLine("static void AssignDelegate<T>(out T del, string funcName){");
                    /**
                     *      IntPtr funcPtr = PlatformAddressPortal.GetAddressDelegate(funcName);
                            del = (funcPtr == IntPtr.Zero) ?
                            default(T) :
                            (T)(object)(Marshal.GetDelegateForFunctionPointer(funcPtr, typeof(T)));
                      */

                    w.WriteLine("IntPtr funcPtr = PlatformAddressPortal.GetAddressDelegate(funcName);");
                    w.WriteLine(" del = (funcPtr == IntPtr.Zero) ? default(T) :  (T)(object)(Marshal.GetDelegateForFunctionPointer(funcPtr, typeof(T)));");
                    w.WriteLine("}");

                    w.WriteLine("public static void LoadAll(){");
                    for (int i = 0; i < outputFuncs.Count; ++i)
                    {
                        Delegate d = outputFuncs[i];
                        w.WriteLine("AssignDelegate(out " + CodeGenSetting.GLDelegateClass + "." + Settings.FunctionPrefix + d.Name + ",\"" + Settings.FunctionPrefix + d.Name + "\");");
                    }
                    w.WriteLine("}"); //close LoadAll()
                                      // 




                    w.WriteLine("}"); //close class
                                      //------------
                    w.WriteLine("}"); //namespace
                    w.Flush();

                    //----------------------
                    //  
                    File.WriteAllText(_esDelegateFile, Encoding.UTF8.GetString(ms.ToArray()));
                    // 
                }
            }


            //
            BindStreamWriter _sw2;
            public BindStreamWriter BeginWriteES()
            {

                _sw2 = new BindStreamWriter(_esGlFile + ".txt");
                _sw2.WriteLine("//autogen " + DateTime.Now.ToString("u"));
                _sw2.WriteLine("namespace OpenTK.Graphics." + _esNamespaceName + " {");
                _sw2.WriteLine(" using System;");
                _sw2.WriteLine(" using System.Text;");
                _sw2.WriteLine(" using System.Runtime.InteropServices; ");
                //
                _sw2.WriteLine("  public partial class GL{");
                _sw2.WriteLine("  public void LoadAll(){");
                _sw2.WriteLine("     GLDelInit.LoadAll();");
                _sw2.WriteLine("}");

                return _sw2;
            }
            public void EndWriteES()
            {
                _sw2.WriteLine("}"); //close GLES class
                _sw2.WriteLine("}"); //close namespace
                _sw2.Flush();
                _sw2.Close();

                //
                //rename
                if (File.Exists(_esGlFile))
                {
                    File.Delete(_esGlFile);
                }
                File.Move(_esGlFile + ".txt", _esGlFile); //move es
            }

            internal void WriteWrapper2(Function f, EnumCollection enums)
            {
                if ((Settings.Compatibility & Settings.Legacy.NoDocumentation) == 0)
                {
                    _hostCSWriter.WriteDocumentation(_sw2, f);
                }
                WriteMethod2(_sw2, f, enums);
                _sw2.WriteLine();
            }
            void WriteMethod2(BindStreamWriter sw, Function f, EnumCollection enums)
            {

                if (!String.IsNullOrEmpty(f.Obsolete))
                {
                    return; // TODO: add note that we 
                            //sw.WriteLine("[Obsolete(\"{0}\")]", f.Obsolete);
                }
                else if (f.Deprecated && Settings.IsEnabled(Settings.Legacy.AddDeprecationWarnings))
                {
                    return; //
                            //sw.WriteLine("[Obsolete(\"Deprecated in OpenGL {0}\")]", f.DeprecatedVersion);
                }


                //--------------------------------------------------------------------------------------------

#if DEBUG
                _dbugWrite_Id++;


                sw.WriteLine("//x* " + _dbugWrite_Id);
#endif

                //we may not need this...
                sw.WriteLine("//[AutoGenerated(Category = \"{0}\", Version = \"{1}\", EntryPoint = \"{2}\")]",
                    f.Category, f.Version, Settings.FunctionPrefix + f.WrappedDelegate.EntryPoint);

                //we may not need to gen [CLSCompliant(false)] if our asm dose not require it
                //if (!f.CLSCompliant)
                //{
                //    sw.WriteLine("[CLSCompliant(false)]");
                //}

                sw.WriteLine($"public static {_hostCSWriter.GetDeclarationString(f, Settings.Compatibility)}");
                //body
                //-----
                //sw.WriteLine("//hello!");
                CreateBody(f, f.CLSCompliant, enums, Settings.FunctionPrefix, f.WrappedDelegate);
                sw.WriteLine(f.Body.ToString());
            }
#if DEBUG
            int _dbugWrite_Id;
#endif 
            //-------------------------------------------------------------------
            readonly List<string> handle_statements = new List<string>();
            readonly List<string> handle_release_statements = new List<string>();
            readonly List<string> fixed_statements = new List<string>();
            readonly List<string> assign_statements = new List<string>();
            readonly List<string> stringOutAlloc_statements = new List<string>();
            readonly List<string> stringOut_set_statements = new List<string>();

            void CreateBody(Function func, bool wantCLSCompliance, EnumCollection enums, string funcPrefix, Delegate wrappedDel)
            {

                string FindBuffSizeVarName(Parameter[] inputPars, string varName, string expectVarName, bool retry = true)
                {
                    for (int i = 0; i < inputPars.Length; ++i)
                    {
                        if (inputPars[i].Name.ToLower() == expectVarName)
                        {
                            return inputPars[i].Name;
                        }
                    }
                    //temp fix
                    if (!retry)
                    {
                        return null;
                    }
                    return FindBuffSizeVarName(inputPars, varName, (varName + "Length").ToLower(), false);
                }


                Function f = new Function(func);
                f.Body.Clear();
                handle_statements.Clear();
                handle_release_statements.Clear();
                fixed_statements.Clear();
                assign_statements.Clear();

                //
                stringOutAlloc_statements.Clear();
                stringOut_set_statements.Clear();
                //
                // Obtain pointers by pinning the parameters

                // check parameter type is match or not



#if DEBUG
                if (func.Parameters.Count != wrappedDel.Parameters.Count)
                {

                }
#endif


                foreach (Parameter p in f.Parameters)
                {

                    if (p.NeedsPin)
                    {
                        if (p.WrapperType == WrapperTypes.GenericParameter)
                        {
                            // Use GCHandle to obtain pointer to generic parameters and 'fixed' for arrays.
                            // This is because fixed can only take the address of fields, not managed objects.
                            handle_statements.Add(String.Format(
                                "{0} {1}_ptr = {0}.Alloc({1}, GCHandleType.Pinned);",
                                "GCHandle", p.Name));

                            handle_release_statements.Add(String.Format("{0}_ptr.Free();", p.Name));

                            // Due to the GCHandle-style pinning (which boxes value types), we need to assign the modified
                            // value back to the reference parameter (but only if it has an out or in/out flow direction).
                            if ((p.Flow == FlowDirection.Out || p.Flow == FlowDirection.Undefined) && p.Reference)
                            {
                                assign_statements.Add(String.Format(
                                    "{0} = ({1}){0}_ptr.Target;",
                                    p.Name, p.QualifiedType));
                            }

                            // Note! The following line modifies f.Parameters, *not* this.Parameters
                            p.Name = "(IntPtr)" + p.Name + "_ptr.AddrOfPinnedObject()";
                        }
                        else if ((
                            p.WrapperType == WrapperTypes.PointerParameter ||
                            p.WrapperType == WrapperTypes.ArrayParameter ||
                            p.WrapperType == WrapperTypes.ReferenceParameter) ||
                            //
                            ((p.WrapperType & WrapperTypes.PointerParameter) == WrapperTypes.PointerParameter) ||
                            ((p.WrapperType & WrapperTypes.ArrayParameter) == WrapperTypes.ArrayParameter) ||
                            ((p.WrapperType & WrapperTypes.ReferenceParameter) == WrapperTypes.ReferenceParameter)
                            )
                        {
                            // A fixed statement is issued for all non-generic pointers, arrays and references.



                            fixed_statements.Add(String.Format(
                                "fixed ({0}{3} {1} = {2})",
                                (wantCLSCompliance && !p.CLSCompliant) ? p.GetCLSCompliantType() : p.QualifiedType,
                                p.Name + "_ptr",
                                p.Array > 0 ? p.Name : "&" + p.Name,
                                pointer_levels[p.IndirectionLevel]));

                            if (p.Name == "pixels_ptr")
                                System.Diagnostics.Debugger.Break();

                            // Arrays are not value types, so we don't need to do anything for them.
                            // Pointers are passed directly by value, so we don't need to assign them back either (they don't change).
                            if ((p.Flow == FlowDirection.Out || p.Flow == FlowDirection.Undefined) && p.Reference)
                            {
                                assign_statements.Add(String.Format("{0} = *{0}_ptr;", p.Name));
                            }

                            p.Name = p.Name + "_ptr";

                            if (p.CurrentType == "IntPtr")
                            {
                                p.ExplicitCastType = "IntPtr";
                            }
                            else if (p.IsEnum)
                            {
                                p.ExplicitCastType = "int*";
                            }
                        }
                        else
                        {
                            throw new ApplicationException("Unknown parameter type");
                        }
                    }
                    else if (p.Flow == FlowDirection.Out && p.QualifiedType.ToLower() == "string")
                    {

                        if (p.Name.StartsWith("@"))
                        {
                            p.TempOutStringName = "c_" + p.Name.Substring(1);
                        }
                        else
                        {
                            p.TempOutStringName = "c_" + p.Name;
                        }

                        //find bufSize 
                        string bufSizeVarName = FindBuffSizeVarName(func.Parameters.ToArray(), p.Name, "bufsize");
                        if (bufSizeVarName == null)
                        {
                            stringOutAlloc_statements.Add("char* " + p.TempOutStringName + " = stackalloc char[256];"); //TODO: review size 256?
                        }
                        else
                        {
                            stringOutAlloc_statements.Add("char* " + p.TempOutStringName + " = stackalloc char[(int)" + bufSizeVarName + "];");
                        }

                        //this char name has change
                        assign_statements.Add(p.Name + "= new string(" + p.TempOutStringName + ");");
                    }
                    else if (p.Flow == FlowDirection.In && p.QualifiedType.ToLower() == "string")
                    {
                        if ((p.WrapperType & WrapperTypes.StringArrayParameter) == WrapperTypes.StringArrayParameter)
                        {

                        }
                        else
                        {
                            //if (p.Name.StartsWith("@"))
                            //{
                            //    p.TempOutStringName = "c_" + p.Name.Substring(1);
                            //}
                            //else
                            //{
                            //    p.TempOutStringName = "c_" + p.Name;
                            //}
                            //fixed_statements.Add("fixed(char* " + p.TempOutStringName + "=" + p.Name + ")");
                        }

                    }
                    else if (p.IsEnum)
                    {
                        if (p.Flow == FlowDirection.Out ||
                            p.Pointer != 0)
                        {
                            p.ExplicitCastType = "int*";
                        }
                    }
                    else if ((
                            p.WrapperType == WrapperTypes.PointerParameter ||
                            p.WrapperType == WrapperTypes.ArrayParameter ||
                            p.WrapperType == WrapperTypes.ReferenceParameter) ||
                            //
                            ((p.WrapperType & WrapperTypes.PointerParameter) == WrapperTypes.PointerParameter) ||
                            ((p.WrapperType & WrapperTypes.ArrayParameter) == WrapperTypes.ArrayParameter) ||
                            ((p.WrapperType & WrapperTypes.ReferenceParameter) == WrapperTypes.ReferenceParameter)
                            )
                    {
                        if (p.CurrentType.Contains("[]"))
                        {

                        }
                    }

                }

                // Automatic OpenGL error checking.
                // See OpenTK.Graphics.ErrorHelper for more information.
                // Make sure that no error checking is added to the GetError function,
                // as that would cause infinite recursion!
                if ((Settings.Compatibility & Settings.Legacy.NoDebugHelpers) == 0)
                {
                    if (f.TrimmedName != "GetError")
                    {
                        f.Body.Add("#if DEBUG");
                        f.Body.Add("using (new ErrorHelper(GraphicsContext.CurrentContext))");
                        f.Body.Add("{");
                        if (f.TrimmedName == "Begin")
                            f.Body.Add("GraphicsContext.CurrentContext.ErrorChecking = false;");
                        f.Body.Add("#endif");
                    }
                }

                if (!f.Unsafe && fixed_statements.Count > 0 || stringOutAlloc_statements.Count > 0)
                {
                    f.Body.Add("unsafe");
                    f.Body.Add("{");
                    f.Body.Indent();
                }


                if (stringOutAlloc_statements.Count > 0)
                {
                    f.Body.AddRange(stringOutAlloc_statements);
                }
                if (fixed_statements.Count > 0)
                {
                    f.Body.AddRange(fixed_statements);
                    f.Body.Add("{");
                    f.Body.Indent();
                }

                if (handle_statements.Count > 0)
                {
                    f.Body.AddRange(handle_statements);
                    f.Body.Add("try");
                    f.Body.Add("{");
                    f.Body.Indent();
                }

                // Hack: When creating untyped enum wrappers, it is possible that the wrapper uses an "All"
                // enum, while the delegate uses a specific enum (e.g. "TextureUnit"). For this reason, we need
                // to modify the parameters before generating the call string.
                // Note: We cannot generate a callstring using WrappedDelegate directly, as its parameters will
                // typically be different than the parameters of the wrapper. We need to modify the parameters
                // of the wrapper directly.
                if ((Settings.Compatibility & Settings.Legacy.KeepUntypedEnums) != 0)
                {
                    int parameter_index = -1; // Used for comparing wrapper parameters with delegate parameters
                    foreach (Parameter p in f.Parameters)
                    {
                        parameter_index++;
                        //if (IsEnum(p.Name, enums) && p.QualifiedType != f.WrappedDelegate.Parameters[parameter_index].QualifiedType)
                        //{
                        //    p.QualifiedType = f.WrappedDelegate.Parameters[parameter_index].QualifiedType;
                        //}
                        if (IsEnum(p.CurrentType, enums))//&& p.QualifiedType != f.WrappedDelegate.Parameters[parameter_index].QualifiedType)
                        {
                            p.QualifiedType = "int"; // f.WrappedDelegate.Parameters[parameter_index].QualifiedType;
                        }
                    }
                }
                string wrappedDelegateEntryPointName = funcPrefix + f.WrappedDelegate.EntryPoint;

                if (assign_statements.Count > 0)
                {
                    // Call function
                    string method_call = f.CallString(wrappedDelegateEntryPointName, f.WrappedDelegate);
                    if (f.ReturnType.CurrentType.ToLower().Contains("void"))
                    {
                        f.Body.Add(String.Format("{0};", method_call));
                    }
                    else if (func.ReturnType.CurrentType.ToLower().Contains("string"))
                    {
                        f.Body.Add(String.Format("{0} {1} = null; unsafe {{ {1} = new string((sbyte*){2}); }}",
                            func.ReturnType.QualifiedType, "retval", method_call));
                    }
                    else if (func.ReturnType.CurrentType.ToLower() == "bool")
                    {
                        f.Body.Add(String.Format("{0} {1} = {2} !=0;", f.ReturnType.QualifiedType, "retval", method_call));
                    }
                    else
                    {
                        f.Body.Add(String.Format("{0} {1} = {2};", f.ReturnType.QualifiedType, "retval", method_call));
                    }
                    // Assign out parameters
                    f.Body.AddRange(assign_statements);

                    // Return
                    if (!f.ReturnType.CurrentType.ToLower().Contains("void"))
                    {
                        f.Body.Add("return retval;");
                    }
                }
                else
                {
                    // Call function and return
                    if (f.ReturnType.CurrentType.ToLower().Contains("void"))
                    {
                        f.Body.Add(String.Format("{0};", f.CallString(wrappedDelegateEntryPointName, f.WrappedDelegate)));
                    }
                    else if (func.ReturnType.CurrentType.ToLower().Contains("string"))
                    {
                        f.Body.Add(String.Format("unsafe {{ return new string((sbyte*){0}); }}",
                            f.CallString(wrappedDelegateEntryPointName, f.WrappedDelegate)));
                    }
                    else if (func.ReturnType.CurrentType.ToLower() == "bool")
                    {
                        f.Body.Add(String.Format($"return  ({ f.CallString(wrappedDelegateEntryPointName, f.WrappedDelegate)}) != 0;"));
                    }
                    else if (func.ReturnType.IsEnum)
                    {
                        //cast enum
                        f.Body.Add(String.Format($"return ({func.ReturnType}) ({ f.CallString(wrappedDelegateEntryPointName, f.WrappedDelegate)});"));
                    }
                    else
                    {
                        f.Body.Add(String.Format("return {0};", f.CallString(wrappedDelegateEntryPointName, f.WrappedDelegate)));
                    }
                }


                // Free all allocated GCHandles
                if (handle_statements.Count > 0)
                {
                    f.Body.Unindent();
                    f.Body.Add("}");
                    f.Body.Add("finally");
                    f.Body.Add("{");
                    f.Body.Indent();

                    f.Body.AddRange(handle_release_statements);

                    f.Body.Unindent();
                    f.Body.Add("}");
                }

                if (!f.Unsafe && fixed_statements.Count > 0 || stringOutAlloc_statements.Count > 0)
                {
                    f.Body.Unindent();
                    f.Body.Add("}");
                }

                if (fixed_statements.Count > 0)
                {
                    f.Body.Unindent();
                    f.Body.Add("}");
                }

                if ((Settings.Compatibility & Settings.Legacy.NoDebugHelpers) == 0)
                {
                    if (f.TrimmedName != "GetError")
                    {
                        f.Body.Add("#if DEBUG");
                        if (f.TrimmedName == "End")
                            f.Body.Add("GraphicsContext.CurrentContext.ErrorChecking = true;");
                        f.Body.Add("}");
                        f.Body.Add("#endif");
                    }
                }

                func.Body = f.Body;
            }


        }
    }

}