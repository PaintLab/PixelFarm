/**
 * Copyright (c) 2014-present, Facebook, Inc.
 * Copyright (c) 2018-present, Marius Klimantavičius
 *
 * This source code is licensed under the MIT license found in the
 * LICENSE file in the root directory of this source tree.
 */

namespace Marius.Yoga
{
    public struct YogaSize
    {
        public float? Width;
        public float? Height;

        public static YogaSize From(float? width, float? height)
        {
            return new YogaSize()
            {
                Width = width,
                Height = height,
            };
        }
    }
}
