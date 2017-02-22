//MIT, 2016-2017, WinterDev
//----------------------------------- 

using System;
using System.Collections.Generic;
using System.Xml;

using PixelFarm.Drawing;
using PixelFarm.Drawing.Fonts;


namespace Typography.Rendering
{
    public class MySimpleFontAtlasBuilder
    {
        Dictionary<int, GlyphData> glyphs = new Dictionary<int, GlyphData>();
        GlyphImage latestGenGlyphImage;
        public void AddGlyph(int codePoint, char c, FontGlyph fontGlyph, GlyphImage glyphImage)
        {
            glyphs[codePoint] = new GlyphData(codePoint, c, fontGlyph, glyphImage);
        }

        public GlyphImage GetLatestGenGlyphImage()
        {
            return latestGenGlyphImage;
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
                if (currentX + g.glyphImage.Width > totalMaxLim)
                {
                    //start new row
                    currentY += maxRowHeight;
                    currentX = 0;
                }
                //-------------------
                g.pxArea = new PixelFarm.Drawing.Rectangle(currentX, currentY, g.glyphImage.Width, g.glyphImage.Height);
                currentX += g.glyphImage.Width;
            }
            currentY += maxRowHeight;
            int imgH = currentY;


            //-------------------------------
            //compact image location
            //TODO: review performance here again***
            SharpFont.BinPacker binPacker = new SharpFont.BinPacker(totalMaxLim, currentY);
            for (int i = glyphList.Count - 1; i >= 0; --i)
            {
                GlyphData g = glyphList[i];
                SharpFont.Rect newRect = binPacker.Insert(g.glyphImage.Width, g.glyphImage.Height);
                g.pxArea = new PixelFarm.Drawing.Rectangle(newRect.X, newRect.Y,
                    g.glyphImage.Width, g.glyphImage.Height);
            }
            //-------------------------------




            //4. create array that can hold data
            int[] totalBuffer = new int[totalMaxLim * imgH];
            for (int i = glyphList.Count - 1; i >= 0; --i)
            {
                GlyphData g = glyphList[i];
                //copy data to totalBuffer
                GlyphImage img = g.glyphImage;
                CopyToDest(img.GetImageBuffer(), img.Width, img.Height, totalBuffer, g.pxArea.Left, g.pxArea.Top, totalMaxLim);
            }
            //------------------

            GlyphImage glyphImage = new PixelFarm.Drawing.Fonts.GlyphImage(totalMaxLim, imgH);
            glyphImage.SetImageBuffer(totalBuffer, true);
            return latestGenGlyphImage = glyphImage;
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

            if (latestGenGlyphImage == null)
            {
                BuildSingleImage();
            }

            {
                //total img element
                XmlElement totalImgElem = xmldoc.CreateElement("total_img");
                totalImgElem.SetAttribute("w", latestGenGlyphImage.Width.ToString());
                totalImgElem.SetAttribute("h", latestGenGlyphImage.Height.ToString());
                totalImgElem.SetAttribute("compo", "4");
                root.AppendChild(totalImgElem);
            }

            foreach (GlyphData g in glyphs.Values)
            {
                XmlElement gElem = xmldoc.CreateElement("glyph");
                //convert char to hex
                string unicode = ("0x" + ((int)g.character).ToString("X"));//code point
                PixelFarm.Drawing.Rectangle area = g.pxArea;
                gElem.SetAttribute("c", g.codePoint.ToString());
                gElem.SetAttribute("uc", unicode);//unicode char
                gElem.SetAttribute("ltwh",
                    area.Left + " " + area.Top + " " + area.Width + " " + area.Height
                    );
                gElem.SetAttribute("borderXY",
                    g.glyphImage.BorderXY + " " + g.glyphImage.BorderXY
                    );
                var mat = g.fontGlyph.glyphMatrix;
                gElem.SetAttribute("mat",
                    mat.advanceX + " " + mat.advanceY + " " +
                    mat.bboxXmin + " " + mat.bboxXmax + " " +
                    mat.bboxYmin + " " + mat.bboxYmax + " " +
                    mat.img_width + " " + mat.img_height + " " +
                    mat.img_horiAdvance + " " + mat.img_horiBearingX + " " +
                    mat.img_horiBearingY + " " +
                    //-----------------------------
                    mat.img_vertAdvance + " " +
                    mat.img_vertBearingX + " " + mat.img_vertBearingY);

                if (g.character > 50)
                {
                    gElem.SetAttribute("example", g.character.ToString());
                }
                root.AppendChild(gElem);
            }



            //if (embededGlyphsImage)
            //{
            //    XmlElement glyphImgElem = xmldoc.CreateElement("msdf_img");
            //    glyphImgElem.SetAttribute("w", latestGenGlyphImage.Width.ToString());
            //    glyphImgElem.SetAttribute("h", latestGenGlyphImage.Height.ToString());
            //    int[] imgBuffer = latestGenGlyphImage.GetImageBuffer();
            //    glyphImgElem.SetAttribute("buff_len", (imgBuffer.Length * 4).ToString());
            //    //----------------------------------------------------------------------
            //    glyphImgElem.AppendChild(
            //        xmldoc.CreateTextNode(ConvertToBase64(imgBuffer)));
            //    //----------------------------------------------------------------------
            //    root.AppendChild(glyphImgElem);
            //    latestGenGlyphImage.GetImageBuffer();
            //}
            xmldoc.Save(filename);
        }


        //read font info from xml document
        public SimpleFontAtlas LoadFontInfo(string filename)
        {
            SimpleFontAtlas simpleFontAtlas = new SimpleFontAtlas();
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(filename);
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
                int codepoint = int.Parse(glyphElem.GetAttribute("c"));
                char c = (char)int.Parse(unicodeHex.Substring(2), System.Globalization.NumberStyles.HexNumber);
                PixelFarm.Drawing.Rectangle area = ParseRect(glyphElem.GetAttribute("ltwh"));
                var glyphData = new TextureFontGlyphData();
                area.Y += area.Height;//*** 
                glyphData.Rect = area;
                float[] borderXY = ParseFloatArray(glyphElem.GetAttribute("borderXY"));
                float[] matrix = ParseFloatArray(glyphElem.GetAttribute("mat"));

                glyphData.BorderX = borderXY[0];
                glyphData.BorderY = borderXY[1];

                glyphData.AdvanceX = matrix[0];
                glyphData.AdvanceY = matrix[1];
                glyphData.BBoxXMin = matrix[2];
                glyphData.BBoxXMax = matrix[3];
                glyphData.BBoxYMin = matrix[4];
                glyphData.BBoxYMax = matrix[5];
                glyphData.ImgWidth = matrix[6];
                glyphData.ImgHeight = matrix[7];
                glyphData.HAdvance = matrix[8];
                glyphData.HBearingX = matrix[9];
                glyphData.HBearingY = matrix[10];
                glyphData.VAdvance = matrix[11];
                glyphData.VBearingX = matrix[12];
                glyphData.VBearingY = matrix[13];
                //--------------- 
                simpleFontAtlas.AddGlyph(codepoint, glyphData);
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
        static PixelFarm.Drawing.Rectangle ParseRect(string str)
        {
            string[] ltwh = str.Split(' ');
            return new PixelFarm.Drawing.Rectangle(
                int.Parse(ltwh[0]),
                int.Parse(ltwh[1]),
                int.Parse(ltwh[2]),
                int.Parse(ltwh[3]));
        }
    }


}