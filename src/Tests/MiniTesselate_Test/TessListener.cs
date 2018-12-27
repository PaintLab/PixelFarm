//BSD, 2014-2018, WinterDev

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
using NUnit.Framework;
using Tesselate;

namespace TessTest
{
    public struct Vertex
    {
        public double m_X;
        public double m_Y;
        public Vertex(double x, double y)
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

    public class TessListener : Tesselator.ITessListener
    {

        List<Vertex> _inputVertextList;
        List<Vertex> _tempVertextList = new List<Vertex>();

        public List<Vertex> resultVertexList = new List<Vertex>();
        public List<int> resultIndexList = new List<int>();


        public TessListener()
        {
            //empty not use
            //not use first item in temp
            _tempVertextList.Add(new Vertex(0, 0));
        }
        void Tesselator.ITessListener.Begin(Tesselator.TriangleListType type)
        {

            Console.WriteLine("begin: " + type.ToString());
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

        void Tesselator.ITessListener.End()
        {
            //Assert.IsTrue(GetNextOutputAsString() == "E");
            Console.WriteLine("end");
        }

        void Tesselator.ITessListener.Vertext(int index)
        {
            //Assert.IsTrue(GetNextOutputAsString() == "V");
            //Assert.AreEqual(GetNextOutputAsInt(), index); 
            resultIndexList.Add(index);

            if (index < 0)
            {
                //use data from temp store
                resultVertexList.Add(this._tempVertextList[-index]);
                Console.WriteLine("temp_v_cb:" + index + ":(" + _tempVertextList[-index] + ")");
            }
            else
            {
                resultVertexList.Add(this._inputVertextList[index]);
                Console.WriteLine("v_cb:" + index + ":(" + _inputVertextList[index] + ")");
            }
        }
        public bool NeedEdgeFlag { get; set; }
        void Tesselator.ITessListener.EdgeFlag(bool boundaryEdge_isEdge)
        {
            Console.WriteLine("edge: " + boundaryEdge_isEdge);
            //Assert.IsTrue(GetNextOutputAsString() == "F");
            //Assert.AreEqual(GetNextOutputAsBool(), IsEdge);
        }
        //public delegate void CallCombineDelegate(
        // double c1, double c2, double c3, ref CombineParameters combinePars, out int outData);
        void Tesselator.ITessListener.Combine(double v0,
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

            outData = -this._tempVertextList.Count;
            //----------------------------------------
            _tempVertextList.Add(new Vertex(v0, v1));
            //----------------------------------------

        }
        public bool NeedMash { get; set; }
        void Tesselator.ITessListener.Mesh(Mesh mesh)
        {

        }
        public void Connect(Tesselate.Tesselator tesselator, bool setEdgeFlag)
        {
            NeedEdgeFlag = setEdgeFlag;
            tesselator.SetListener(this);
        }
        public void Connect(List<Vertex> vertextList,
            Tesselate.Tesselator tesselator,
            Tesselator.WindingRuleType windingRule, bool setEdgeFlag)
        {
            this._inputVertextList = vertextList;

            tesselator.WindingRule = windingRule;
            NeedEdgeFlag = setEdgeFlag;

            tesselator.SetListener(this);
        }
    }
}