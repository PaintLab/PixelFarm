//Apache2, 2019s-present, WinterDev

using System;
using PixelFarm.Drawing;
namespace LayoutFarm.TextFlow
{
    public interface ITextFlowLayerOwner
    {
        int Width { get; }
        int Height { get; }
        void ClientLayerBubbleUpInvalidateArea(Rectangle clientInvalidatedArea);
    }

}