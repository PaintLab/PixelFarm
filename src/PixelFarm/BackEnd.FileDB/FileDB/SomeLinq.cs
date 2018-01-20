//
// Enumerable.cs
//
// Authors:
//  Marek Safar (marek.safar@gmail.com)
//  Antonello Provenzano  <antonello@deveel.com>
//  Alejandro Serrano "Serras" (trupill@yahoo.es)
//  Jb Evain (jbevain@novell.com)
//
// Copyright (C) 2007 Novell, Inc (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

// precious: http://www.hookedonlinq.com

using System;
using System.Collections.Generic;
#if NET20
namespace System.Collections.Generic
{
    public static class SomeLinqExtension
    {
        public static T[] ToArray<T>(this IEnumerable<T> source)
        {
            List<T> list = new List<T>();
            foreach (var t in source)
            {
                list.Add(t);
            }
            return list.ToArray();
        }
        public static IEnumerable<TSource> Skip<TSource>(this IEnumerable<TSource> source, int count)
        {
            return CreateSkipIterator(source, count);
        }
        static IEnumerable<TSource> CreateSkipIterator<TSource>(IEnumerable<TSource> source, int count)
        {
            var enumerator = source.GetEnumerator();
            try
            {
                while (count-- > 0)
                    if (!enumerator.MoveNext())
                        yield break;

                while (enumerator.MoveNext())
                    yield return enumerator.Current;

            }
            finally
            {
                enumerator.Dispose();
            }
        }
        public static IEnumerable<TSource> Take<TSource>(this IEnumerable<TSource> source, int count)
        {
            if (count <= 0)
                return EmptyOf<TSource>.Instance;
            return CreateTakeIterator(source, count);
        }

        static IEnumerable<TSource> CreateTakeIterator<TSource>(IEnumerable<TSource> source, int count)
        {
            int counter = 0;
            foreach (TSource element in source)
            {
                yield return element;

                if (++counter == count)
                    yield break;
            }
        }
        public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> source)
        {
            return Distinct<TSource>(source, null);
        }
        public static TSource First<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            foreach (TSource t in source)
            {
                if (predicate(t))
                {
                    return t;
                }
            }
            return default(TSource);
        }
        public static IEnumerable<TSource> Where<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            foreach (TSource t in source)
            {
                if (predicate(t))
                {
                    yield return t;
                }
            }
        }
        public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
        {

            if (comparer == null)
                comparer = EqualityComparer<TSource>.Default;

            return CreateDistinctIterator(source, comparer);
        }

        static IEnumerable<TSource> CreateDistinctIterator<TSource>(IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
        {
            Hashtable items = new Hashtable();
            foreach (var element in source)
            {
                if (!items.ContainsKey(element))
                {
                    items.Add(element, 0);
                    yield return element;
                }
            }
        }
    }
    static class EmptyOf<T>
    {
        public static readonly T[] Instance = new T[0];
    }


    public delegate R Func<T, R>(T t);
}
#endif