/* Copyright (c) 2006, 2007 Stefanos Apostolopoulos
 * See license.txt for license info
 */

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Bind.Structures
{
    /// <summary>
    /// Represents a single parameter of an opengl function.
    /// </summary>
    internal class Parameter : Type, IComparable<Parameter>, IEquatable<Parameter>
    {
        private string cache;
#if DEBUG
        static int dbugTotalId;
        public readonly int dbugId = dbugTotalId++;
#endif
        /// <summary>
        /// Creates a new Parameter without type and name.
        /// </summary>
        public Parameter()
        {
#if DEBUG
            if (dbugId == 13631 || dbugId == 13655)
            {

            }
#endif
        }

        /// <summary>
        /// Creates a new parameter from the parameters passed (deep copy).
        /// </summary>
        /// <param name="p">The parameter to copy from.</param>
        public Parameter(Parameter p)
            : base(p)
        {

#if DEBUG
            if (dbugId == 13631 || dbugId == 13655)
            {

            }
#endif
            if (p == null)
            {
                return;
            }

            Name = p.Name;
            Unchecked = p.Unchecked;
            UnmanagedType = p.UnmanagedType;
            Generic = p.Generic;
            Flow = p.Flow;
            cache = p.cache;
            ComputeSize = p.ComputeSize;
            //this.rebuild = false;
        }

        /// <summary>
        /// Gets or sets the raw name of the parameter.
        /// </summary>
        public string RawName
        {
            get;
            private set;
        }

        public string TempOutStringName { get; set; }
        public string ExplicitCastType { get; set; }
        /// <summary>
        /// Gets the name of the parameter. If the name matches a keyword of the current language,
        /// then it is escaped with <see cref="Settings.KeywordEscapeCharacter"/>.
        /// </summary>
        public string Name
        {
            get
            {
                return RawName;
            }
            set
            {
                if (RawName != value)
                {
                    while (value.StartsWith("*"))
                    {
                        Pointer++;
                        value = value.Substring(1);
                    }
                    RawName = value;
                }
            }
        }

        private UnmanagedType _unmanaged_type;
        /// <summary>
        /// Gets or sets the name of the parameter.
        /// </summary>
        private UnmanagedType UnmanagedType
        {
            get { return _unmanaged_type; }
            set
            {
                if (_unmanaged_type != value)
                {
                    _unmanaged_type = value;
                }
            }
        }

        private FlowDirection _flow;

        /// <summary>
        /// Gets or sets the flow of the parameter.
        /// </summary>
        public FlowDirection Flow
        {
            get { return _flow; }
            set
            {
                if (_flow != value)
                {
                    _flow = value;
                }
            }
        }

        public bool NeedsPin
        {
            get
            {
                return (Array > 0 || Reference || CurrentType == "object") &&
                        !CurrentType.ToLower().Contains("string");
            }
        }

        private bool _unchecked;

        public bool Unchecked
        {
            get { return _unchecked; }
            set
            {
                if (_unchecked != value)
                {
                    _unchecked = value;
                }
            }
        }

        public bool Generic { get; set; }

        // Returns true if this parameter differs only on reference compared to another parameter, i.e:
        // returns true for 'int' & 'ref int'
        // returns true for 'ref float' & 'float'
        // returns false 'int' & 'int*'
        // returns false 'int' & 'int[]'
        // returns false 'int' & 'float'
        public bool DiffersOnlyOnReference(Parameter other)
        {
            return
                CurrentType == other.CurrentType &&
                (Reference && !(other.Reference || other.Array > 0 || other.Pointer != 0) ||
                other.Reference && !(Reference || Array > 0 || Pointer != 0));
        }

        public string ComputeSize { get; set; }

        // Returns the FlowDirection that matches the specified string
        // ("out" or "in", otherwise undefined).
        public static FlowDirection GetFlowDirection(string direction)
        {
            return direction == "out" ? FlowDirection.Out : direction == "in" ? FlowDirection.In : FlowDirection.Undefined;
        }


        public int CompareTo(Parameter other)
        {
            int result = base.CompareTo(other);
            if (result == 0)
            {
                result = Name.CompareTo(other.Name);
            }
            return result;
        }

        public override string ToString()
        {
            return String.Format("{2}{0} {1}",
                base.ToString(),
                Name,
                Reference ?
                    Flow == FlowDirection.Out ? "out " : "ref " :
                    String.Empty);
        }

        public bool Equals(Parameter other)
        {
            bool result =
                base.Equals(other as Type) &&
                Name.Equals(other.Name);

            return result;
        }
    }

    /// <summary>
    /// Holds the parameter list of an opengl function.
    /// </summary>
    internal class ParameterCollection : IList<Parameter>, IComparable<ParameterCollection>, IEquatable<ParameterCollection>
    {
        private readonly List<Parameter> Parameters = new List<Parameter>();

        private bool hasPointerParameters;
        private bool hasReferenceParameters;
        private bool hasUnsignedParameters;
        private bool hasGenericParameters;

        public bool Rebuild { get; set; }
        private Settings Settings { get; set; }

        public ParameterCollection()
        {
        }

        public ParameterCollection(ParameterCollection pc)
        {
            foreach (Parameter p in pc)
            {
                Add(new Parameter(p));
            }
        }

        public ParameterCollection(IEnumerable<Parameter> parameters)
        {
            foreach (Parameter p in parameters)
            {
                Add(new Parameter(p));
            }
        }

        private void BuildCache()
        {
            BuildReferenceAndPointerParametersCache();
            BuildCallStringCache();
            Rebuild = false;
        }

        public bool HasPointerParameters
        {
            get
            {
                if (Rebuild)
                {
                    BuildCache();
                }

                return hasPointerParameters;
            }
        }

        public bool HasReferenceParameters
        {
            get
            {
                if (Rebuild)
                {
                    BuildCache();
                }

                return hasReferenceParameters;
            }
        }

        public bool HasUnsignedParameters
        {
            get
            {
                if (Rebuild)
                {
                    BuildCache();
                }

                return hasUnsignedParameters;
            }
        }

        public bool HasGenericParameters
        {
            get
            {
                if (Rebuild)
                {
                    BuildCache();
                }

                return hasGenericParameters;
            }
        }
        /// <summary>
        /// Builds a call string instance and caches it.
        /// </summary>
        private string BuildCallStringCache2(Delegate forOutput)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("(");

            if (Count > 0)
            {
                int p_index = 0;


                foreach (Parameter p in this)
                {

                    //
                    if (p.Unchecked)
                        sb.Append("unchecked((" + p.QualifiedType + ")");

                    if (!p.Generic && p.CurrentType != "object")
                    {
                        if (p.ExplicitCastType != null)
                        {
                            sb.Append(String.Format("({0})", p.ExplicitCastType));
                        }
                        else if (p.CurrentType.ToLower().Contains("string"))
                        {
                            if (p.TempOutStringName != null)
                            {
                                sb.Append(String.Format("({0}{1})",
                                  "IntPtr", (p.Array > 0) ? "[]" : ""));
                            }
                            else
                            {
                                sb.Append(String.Format("({0}{1})",
                                p.QualifiedType, (p.Array > 0) ? "[]" : ""));
                            }

                        }
                        else if (p.CurrentType.ToLower() == "int")
                        {
                            sb.Append(String.Format("({0})", p.QualifiedType));
                        }
                        else if (p.IndirectionLevel != 0)
                        {
                            if (((Settings.Legacy.TurnVoidPointersToIntPtr) != Settings.Legacy.None) &&
                                p.Pointer != 0 && p.CurrentType.Contains("void"))
                                sb.Append("(IntPtr)");
                            else
                            {
                                Parameter targetP = forOutput.Parameters[p_index];

                                sb.Append("(");
                                //sb.Append(p.QualifiedType);

                                sb.Append(targetP.QualifiedType);


                                for (int i = 0; i < p.IndirectionLevel; i++)
                                    sb.Append("*");
                                sb.Append(")");
                            }
                        }
                        else
                        {
                            Parameter targetP = forOutput.Parameters[p_index];
                            //sb.Append(String.Format("({0})", p.QualifiedType));
                            sb.Append(String.Format("({0})", targetP.QualifiedType));
                        }
                    }

                    if (p.TempOutStringName != null)
                    {
                        sb.Append(p.TempOutStringName);
                    }
                    else
                    {
                        sb.Append(p.Name);
                    }

                    if (p.Unchecked)
                        sb.Append(")");

                    sb.Append(", ");

                    //

                    p_index++;
                }
                sb.Replace(", ", ")", sb.Length - 2, 2);
            }
            else
            {
                sb.Append(")");
            }

            return sb.ToString();
        }
        /// <summary>
        /// Builds a call string instance and caches it.
        /// </summary>
        private void BuildCallStringCache()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("(");

            if (Count > 0)
            {
                foreach (Parameter p in this)
                {
                    if (p.Unchecked)
                        sb.Append("unchecked((" + p.QualifiedType + ")");

                    if (!p.Generic && p.CurrentType != "object")
                    {
                        if (p.CurrentType.ToLower().Contains("string"))
                        {
                            sb.Append(String.Format("({0}{1})",
                                p.QualifiedType, (p.Array > 0) ? "[]" : ""));
                        }
                        else if (p.IndirectionLevel != 0)
                        {
                            if (((Settings.Legacy.TurnVoidPointersToIntPtr) != Settings.Legacy.None) &&
                                p.Pointer != 0 && p.CurrentType.Contains("void"))
                                sb.Append("(IntPtr)");
                            else
                            {
                                sb.Append("(");
                                sb.Append(p.QualifiedType);
                                for (int i = 0; i < p.IndirectionLevel; i++)
                                    sb.Append("*");
                                sb.Append(")");
                            }
                        }
                        else
                        {
                            sb.Append(String.Format("({0})", p.QualifiedType));
                        }
                    }

                    sb.Append(p.Name);

                    if (p.Unchecked)
                        sb.Append(")");

                    sb.Append(", ");
                }
                sb.Replace(", ", ")", sb.Length - 2, 2);
            }
            else
            {
                sb.Append(")");
            }

            callStringCache = sb.ToString();
        }
        private void BuildReferenceAndPointerParametersCache()
        {
            foreach (Parameter p in this)
            {
                if (p.Pointer != 0 || p.CurrentType.Contains("IntPtr"))
                {
                    hasPointerParameters = true;
                }

                if (p.Reference)
                {
                    hasReferenceParameters = true;
                }

                if (p.Unsigned)
                {
                    hasUnsignedParameters = true;
                }

                if (p.Generic)
                {
                    hasGenericParameters = true;
                }
            }
        }

        string callStringCache = String.Empty;
        public string CallString(Delegate forOutputDel)
        {
            return BuildCallStringCache2(forOutputDel);

            //if (!Rebuild)
            //{
            //    return callStringCache;
            //}
            //else
            //{
            //    BuildCache();
            //    return callStringCache;
            //}
        }

        // Only use for debugging, not for code generation!
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            if (Count > 0)
            {
                foreach (Parameter p in this)
                {
                    sb.Append(p.ToString());
                    sb.Append(", ");
                }
                sb.Replace(", ", ")", sb.Length - 2, 2);
            }
            else
            {
                sb.Append(")");
            }

            return sb.ToString();
        }

        public bool ContainsType(string type)
        {
            foreach (Parameter p in this)
            {
                if (p.CurrentType == type)
                {
                    return true;
                }
            }
            return false;
        }

        public void Add(Parameter p)
        {
            Parameters.Add(p);
            Rebuild = true;
        }

        public void Clear()
        {
            Parameters.Clear();
            Rebuild = true;
        }

        public bool Contains(Parameter item)
        {
            return Parameters.Contains(item);
        }

        public void CopyTo(Parameter[] array, int arrayIndex)
        {
            Parameters.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return Parameters.Count; }
        }

        public bool IsReadOnly
        {
            get { return (Parameters as ICollection<Parameter>).IsReadOnly; }
        }

        public bool Remove(Parameter item)
        {
            var result = Parameters.Remove(item);
            if (result)
            {
                Rebuild = true;
            }
            return result;
        }

        public IEnumerator<Parameter> GetEnumerator()
        {
            return Parameters.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Parameters.GetEnumerator();
        }

        public int IndexOf(Parameter item)
        {
            return Parameters.IndexOf(item);
        }

        public void Insert(int index, Parameter item)
        {
            Parameters.Insert(index, item);
            Rebuild = true;
        }

        public void RemoveAt(int index)
        {
            Parameters.RemoveAt(index);
            Rebuild = true;
        }

        public Parameter this[int index]
        {
            get
            {
                return Parameters[index];
            }
            set
            {
                Parameters[index] = value;
            }
        }

        public int CompareTo(ParameterCollection other)
        {
            if (Count < other.Count)
            {
                return -1;
            }
            else if (Count > other.Count)
            {
                return 1;
            }
            else
            {
                for (int i = 0; i < Count; i++)
                {
                    int result = this[i].CompareTo(other[i]);
                    if (result != 0)
                    {
                        return result;
                    }
                }
                return 0;
            }
        }

        public bool Equals(ParameterCollection other)
        {
            if (Count != other.Count)
            {
                return false;
            }

            bool result = true;
            for (int i = 0; i < Count && result; i++)
            {
                result &= this[i].Equals(other[i]);
            }
            return result;
        }
    }
}
