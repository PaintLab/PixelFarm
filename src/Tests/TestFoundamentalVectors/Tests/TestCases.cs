//MIT, 2017, WinterDev
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using PixelFarm.VectorMath;

namespace TestFoundamentalVectors
{


    class TestCases
    {
        public void TestLineCut(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;

            System.Drawing.PointF p0 = new System.Drawing.PointF(0, 0);
            System.Drawing.PointF p1 = new System.Drawing.PointF(100, 150);
            System.Drawing.PointF p2 = new System.Drawing.PointF(20, 120);
            System.Drawing.PointF p3 = new System.Drawing.PointF(80, 0);

            g.DrawLine(Pens.Red, p0, p1);
            g.DrawLine(Pens.Black, p2, p3);

            System.Drawing.PointF cutPoint = FindCutPoint(p0, p1, p2, p3);
            g.FillRectangle(Brushes.Blue, cutPoint.X, cutPoint.Y, 3, 3);

        }
        public void TestLines(Graphics g)
        {

            int lineLen = 10;
            int x0 = 30;
            int y0 = 30;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            //
            g.Clear(Color.White);

            //1. 
            using (GraphicsPath gxPath = new GraphicsPath())
            {
                for (int i = 0; i < 360; i += 15)
                {
                    //draw from (x0,y0) to (x1,y1) with specific line length
                    gxPath.AddLine(x0, y0,
                        (float)(x0 + lineLen * Math.Cos(DegToRad(i))),
                        (float)(y0 + lineLen * Math.Sin(DegToRad(i))));
                    y0 += 5; //increse y in each steps
                             //
                             //we need to draw separate line
                    gxPath.CloseFigure();
                }

                y0 = 200;
                for (int i = 0; i < 360; i += 15)
                {
                    //draw from (x0,y0) to (x1,y1) with specific line length
                    gxPath.AddLine(x0, y0,
                        (float)(x0 + lineLen * Math.Cos(DegToRad(i))),
                        (float)(y0 + lineLen * Math.Sin(DegToRad(i))));

                    //
                    //we need to draw separate line
                    gxPath.CloseFigure();
                }
                g.DrawPath(Pens.Black, gxPath);
            }
            //
            //2.

            y0 = 300;
            using (GraphicsPath gxPath = new GraphicsPath())
            {
                float x1 = x0 + 100;
                float y1 = y0 + 150;
                gxPath.AddLine(x0, y0, x1, y1);
                gxPath.CloseFigure();
                //
                Vector v0 = new Vector(x0, y0);
                Vector v1 = new Vector(x1, y1);
                Vector midPoint = (v1 + v0) / 2;

                Vector newTop, newBottom, delta;


                GeneratePerpendicularLines(x0, y0, x1, y1, 5, out delta);
                {
                    newTop = midPoint + delta;
                    gxPath.AddLine((float)midPoint.X, (float)midPoint.Y, (float)newTop.X, (float)newTop.Y);
                    gxPath.CloseFigure();
                    //
                    newBottom = midPoint - delta;
                    gxPath.AddLine((float)midPoint.X, (float)midPoint.Y, (float)newBottom.X, (float)newBottom.Y);
                    gxPath.CloseFigure();
                }

                {
                    newTop = v0 + delta;
                    gxPath.AddLine((float)v0.X, (float)v0.Y, (float)newTop.X, (float)newTop.Y);
                    gxPath.CloseFigure();
                    //
                    newBottom = v0 - delta;
                    gxPath.AddLine((float)v0.X, (float)v0.Y, (float)newBottom.X, (float)newBottom.Y);
                    gxPath.CloseFigure();
                }
                {
                    newTop = v1 + delta;
                    gxPath.AddLine((float)v1.X, (float)v1.Y, (float)newTop.X, (float)newTop.Y);
                    gxPath.CloseFigure();
                    //
                    newBottom = v1 - delta;
                    gxPath.AddLine((float)v1.X, (float)v1.Y, (float)newBottom.X, (float)newBottom.Y);
                    gxPath.CloseFigure();
                }


                //
                g.DrawPath(Pens.Black, gxPath);
            }
        }
        static double DegToRad(double degree)
        {
            return degree * (Math.PI / 180d);
        }
        static double RadToDeg(double degree)
        {
            return degree * (180d / Math.PI);
        }
        void GeneratePerpendicularLines(
            float x0, float y0, float x1, float y1, float len,
            out Vector delta)
        {
            Vector v0 = new Vector(x0, y0);
            Vector v1 = new Vector(x1, y1);

            delta = (v1 - v0) / 2; // 2,4 etc 
                                   //midpoint

            delta = delta.NewLength(len);
            delta.Rotate(90);

        }

        static System.Drawing.PointF FindCutPoint(System.Drawing.PointF p0, System.Drawing.PointF p1, System.Drawing.PointF p2, float cutAngle)
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
                return new System.Drawing.PointF(p1.X, p2.Y);
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
            double angle = Math.Atan2(y1diff, x1diff); //rad in degree 
                                                       //double m2 = -1 / m1;

            double m2 = cutAngle == 90 ?
                //short cut
                (-1 / m1) :
                //or 
                Math.Tan(
                //radial_angle of original line + radial of cutAngle
                //return new line slope
                Math.Atan2(y1diff, x1diff) +
                DegreesToRadians(cutAngle)); //new m 
            //---------------------


            //from (6)
            double b2 = p2.Y - (m2) * p2.X;
            //find cut point

            //check if (m1-m2 !=0)
            double cutx = (b2 - b1) / (m1 - m2); //from  (14)
            double cuty = (m1 * cutx) + b1;  //from (15)
            return new System.Drawing.PointF((float)cutx, (float)cuty);


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
        static System.Drawing.PointF FindCutPoint(
            System.Drawing.PointF p0, System.Drawing.PointF p1,
            System.Drawing.PointF p2, System.Drawing.PointF p3)
        {
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
                return new System.Drawing.PointF(p1.X, p2.Y);
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
            return new System.Drawing.PointF((float)cutx, (float)cuty);

        }
        const double degToRad = System.Math.PI / 180.0f;
        const double radToDeg = 180.0f / System.Math.PI;
        /// <summary>
        /// Convert degrees to radians
        /// </summary>
        /// <param name="degrees">An angle in degrees</param>
        /// <returns>The angle expressed in radians</returns>
        public static double DegreesToRadians(double degrees)
        {

            return degrees * degToRad;
        }

        /// <summary>
        /// Convert radians to degrees
        /// </summary>
        /// <param name="radians">An angle in radians</param>
        /// <returns>The angle expressed in degrees</returns>
        public static double RadiansToDegrees(double radians)
        {

            return radians * radToDeg;
        }
    }

}