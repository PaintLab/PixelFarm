//Apache2, 2014-present, WinterDev

using LayoutFarm.UI;
namespace LayoutFarm
{
    public static class AppHostExtensions
    {
        public static void AddChild(this AppHost appHost, UIElement ui)
        {
            appHost.AddChild(ui.GetPrimaryRenderElement(appHost.RootGfx)); 
        }
    }
}