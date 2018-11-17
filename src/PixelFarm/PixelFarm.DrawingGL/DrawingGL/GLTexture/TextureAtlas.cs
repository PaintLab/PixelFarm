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
            this._width = w;
            this._height = h;
        }
        public int Width { get { return this._width; } }
        public int Height { get { return this._height; } }

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
            if (w > this._width)
            {
                areaId = x = y = 0;
                return TextureAtlasAllocResult.WidthOverLimit;
            }
            if (h > this._height)
            {
                areaId = x = y = 0;
                return TextureAtlasAllocResult.HeightOverLimit;
            }
            //-------------------------
            if (this._currentXPos + w > this._width)
            {
                //start to new line
                this._currentXPos = 0;
                this._currentYPos += _currentLineMaxHeight;
                this._currentLineMaxHeight = 0;
            }
            if (this._currentYPos + h > this._height)
            {
                areaId = x = y = 0;
                return TextureAtlasAllocResult.FullSpace;
            }
            //-------------------------
            x = _currentXPos;
            y = _currentYPos;
            areaId = this._areas.Count + 1;
            this._areas.Add(new Drawing.RectangleF(x, y, w, h));
            //move xpos to next
            this._currentXPos += w;
            if (_currentLineMaxHeight < h)
            {
                _currentLineMaxHeight = h;
            }
            //------------------------- 

            return TextureAtlasAllocResult.Ok;
        }
    }
}