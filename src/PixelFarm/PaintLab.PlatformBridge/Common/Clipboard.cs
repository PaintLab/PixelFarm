//Apache2, 2014-present, WinterDev

using System.Collections.Generic;

namespace LayoutFarm.UI
{

    /// <summary>
    /// clipboard services
    /// </summary>
    public static class Clipboard
    {
        //TODO: review this again, ***

        static UIPlatform s_currentUIPlatform;
        public static void Clear() => s_currentUIPlatform.ClearClipboardData();

        public static void SetText(string text) => s_currentUIPlatform.SetClipboardText(text);

        public static bool ContainsUnicodeText() => s_currentUIPlatform.ContainsClipboardData("text");

        public static bool ContainsImage() => s_currentUIPlatform.ContainsClipboardData("image");

        public static bool ContainsFileDrops() => s_currentUIPlatform.ContainsClipboardData("filedrops");

        public static string GetUnicodeText() => s_currentUIPlatform.GetClipboardText();

        public static void SetUIPlatform(UIPlatform uiPlatform) => s_currentUIPlatform = uiPlatform;

        public static IEnumerable<string> GetFileDropList() => s_currentUIPlatform.GetClipboardFileDropList();
        public static void SetFileDropList(string[] filedrops) => s_currentUIPlatform.SetClipboardFileDropList(filedrops);

        public static PixelFarm.Drawing.Image GetImage() => s_currentUIPlatform.GetClipboardImage();

        public static void SetImage(PixelFarm.Drawing.Image img) => s_currentUIPlatform.SetClipboardImage(img);
    }

}