//MIT, 2017, WinterDev
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TestFoundamentalVectors
{
    public struct Vector
    {
        double _x, _y;
        public Vector(double x, double y)
        {
            _x = x; _y = y;
        }
        public Vector(PointF pt)
        {
            _x = pt.X;
            _y = pt.Y;
        }
        public Vector(PointF st, PointF end)
        {
            _x = end.X - st.X;
            _y = end.Y - st.Y;
        }

        public double X
        {
            get { return _x; }
            set { _x = value; }
        }

        public double Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public double Magnitude
        {
            get { return Math.Sqrt(X * X + Y * Y); }
        }

        public static Vector operator +(Vector v1, Vector v2)
        {
            return new Vector(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static Vector operator -(Vector v1, Vector v2)
        {
            return new Vector(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static Vector operator -(Vector v)
        {
            return new Vector(-v.X, -v.Y);
        }

        public static Vector operator *(double c, Vector v)
        {
            return new Vector(c * v.X, c * v.Y);
        }

        public static Vector operator *(Vector v, double c)
        {
            return new Vector(c * v.X, c * v.Y);
        }

        public static Vector operator /(Vector v, double c)
        {
            return new Vector(v.X / c, v.Y / c);
        }

        // A * B =|A|.|B|.sin(angle AOB)
        public double CrossProduct(Vector v)
        {
            return _x * v.Y - v.X * _y;
        }

        // A. B=|A|.|B|.cos(angle AOB)
        public double DotProduct(Vector v)
        {
            return _x * v.X + _y * v.Y;
        }

        public static bool IsClockwise(PointF pt1, PointF pt2, PointF pt3)
        {
            Vector V21 = new Vector(pt2, pt1);
            Vector v23 = new Vector(pt2, pt3);
            return V21.CrossProduct(v23) < 0; // sin(angle pt1 pt2 pt3) > 0, 0<angle pt1 pt2 pt3 <180
        }

        public static bool IsCCW(PointF pt1, PointF pt2, PointF pt3)
        {
            Vector V21 = new Vector(pt2, pt1);
            Vector v23 = new Vector(pt2, pt3);
            return V21.CrossProduct(v23) > 0;  // sin(angle pt2 pt1 pt3) < 0, 180<angle pt2 pt1 pt3 <360
        }

        public static double DistancePointLine(PointF pt, PointF lnA, PointF lnB)
        {
            Vector v1 = new Vector(lnA, lnB);
            Vector v2 = new Vector(lnA, pt);
            v1 /= v1.Magnitude;
            return Math.Abs(v2.CrossProduct(v1));
        }
        public PointF ToPointF()
        {
            return new PointF((float)_x, (float)_y);
        }
        public void Rotate(int Degree)
        {
            double radian = Degree * Math.PI / 180.0;
            double sin = Math.Sin(radian);
            double cos = Math.Cos(radian);
            double nx = _x * cos - _y * sin;
            double ny = _x * sin + _y * cos;
            _x = nx;
            _y = ny;
        }
        public Vector NewLength(double newLength)
        {
            //radian
            double atan = Math.Atan2(_y, _x);
            return new Vector(Math.Cos(atan) * newLength,
                        Math.Sin(atan) * newLength);
        }
    }


    class TestCases
    {
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
    }

}