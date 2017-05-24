//MIT, 2016-2017, WinterDev

using System.Collections.Generic;

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
            get;
            set;

        }
        public void InitVertexBufferIfNeed(TessTool tess)
        {
            if (_vboArea == null)
            {
                GetAreaTess2(tess);
                //create index buffer
                _vboArea = new VertexBufferObject();
                _vboArea.CreateBuffers(coordXYs, indexListArray);
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
        public float[] GetSmoothBorders(SmoothBorderBuilder smoothBorderBuilder)
        {
            if (smoothBorderTess == null)
            {
                return smoothBorderTess = smoothBorderBuilder.BuildSmoothBorders(coordXYs, out borderTriangleStripCount);
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
        public ushort[] indexListArray;

    }



    class SmoothBorderBuilder
    {
        List<float> expandCoords = new List<float>();
        public float[] BuildSmoothBorders(float[] coordXYs, out int borderTriangleStripCount)
        {
            expandCoords.Clear();
            float[] coords = coordXYs;
            int coordCount = coordXYs.Length;
            //from user input coords
            //expand it
            //TODO: review this again***
            int lim = coordCount - 2;
            for (int i = 0; i < lim;)
            {
                CreateLineSegment(expandCoords, coords[i], coords[i + 1], coords[i + 2], coords[i + 3]);
                i += 2;
            }
            //close coord
            CreateLineSegment(expandCoords, coords[coordCount - 2], coords[coordCount - 1], coords[0], coords[1]);

            borderTriangleStripCount = coordCount * 2;
            //
            float[] result = expandCoords.ToArray();
            expandCoords.Clear();
            //
            return result;
        }
        static void CreateLineSegment(List<float> coords, float x1, float y1, float x2, float y2)
        {
            //create wiht no line join
            //TODO: implement line join ***
            //we can calculate rad on server-side, so=> reduce num of vertex
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
    /// <summary>
    /// a wrapper of internal private class
    /// </summary>
    public struct InternalGraphicsPath
    {
        //since Figure is private=> we use this to expose to public


        readonly Figure _figure;
        readonly List<Figure> figures;
        internal InternalGraphicsPath(List<Figure> figures)
        {

            this.figures = figures;
            _figure = null;
        }
        internal InternalGraphicsPath(Figure fig)
        {
            this.figures = null;
            _figure = fig;
        }
      

        internal int FigCount
        {
            get
            {
                if (_figure != null)
                {
                    return 1;
                }
                if (figures != null)
                {
                    return figures.Count;
                }
                return 0;
            }
        }
        internal Figure GetFig(int index)
        {
            if (index == 0)
            {
                return _figure ?? figures[0];
            }
            else
            {
                return figures[index];
            }
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