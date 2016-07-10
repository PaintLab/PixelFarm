//MIT,2016, WinterDev
//----------------------------------- 

using System;
using System.Collections.Generic;
using System.Xml;
namespace PixelFarm.Drawing.Fonts
{
    public class SimpleFontAtlas
    {
        GlyphImage totalGlyphImage;
        Dictionary<char, Rectangle> charLocations = new Dictionary<char, Rectangle>();

        public void AddGlyph(char c, Rectangle rect)
        {
            charLocations.Add(c, rect);
        }
        public void SetImage(GlyphImage totalGlyphImage)
        {
            this.totalGlyphImage = totalGlyphImage;
        }
        public bool GetRect(char c, out Rectangle rect)
        {
            if (!charLocations.TryGetValue(c, out rect))
            {
                rect = Rectangle.Empty;
                return false;
            }
            return true;
        }
    }
    public class SimpleFontAtlasBuilder
    {
        Dictionary<char, GlyphData> glyphs = new Dictionary<char, GlyphData>();

        public void AddGlyph(char c, FontGlyph fontGlyph, GlyphImage glyphImage)
        {
            var glyphData = new GlyphData(c, fontGlyph, glyphImage);
            glyphs[c] = glyphData;
        }

        public GlyphImage BuildSingleImage()
        {
            //1. add to list 
            var glyphList = new List<GlyphData>(glyphs.Count);
            foreach (GlyphData glyphData in glyphs.Values)
            {
                //sort data
                glyphList.Add(glyphData);
            }
            //2. sort
            glyphList.Sort((a, b) =>
            {
                return a.glyphImage.Width.CompareTo(b.glyphImage.Width);
            });
            //3. layout
            int totalWidth = 0;
            int totalMaxLim = 800;
            int maxRowHeight = 0;
            int currentY = 0;
            int currentX = 0;
            for (int i = glyphList.Count - 1; i >= 0; --i)
            {
                GlyphData g = glyphList[i];
                if (g.glyphImage.Height > maxRowHeight)
                {
                    maxRowHeight = g.glyphImage.Height;
                }
                if (totalWidth + g.glyphImage.Width > totalMaxLim)
                {
                    //start new row
                    currentY += maxRowHeight;
                    currentX = 0;
                }
                //-------------------
                g.area = new Rectangle(currentX, currentY, g.glyphImage.Width, g.glyphImage.Height);
                currentX += g.glyphImage.Width;
            }
            currentY += maxRowHeight;
            //------------------
            //4. create array that can hold data
            int[] totalBuffer = new int[totalMaxLim * currentY];
            for (int i = glyphList.Count - 1; i >= 0; --i)
            {
                GlyphData g = glyphList[i];
                //copy data to totalBuffer
                GlyphImage img = g.glyphImage;
                CopyToDest(img.GetImageBuffer(), img.Width, img.Height, totalBuffer, g.area.Left, g.area.Top, totalMaxLim);
            }
            //------------------
            GlyphImage glyphImage = new Fonts.GlyphImage(totalMaxLim, currentY);
            glyphImage.SetImageBuffer(totalBuffer, true);
            return glyphImage;
        }
        static void CopyToDest(int[] srcPixels, int srcW, int srcH, int[] targetPixels, int targetX, int targetY, int totalTargetWidth)
        {
            int srcIndex = 0;
            unsafe
            {

                for (int r = 0; r < srcH; ++r)
                {
                    //for each row 
                    int targetP = ((targetY + r) * totalTargetWidth) + targetX;
                    for (int c = 0; c < srcW; ++c)
                    {
                        targetPixels[targetP] = srcPixels[srcIndex];
                        srcIndex++;
                        targetP++;
                    }
                }
            }
        }


        /// <summary>
        /// save font info into xml document
        /// </summary>
        /// <param name="filename"></param>
        public void SaveFontInfo(string filename)
        {
            //save font info as xml 
            //save position of each font
            XmlDocument xmldoc = new XmlDocument();
            XmlElement root = xmldoc.CreateElement("font");
            xmldoc.AppendChild(root);

            foreach (GlyphData g in glyphs.Values)
            {
                XmlElement gElem = xmldoc.CreateElement("glyph");
                //convert char to hex
                string unicode = ("0x" + ((int)g.character).ToString("X")); //accept unicode char
                Rectangle area = g.area;
                gElem.SetAttribute("uc", unicode);//unicode char
                gElem.SetAttribute("ltwh",
                    area.Left + " " + area.Top + " " + area.Width + " " + area.Height
                    );
                gElem.SetAttribute("offset",
                    g.glyphImage.OffsetX + " " + g.glyphImage.OffsetY
                    );
                root.AppendChild(gElem);
            }
            xmldoc.Save(filename);
        }
        //read font info from xml document
        public SimpleFontAtlas LoadFontInfo(string filename)
        {
            SimpleFontAtlas simpleFontAtlas = new Fonts.SimpleFontAtlas();
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(filename);
            //read
            foreach (XmlElement glyphElem in xmldoc.GetElementsByTagName("glyph"))
            {
                //read
                string unicodeHex = glyphElem.GetAttribute("uc");
                Rectangle area = ParseRect(glyphElem.GetAttribute("ltwh"));
                char c = (char)int.Parse(unicodeHex.Substring(2), System.Globalization.NumberStyles.HexNumber);
                simpleFontAtlas.AddGlyph(c, area);
            }
            return simpleFontAtlas;
        }
        static Rectangle ParseRect(string str)
        {
            string[] ltwh = str.Split(' ');
            return new Rectangle(
                int.Parse(ltwh[0]),
                int.Parse(ltwh[1]),
                int.Parse(ltwh[2]),
                int.Parse(ltwh[3]));

        }
    }
    class GlyphData
    {
        public FontGlyph fontGlyph;
        public GlyphImage glyphImage;
        public Rectangle area;
        public char character;
        public GlyphData(char c, FontGlyph fontGlyph, GlyphImage glyphImage)
        {
            this.character = c;
            this.fontGlyph = fontGlyph;
            this.glyphImage = glyphImage;

        }
    }

}