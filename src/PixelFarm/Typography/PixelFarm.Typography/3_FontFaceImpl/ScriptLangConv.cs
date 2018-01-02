//MIT, 2016-2018, WinterDev  

namespace PixelFarm.Drawing.Fonts
{
    public static class ScriptLangConv
    {
        public static Typography.OpenFont.ScriptLang GetOpenFontScriptLang(string shortName)
        {
            return Typography.OpenFont.ScriptLangs.GetRegisteredScriptLang(shortName);
        }
    }
}