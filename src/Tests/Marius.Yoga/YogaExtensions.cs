/**
 * Copyright (c) 2014-present, Facebook, Inc.
 * Copyright (c) 2018-present, Marius Klimantavičius
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

//https://github.com/marius-klimantavicius/yoga

namespace LayoutFarm.MariusYoga
{
    public static class YogaExtensions
    {
        public static YogaValue Percent(this float value)
        {
            return YogaValue.Percent(value);
        }

        public static YogaValue Pt(this float value)
        {
            return YogaValue.Point(value);
        }

        public static YogaValue Percent(this int value)
        {
            return YogaValue.Percent(value);
        }

        public static YogaValue Pt(this int value)
        {
            return YogaValue.Point(value);
        }

        public static bool IsRow(this YogaFlexDirection flexDirection)
        {
            return flexDirection == YogaFlexDirection.Row || flexDirection == YogaFlexDirection.RowReverse;
        }

        public static bool IsColumn(this YogaFlexDirection flexDirection)
        {
            return flexDirection == YogaFlexDirection.Column || flexDirection == YogaFlexDirection.ColumnReverse;
        }

        public static YogaFlexDirection ResolveFlexDirection(this YogaFlexDirection flexDirection, YogaDirection direction)
        {
            if (direction == YogaDirection.RightToLeft)
            {
                if (flexDirection == YogaFlexDirection.Row)
                    return YogaFlexDirection.RowReverse;
                else if (flexDirection == YogaFlexDirection.RowReverse)
                    return YogaFlexDirection.Row;
            }

            return flexDirection;
        }

        public static YogaFlexDirection FlexDirectionCross(this YogaFlexDirection flexDirection, YogaDirection direction)
        {
            return flexDirection.IsColumn()
                ? YogaFlexDirection.Row.ResolveFlexDirection(direction)
                : YogaFlexDirection.Column;
        }
    }
}
