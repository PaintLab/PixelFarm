//2014 MIT,WinterDev   
 
using System;
using System.Collections.Generic;
using System.Text;

using PixelFarm.Agg.Image;
using PixelFarm.Agg.VertexSource;
using OpenTK;
using OpenTK.Graphics.ES20;
using LayoutFarm.DrawingGL;

namespace PixelFarm.Agg
{
    class CoordList3f
    {
        ArrayList<float> data = new ArrayList<float>();
        int coordCount = 0;
        public CoordList3f()
        {
        }
        public void AddCoord(int x, int y, int z)
        {
            this.data.AddVertex(x);
            this.data.AddVertex(y);
            this.data.AddVertex(z);
            this.coordCount++;
        }
        public void Clear()
        {
            this.coordCount = 0;
            this.data.Clear();
        }
        public int Count
        {
            get { return this.coordCount; }
        }

        public float[] GetInternalArray()
        {
            return this.data.Array;
        }

    }
    class CoordList2f
    {
        ArrayList<float> data = new ArrayList<float>();
        int coordCount = 0;
        public CoordList2f()
        {
        }
        public void AddCoord(float x, float y)
        {
            this.data.AddVertex(x);
            this.data.AddVertex(y);
            this.coordCount++;
        }
        public void Clear()
        {
            this.coordCount = 0;
            this.data.Clear();
        }
        public int Count
        {
            get { return this.coordCount; }
        }

        public float[] GetInternalArray()
        {
            return this.data.Array;
        }

    }
}