//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
namespace LayoutFarm.UI
{
    public class UIDragHitCollection
    {
        List<UIElement> _hitList;
        Rectangle _hitArea;
        public UIDragHitCollection(List<UIElement> hitList, Rectangle hitArea)
        {
            _hitArea = hitArea;
            _hitList = hitList;
        }
        public Rectangle HitArea => _hitArea;
    }
}