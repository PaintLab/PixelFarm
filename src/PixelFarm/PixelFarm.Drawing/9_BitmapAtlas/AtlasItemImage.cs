//MIT, 2016-present, WinterDev
using System;
using PixelFarm.CpuBlit;
namespace PixelFarm.Drawing.BitmapAtlas
{
    public class AtlasItemImage
    {
        public AtlasItemImage(int w, int h)
        {
            this.Width = w;
            this.Height = h;
        }
        public bool IsBigEndian { get; private set; }
      
        /// <summary>
        /// physical width of item
        /// </summary>
        public int Width { get; }
        /// <summary>
        /// physical height of item
        /// </summary>
        public int Height { get; }

        public MemBitmap Bitmap { get; private set; } 

        public void SetBitmap(MemBitmap bmp, bool isBigEndian)
        {
            Bitmap = bmp;
            this.IsBigEndian = isBigEndian;
        }
        /// <summary>
        /// texture offset X from original reference x
        /// </summary>
        public short TextureOffsetX { get; set; }
        /// <summary>
        /// texture offset Y from original reference y  
        /// </summary>
        public short TextureOffsetY { get; set; }


    }

    class CacheBmp
    {
        internal AtlasItemImage img;
        public Rectangle area;
        public ushort imgIndex;

#if DEBUG
        public CacheBmp()
        {
        }
#endif
    }

    public class BitmapMapData
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public float TextureXOffset { get; set; }
        public float TextureYOffset { get; set; } 
    }

}