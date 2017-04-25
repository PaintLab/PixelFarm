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


        internal GlyphEdge(GlyphPoint p0, GlyphPoint p1, EdgeLine edgeLine)
        {
            this._P = p0;
            this._Q = p1;
            this._edgeLine = edgeLine;
            _newMidPoint = this.GetMidPoint(); //original
            //----------- 
#if DEBUG
            edgeLine.dbugGlyphEdge = this;
#endif
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
            Vector2 cutpoint = FindCutPoint(e0._newMidPoint, tmp_e0_q, e1._newMidPoint, tmp_e1_p);

            e0.newEdgeCut_Q_X = e1.newEdgeCut_P_X = cutpoint.X;
            e0.newEdgeCut_Q_Y = e1.newEdgeCut_P_Y = cutpoint.Y;

        }
        static Vector2 FindCutPoint(
            Vector2 p0, Vector2 p1,
            Vector2 p2, Vector2 p3)
        {
            //TODO: refactor here...




            //find cut point of 2 line 
            //y = mx + b
            //from line equation
            //y = mx + b ... (1)
            //from (1)
            //b = y- mx ... (2) 
            //----------------------------------
            //line1:
            //y1 = (m1 * x1) + b1 ...(3)            
            //line2:
            //y2 = (m2 * x2) + b2 ...(4)
            //----------------------------------
            //from (3),
            //b1 = y1 - (m1 * x1) ...(5)
            //b2 = y2 - (m2 * x2) ...(6)
            //----------------------------------
            //at cutpoint of line1 and line2 => (x1,y1)== (x2,y2)
            //or find (x,y) where (3)==(4)
            //---------------------------------- 
            //at cutpoint, find x
            // (m1 * x1) + b1 = (m2 * x1) + b2  ...(11), replace x2 with x1
            // (m1 * x1) - (m2 * x1) = b2 - b1  ...(12)
            //  x1 * (m1-m2) = b2 - b1          ...(13)
            //  x1 = (b2-b1)/(m1-m2)            ...(14), now we know x1
            //---------------------------------- 
            //at cutpoint, find y
            //  y1 = (m1 * x1) + b1 ... (15), replace x1 with value from (14)
            //Ans: (x1,y1)
            //----------------------------------

            double y1diff = p1.Y - p0.Y;
            double x1diff = p1.X - p0.X;


            if (x1diff == 0)
            {
                //90 or 180 degree
                return new Vector2(p1.X, p2.Y);
            }
            if (y1diff == 0)
            {
                //90 or 180 degree
                return new Vector2(p2.X, p1.Y);
            }
            //------------------------------
            //
            //find slope 
            double m1 = y1diff / x1diff;
            //from (2) b = y-mx, and (5)
            //so ...
            double b1 = p0.Y - (m1 * p0.X);

            //------------------------------
            double y2diff = p3.Y - p2.Y;
            double x2diff = p3.X - p2.X;

            double m2 = y2diff / x2diff;

            // 
            //from (6)
            double b2 = p2.Y - (m2) * p2.X;
            //find cut point

            //check if (m1-m2 !=0)
            double cutx = (b2 - b1) / (m1 - m2); //from  (14)
            double cuty = (m1 * cutx) + b1;  //from (15)
            return new Vector2((float)cutx, (float)cuty);

        }
      
#if DEBUG

        public override string ToString()
        {
            return this._P + "=>" + this._Q;
        }
#endif
    }

}