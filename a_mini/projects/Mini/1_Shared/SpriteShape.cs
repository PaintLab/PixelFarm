
//BSD 2014, WinterDev
//MattersHackers
//AGG 2.4


using System;
using System.Globalization;
using PixelFarm.Agg.Transform;
using PixelFarm.Agg.Image;
using PixelFarm.Agg.VertexSource;
using PixelFarm.Agg;
using PixelFarm.VectorMath;

namespace PixelFarm.Agg
{
    public class SpriteShape
    {
        PathStorage path = new PathStorage();
        ColorRGBA[] colors = new ColorRGBA[100];
        int[] pathIndexList = new int[100];
        int numPaths = 0;
        RectangleDouble boundingRect;
        Vector2 center;
        public SpriteShape()
        {

        }

        public PathStorage Path
        {
            get
            {
                return path;
            }


        }

        public int NumPaths
        {
            get
            {
                return numPaths;
            }
        }

        public RectangleDouble Bounds
        {
            get
            {
                return boundingRect;
            }
        }

        public ColorRGBA[] Colors
        {
            get
            {
                return colors;
            }

        }

        public int[] PathIndexList
        {
            get
            {
                return pathIndexList;
            }

        }

        public Vector2 Center
        {
            get
            {
                return center;
            }
        }

        public void ParseLion()
        {

            numPaths = PixelFarm.Agg.LionDataStore.LoadLionData(path, colors, pathIndexList);
            PixelFarm.Agg.BoundingRect.GetBoundingRect(path, pathIndexList, numPaths, out boundingRect);
            center.x = (boundingRect.Right - boundingRect.Left) / 2.0;
            center.y = (boundingRect.Top - boundingRect.Bottom) / 2.0;
        }
        public static void UnsafeDirectSetData(SpriteShape lion,
            int numPaths,
            PathStorage pathStore,
            ColorRGBA[] colors,
            int[] pathIndice)
        {
            lion.path = pathStore;
            lion.colors = colors;
            lion.pathIndexList = pathIndice;
            lion.numPaths = numPaths;
            lion.UpdateBoundingRect();
        }
        void UpdateBoundingRect()
        {
            PixelFarm.Agg.BoundingRect.GetBoundingRect(path, pathIndexList, numPaths, out boundingRect);
            center.x = (boundingRect.Right - boundingRect.Left) / 2.0;
            center.y = (boundingRect.Top - boundingRect.Bottom) / 2.0;
        }

    }
}