//Apache2, 2014-present, WinterDev

namespace LayoutFarm.UI
{
    public static class Clipboard
    {
        static UIPlatform s_currentUIPlatform;
        public static void Clear()
        {
        }
        public static void SetText(string text)
        {
            //textdata = text;
            s_currentUIPlatform.SetClipboardData(text);
        }
        public static bool ContainUnicodeText()
        {
            return s_currentUIPlatform.GetClipboardData() != null;
        }
        public static string GetUnicodeText()
        {
            return s_currentUIPlatform.GetClipboardData();
        }

        public static void SetUIPlatform(UIPlatform uiPlatform)
        {
            s_currentUIPlatform = uiPlatform;
        }
    }
}