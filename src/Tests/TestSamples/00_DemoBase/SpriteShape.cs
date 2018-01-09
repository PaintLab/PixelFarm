
//BSD, 2014-2018, WinterDev
//MattersHackers
//AGG 2.4

using PixelFarm.Drawing;
using PixelFarm.Agg.VertexSource;
using PixelFarm.VectorMath;

using System.Text;

namespace PixelFarm.Agg
{


    //TODO: review here again***
    //move to SVG or renderVX

    public class SpriteShape
    {
        PathWriter path = new PathWriter();
        Color[] colors = new Color[100];
        int[] pathIndexList = new int[100];
        int numPaths = 0;
        RectD boundingRect;
        Vector2 center;

        VertexStore _lionVxs;
        public SpriteShape()
        {
        }


        public VertexStore Vxs
        {
            get
            {
                return _lionVxs;

            }
        }
        public int NumPaths
        {
            get
            {
                return numPaths;
            }
        }

        public RectD Bounds
        {
            get
            {
                return boundingRect;
            }
        }

        public Color[] Colors
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


        static string ColorToHex(Color c)
        {

            return "#" + c.R.ToString("X") + c.G.ToString("X") + c.B.ToString("X");
        }
        public void ParseLion()
        {
            numPaths = PixelFarm.Agg.LionDataStore.LoadLionData(path, colors, pathIndexList);
            _lionVxs = path.Vxs;
            PixelFarm.Agg.BoundingRect.GetBoundingRect(_lionVxs, pathIndexList, numPaths, out boundingRect);
            center.x = (boundingRect.Right - boundingRect.Left) / 2.0;
            center.y = (boundingRect.Top - boundingRect.Bottom) / 2.0;


            ////--------------------------
            ////save original lion to svg

            //StringBuilder stbuilder = new StringBuilder();
            //stbuilder.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?>");
            //stbuilder.Append("<svg id=\"svg2\" xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 900 900\" version=\"1.1\">");
            //int j = pathIndexList.Length;
            //for (int i = 0; i < j; ++i)
            //{
            //    int pathIndex = pathIndexList[i];
            //    if (i > 0 && pathIndex == 0)
            //        break;
            //    VertexStoreSnap snap = new VertexStoreSnap(_lionVxs, pathIndex);
            //    //read content from the snap iter
            //    VertexSnapIter snapIter = snap.GetVertexSnapIter();

            //    Color c = colors[i];

            //    stbuilder.Append("<g fill=\"" + ColorToHex(c) + "\">");


            //    StringBuilder pathdef = new StringBuilder();

            //    double x, y;
            //    VertexCmd cmd;
            //    while ((cmd = snapIter.GetNextVertex(out x, out y)) != VertexCmd.NoMore)
            //    {
            //        switch (cmd)
            //        {
            //            case VertexCmd.LineTo:
            //                pathdef.Append("L");
            //                pathdef.Append(((float)x).ToString());
            //                pathdef.Append(",");
            //                pathdef.Append(((float)y).ToString());
            //                pathdef.Append(" ");

            //                break;
            //            case VertexCmd.MoveTo:
            //                pathdef.Append("M");
            //                pathdef.Append(((float)x).ToString());
            //                pathdef.Append(",");
            //                pathdef.Append(((float)y).ToString());
            //                pathdef.Append(" ");
            //                break;
            //            case VertexCmd.Close:
            //                pathdef.Append("z");
            //                break;
            //            case VertexCmd.CloseAndEndFigure:
            //                pathdef.Append("z");
            //                break;
            //            case VertexCmd.EndFigure:
            //                pathdef.Append("z");
            //                break;
            //            default:
            //                break;

            //        }
            //    }
            //    stbuilder.Append("<path d=\"" + pathdef.ToString() + "\"/>");


            //    stbuilder.Append("</g>");


            //}
            //stbuilder.Append("</svg>");

            //////since lion is bottom-up
            //////we invert it
            ////Transform.Affine aff = Transform.Affine.NewMatix(
            ////    new Transform.AffinePlan(Transform.AffineMatrixCommand.Scale, 1, -1));
            ////VertexStore newvxs = new VertexStore();
            ////_lionVxs = aff.TransformToVxs(_lionVxs, newvxs);

            //System.IO.File.WriteAllText("d:\\WImageTest\\lion.svg", stbuilder.ToString());

        }
        public static void UnsafeDirectSetData(SpriteShape lion,
            int numPaths,
            PathWriter pathStore,
            Color[] colors,
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
            PixelFarm.Agg.BoundingRect.GetBoundingRect(path.Vxs, pathIndexList, numPaths, out boundingRect);
            center.x = (boundingRect.Right - boundingRect.Left) / 2.0;
            center.y = (boundingRect.Top - boundingRect.Bottom) / 2.0;
        }
    }
}