//MIT, 2014-present, WinterDev
using System.Collections.Generic;
namespace PixelFarm.DrawingGL
{
    public enum TextureAtlasAllocResult
    {
        Ok,
        //-------
        //fail
        Unknown,
        FullSpace,
        WidthOverLimit,
        HeightOverLimit
    }
    public class TextureAtlas
    {
        int _width;
        int _height;
        int _currentXPos;
        int _currentYPos;
        int _currentLineMaxHeight = 0;
        List<PixelFarm.Drawing.RectangleF> _areas = new List<Drawing.RectangleF>();
        public TextureAtlas(int w, int h)
        {
            _width = w;
            _height = h;
        }
        public int Width { get { return _width; } }
        public int Height { get { return _height; } }

        public uint GraphicsTextureId
        {
            get;
            set;
        }
        public void Dispose()
        {
            if (this.GraphicsTextureId != 0)
            {
                //clear GL texture
            }
        }
        public TextureAtlasAllocResult AllocNewRectArea(int w, int h,
            out int areaId, out int x, out int y)
        {
            //simple***
            //alloc new area
            //find new area for w and h 

            //-------------------------
            if (w > _width)
            {
                areaId = x = y = 0;
                return TextureAtlasAllocResult.WidthOverLimit;
            }
            if (h > _height)
            {
                areaId = x = y = 0;
                return TextureAtlasAllocResult.HeightOverLimit;
            }
            //-------------------------
            if (_currentXPos + w > _width)
            {
                //start to new line
                _currentXPos = 0;
                _currentYPos += _currentLineMaxHeight;
                _currentLineMaxHeight = 0;
            }
            if (_currentYPos + h > _height)
            {
                areaId = x = y = 0;
                return TextureAtlasAllocResult.FullSpace;
            }
            //-------------------------
            x = _currentXPos;
            y = _currentYPos;
            areaId = _areas.Count + 1;
            _areas.Add(new Drawing.RectangleF(x, y, w, h));
            //move xpos to next
            _currentXPos += w;
            if (_currentLineMaxHeight < h)
            {
                _currentLineMaxHeight = h;
            }
            //------------------------- 

            return TextureAtlasAllocResult.Ok;
        }
    }
}