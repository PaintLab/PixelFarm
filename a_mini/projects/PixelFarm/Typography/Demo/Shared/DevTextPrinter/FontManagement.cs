//MIT, 2016-2017, WinterDev 
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Typography.OpenFont;

namespace Typography.FontManagement
{
    public interface IInstalledFontProvider
    {
        IEnumerable<string> GetInstalledFontIter();
    }
    public interface IFontLoader
    {
        InstalledFont GetFont(string fontName, InstalledFontStyle style);
    }


    public interface FontStreamSource
    {
        Stream ReadFontStream();
    }

    public class FontFileStreamProvider : FontStreamSource
    {
        public FontFileStreamProvider(string filename)
        {
            this.FileName = filename;
        }
        public string FileName { get; private set; }
        public Stream ReadFontStream()
        {
            FileStream fs = new FileStream(this.FileName, FileMode.Open, FileAccess.Read);
            return fs;
        }
    }



    [Flags]
    public enum InstalledFontStyle
    {
        Regular,
        Bold = 1 << 1,
        Italic = 1 << 2,
    }

    public delegate InstalledFont FontNotFoundHandler(InstalledFontCollection fontCollection, string fontName, InstalledFontStyle style);

    public class InstalledFontCollection
    {

        Dictionary<string, InstalledFont> regular_Fonts = new Dictionary<string, InstalledFont>();
        Dictionary<string, InstalledFont> italic_Fonts = new Dictionary<string, InstalledFont>();
        Dictionary<string, InstalledFont> bold_Fonts = new Dictionary<string, InstalledFont>();

        Dictionary<string, InstalledFont> boldItalic_Fonts = new Dictionary<string, InstalledFont>();
        Dictionary<string, InstalledFont> gras_Fonts = new Dictionary<string, InstalledFont>();
        Dictionary<string, InstalledFont> grasItalic_Fonts = new Dictionary<string, InstalledFont>();
        //
        Dictionary<string, Dictionary<string, InstalledFont>> _fontGroups = new Dictionary<string, Dictionary<string, InstalledFont>>();
        FontNotFoundHandler fontNotFoundHandler;
        //
        public InstalledFontCollection()
        {
            regular_Fonts = CreateNewFontGroup("normal", "regular");
            italic_Fonts = CreateNewFontGroup("italic", "italique");
            bold_Fonts = CreateNewFontGroup("bold");
            boldItalic_Fonts = CreateNewFontGroup("bold italic");
            gras_Fonts = CreateNewFontGroup("gras");
            grasItalic_Fonts = CreateNewFontGroup("gras italique");
        }
        Dictionary<string, InstalledFont> CreateNewFontGroup(params string[] names)
        {
            //single dic may be called by many names
            var fontGroup = new Dictionary<string, InstalledFont>();
            foreach (string name in names)
            {
                _fontGroups.Add(name.ToUpper(), fontGroup);
            }
            return fontGroup;
        }

        public void SetFontNotFoundHandler(FontNotFoundHandler handler)
        {
            fontNotFoundHandler = handler;
        }
        public void AddFont(FontStreamSource src)
        {
            //preview data of font
            using (Stream stream = src.ReadFontStream())
            {
                var reader = new OpenFontReader();
                RegisterFont(reader.ReadPreview(stream));
            }
        }


        void RegisterFont(InstalledFont f)
        {
            if (f == null || f.FontName == "" || f.FontName.StartsWith("\0"))
            {
                //no font name?
                return;
            }


            Dictionary<string, InstalledFont> selectedFontGroup;
            if (!_fontGroups.TryGetValue(f.FontSubFamily.ToUpper(), out selectedFontGroup))
            {
                throw new NotSupportedException();
                //TODO: implement a mising group
            }

            string fontNameUpper = f.FontName.ToUpper();
            if (selectedFontGroup.ContainsKey(fontNameUpper))
            {
                //TODO:
                //we already have this font name
                //(but may be different file
                //we let user to handle it        

            }
            else
            {
                selectedFontGroup.Add(fontNameUpper, f);
            }

        }
        public void LoadInstalledFont(IEnumerable<string> getFontFileIter)
        {
            List<InstalledFont> installedFonts = ReadPreviewFontData(getFontFileIter);
            //classify
            //do 
            int j = installedFonts.Count;
            for (int i = 0; i < j; ++i)
            {
                RegisterFont(installedFonts[i]);
            }
        }

        public InstalledFont GetFont(string fontName, InstalledFontStyle style)
        {
            //request font from installed font
            InstalledFont found;
            switch (style)
            {
                case (InstalledFontStyle.Bold | InstalledFontStyle.Italic):
                    {
                        //check if we have bold & italic 
                        //version of this font ?  
                        if (!boldItalic_Fonts.TryGetValue(fontName.ToUpper(), out found))
                        {
                            //if not found then goto italic 
                            goto case InstalledFontStyle.Italic;
                        }
                        return found;
                    }
                case InstalledFontStyle.Bold:
                    {

                        if (!bold_Fonts.TryGetValue(fontName.ToUpper(), out found))
                        {
                            //goto regular
                            goto default;
                        }
                        return found;
                    }
                case InstalledFontStyle.Italic:
                    {
                        //if not found then choose regular
                        if (!italic_Fonts.TryGetValue(fontName.ToUpper(), out found))
                        {
                            goto default;
                        }
                        return found;
                    }
                default:
                    {
                        //we skip gras style ?
                        if (!regular_Fonts.TryGetValue(fontName.ToUpper(), out found))
                        {

                            if (fontNotFoundHandler != null)
                            {
                                return fontNotFoundHandler(
                                    this,
                                    fontName,
                                    style);
                            }
                            return null;
                        }
                        return found;
                    }
            }
        }
        public static List<InstalledFont> ReadPreviewFontData(IEnumerable<string> getFontFileIter)
        {
            //-------------------------------------------------
            //TODO: review here, this is not platform depend
            //-------------------------------------------------
            //check if MAC or linux font folder too
            //------------------------------------------------- 
            List<InstalledFont> installedFonts = new List<InstalledFont>();
            foreach (string fontFilename in getFontFileIter)
            {
                using (Stream stream = new FileStream(fontFilename, FileMode.Open, FileAccess.Read))
                {
                    var reader = new OpenFontReader();
                    InstalledFont installedFont = reader.ReadPreview(stream);
                    installedFonts.Add(installedFont);
                }
            }
            return installedFonts;
        }

        public IEnumerable<InstalledFont> GetInstalledFontIter()
        {
            foreach (InstalledFont f in regular_Fonts.Values)
            {
                yield return f;
            }
            //
            foreach (InstalledFont f in italic_Fonts.Values)
            {
                yield return f;
            }
            //
            foreach (InstalledFont f in bold_Fonts.Values)
            {
                yield return f;
            }
            foreach (InstalledFont f in boldItalic_Fonts.Values)
            {
                yield return f;
            }
            //
            foreach (InstalledFont f in gras_Fonts.Values)
            {
                yield return f;
            }
            foreach (InstalledFont f in grasItalic_Fonts.Values)
            {
                yield return f;
            }
        }
    }


    public static class InstalledFontCollectionExtension
    {

        public static void LoadWinSystemFonts(this InstalledFontCollection fontCollection)
        {
            //implement
        }
        public static void LoadMacSystemFonts(this InstalledFontCollection fontCollection)
        {
            //implement
        }
        public static void LoadLinuxSystemFonts(this InstalledFontCollection fontCollection)
        {
            //implement
        }
    }
}