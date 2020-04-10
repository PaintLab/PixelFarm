//Apache2, 2014-present, WinterDev

namespace LayoutFarm.UI
{

    /// <summary>
    /// clipboard services
    /// </summary>
    public static class Clipboard
    {
        //TODO: review this again, ***

        static UIPlatform s_currentUIPlatform;
        public static void Clear()
        {
            s_currentUIPlatform.ClearClipboardData();
        }
        public static void SetText(string text)
        {
            //textdata = text;
            s_currentUIPlatform.SetClipboardData(text);
        }
        public static bool ContainsUnicodeText()
        {
            //TODO: review here
            return s_currentUIPlatform.ContainsClipboardData("text");
        }
        public static bool ContainsImage()
        {
            //TODO: review here
            return s_currentUIPlatform.ContainsClipboardData("image");
        }
        public static bool ContainsFileDrops()
        {
            return s_currentUIPlatform.ContainsClipboardData("filedrops");
        }
        public static string GetUnicodeText()
        {
            //TODO: review here
            return s_currentUIPlatform.GetClipboardText();
        }
        public static void SetUIPlatform(UIPlatform uiPlatform)
        {   
            //TODO: review here
            s_currentUIPlatform = uiPlatform;
        }
        public static System.Collections.Generic.List<string> GetFileDropList()
        {
            return s_currentUIPlatform.GetClipboardFileDropList();
        }
        public static PixelFarm.Drawing.Image GetImage()
        {
            return s_currentUIPlatform.GetClipboardImage();
        }
        public static void SetImage(PixelFarm.Drawing.Image img)
        {
            s_currentUIPlatform.SetClipboardImage(img);
        }
    }

}