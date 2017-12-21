// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// (from https://github.com/wieslawsoltes/ColorBlender)

namespace ColorBlender
{
    public class Blend
    {
        public HSV[] Colors { get; set; }

        public Blend()
        {
            Colors = new HSV[6];
        }
    }
}
