
//for .NET 2.0 
namespace System
{
    //public delegate R Func<R>();
    //public delegate R Func<T, R>(T t1);
    //public delegate R Func<T1, T2, R>(T1 t1, T2 t2);
    //public delegate R Func<T1, T2, T3, R>(T1 t1, T2 t2, T3 t3);
    public delegate void Action<in T1, in T2>(T1 arg1, T2 arg2);
    public delegate void Action<in T1, in T2, in T3>(T1 arg1, T2 arg2, T3 arg3);
    public delegate void Action<in T1, in T2, in T3, in T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    public delegate void Action<in T1, in T2, in T3, in T4, in T5>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    public delegate void Action<in T1, in T2, in T3, in T4, in T5, in T6>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
    public delegate void Action<in T1, in T2, in T3, in T4, in T5, in T6, in T7>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
    public delegate void Action<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);

#if !NETSTANDARD
    public delegate TResult Func<out TResult>();
    public delegate TResult Func<in T, out TResult>(T arg);
    public delegate TResult Func<in T1, in T2, out TResult>(T1 arg1, T2 arg2);
    public delegate TResult Func<in T1, in T2, in T3, out TResult>(T1 arg1, T2 arg2, T3 arg3);
    public delegate TResult Func<in T1, in T2, in T3, in T4, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    public delegate TResult Func<in T1, in T2, in T3, in T4, in T5, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    public delegate TResult Func<in T1, in T2, in T3, in T4, in T5, in T6, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
    public delegate TResult Func<in T1, in T2, in T3, in T4, in T5, in T6, in T7, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7);
    public delegate TResult Func<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8);
#endif
}
namespace System.Runtime.InteropServices
{
    public partial class TargetedPatchingOptOutAttribute : Attribute
    {
        public TargetedPatchingOptOutAttribute(string msg) { }
    }
}
namespace System.Runtime.CompilerServices
{
    public partial class ExtensionAttribute : Attribute { }
}

namespace System.Collections.Generic
{
    public class HashSet<T> : IEnumerable<T>
    {
        //for .NET 2.0
        Dictionary<int, T> _dic = new Dictionary<int, T>();
        public void Add(T data)
        {
            _dic[data.GetHashCode()] = data;
        }
        public bool Remove(T data)
        {
            return _dic.Remove(data.GetHashCode());
        }
        public bool Contains(T data)
        {
            return _dic.ContainsKey(data.GetHashCode());
        }
        public void Clear()
        {
            _dic.Clear();
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (T t in _dic.Values)
            {
                yield return t;
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (T t in _dic.Values)
            {
                yield return t;
            }
        }
        public int Count => _dic.Count;
    }

    public static class MyLinq
    {
        public static System.Collections.Generic.IEnumerable<Output> Select<TSource, Output>(this System.Collections.Generic.IEnumerable<TSource> source, Func<TSource, Output> func)
        {
            foreach (TSource t in source)
            {
                yield return func(t);
            }
        }
        public static System.Collections.Generic.IEnumerable<TSource> Skip<TSource>(this System.Collections.Generic.IEnumerable<TSource> source, int count)
        {
            int c = 0;
            foreach (TSource t in source)
            {
                c++;
                if (c < count)
                {
                    continue;
                }
                yield return t;
            }
        }
        public static System.Collections.Generic.IEnumerable<TSource> Concat<TSource>(this System.Collections.Generic.IEnumerable<TSource> first,
            System.Collections.Generic.IEnumerable<TSource> second)
        {
            foreach (TSource t in first)
            {
                yield return t;
            }
            foreach (TSource t in second)
            {
                yield return t;
            }
        }
        public static int Count<T>(this IEnumerable<T> list)
        {
            int count = 0;
            foreach (T t in list)
            {
                count++;
            }
            return count;
        }
        public static List<T> ToList<T>(this IEnumerable<T> list)
        {
            return new List<T>(list);
        }
        public static bool Any<T>(this IEnumerable<T> list)
        {

            foreach (T t in list)
            {
                return true;
            }
            return false;
        }
        public static T FirstOrDefault<T>(this IEnumerable<T> list, Func<T, bool> predicate)
        {
            foreach (T t in list)
            {
                if (predicate(t))
                {
                    return t;
                }
            }
            return default(T);
        }
        public static T First<T>(this IEnumerable<T> list)
        {

            if (list is T[] arr)
            {
                if (arr.Length > 0)
                {
                    return arr[0];
                }
                else
                {
                    return default(T);
                }

            }
            else if (list is List<T> list2)
            {
                if (list2.Count > 0)
                {
                    return list2[0];
                }
                else
                {
                    return default(T);
                }
            }
            else if (list is LinkedList<T> linkedlist)
            {
                if (linkedlist.Count > 0)
                {
                    return linkedlist.First.Value;
                }
                else
                {
                    return default(T);
                }
            }
            else
            {
                T lastOne = default(T);
                foreach (T t in list)
                {
                    return t;
                }
                return lastOne;
            }

        }
        public static T Last<T>(this IEnumerable<T> list)
        {
            if (list is T[] arr)
            {
                if (arr.Length > 0)
                {
                    return arr[arr.Length - 1];
                }
                else
                {
                    return default(T);
                }

            }
            else if (list is List<T> list2)
            {
                if (list2.Count > 0)
                {
                    return list2[list2.Count - 1];
                }
                else
                {
                    return default(T);
                }
            }
            else if (list is LinkedList<T> linkedlist)
            {
                if (linkedlist.Count > 0)
                {
                    return linkedlist.Last.Value;
                }
                else
                {
                    return default(T);
                }
            }
            else
            {
                T lastOne = default(T);
                foreach (T t in list)
                {
                    lastOne = t;
                }
                return lastOne;
            }
        }
        public static IEnumerable<T> Where<T>(this IEnumerable<T> list, Func<T, bool> predicate)
        {
            foreach (T t in list)
            {
                if (predicate(t))
                    yield return t;
            }
        }
        public static bool Contains<T>(this IEnumerable<T> list, T value)
        {
            foreach (T t in list)
            {
                if (value.Equals(t))
                {
                    return true;
                }
            }
            return false;
        }
        public static T[] ToArray<T>(this IEnumerable<T> list)
        {
            List<T> list2 = new List<T>();
            foreach (T t in list)
            {
                list2.Add(t);
            }
            return list2.ToArray();
        }

    }

    public static class EnumerableEmpty<T>
    {
        static readonly T[] s_empty = new T[0];
        public static IEnumerable<T> Empty() => s_empty;
    }

}

