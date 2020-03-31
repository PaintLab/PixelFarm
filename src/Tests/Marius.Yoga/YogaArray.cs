/**
 * Copyright (c) 2014-present, Facebook, Inc.
 * Copyright (c) 2018-present, Marius Klimantavičius
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

using System;

namespace Marius.Yoga
{
    public static class YogaArray
    {
        public static YogaArray<T> From<T>(T[] other)
        {
            var result = new T[other.Length];
            for (var i = 0; i < result.Length; i++)
                result[i] = other[i];
            return new YogaArray<T>(result);
        }

        public static YogaArray<T> From<T>(YogaArray<T> other)
        {
            var result = new T[other.Length];
            for (var i = 0; i < result.Length; i++)
                result[i] = other[i];
            return new YogaArray<T>(result);
        }

        public static bool Equal(YogaArray<float> val1, YogaArray<float> val2)
        {
            var areEqual = true;
            for (var i = 0; i < val1.Length && areEqual; ++i)
                areEqual = FloatsEqual(val1[i], val2[i]);

            return areEqual;
        }

        public static bool Equal(YogaArray<float?> val1, YogaArray<float?> val2)
        {
            var areEqual = true;
            for (var i = 0; i < val1.Length && areEqual; ++i)
                areEqual = FloatsEqual(val1[i], val2[i]);

            return areEqual;
        }

        public static bool Equal(YogaArray<YogaValue> val1, YogaArray<YogaValue> val2)
        {
            var areEqual = true;
            for (var i = 0; i < val1.Length && areEqual; ++i)
                areEqual = val1[i].Equals(val2[i]);

            return areEqual;
        }

        private static bool FloatsEqual(float? a, float? b)
        {
            if (a != null && b != null)
                return Math.Abs(a.Value - b.Value) < 0.0001f;

            return a == null && b == null;
        }
    }

    public class YogaArray<T>
    {
        private readonly T[] _array;

        public T this[int index]
        {
            get { return _array[index]; }
            set { _array[index] = value; }
        }

        public T this[YogaEdge index]
        {
            get { return _array[(int)index]; }
            set { _array[(int)index] = value; }
        }

        public T this[YogaFlexDirection index]
        {
            get { return _array[(int)index]; }
            set { _array[(int)index] = value; }
        }

        public T this[YogaDimension index]
        {
            get { return _array[(int)index]; }
            set { _array[(int)index] = value; }
        }

        public int Length
        {
            get { return _array.Length; }
        }

        public YogaArray(int length)
        {
            _array = new T[length];
        }

        public YogaArray(params T[] values)
        {
            _array = values;
        }

        public void CopyFrom(YogaArray<T> from)
        {
            from._array.CopyTo(_array, 0);
        }

        public void Clear()
        {
            for (var i = 0; i < _array.Length; i++)
                _array[i] = default(T);
        }

        public override int GetHashCode()
        {
            if (_array == null)
                return 0;

            var hashCode = 0;
            for (var i = 0; i < _array.Length; i++)
                hashCode = (hashCode << 5) ^ _array[i].GetHashCode();

            return hashCode;
        }
    }
}
