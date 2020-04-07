//Apache2, 2014-present, WinterDev

namespace LayoutFarm.UI
{
    public static class ClipboardService
    {
        static ClipboardDataProvider s_provider;
        public static ClipboardDataProvider Provider => s_provider;
    }
    public abstract class ClipboardDataProvider
    {
        public abstract bool ContainsImage();
        public abstract bool ContainsText();
        public abstract bool ContainsFileDropList();
        public abstract void Clear();
        public abstract PixelFarm.Drawing.Image GetImage();
        public abstract string GetText();
        public abstract System.Collections.Generic.List<string> GetFileDropList();
    }
}