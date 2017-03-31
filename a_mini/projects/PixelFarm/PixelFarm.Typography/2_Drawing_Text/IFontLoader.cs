using Typography.Rendering;
namespace PixelFarm.Drawing.Fonts
{
    public interface IFontLoader
    {
        InstalledFont GetFont(string fontName, InstalledFontStyle style);
    }
}