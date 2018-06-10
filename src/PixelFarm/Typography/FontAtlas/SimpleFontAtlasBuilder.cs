//MIT, 2016-2018, WinterDev
//----------------------------------- 

using System.Collections.Generic;
using System.Xml;
using PixelFarm.Drawing.Fonts;
using Typography.Contours;

namespace Typography.Rendering
{

    public class SimpleFontAtlasBuilder
    {
        GlyphImage _latestGenGlyphImage;
        Dictionary<int, CacheGlyph> _glyphs = new Dictionary<int, CacheGlyph>();

        public SimpleFontAtlasBuilder()
        {
            SpaceCompactOption = CompactOption.BinPack; //default
            MaxAtlasWidth = 800;
        }
        public int MaxAtlasWidth { get; set; }

        public TextureKind TextureKind { get; private set; }
        public float FontSizeInPoints { get; private set; }


        public enum CompactOption
        {
            None,
            BinPack,
            ArrangeByHeight
        }


        /// <summary>
        /// add or replace
        /// </summary>
        /// <param name="glyphIndex"></param>
        /// <param name="img"></param>
        public void AddGlyph(int glyphIndex, GlyphImage img)
        {
            var glyphCache = new CacheGlyph();
            glyphCache.glyphIndex = glyphIndex;
            glyphCache.img = img;

            _glyphs[glyphIndex] = glyphCache;
        }

        public void SetAtlasInfo(TextureKind textureKind, float fontSizeInPts)
        {
            this.TextureKind = textureKind;
            this.FontSizeInPoints = fontSizeInPts;
        }

        public CompactOption SpaceCompactOption { get; set; }

        public GlyphImage BuildSingleImage()
        {
            //1. add to list 
            var glyphList = new List<CacheGlyph>(_glyphs.Count);
            foreach (CacheGlyph glyphImg in _glyphs.Values)
            {
                //sort data
                glyphList.Add(glyphImg);
            }

            int totalMaxLim = MaxAtlasWidth;
            int maxRowHeight = 0;
            int currentY = 0;
            int currentX = 0;

            switch (this.SpaceCompactOption)
            {
                default:
                    throw new System.NotSupportedException();
                case CompactOption.BinPack:
                    {
                        //2. sort by glyph width
                        glyphList.Sort((a, b) =>
                        {
                            return a.img.Width.CompareTo(b.img.Width);
                        });
                        //3. layout 
                        for (int i = glyphList.Count - 1; i >= 0; --i)
                        {
                            CacheGlyph g = glyphList[i];
                            if (g.img.Height > maxRowHeight)
                            {
                                maxRowHeight = g.img.Height;
                            }
                            if (currentX + g.img.Width > totalMaxLim)
                            {
                                //start new row
                                currentY += maxRowHeight;
                                currentX = 0;
                            }
                            //-------------------
                            g.area = new Rectangle(currentX, currentY, g.img.Width, g.img.Height);
                            currentX += g.img.Width;
                        }

                    }
                    break;
                case CompactOption.ArrangeByHeight:
                    {
                        //2. sort by height
                        glyphList.Sort((a, b) =>
                        {
                            return a.img.Height.CompareTo(b.img.Height);
                        });
                        //3. layout 
                        int glyphCount = glyphList.Count;
                        for (int i = 0; i < glyphCount; ++i)
                        {
                            CacheGlyph g = glyphList[i];
                            if (g.img.Height > maxRowHeight)
                            {
                                maxRowHeight = g.img.Height;
                            }
                            if (currentX + g.img.Width > totalMaxLim)
                            {
                                //start new row
                                currentY += maxRowHeight;
                                currentX = 0;
                                maxRowHeight = 0;//reset, after start new row
                            }
                            //-------------------
                            g.area = new Rectangle(currentX, currentY, g.img.Width, g.img.Height);
                            currentX += g.img.Width;
                        }

                    }
                    break;
                case CompactOption.None:
                    {
                        //3. layout 
                        int glyphCount = glyphList.Count;
                        for (int i = 0; i < glyphCount; ++i)
                        {
                            CacheGlyph g = glyphList[i];
                            if (g.img.Height > maxRowHeight)
                            {
                                maxRowHeight = g.img.Height;
                            }
                            if (currentX + g.img.Width > totalMaxLim)
                            {
                                //start new row
                                currentY += maxRowHeight;
                                currentX = 0;
                                maxRowHeight = 0;//reset, after start new row
                            }
                            //-------------------
                            g.area = new Rectangle(currentX, currentY, g.img.Width, g.img.Height);
                            currentX += g.img.Width;
                        }
                    }
                    break;
            }

            currentY += maxRowHeight;
            int imgH = currentY;
            // -------------------------------
            //compact image location
            // TODO: review performance here again***

            int totalImgWidth = totalMaxLim;
            if (SpaceCompactOption == CompactOption.BinPack) //again here?
            {
                totalImgWidth = 0;//reset
                //use bin packer
                BinPacker binPacker = new BinPacker(totalMaxLim, currentY);
                for (int i = glyphList.Count - 1; i >= 0; --i)
                {
                    CacheGlyph g = glyphList[i];
                    BinPackRect newRect = binPacker.Insert(g.img.Width, g.img.Height);
                    g.area = new Rectangle(newRect.X, newRect.Y,
                        g.img.Width, g.img.Height);


                    //recalculate proper max midth again, after arrange and compact space
                    if (newRect.Right > totalImgWidth)
                    {
                        totalImgWidth = newRect.Right;
                    }
                }
            }
            // ------------------------------- 
            //4. create array that can hold data  
            int[] totalBuffer = new int[totalImgWidth * imgH];
            if (SpaceCompactOption == CompactOption.BinPack) //again here?
            {
                for (int i = glyphList.Count - 1; i >= 0; --i)
                {
                    CacheGlyph g = glyphList[i];
                    //copy data to totalBuffer
                    GlyphImage img = g.img;
                    CopyToDest(img.GetImageBuffer(), img.Width, img.Height, totalBuffer, g.area.Left, g.area.Top, totalImgWidth);
                }

            }
            else
            {
                int glyphCount = glyphList.Count;
                for (int i = 0; i < glyphCount; ++i)
                {
                    CacheGlyph g = glyphList[i];
                    //copy data to totalBuffer
                    GlyphImage img = g.img;
                    CopyToDest(img.GetImageBuffer(), img.Width, img.Height, totalBuffer, g.area.Left, g.area.Top, totalImgWidth);
                }
            }

            GlyphImage glyphImage = new GlyphImage(totalImgWidth, imgH);
            glyphImage.SetImageBuffer(totalBuffer, true);
            

            _latestGenGlyphImage = glyphImage;

            return glyphImage;

        }

        /// <summary>
        /// save font info into xml document
        /// </summary>
        /// <param name="filename"></param>
        public void SaveAtlasInfo(System.IO.Stream outputStream)
        {
            //save font info as xml 
            //save position of each font
            XmlDocument xmldoc = new XmlDocument();
            XmlElement root = xmldoc.CreateElement("font");
            xmldoc.AppendChild(root);

            if (_latestGenGlyphImage == null)
            {
                BuildSingleImage();
            }

            {
                //total img element
                XmlElement totalImgElem = xmldoc.CreateElement("total_img");
                totalImgElem.SetAttribute("w", _latestGenGlyphImage.Width.ToString());
                totalImgElem.SetAttribute("h", _latestGenGlyphImage.Height.ToString());
                totalImgElem.SetAttribute("compo", "4");
                root.AppendChild(totalImgElem);
            }

            foreach (CacheGlyph g in _glyphs.Values)
            {
                XmlElement gElem = xmldoc.CreateElement("glyph");
                //convert char to hex
                string unicode = ("0x" + ((int)g.character).ToString("X"));//code point
                Rectangle area = g.area;
                gElem.SetAttribute("c", g.glyphIndex.ToString());
                gElem.SetAttribute("uc", unicode);//unicode char
                gElem.SetAttribute("ltwh",
                    area.Left + " " + area.Top + " " + area.Width + " " + area.Height
                    );
                gElem.SetAttribute("tx",
                    g.img.Width + " " +
                    g.borderX + " " + g.borderY + " " +
                    g.img.TextureOffsetX + " " + g.img.TextureOffsetY
                    );
                if (g.character > 50)
                {
                    gElem.SetAttribute("example", g.character.ToString());
                }
                root.AppendChild(gElem);
            }

            //save to stream
            xmldoc.Save(outputStream);
        }

        public SimpleFontAtlas CreateSimpleFontAtlas()
        {
            SimpleFontAtlas simpleFontAtlas = new SimpleFontAtlas();
            simpleFontAtlas.TextureKind = this.TextureKind;
            simpleFontAtlas.OriginalFontSizePts = this.FontSizeInPoints;
            foreach (CacheGlyph cacheGlyph in _glyphs.Values)
            {
                //convert char to hex
                string unicode = ("0x" + ((int)cacheGlyph.character).ToString("X"));//code point
                Rectangle area = cacheGlyph.area;
                TextureFontGlyphData glyphData = new TextureFontGlyphData();
                area.Y += area.Height;//*** 

                //set font matrix to glyph font data
                glyphData.Rect = Rectangle.FromLTRB(area.X, area.Top, area.Right, area.Bottom);
                glyphData.AdvanceY = 0;// cacheGlyph.glyphMatrix.advanceY;
                glyphData.ImgWidth = cacheGlyph.img.Width;
                glyphData.TextureXOffset = cacheGlyph.img.TextureOffsetX;
                glyphData.TextureYOffset = cacheGlyph.img.TextureOffsetY;

                simpleFontAtlas.AddGlyph(cacheGlyph.glyphIndex, glyphData);
            }

            return simpleFontAtlas;
        }
        //read font info from xml document
        public SimpleFontAtlas LoadAtlasInfo(System.IO.Stream inputStream)
        {
            SimpleFontAtlas simpleFontAtlas = new SimpleFontAtlas();

            simpleFontAtlas.TextureKind = this.TextureKind;
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(inputStream);
            //read
            int total_W = 0;
            int total_H = 0;
            {
                foreach (XmlElement xmlelem in xmldoc.GetElementsByTagName("total_img"))
                {
                    simpleFontAtlas.Width = total_W = int.Parse(xmlelem.GetAttribute("w"));
                    simpleFontAtlas.Height = total_H = int.Parse(xmlelem.GetAttribute("h"));
                    //only 1...

                    break;
                }
            }
            foreach (XmlElement glyphElem in xmldoc.GetElementsByTagName("glyph"))
            {
                //read
                string unicodeHex = glyphElem.GetAttribute("uc");
                int glyphIndex = int.Parse(glyphElem.GetAttribute("c"));
                //TODO: this should be codepoint
                char c = (char)int.Parse(unicodeHex.Substring(2), System.Globalization.NumberStyles.HexNumber);
                Rectangle area = ParseRect(glyphElem.GetAttribute("ltwh"));
                area.Y += area.Height;//*** 
                var glyphData = new TextureFontGlyphData();
                glyphData.Rect = Rectangle.FromLTRB(area.X, area.Top, area.Right, area.Bottom);
                float[] borderAndTransform = ParseFloatArray(glyphElem.GetAttribute("tx"));
                glyphData.ImgWidth = borderAndTransform[0];
                glyphData.BorderX = borderAndTransform[1];
                glyphData.BorderY = borderAndTransform[2];
                glyphData.TextureXOffset = borderAndTransform[3];
                glyphData.TextureYOffset = borderAndTransform[4];

                //--------------- 
                simpleFontAtlas.AddGlyph(glyphIndex, glyphData);
            }
            return simpleFontAtlas;
        }

        static float[] ParseFloatArray(string str)
        {
            string[] str_values = str.Split(' ');
            int j = str_values.Length;
            float[] f_values = new float[j];
            for (int i = 0; i < j; ++i)
            {
                f_values[i] = float.Parse(str_values[i]);
            }
            return f_values;
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

    }


}