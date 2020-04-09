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
    public delegate YogaSize YogaMeasure(YogaNode node, float? width, YogaMeasureMode widthMode, float? height, YogaMeasureMode heightMode);
    public delegate float YogaBaseline(YogaNode node, float? width, float? height);
    public delegate void YogaDirtied(YogaNode node);
    public delegate void YogaPrint(YogaNode node);
    public delegate void YogaNodeCloned(YogaNode oldNode, YogaNode newNode, YogaNode owner, int childIndex);
}
