//MIT, 2016-2017, WinterDev

using System.Collections.Generic;
using PixelFarm.Agg;
namespace PixelFarm.DrawingGL
{
    class Figure
    {
        //TODO: review here again***

        int[] contourEnds = new int[1];
        public float[] coordXYs; //this is user provide coord
        //---------
        //system tess ...
        public float[] areaTess;
        float[] smoothBorderTess;
        int borderTriangleStripCount;
        int tessAreaTriangleCount;

        //---------
        VertexBufferObject _vboArea;
        //---------
        public Figure(float[] coordXYs)
        {
            this.coordXYs = coordXYs;
        }
        public int BorderTriangleStripCount { get { return borderTriangleStripCount; } }
        public int TessAreaTriangleCount { get { return tessAreaTriangleCount; } }

        public bool SupportVertexBuffer
        {
            get
            {
                return true;
            }
        }
        public void InitVertexBufferIfNeed(TessTool tess)
        {
            if (_vboArea == null)
            {
                _vboArea = new VertexBufferObject();
                GetAreaTess2(tess);
                //create index buffer
                _vboArea.SetupVertexData(coordXYs, indexListArray);
            }
        }
        /// <summary>
        /// vertex buffer of the solid area part
        /// </summary>
        public VertexBufferObject VBOArea
        {
            get
            {
                return _vboArea;
            }
        }
        public float[] GetSmoothBorders()
        {
            if (smoothBorderTess == null)
            {
                return smoothBorderTess = SmoothBorderBuilder.BuildSmoothBorders(coordXYs, out borderTriangleStripCount);
            }
            return smoothBorderTess;
        }

        public float[] GetAreaTess(TessTool tess)
        {
            if (areaTess == null)
            {
                //triangle list
                contourEnds[0] = coordXYs.Length - 1;
                return areaTess = tess.TessPolygon(coordXYs, contourEnds, out this.tessAreaTriangleCount);
            }
            return areaTess;
        }
        public void GetAreaTess2(TessTool tess)
        {
            //triangle list
            contourEnds[0] = coordXYs.Length - 1;
            indexListArray = tess.TessPolygon2(coordXYs, contourEnds, out this.tessAreaTriangleCount);
        }
        public int[] indexListArray;

    }



    static class SmoothBorderBuilder
    {
        public static float[] BuildSmoothBorders(float[] coordXYs, out int borderTriangleStripCount)
        {
            float[] coords = coordXYs;
            int coordCount = coordXYs.Length;
            //from user input coords
            //expand it
            List<float> expandCoords = new List<float>();
            int lim = coordCount - 2;
            for (int i = 0; i < lim;)
            {
                CreateLineSegment(expandCoords, coords[i], coords[i + 1], coords[i + 2], coords[i + 3]);
                i += 2;
            }
            //close coord
            CreateLineSegment(expandCoords, coords[coordCount - 2], coords[coordCount - 1], coords[0], coords[1]);

            borderTriangleStripCount = (coordCount) * 2;
            return expandCoords.ToArray();
        }
        static void CreateLineSegment(List<float> coords, float x1, float y1, float x2, float y2)
        {
            //create wiht no line join
            float dx = x2 - x1;
            float dy = y2 - y1;
            float rad1 = (float)System.Math.Atan2(
                   y2 - y1,  //dy
                   x2 - x1); //dx
            coords.Add(x1); coords.Add(y1); coords.Add(0); coords.Add(rad1);
            coords.Add(x1); coords.Add(y1); coords.Add(1); coords.Add(rad1);
            coords.Add(x2); coords.Add(y2); coords.Add(0); coords.Add(rad1);
            coords.Add(x2); coords.Add(y2); coords.Add(1); coords.Add(rad1);
        }
    }
    public struct InternalGraphicsPath
    {
        internal readonly List<Figure> figures;
        private InternalGraphicsPath(List<Figure> figures)
        {
            this.figures = figures;
        }
        public static InternalGraphicsPath CreatePolygonGraphicsPath(float[] xycoords)
        {
            List<Figure> figures = new List<Figure>(1);
            figures.Add(new Figure(xycoords));
            return new InternalGraphicsPath(figures);
        }
        public static InternalGraphicsPath CreateGraphicsPath(VertexStoreSnap vxsSnap)
        {
            VertexSnapIter vxsIter = vxsSnap.GetVertexSnapIter();
            double prevX = 0;
            double prevY = 0;
            double prevMoveToX = 0;
            double prevMoveToY = 0;
            //TODO: reivew here 
            //about how to reuse this list
            List<List<float>> allXYlist = new List<List<float>>(); //all include sub path
            List<float> xylist = new List<float>();
            allXYlist.Add(xylist);
            bool isAddToList = true;
            //bool vxsMoreThan1 =  vxsSnap.VxsHasMoreThanOnePart;
            for (;;)
            {
                double x, y;
                VertexCmd cmd = vxsIter.GetNextVertex(out x, out y);
                switch (cmd)
                {
                    case PixelFarm.Agg.VertexCmd.MoveTo:
                        if (!isAddToList)
                        {
                            allXYlist.Add(xylist);
                            isAddToList = true;
                        }
                        prevMoveToX = prevX = x;
                        prevMoveToY = prevY = y;
                        xylist.Add((float)x);
                        xylist.Add((float)y);
                        break;
                    case PixelFarm.Agg.VertexCmd.LineTo:
                        xylist.Add((float)x);
                        xylist.Add((float)y);
                        prevX = x;
                        prevY = y;
                        break;
                    case PixelFarm.Agg.VertexCmd.Close:
                        //from current point 
                        xylist.Add((float)prevMoveToX);
                        xylist.Add((float)prevMoveToY);
                        prevX = prevMoveToX;
                        prevY = prevMoveToY;
                        //xylist = new List<float>();
                        //isAddToList = false;
                        break;
                    case VertexCmd.CloseAndEndFigure:
                        //from current point 
                        xylist.Add((float)prevMoveToX);
                        xylist.Add((float)prevMoveToY);
                        prevX = prevMoveToX;
                        prevY = prevMoveToY;
                        //
                        xylist = new List<float>();
                        isAddToList = false;
                        break;
                    case PixelFarm.Agg.VertexCmd.NoMore:
                        goto EXIT_LOOP;
                    default:
                        throw new System.NotSupportedException();
                }
            }
            EXIT_LOOP:

            int j = allXYlist.Count;
            List<Figure> figures = new List<Figure>(j);
            for (int i = 0; i < j; ++i)
            {
                figures.Add(new Figure(allXYlist[i].ToArray()));
            }
            return new InternalGraphicsPath(figures);
        }
    }

    class GLRenderVx : PixelFarm.Drawing.RenderVx
    {
        internal InternalGraphicsPath gxpth;
        public GLRenderVx(InternalGraphicsPath gxpth)
        {
            this.gxpth = gxpth;
        }
    }
    class GLRenderVxFormattedString : PixelFarm.Drawing.RenderVxFormattedString
    {
        public GLRenderVxFormattedString(string str)
        {
            this.OriginalString = str;
        }
    }
}