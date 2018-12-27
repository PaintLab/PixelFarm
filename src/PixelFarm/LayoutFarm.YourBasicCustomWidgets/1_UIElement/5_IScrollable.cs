//Apache2, 2014-present, WinterDev

using System;
namespace LayoutFarm.UI
{
    public interface IScrollable
    {
        void SetViewport(int left, int top, object reqBy);
        int ViewportLeft { get; }
        int ViewportTop { get; }
        int ViewportWidth { get; }
        int ViewportHeight { get; }
        int InnerHeight { get; }
        int InnerWidth { get; }
        event EventHandler ViewportChanged;
        event EventHandler LayoutFinished;
    }
}