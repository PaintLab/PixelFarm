//Apache2, 2014-present, WinterDev

using System;
namespace LayoutFarm.UI
{
    public interface IScrollable
    {
        void SetViewport(int x, int y, object reqBy);
        int ViewportX { get; }
        int ViewportY { get; }
        int ViewportWidth { get; }
        int ViewportHeight { get; }
        int DesiredHeight { get; }
        int DesiredWidth { get; }
        event EventHandler ViewportChanged;
        event EventHandler LayoutFinished;
    }
}