//MIT, 2017, WinterDev
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace TestFoundamentalVectors
{
   

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