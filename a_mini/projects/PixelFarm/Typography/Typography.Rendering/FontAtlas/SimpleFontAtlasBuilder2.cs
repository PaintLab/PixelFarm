//MIT, 2016-2017, WinterDev
//----------------------------------- 

using System;
using System.Collections.Generic;
using System.Xml;

using PixelFarm.Drawing;
using PixelFarm.Drawing.Fonts;
using PixelFarm.Agg;

namespace Typography.Rendering
{
    public class GlyphImage2
    {
        int[] pixelBuffer;
        public GlyphImage2(int w, int h)
        {
            this.Width = w;
            this.Height = h;
        }
        public RectangleF OriginalGlyphBounds
        {
            get;
            set;
        }
        public int Width
        {
            get;
            private set;
        }
        public int Height
        {
            get;
            private set;
        }
        public bool IsBigEndian
        {
            get;
            private set;
        }
        public int BorderXY
        {
            get;
            set;
        }
        public int[] GetImageBuffer()
        {
            return pixelBuffer;
        }
        public void SetImageBuffer(int[] pixelBuffer, bool isBigEndian)
        {
            this.pixelBuffer = pixelBuffer;
            this.IsBigEndian = isBigEndian;
        }
    }

    class CacheGlyph
    {
        public ActualImage img;
        public Rectangle area;

    }
    public class SimpleFontAtlasBuilder2
    {
        Dictionary<int, CacheGlyph> glyphs = new Dictionary<int, CacheGlyph>();
        public void AddGlyph(int codePoint, ActualImage img)
        {
            var glyphCache = new CacheGlyph();
            glyphCache.img = img;
            glyphs[codePoint] = glyphCache;
        }
        public GlyphImage2 BuildSingleImage()
        {
            //1. add to list 
            var glyphList = new List<CacheGlyph>(glyphs.Count);
            foreach (CacheGlyph glyphImg in glyphs.Values)
            {
                //sort data
                glyphList.Add(glyphImg);
            }
            //2. sort
            glyphList.Sort((a, b) =>
            {
                return a.img.Width.CompareTo(b.img.Width);
            });
            //3. layout

            int totalMaxLim = 800;
            int maxRowHeight = 0;
            int currentY = 0;
            int currentX = 0;
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
            currentY += maxRowHeight;
            int imgH = currentY;
            //-------------------------------
            //compact image location
            //TODO: review performance here again***
            SharpFont.BinPacker binPacker = new SharpFont.BinPacker(totalMaxLim, currentY);
            for (int i = glyphList.Count - 1; i >= 0; --i)
            {
                CacheGlyph g = glyphList[i];
                SharpFont.Rect newRect = binPacker.Insert(g.img.Width, g.img.Height);
                g.area = new Rectangle(newRect.X, newRect.Y,
                    g.img.Width, g.img.Height);
            }
            //------------------------------- 

            //4. create array that can hold data
            int[] totalBuffer = new int[totalMaxLim * imgH];
            for (int i = glyphList.Count - 1; i >= 0; --i)
            {
                CacheGlyph g = glyphList[i];
                //copy data to totalBuffer
                ActualImage img = g.img;
                CopyToDest(ActualImage.GetBuffer2(img), img.Width, img.Height, totalBuffer, g.area.Left, g.area.Top, totalMaxLim);
            }
            //------------------

            GlyphImage2 glyphImage = new GlyphImage2(totalMaxLim, imgH);
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

    }
  

}