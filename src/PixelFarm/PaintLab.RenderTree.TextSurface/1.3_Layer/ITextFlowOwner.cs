//Apache2, 2019s-present, WinterDev

using System;
using PixelFarm.Drawing;
namespace LayoutFarm.TextEditing
{
    public interface ITextFlowLayerOwner
    {
        void ClientLayerBubbleUpInvalidateArea(Rectangle clientInvalidatedArea);
    }

}