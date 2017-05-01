//MIT, 2016-2017, WinterDev
using System;
using System.Numerics;

namespace Typography.Rendering
{
    public class GlyphEdge
    {
        internal readonly EdgeLine _edgeLine;
        public readonly GlyphPoint _P;
        public readonly GlyphPoint _Q;
        Vector2 _newMidPoint;

        /// <summary>
        /// calculated edge CutX  from 2 outside cutpoint (E0,E1)
        /// </summary>
        public float newEdgeCut_P_X;
        /// <summary>
        /// calculated edge CutY  from 2 outside cutpoint (E0,E1)
        /// </summary>
        public float newEdgeCut_P_Y;


        //---------------------
        public float newEdgeCut_Q_X;
        /// <summary>
        /// calculated edge CutY  from 2 outside cutpoint (E0,E1)
        /// </summary>
        public float newEdgeCut_Q_Y;
        //---------------------

#if DEBUG
        public static int dbugTotalId;
        public readonly int dbugId = dbugTotalId++;
#endif
        internal GlyphEdge(GlyphPoint p0, GlyphPoint p1, EdgeLine edgeLine)
        {
#if DEBUG
            edgeLine.dbugGlyphEdge = this;
#endif
            this._P = p0;
            this._Q = p1;
            this._edgeLine = edgeLine;
            _newMidPoint = this.GetMidPoint(); //original 
        }
        internal void FindPerpendicularBones()
        {
            _edgeLine.GlyphPoint_P.EvaluatePerpendicularBone();
            _edgeLine.GlyphPoint_Q.EvaluatePerpendicularBone();
        }
        /// <summary>
        /// get mid point of master outline
        /// </summary>
        /// <returns></returns>
        public Vector2 GetMidPoint()
        {
            return new Vector2((_P.x + _Q.x) / 2, (_P.y + _Q.y) / 2);
        }
        public Vector2 GetNewMidPoint()
        {
            return _newMidPoint;
        }

        public Vector2 NewPos_P
        {
            get { return new Vector2(_P.newX, _P.newY); }
        }
        public Vector2 NewPos_Q
        {
            get { return new Vector2(_Q.newX, _Q.newY); }
        }
        Vector2 GetEdgeVector()
        {
            GlyphPoint p0 = this._P, p1 = this._Q;
            return new Vector2((float)(p1.x - p0.x), (float)(p1.y - p0.y));
        }



        internal void ApplyNewEdgeFromMasterOutline(float newEdgeOffsetFromMasterOutline)
        {

            //TODO: refactor here...
            //this is relative len from current edge              
            //origianl vector
            Vector2 _o_edgeVector = GetEdgeVector();
            //rotate 90
            Vector2 _rotate = _o_edgeVector.Rotate(90);
            //
            Vector2 _deltaVector = _rotate.NewLength(newEdgeOffsetFromMasterOutline);
            //new len  
            _newMidPoint = GetMidPoint() + _deltaVector;
        }

        public static void UpdateEdgeCutPoint(GlyphEdge e0, GlyphEdge e1)
        {

            //TODO: refactor here...
            //find cutpoint from e0.q to e1.p 
            //new sample

            Vector2 tmp_e0_q = e0._newMidPoint + e0.GetEdgeVector();
            Vector2 tmp_e1_p = e1._newMidPoint - e1.GetEdgeVector();

            Vector2 cutpoint;
            if (MyMath.FindCutPoint(e0._newMidPoint, tmp_e0_q, e1._newMidPoint, tmp_e1_p, out cutpoint))
            {
                e0.newEdgeCut_Q_X = e1.newEdgeCut_P_X = cutpoint.X;
                e0.newEdgeCut_Q_Y = e1.newEdgeCut_P_Y = cutpoint.Y;
            }
            else
            {
                //2 edge is pararell
            }

        }


#if DEBUG
        public EdgeLine dbugGetInternalEdgeLine()
        {
            return this._edgeLine;
        }
        public override string ToString()
        {
            return this._P + "=>" + this._Q;
        }
#endif
    }

}