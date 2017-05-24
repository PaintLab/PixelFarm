//BSD, 2014-2017, WinterDev

/*
 * Created by SharpDevelop.
 * User: lbrubaker
 * Date: 3/26/2010
 * Time: 4:37 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;
using Tesselate;
namespace PixelFarm.DrawingGL
{

    struct TessTempVertex
    {
        public double m_X;
        public double m_Y;
        public TessTempVertex(double x, double y)
        {
            m_X = x;
            m_Y = y;
        }
#if DEBUG
        public override string ToString()
        {
            return this.m_X + "," + this.m_Y;
        }
#endif

    }

    /// <summary>
    /// listen and handle the event from tesslator
    /// </summary>
    class TessListener
    {   
        internal List<TessTempVertex> tempVertextList = new List<TessTempVertex>(); 
        internal List<ushort> resultIndexList = new List<ushort>();
        int inputVertexCount;

        public Tesselator.TriangleListType triangleListType;
        public TessListener()
        {
            //empty not use
            //not use first item in temp
            tempVertextList.Add(new TessTempVertex(0, 0));
        }
        public void BeginCallBack(Tesselator.TriangleListType type)
        {
            if (type != Tesselator.TriangleListType.Triangles)
            {

            }
            this.triangleListType = type;

            //what type of triangle list
            //Console.WriteLine("begin: " + type.ToString());
            //Assert.IsTrue(GetNextOutputAsString() == "B");
            //switch (type)
            //{
            //    case Tesselator.TriangleListType.Triangles:
            //        Assert.IsTrue(GetNextOutputAsString() == "TRI");
            //        break;

            //    case Tesselator.TriangleListType.TriangleFan:
            //        Assert.IsTrue(GetNextOutputAsString() == "FAN");
            //        break;

            //    case Tesselator.TriangleListType.TriangleStrip:
            //        Assert.IsTrue(GetNextOutputAsString() == "STRIP");
            //        break;

            //    default:
            //        throw new Exception("unknown TriangleListType '" + type.ToString() + "'.");
            //}
        }

        public void EndCallBack()
        {
            //Assert.IsTrue(GetNextOutputAsString() == "E");
            //Console.WriteLine("end");
        }

        public void VertexCallBack(int index)
        {
            //Assert.IsTrue(GetNextOutputAsString() == "V");
            //Assert.AreEqual(GetNextOutputAsInt(), index); 
            if (index < 0)
            {
                resultIndexList.Add((ushort)(inputVertexCount + (-index)));
                //use data from temp store***
                //resultVertexList.Add(this.tempVertextList[-index]);
                //Console.WriteLine("temp_v_cb:" + index + ":(" + tempVertextList[-index] + ")");
            }
            else
            {
                resultIndexList.Add((ushort)index);
                //resultVertexList.Add(this.inputVertextList[index]);
                // Console.WriteLine("v_cb:" + index + ":(" + inputVertextList[index] + ")");
            }
        }

        public void EdgeFlagCallBack(bool IsEdge)
        {
            //Console.WriteLine("edge: " + IsEdge);
            //Assert.IsTrue(GetNextOutputAsString() == "F");
            //Assert.AreEqual(GetNextOutputAsBool(), IsEdge);
        }

        public void CombineCallBack(double v0,
            double v1,
            double v2,
            ref Tesselator.CombineParameters combinePars,
            out int outData)
        {
            //double error = .001;
            //Assert.IsTrue(GetNextOutputAsString() == "C");
            //Assert.AreEqual(GetNextOutputAsDouble(), v0, error);
            //Assert.AreEqual(GetNextOutputAsDouble(), v1, error);
            //Assert.AreEqual(GetNextOutputAsInt(), data4[0]);
            //Assert.AreEqual(GetNextOutputAsInt(), data4[1]);
            //Assert.AreEqual(GetNextOutputAsInt(), data4[2]);
            //Assert.AreEqual(GetNextOutputAsInt(), data4[3]);
            //Assert.AreEqual(GetNextOutputAsDouble(), weight4[0], error);
            //Assert.AreEqual(GetNextOutputAsDouble(), weight4[1], error);
            //Assert.AreEqual(GetNextOutputAsDouble(), weight4[2], error);
            //Assert.AreEqual(GetNextOutputAsDouble(), weight4[3], error); 
            //here , outData = index of newly add vertext


            //----------------------------------------------------------------------
            //*** new vertext is added into user vertext list ***            
            //use negative to note that this vertext is from temporary source 

            //other implementation:
            // append to end of input list is ok if the input list can grow up ***
            //----------------------------------------------------------------------
            outData = -this.tempVertextList.Count;
            //----------------------------------------
            tempVertextList.Add(new TessTempVertex(v0, v1));
            //----------------------------------------
        }

        /// <summary>
        /// connect to actual Tesselator
        /// </summary>
        /// <param name="tesselator"></param>
        /// <param name="setEdgeFlag"></param>
        public void Connect(Tesselator tesselator, bool setEdgeFlag)
        {
            tesselator.callBegin = BeginCallBack;
            tesselator.callEnd = EndCallBack;
            tesselator.callVertex = VertexCallBack;
            tesselator.callCombine = CombineCallBack;
            if (setEdgeFlag)
            {
                tesselator.callEdgeFlag = EdgeFlagCallBack;
            }
        }
        /// <summary>
        /// clear previous results and load a new input vertex list
        /// </summary>
        /// <param name="inputVertextList"></param>
        public void ResetAndLoadInputVertexList(int inputVertexCount)
        {
            this.inputVertexCount = inputVertexCount;
            //1. reset
            this.triangleListType = Tesselator.TriangleListType.LineLoop;//?
            this.tempVertextList.Clear(); 
            resultIndexList.Clear(); 
        }
    }


    class TessTool
    {
        internal readonly Tesselator tess;
        internal readonly TessListener tessListener;
        //List<Vertex> vertexts = new List<Vertex>();
        public TessTool() : this(new Tesselator() { WindingRule = Tesselator.WindingRuleType.Odd }) { }
        public TessTool(Tesselator tess)
        {
            this.tess = tess;
            this.tessListener = new TessListener();
            tessListener.Connect(tess, true);
        }


        public float[] TessPolygon(float[] vertex2dCoords, int[] contourEndPoints, out int areaCount)
        {
            
            int ncoords = vertex2dCoords.Length / 2;
            if (ncoords == 0) { areaCount = 0; return null; }
             
            tessListener.ResetAndLoadInputVertexList(ncoords);
            //-----------------------
            tess.BeginPolygon();
            int nContourCount = contourEndPoints.Length;
            int beginAt = 0;
            int n = 0;
            for (int m = 0; m < nContourCount; ++m)
            {
                int thisContourEndAt = (contourEndPoints[m] + 1) / 2;
                tess.BeginContour();
                for (int i = beginAt; i < thisContourEndAt; ++i)
                {
                    n = i * 2;
                    tess.AddVertex(
                        vertex2dCoords[n],
                        vertex2dCoords[n + 1], 0, i);
                }
                beginAt = thisContourEndAt + 1;
                tess.EndContour();
            }
            tess.EndPolygon();
            //-----------------------

            int originalVertexCount = ncoords;
            List<ushort> indexList = tessListener.resultIndexList;
            List<TessTempVertex> tempVertexList = tessListener.tempVertextList;

            //-----------------------------   
            //switch how to fill polygon
            int j = indexList.Count;
            float[] vtx = new float[j * 2];//***
            n = 0; //reset
            for (int p = 0; p < j; ++p)
            {
                ushort index = indexList[p];
                if (index >= ncoords)
                {
                    //extra coord (newly created)
                    TessTempVertex extraVertex = tempVertexList[index - ncoords];
                    vtx[n] = (float)extraVertex.m_X;
                    vtx[n + 1] = (float)extraVertex.m_Y;
                }
                else
                {
                    //original corrd
                    vtx[n] = (float)vertex2dCoords[index * 2];
                    vtx[n + 1] = (float)vertex2dCoords[(index * 2) + 1];
                }
                n += 2;
            }
            //triangle list
            areaCount = j;
            return vtx;
        }

        public ushort[] TessPolygon2(float[] vertex2dCoords, int[] contourEndPoints, out float[] outputCoords, out int areaCount)
        {
             
            int ncoords = vertex2dCoords.Length / 2;
            if (ncoords == 0) { areaCount = 0; outputCoords = null; return null; }
 
            tessListener.ResetAndLoadInputVertexList(ncoords);
            //-----------------------
            tess.BeginPolygon();
            int nContourCount = contourEndPoints.Length;
            int beginAt = 0;
            int n = 0;
            for (int m = 0; m < nContourCount; ++m)
            {
                int thisContourEndAt = (contourEndPoints[m] + 1) / 2;
                tess.BeginContour();
                for (int i = beginAt; i < thisContourEndAt; ++i)
                {
                    n = i * 2;
                    tess.AddVertex(
                        vertex2dCoords[n],
                        vertex2dCoords[n + 1], 0, i);
                }
                beginAt = thisContourEndAt + 1;
                tess.EndContour();
            }
            tess.EndPolygon();
            //-----------------------
            List<ushort> vertextList = tessListener.resultIndexList;
            List<TessTempVertex> tempVertexList = tessListener.tempVertextList;
            //-----------------------------   
            areaCount = vertextList.Count;

            int tempVertListCount = tessListener.tempVertextList.Count;
            outputCoords = new float[vertex2dCoords.Length + tempVertListCount * 2];
            Array.Copy(vertex2dCoords, outputCoords, vertex2dCoords.Length);
            int endAt = vertex2dCoords.Length + tempVertListCount;

            int p = 0;
            int q = vertex2dCoords.Length;
            for (int i = vertex2dCoords.Length; i < endAt; ++i)
            {
                TessTempVertex v = tessListener.tempVertextList[p];
                outputCoords[q] = (float)v.m_X;
                outputCoords[q + 1] = (float)v.m_Y;
                p++;
                q += 2;
            }

            return tessListener.resultIndexList.ToArray();

        }
    }
}