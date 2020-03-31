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
    public struct YogaValue: IEquatable<YogaValue>
    {
        public static readonly YogaValue Zero = new YogaValue { Value = 0, Unit = YogaUnit.Point };
        public static readonly YogaValue Undefined = new YogaValue { Value = null, Unit = YogaUnit.Undefined };
        public static readonly YogaValue Auto = new YogaValue { Value = null, Unit = YogaUnit.Auto };

        public float? Value;
        public YogaUnit Unit;

        public override string ToString()
        {
            if (Unit == YogaUnit.Undefined)
                return "undefined";

            if (Unit == YogaUnit.Auto)
                return "auto";

            if (Unit == YogaUnit.Point)
                return $"{Value} pt";

            return $"{Value} %";
        }

        public float? Resolve(float? ownerSize)
        {
            switch (Unit)
            {
                case YogaUnit.Undefined:
                case YogaUnit.Auto:
                    return null;
                case YogaUnit.Point:
                    return Value;
                case YogaUnit.Percent:
                    return Value * ownerSize * 0.01f;
            }

            return null;
        }

        public static YogaValue Percent(float percentValue)
        {
            return new YogaValue() { Unit = YogaUnit.Percent, Value = percentValue };
        }

        public static YogaValue Point(float pointValue)
        {
            return new YogaValue() { Unit = YogaUnit.Point, Value = pointValue };
        }

        public bool Equals(YogaValue b)
        {
            if (Unit != b.Unit)
                return false;

            if (Unit == YogaUnit.Undefined || (Value == null && b.Value == null))
                return true;

            if (Value == null && b.Value == null)
                return true;

            if (Value == null || b.Value == null)
                return false;

            return Math.Abs(Value.Value - b.Value.Value) < 0.0001f;
        }

        public static implicit operator YogaValue(float pointValue)
        {
            return Point(pointValue);
        }
    }
}
