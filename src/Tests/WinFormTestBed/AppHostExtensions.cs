//Apache2, 2014-present, WinterDev

using LayoutFarm.UI;
namespace LayoutFarm
{
    public static class AppHostExtensions
    {
        public static void AddChild(this AppHost viewport, UIElement ui)
        {
            viewport.ViewportControl.AddChild(
                ui.GetPrimaryRenderElement(viewport.ViewportControl.RootGfx),
                ui);
        }
    }
}