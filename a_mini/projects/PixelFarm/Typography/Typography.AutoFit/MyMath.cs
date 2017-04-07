//MIT, 2017, WinterDev
using System.Numerics;
namespace Typography.Rendering
{

    static class MyMath
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

        static double SquareDistance(Vector2 v0, Vector2 v1)
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
        public static Vector2 FindPerpendicularCutPoint(EdgeLine edge, Vector2 p2)
        {
            return FindPerpendicularCutPoint(
                new Vector2((float)edge.x0, (float)edge.y0),
                new Vector2((float)edge.x1, (float)edge.y1),
                p2);
        }
        public static Vector2 FindPerpendicularCutPoint(Vector2 p0, Vector2 p1, Vector2 p2)
        {
            //a line from p0 to p1
            //p2 is any point
            //return p3 -> cutpoint on p0,p1 
            //find slope 

            double xdiff = p1.X - p0.X;
            double ydiff = p1.Y - p0.Y;
            if (xdiff == 0)
            {
                //90 or 180 degree
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