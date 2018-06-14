//Apache2, 2014-present, WinterDev

using LayoutFarm.UI;
namespace LayoutFarm
{
    public static class SampleViewportExtension
    {
        public static void AddChild(this SampleViewport viewport, UIElement ui)
        {
            viewport.ViewportControl.AddChild(
                ui.GetPrimaryRenderElement(viewport.ViewportControl.RootGfx),
                ui);
        }
        
    }
}