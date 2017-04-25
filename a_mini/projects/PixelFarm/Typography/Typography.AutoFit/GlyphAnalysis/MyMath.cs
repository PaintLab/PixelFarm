//MIT, 2017, WinterDev
using System.Numerics;
namespace Typography.Rendering
{

    public static class MyMath
    {

        /// <summary>
        /// Convert degrees to radians
        /// </summary>
        /// <param name="degrees">An angle in degrees</param>
        /// <returns>The angle expressed in radians</returns>
        public static double DegreesToRadians(double degrees)
        {
            const double degToRad = System.Math.PI / 180.0f;
            return degrees * degToRad;
        }
        public static bool MinDistanceFirst(Vector2 baseVec, Vector2 compare0, Vector2 compare1)
        {
            return (SquareDistance(baseVec, compare0) < SquareDistance(baseVec, compare1)) ? true : false;
        }

        public static double SquareDistance(Vector2 v0, Vector2 v1)
        {
            double xdiff = v1.X - v0.X;
            double ydiff = v1.Y - v0.Y;
            return (xdiff * xdiff) + (ydiff * ydiff);
        }
        public static int Min(double v0, double v1, double v2)
        {
            //find min of 3
            unsafe
            {
                double* doubleArr = stackalloc double[3];
                doubleArr[0] = v0;
                doubleArr[1] = v1;
                doubleArr[2] = v2;

                double min = double.MaxValue;
                int foundAt = 0;
                for (int i = 0; i < 3; ++i)
                {
                    if (doubleArr[i] < min)
                    {
                        foundAt = i;
                        min = doubleArr[i];
                    }
                }
                return foundAt;
            }

        }
        /// <summary>
        /// find cut point and check if the cut point is on the edge
        /// </summary>
        /// <param name="edge"></param>
        /// <param name="p2"></param>
        /// <param name="cutResult"></param>
        /// <returns></returns>
        public static bool FindPerpendicularCutPoint(EdgeLine edge, Vector2 p2, out Vector2 cutResult)
        {
            cutResult = FindPerpendicularCutPoint(
                new Vector2((float)edge.x0, (float)edge.y0),
                new Vector2((float)edge.x1, (float)edge.y1),
                p2);
            //also check if result cutpoint is on current line segment or not

            Vector2 min, max;
            GetMinMax(edge, out min, out max);
            return (cutResult.X >= min.X && cutResult.X <= max.X && cutResult.Y >= min.Y && cutResult.Y <= max.Y);
        }
        /// <summary>
        /// which one is min,max
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        static void GetMinMax(EdgeLine edge, out Vector2 min, out Vector2 max)
        {
            Vector2 a_pos = new Vector2((float)edge.x0, (float)edge.y0);
            Vector2 b_pos = new Vector2((float)edge.x1, (float)edge.y1);
            min = Vector2.Min(a_pos, b_pos);
            max = Vector2.Max(a_pos, b_pos);
        }
        static void GetMinMax(Vector2 a_pos, Vector2 b_pos, out Vector2 min, out Vector2 max)
        {

            min = Vector2.Min(a_pos, b_pos);
            max = Vector2.Max(a_pos, b_pos);
        }
        public static int FindMin(Vector2 a, Vector2 b)
        {
            if (a.X < b.X)
            {
                return 0;
            }
            else if (a.X > b.X)
            {
                return 1;
            }
            else
            {
                if (a.Y < b.Y)
                {
                    return 0;
                }
                else if (a.Y > b.Y)
                {
                    return 1;
                }
                else
                {
                    return -1;//eq
                }
            }
        }



        static void GetMinMax(GlyphBone bone, out Vector2 min, out Vector2 max)
        {
            if (bone.JointB != null)
            {
                var a_pos = bone.JointA.Position;
                var b_pos = bone.JointB.Position;

                min = Vector2.Min(a_pos, b_pos);
                max = Vector2.Max(a_pos, b_pos);

            }
            else if (bone.TipEdge != null)
            {
                var a_pos = bone.JointA.Position;
                var tip_pos = bone.TipEdge.GetMidPoint();
                min = Vector2.Min(a_pos, tip_pos);
                max = Vector2.Max(a_pos, tip_pos);
            }
            else
            {
                throw new System.NotSupportedException();
            }
        }
        /// <summary>
        /// find a perpendicular cut-point from p to bone
        /// </summary>
        /// <param name="bone"></param>
        /// <param name="p"></param>
        /// <param name="cutPoint"></param>
        /// <returns></returns>
        public static bool FindPerpendicularCutPoint(GlyphBone bone, Vector2 p, out Vector2 cutPoint)
        {
            if (bone.JointB != null)
            {
                cutPoint = FindPerpendicularCutPoint(
                  bone.JointA.Position,
                  bone.JointB.Position,
                  p);
                //find min /max
                Vector2 min, max;
                GetMinMax(bone, out min, out max);
                return cutPoint.X >= min.X && cutPoint.X <= max.X && cutPoint.Y >= min.Y && cutPoint.Y <= max.Y;
            }
            else
            {
                //to tip
                if (bone.TipEdge != null)
                {
                    cutPoint = FindPerpendicularCutPoint(
                        bone.JointA.Position,
                        bone.TipEdge.GetMidPoint(),
                        p);
                    Vector2 min, max;
                    GetMinMax(bone, out min, out max);
                    return cutPoint.X >= min.X && cutPoint.X <= max.X && cutPoint.Y >= min.Y && cutPoint.Y <= max.Y;
                }
                else
                {
                    throw new System.NotSupportedException();
                }
            }
        }
        public static Vector2 FindPerpendicularCutPoint(Vector2 p0, Vector2 p1, Vector2 p2)
        {
            //a line from p0 to p1
            //p2 is any point
            //return p3 -> cutpoint on p0,p1 


            double xdiff = p1.X - p0.X;
            double ydiff = p1.Y - p0.Y;
            if (xdiff == 0)
            {
                return new Vector2(p1.X, p2.Y);
            }
            if (ydiff == 0)
            {
                return new Vector2(p2.X, p1.Y);
            }

            double m1 = ydiff / xdiff;
            double b1 = FindB(p0, p1);

            double m2 = -1 / m1;
            double b2 = p2.Y - (m2) * p2.X;
            //find cut point
            double cutx = (b2 - b1) / (m1 - m2);
            double cuty = (m2 * cutx) + b2;



            return new Vector2((float)cutx, (float)cuty);
        }
        static Vector2 FindCutPoint(Vector2 p0, Vector2 p1, Vector2 p2, float cutAngle)
        {
            //a line from p0 to p1
            //p2 is any point
            //return p3 -> cutpoint on p0,p1

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
            //y1diff = p1.Y-p0.Y  ...(7)
            //x1diff = p1.X-p0.X  ...(8)
            //
            //m1 = (y1diff/x1diff) ...(9)
            //m2 = cutAngle of m1 ...(10)
            //
            //replace value (x1,y1) and (x2,y2)
            //we know b1 and b2         
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
                return new Vector2(p2.X, p1.Y);
            }
            //------------------------------
            //
            //find slope 
            double m1 = y1diff / x1diff;
            //from (2) b = y-mx, and (5)
            //so ...
            double b1 = p0.Y - (m1 * p0.X);
            // 
            //from (10)
            //double invert_m = -(1 / slope_m);
            //double m2 = -1 / m1;   //rotate m1
            //---------------------
            double angle = System.Math.Atan2(y1diff, x1diff); //rad in degree 
                                                              //double m2 = -1 / m1;

            double m2 = cutAngle == 90 ?
                //short cut
                (-1 / m1) :
                //or 
                System.Math.Tan(
                //radial_angle of original line + radial of cutAngle
                //return new line slope
                System.Math.Atan2(y1diff, x1diff) +
                MyMath.DegreesToRadians(cutAngle)); //new m 
            //---------------------


            //from (6)
            double b2 = p2.Y - (m2) * p2.X;
            //find cut point

            //check if (m1-m2 !=0)
            double cutx = (b2 - b1) / (m1 - m2); //from  (14)
            double cuty = (m1 * cutx) + b1;  //from (15)
            return new Vector2((float)cutx, (float)cuty);
            //------
            //at cutpoint of line1 and line2 => (x1,y1)== (x2,y2)
            //or find (x,y) where (3)==(4)
            //-----
            //if (3)==(4)
            //(m1 * x1) + b1 = (m2 * x2) + b2;
            //from given p0 and p1,
            //now we know m1 and b1, ( from (2),  b1 = y1-(m1*x1) )
            //and we now m2 since => it is a 90 degree of m1.
            //and we also know x2, since at the cut point x2 also =x1
            //now we can find b2...
            // (m1 * x1) + b1 = (m2 * x1) + b2  ...(5), replace x2 with x1
            // b2 = (m1 * x1) + b1 - (m2 * x1)  ...(6), move  (m2 * x1)
            // b2 = ((m1 - m2) * x1) + b1       ...(7), we can find b2
            //---------------------------------------------
        }

        public static bool FindPerpendicularCutPoint2(Vector2 p0, Vector2 p1, Vector2 p2, out Vector2 cutPoint)
        {
            //a line from p0 to p1
            //p2 is any point
            //return p3 -> cutpoint on p0,p1 


            double xdiff = p1.X - p0.X;
            double ydiff = p1.Y - p0.Y;
            if (xdiff == 0)
            {
                //90 or 180 degree
                cutPoint = new Vector2(p1.X, p2.Y);
                Vector2 min, max;
                GetMinMax(p0, p1, out min, out max);
                return cutPoint.X >= min.X && cutPoint.X <= max.X && cutPoint.Y >= min.Y && cutPoint.Y <= max.Y;

            }
            if (ydiff == 0)
            {
                cutPoint = new Vector2(p2.X, p1.Y);
                Vector2 min, max;
                GetMinMax(p0, p1, out min, out max);
                return cutPoint.X >= min.X && cutPoint.X <= max.X && cutPoint.Y >= min.Y && cutPoint.Y <= max.Y;
            }

            double m1 = ydiff / xdiff;
            double b1 = FindB(p0, p1);

            double m2 = -1 / m1;
            double b2 = p2.Y - (m2) * p2.X;
            //find cut point
            double cutx = (b2 - b1) / (m1 - m2);
            double cuty = (m2 * cutx) + b2;
            cutPoint = new Vector2((float)cutx, (float)cuty);
            //
            {
                Vector2 min, max;
                GetMinMax(p0, p1, out min, out max);
                return cutPoint.X >= min.X && cutPoint.X <= max.X && cutPoint.Y >= min.Y && cutPoint.Y <= max.Y;
            }
        }
        static double FindB(Vector2 p0, Vector2 p1)
        {

            double m1 = (p1.Y - p0.Y) / (p1.X - p0.X);
            //y = mx + b ...(1)
            //b = y- mx

            //substitue with known value to gett b 
            //double b0 = p0.Y - (slope_m) * p0.X;
            //double b1 = p1.Y - (slope_m) * p1.X;
            //return b0;

            return p0.Y - (m1) * p0.X;
        }


        internal static readonly double _85degreeToRad = MyMath.DegreesToRadians(85);
        internal static readonly double _15degreeToRad = MyMath.DegreesToRadians(15);
        internal static readonly double _03degreeToRad = MyMath.DegreesToRadians(3);
        internal static readonly double _90degreeToRad = MyMath.DegreesToRadians(90);



        internal static float FindDiffToFitInteger(float actualValue)
        {
            int floor = (int)actualValue;
            float diff = actualValue - floor;
            if (diff >= 0.5)
            {
                //move up
                return (floor + 1) - actualValue;
            }
            else
            {
                //move down
                return actualValue - floor;
            }
        }
    }
}