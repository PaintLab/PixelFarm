//MIT, 2016-present, WinterDev
using System.Collections.Generic;
namespace ExtMsdfGen
{
    public class SpriteTextureMapDataList<T>
    {
        List<SpriteTextureMapData<T>> _list = new List<SpriteTextureMapData<T>>();
        public SpriteTextureMapDataList()
        {
        }
        public T Source { get; internal set; }
        public void AddData(SpriteTextureMapData<T> item) => _list.Add(item);
        public int Count => _list.Count;
        public SpriteTextureMapData<T> GetItem(int index) => _list[index];
    }

    public class SpriteTextureMapData<T>
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public float TextureXOffset { get; set; }
        public float TextureYOffset { get; set; }
        public T Source { get; set; }

        public SpriteTextureMapData(int left, int top, int width, int height)
        {
            Left = left;
            Top = top;
            Width = width;
            Height = height;
        }
        public void GetRect(out int x, out int y, out int w, out int h)
        {
            x = Left;
            y = Top;
            w = Width;
            h = Height;
        }

    }

}