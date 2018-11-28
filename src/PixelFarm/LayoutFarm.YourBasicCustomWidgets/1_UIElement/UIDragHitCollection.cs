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
            this._hitArea = hitArea;
            this._hitList = hitList;
        }
        public Rectangle HitArea
        {
            get { return this._hitArea; }
        }
    }
}