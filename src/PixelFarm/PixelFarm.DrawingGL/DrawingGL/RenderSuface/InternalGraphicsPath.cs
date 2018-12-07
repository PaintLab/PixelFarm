//MIT, 2016-present, WinterDev

using System.Collections.Generic;

namespace PixelFarm.DrawingGL
{
    class MultiFigures
    {
        public float[] areaTess;
        List<Figure> _figures = new List<Figure>();
        List<float> _coordXYs = new List<float>();
        List<int> _contourEndPoints = new List<int>();
        float[] _smoothBorderTess; //smooth border result

        int _tessAreaVertexCount;
        public MultiFigures() { }
        public int TessAreaVertexCount => _tessAreaVertexCount;
        public void LoadFigure(Figure figure)
        {
            _figures.Add(figure);
            _coordXYs.AddRange(figure.coordXYs);
            _contourEndPoints.Add(_coordXYs.Count - 1);
        }
        public float[] GetAreaTess(TessTool tess)
        {
            //triangle list                
            return areaTess ??
                   (areaTess = tess.TessAsTriVertexArray(_coordXYs.ToArray(),
                   _contourEndPoints.ToArray(),
                   out _tessAreaVertexCount));
        }

        //------------
        int _borderTriangleStripCount;//for smoothborder
        public float[] GetSmoothBorders(SmoothBorderBuilder smoothBorderBuilder)
        {
            //return existing result if not null
            //or create a newone if the old result is not exist

            return _smoothBorderTess ??
                    (_smoothBorderTess =
                    smoothBorderBuilder.BuildSmoothBorders(_figures, IsClosedFigure, out _borderTriangleStripCount));
        }
        public int BorderTriangleStripCount => _borderTriangleStripCount;
        public bool IsClosedFigure { get; set; }
    }



    class Figure
    {
        //TODO: review here again*** 
        public float[] coordXYs; //this is user provide coord
        //---------
        //system tess ...
        public float[] areaTess;
        float[] _smoothBorderTess; //smooth border result
        int _borderTriangleStripCount;//for smoothborder
        int _tessAreaVertexCount;

        //---------
        public ushort[] indexListArray;//for VBO
        float[] _tessXYCoords2;//for VBO         
        VertexBufferObject _vboArea;
        //---------

        public Figure(float[] coordXYs)
        {
            this.coordXYs = coordXYs;
        }
        public bool IsClosedFigure { get; set; }
        public int BorderTriangleStripCount => _borderTriangleStripCount;
        public int TessAreaVertexCount => _tessAreaVertexCount;
        public bool SupportVertexBuffer { get; set; }
        public float[] GetSmoothBorders(SmoothBorderBuilder smoothBorderBuilder)
        {
            //return existing result if not null
            //or create a newone if the old result is not exist

            return _smoothBorderTess ??
                    (_smoothBorderTess =
                    smoothBorderBuilder.BuildSmoothBorders(coordXYs, IsClosedFigure, out _borderTriangleStripCount));
        }

        public float[] GetAreaTess(TessTool tess)
        {
            //triangle list                
            return areaTess ??
                   (areaTess = tess.TessAsTriVertexArray(coordXYs, null, out _tessAreaVertexCount));
        }
        /// <summary>
        /// vertex buffer of the solid area part
        /// </summary>
        public VertexBufferObject GetAreaTessAsVBO(TessTool tess)
        {
            if (_vboArea == null)
            {
                //tess
                indexListArray = tess.TessAsTriIndexArray(coordXYs, null,
                    out _tessXYCoords2,
                    out _tessAreaVertexCount);
                _vboArea = new VertexBufferObject();
                _vboArea.CreateBuffers(_tessXYCoords2, indexListArray);
            }
            return _vboArea;
        }

    }
    class SmoothBorderBuilder
    {
        List<float> _expandCoords = new List<float>();

        public float[] BuildSmoothBorders(float[] coordXYs, bool isClosedFigure, out int borderTriangleStripCount)
        {
            _expandCoords.Clear();

            float[] coords = coordXYs;
            int coordCount = coordXYs.Length;

            //from user input coords
            //expand it
            //TODO: review this again***

            if (isClosedFigure)
            {
                int lim = coordCount - 2;
                for (int i = 0; i < lim;)
                {
                    CreateSmoothLineSegment(_expandCoords, coords[i], coords[i + 1], coords[i + 2], coords[i + 3]);
                    i += 2;
                }
                //close coord
                CreateSmoothLineSegment(_expandCoords, coords[coordCount - 2], coords[coordCount - 1], coords[0], coords[1]);
                borderTriangleStripCount = coordCount * 2;

            }
            else
            {

                int lim = coordCount - 2;
                for (int i = 0; i < lim;)
                {
                    CreateSmoothLineSegment(_expandCoords, coords[i], coords[i + 1], coords[i + 2], coords[i + 3]);
                    i += 2;
                }
                //TODO: review here
                //close coord-- TEMP fix***
                CreateSmoothLineSegment(_expandCoords, coords[coordCount - 2], coords[coordCount - 1], coords[coordCount - 2] + 0.01f, coords[coordCount - 1] + 0.01f);
                borderTriangleStripCount = coordCount * 2;
            }
            float[] result = _expandCoords.ToArray();
            _expandCoords.Clear();
            //
            return result;
        }
        public float[] BuildSmoothBorders(List<Figure> figures, bool isClosedFigure, out int borderTriangleStripCount)
        {
            int j = figures.Count;
            List<float> totalCoordXys = new List<float>();
            int total_b_triangleStripCount = 0;
            for (int i = 0; i < j; ++i)
            {
                Figure fig = figures[i];


                float[] tessResult = BuildSmoothBorders(fig.coordXYs, fig.IsClosedFigure, out int b_triangleStripCount);

                if (i > 0)
                {
                    //add degenerative triangle
                    //for GLES30 consider=>GL_PRIMITIVE_RESTART_FIXED_INDEX
                    //see https://developer.apple.com/library/archive/documentation/3DDrawing/Conceptual/OpenGLES_ProgrammingGuide/TechniquesforWorkingwithVertexData/TechniquesforWorkingwithVertexData.html


                    //To merge two triangle strips, duplicate the last vertex of the first strip and the first vertex of the second strip,
                    //as shown in Figure 8-6. When this strip is submitted to OpenGL ES, 
                    //triangles DEE, EEF, EFF, and FFG are considered degenerate and not processed or rasterized.

                    float prev_x = totalCoordXys[totalCoordXys.Count - 4];
                    float prev_y = totalCoordXys[totalCoordXys.Count - 3];
                    float prev_z = totalCoordXys[totalCoordXys.Count - 2];
                    float prev_w = totalCoordXys[totalCoordXys.Count - 1];

                    //***
                    totalCoordXys.Add(prev_x);
                    totalCoordXys.Add(prev_y);
                    totalCoordXys.Add(prev_z);
                    totalCoordXys.Add(prev_w);
                    //***
                    totalCoordXys.Add(tessResult[0]);
                    totalCoordXys.Add(tessResult[1]);
                    totalCoordXys.Add(tessResult[2]);
                    totalCoordXys.Add(tessResult[3]);

                    b_triangleStripCount += 1;//***
                }

                totalCoordXys.AddRange(tessResult);
                total_b_triangleStripCount += b_triangleStripCount;
            }

            borderTriangleStripCount = total_b_triangleStripCount;
            return totalCoordXys.ToArray();

        }
        public float[] BuildSmoothBorders(float[] coordXYs, int segStartAt, int len, out int borderTriangleStripCount)
        {
            _expandCoords.Clear();
            float[] coords = coordXYs;
            //from user input coords
            //expand it
            //TODO: review this again***

            int lim = (segStartAt + len);
            for (int i = segStartAt; i < lim;)
            {
                CreateSmoothLineSegment(_expandCoords,/*x0*/ coords[i], /*y0*/coords[i + 1],/*x1*/ coords[i + 2],/*y1*/ coords[i + 3]);
                i += 2;
            }
            //close coord
            int last = segStartAt + len;
            CreateSmoothLineSegment(_expandCoords, coords[last], coords[last + 1], coords[segStartAt], coords[segStartAt + 1]);

            borderTriangleStripCount = (len + 2) * 2;
            //
            float[] result = _expandCoords.ToArray();
            _expandCoords.Clear();
            //
            return result;
        }
        static void CreateSmoothLineSegment(List<float> coords, float x1, float y1, float x2, float y2)
        {
            //create with no line join
            //TODO: implement line join ***
            //we can calculate rad on server-side (GPU), so=> reduce num of vertex

            //from https://blog.mapbox.com/drawing-antialiased-lines-with-opengl-8766f34192dc

            float rad1 = (float)System.Math.Atan2(
                   y2 - y1,  //dy
                   x2 - x1); //dx
            coords.Add(x1); coords.Add(y1); coords.Add(0); coords.Add(rad1); //1 vertex
            coords.Add(x1); coords.Add(y1); coords.Add(1); coords.Add(rad1); //1 vertex
            coords.Add(x2); coords.Add(y2); coords.Add(0); coords.Add(rad1); //1 vertex
            coords.Add(x2); coords.Add(y2); coords.Add(1); coords.Add(rad1); //1 vertex
        }
    }


    /// <summary>
    /// a wrapper of internal private class
    /// </summary>
    public struct InternalGraphicsPath
    {
        //since Figure is private=> we use this to expose to public


        readonly Figure _figure;
        readonly List<Figure> _figures;
        MultiFigures _multiFigures;

        internal InternalGraphicsPath(List<Figure> figures)
        {
            _figure = null;
            //_mutltiPartTess = null;
            _multiFigures = null;
            _figures = figures;
        }
        internal InternalGraphicsPath(Figure fig)
        {
            _figures = null;

            _multiFigures = null;
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
                if (_figures != null)
                {
                    return _figures.Count;
                }
                return 0;
            }
        }
        internal Figure GetFig(int index)
        {
            if (index == 0)
            {
                return _figure ?? _figures[0];
            }
            else
            {
                return _figures[index];
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
    public class GLRenderVxFormattedString : PixelFarm.Drawing.RenderVxFormattedString
    {
        char[] _charBuffer;
        DrawingGL.VertexBufferObject2 _vbo2;

        internal GLRenderVxFormattedString(char[] charBuffer)
        {
            _charBuffer = charBuffer;
        }
        public override string OriginalString
        {
            get { return new string(_charBuffer); }
        }
        public float[] VertexCoords { get; set; }
        public ushort[] IndexArray { get; set; }
        public int VertexCount { get; set; }

        public DrawingGL.VertexBufferObject2 GetVbo()
        {
            if (_vbo2 != null)
            {
                return _vbo2;
            }

            _vbo2 = new VertexBufferObject2();
            _vbo2.CreateBuffers(this.VertexCoords, this.IndexArray);
            return _vbo2;
        }
        public override void Dispose()
        {
            //
            base.Dispose();
        }
    }
}