//Apache2, 2019s-present, WinterDev

using System;
using PixelFarm.Drawing;
namespace LayoutFarm.TextEditing
{
    public interface ITextFlowLayerOwner
    {
        int Width { get; }
        void ClientLayerBubbleUpInvalidateArea(Rectangle clientInvalidatedArea);
    }

}