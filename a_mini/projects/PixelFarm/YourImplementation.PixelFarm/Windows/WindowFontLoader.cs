//MIT, 2014-2017, WinterDev   

using PixelFarm.Drawing.Fonts;
using Typography.Rendering;
namespace PixelFarm.Drawing
{
    class WindowsFontLoader : IFontLoader
    {
        FontNotFoundHandler fontNotFoundHandler;
        InstalledFontCollection installFontCollection = new InstalledFontCollection();
        FontNotFoundHandler _defaultFontNotFoundHandler;
        public WindowsFontLoader()
        {
            //iterate
            var installFontsWin32 = new InstallFontsProviderWin32();
            installFontCollection.LoadWindowsSystemFonts();
            _defaultFontNotFoundHandler = (fontCollection, fontName, subfamName, style) =>
            {
                //TODO: implement font not found mapping here
                //_fontsMapping["monospace"] = "Courier New";
                //_fontsMapping["Helvetica"] = "Arial";
                fontName = fontName.ToUpper();
                switch (fontName)
                {
                    case "MONOSPACE":
                        return fontCollection.GetFont("Courier New", style);
                    case "HELVETICA":
                        return fontCollection.GetFont("Arial", style);
                    case "TAHOMA":
                        //default font must found
                        //if not throw err 
                        //this prevent infinit loop
                        throw new System.NotSupportedException();
                    default:
                        return fontCollection.GetFont("tahoma", style);
                }
            };

        }
        public InstalledFont GetFont(string fontName, InstalledFontStyle style)
        {
            InstalledFont found = installFontCollection.GetFont(fontName, style);
            if (found == null)
            {
                //not found
                if (fontNotFoundHandler != null)
                {
                    return fontNotFoundHandler(installFontCollection, fontName, null, style);
                }
                else
                {
                    return _defaultFontNotFoundHandler(installFontCollection, fontName, null, style);
                }

            }
            return found;
        }
        public void SetFontNotFoundHandler(FontNotFoundHandler fontNotFoundHandler)
        {
            this.fontNotFoundHandler = fontNotFoundHandler;
        }


    }
}