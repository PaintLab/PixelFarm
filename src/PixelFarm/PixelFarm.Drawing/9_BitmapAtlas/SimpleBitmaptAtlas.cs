//MIT, 2016-present, WinterDev
//----------------------------------- 

using System;
using System.Collections.Generic;


namespace PixelFarm.Drawing.BitmapAtlas
{
    public enum TextureKind : byte
    {
        StencilLcdEffect, //default
        StencilGreyScale,
        Msdf,
        Bitmap
    }
    public class SimpleBitmaptAtlas
    {
        AtlasItemImage _totalImg;
        Dictionary<ushort, BitmapMapData> _locations = new Dictionary<ushort, BitmapMapData>();

        public int Width { get; set; }
        public int Height { get; set; }
        /// <summary>
        /// original font size in point unit
        /// </summary>

        public TextureKind TextureKind { get; set; }
        public string BitmapFilename { get; set; }


        public void AddBitmapMapData(ushort imgIndex, BitmapMapData bmpMapData)
        {
            _locations.Add(imgIndex, bmpMapData);
        }
        public AtlasItemImage TotalImg
        {
            get => _totalImg;
            set => _totalImg = value;
        }
        public bool TryGetBitmapMapData(ushort imgIndex, out BitmapMapData bmpMapData)
        {
            if (!_locations.TryGetValue(imgIndex, out bmpMapData))
            {
                bmpMapData = null;
                return false;
            }
            return true;
        }
    }

}