//MIT, 2017, WinterDev
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

//
using Typography.OpenFont;
//
using DrawingGL;
using DrawingGL.Text;
//


namespace Test_WinForm_TessGlyph
{
    public partial class FormTess : Form
    {
        Graphics g;
        float[] glyphPoints2;
        int[] contourEnds;

        TessTool tessTool = new TessTool();
        public FormTess()
        {
            InitializeComponent();
        }
        private void FormTess_Load(object sender, EventArgs e)
        {
            g = this.pnlGlyph.CreateGraphics();
            pnlGlyph.MouseDown += PnlGlyph_MouseDown;
            //string testFont = "d:\\WImageTest\\DroidSans.ttf";
            string testFont = "c:\\Windows\\Fonts\\Tahoma.ttf";
            using (FileStream fs = new FileStream(testFont, FileMode.Open, FileAccess.Read))
            {
                OpenFontReader reader = new OpenFontReader();
                Typeface typeface = reader.Read(fs);

                //--
                var builder = new Typography.Rendering.GlyphPathBuilder(typeface);
                builder.BuildFromGlyphIndex(typeface.LookupIndex('a'), 256);

                var txToPath = new GlyphTranslatorToPath();
                var writablePath = new WritablePath();
                txToPath.SetOutput(writablePath);
                builder.ReadShapes(txToPath);
                //from contour to  
                var curveFlattener = new SimpleCurveFlattener();
                float[] flattenPoints = curveFlattener.Flatten(writablePath._points, out contourEnds);
                glyphPoints2 = flattenPoints;
                ////--------------------------------------
                ////raw glyph points
                //int j = glyphPoints.Length;
                //float scale = typeface.CalculateToPixelScaleFromPointSize(256);
                //glyphPoints2 = new float[j * 2];
                //int n = 0;
                //for (int i = 0; i < j; ++i)
                //{
                //    GlyphPointF pp = glyphPoints[i];
                //    glyphPoints2[n] = pp.X * scale;
                //    n++;
                //    glyphPoints2[n] = pp.Y * scale;
                //    n++;
                //}
                ////--------------------------------------
            }
        }

        int mdown_x = -1;
        int mdown_y = -1;
        private void PnlGlyph_MouseDown(object sender, MouseEventArgs e)
        {
            mdown_x = e.X;
            mdown_y = e.Y;
            //Test2();//update 
        }

        float[] GetPolygonData(out int[] endContours)
        {
            endContours = this.contourEnds;
            return glyphPoints2;

            ////--
            ////for test

            //return new float[]
            //{
            //        10,10,
            //        200,10,
            //        100,100,
            //        150,200,
            //        20,200,
            //        50,100
            //};
        }
        void DrawOutput()
        {
            //-----------
            //for GDI+ only
            bool drawInvert = chkInvert.Checked;
            int viewHeight = this.pnlGlyph.Height;
            if (drawInvert)
            {
                g.ScaleTransform(1, -1);
                g.TranslateTransform(0, -viewHeight);
            }
            //----------- 
            //show tess
            g.Clear(Color.White);
            int[] contourEndIndices;
            float[] polygon1 = GetPolygonData(out contourEndIndices);


            using (Pen pen1 = new Pen(Color.LightGray, 6))
            {
                int nn = polygon1.Length;
                int a = 0;
                PointF p0;
                PointF p1;

                int contourCount = contourEndIndices.Length;
                int startAt = 3;
                for (int cnt_index = 0; cnt_index < contourCount; ++cnt_index)
                {
                    int endAt = contourEndIndices[cnt_index];
                    for (int m = startAt; m <= endAt;)
                    {
                        p0 = new PointF(polygon1[m - 3], polygon1[m - 2]);
                        p1 = new PointF(polygon1[m - 1], polygon1[m]);
                        g.DrawLine(pen1, p0, p1);
                        g.DrawString(a.ToString(), this.Font, Brushes.Black, p0);
                        m += 2;
                        a++;
                    }
                    //close coutour 

                    p0 = new PointF(polygon1[endAt - 1], polygon1[endAt]);
                    p1 = new PointF(polygon1[startAt - 3], polygon1[startAt - 2]);
                    g.DrawLine(pen1, p0, p1);
                    g.DrawString(a.ToString(), this.Font, Brushes.Black, p0);
                    //
                    startAt = (endAt + 1) + 3;
                }
            }
            int areaCount;
            float[] tessData = tessTool.TessPolygon(polygon1, contourEnds, out areaCount);
            //draw tess 
            int j = tessData.Length;
            for (int i = 0; i < j;)
            {
                var p0 = new PointF(tessData[i], tessData[i + 1]);
                var p1 = new PointF(tessData[i + 2], tessData[i + 3]);
                var p2 = new PointF(tessData[i + 4], tessData[i + 5]);

                g.DrawLine(Pens.Red, p0, p1);
                g.DrawLine(Pens.Red, p1, p2);
                g.DrawLine(Pens.Red, p2, p0);

                i += 6;
            }

            //-----------
            //for GDI+ only
            g.ResetTransform();
            //-----------
        }
        private void cmdDrawGlyph_Click(object sender, EventArgs e)
        {
            DrawOutput();
        }

        private void chkInvert_CheckedChanged(object sender, EventArgs e)
        {
            DrawOutput();
        }

        private void cmdTestDrawCurve_Click(object sender, EventArgs e)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            //Test2();
        }
        void Test1()
        {
            for (int i = 0; i < 10; ++i)
            {
                Point p0 = new Point(0, 0);
                Point p1 = new Point(20, 50);
                Point p2 = new Point(80, 50);
                Point p3 = new Point(100, i * 10);

                g.DrawRectangle(Pens.Red, new Rectangle(p0, new System.Drawing.Size(1, 1)));
                g.DrawRectangle(Pens.Green, new Rectangle(p1, new System.Drawing.Size(1, 1)));
                g.DrawRectangle(Pens.Green, new Rectangle(p2, new System.Drawing.Size(1, 1)));
                g.DrawRectangle(Pens.Red, new Rectangle(p3, new System.Drawing.Size(1, 1)));

                System.Drawing.Drawing2D.GraphicsPath p = new System.Drawing.Drawing2D.GraphicsPath();
                p.AddBezier(p0, p1, p2, p3);
                g.DrawPath(Pens.Black, p);
            }
        }
        //const double degToRad = System.Math.PI / 180.0f;
        //const double radToDeg = 180.0f / System.Math.PI;
        ///// <summary>
        ///// Convert degrees to radians
        ///// </summary>
        ///// <param name="degrees">An angle in degrees</param>
        ///// <returns>The angle expressed in radians</returns>
        //public static double DegreesToRadians(double degrees)
        //{

        //    return degrees * degToRad;
        //}

        ///// <summary>
        ///// Convert radians to degrees
        ///// </summary>
        ///// <param name="radians">An angle in radians</param>
        ///// <returns>The angle expressed in degrees</returns>
        //public static double RadiansToDegrees(double radians)
        //{

        //    return radians * radToDeg;
        //}

        //void Test2()
        //{
        //    g.Clear(Color.White);
        //    //
        //    PointF p0 = new PointF(50, 50);
        //    //PointF p1 = new PointF(100, 120);
        //    //PointF p1 = new PointF(50, 120);
        //    //PointF p1 = new PointF(120, 50);
        //    PointF p1 = new PointF(120, 100);

        //    g.DrawLine(Pens.Red, p0, p1);

        //    float cutAngle = -90;
        //    //find slope 
        //    double xdiff = p1.X - p0.X;
        //    if (xdiff == 0)
        //    {

        //        PointF cutP = FindCutPoint(p0, p1, new PointF(mdown_x, mdown_y), cutAngle);
        //        g.FillRectangle(Brushes.Magenta,
        //             new RectangleF((float)cutP.X, (float)cutP.Y, 5, 5));

        //        g.DrawLine(Pens.Red, new PointF((float)cutP.X, (float)cutP.Y),
        //            new PointF(mdown_x, mdown_y));

        //    }
        //    else
        //    {
        //        double m1 = (p1.Y - p0.Y) / (xdiff);
        //        //b = y- mx ... (2) 
        //        double b1 = p0.Y - m1 * p0.X;
        //        //-------------------------------------
        //        double angle = Math.Atan2(p1.Y - p0.Y, xdiff); //rad
        //        double angle2 = DegreesToRadians((RadiansToDegrees(angle) + cutAngle));

        //        //double m2 = -1 / m1;
        //        double m3 = Math.Tan(angle2);
        //        double m2 = m3;

        //        //-------------------------------------
        //        //y = mx+b
        //        for (int i = 30; i < 200; i++)
        //        {
        //            PointF testY = new PointF(i, (float)((m1 * i) + b1));
        //            g.FillRectangle(Brushes.Blue, new RectangleF(testY, new SizeF(2, 2)));
        //        }
        //        //-------------------------------------
        //        //perpendicular line

        //        for (int n = 1; n <= 3; ++n)
        //        {
        //            for (int i = 30; i < 200; i++)
        //            {
        //                PointF testY = new PointF(i, (float)(((m1) * i) + b1 + n * 100));
        //                g.FillRectangle(Brushes.Blue, new RectangleF(testY, new SizeF(2, 2)));
        //            }
        //        }
        //        //-------------------------------------

        //        g.FillRectangle(Brushes.Red, new RectangleF(mdown_x, mdown_y, 3, 3));
        //        //-------------------------------------
        //        //
        //        //find b2
        //        //
        //        double b2 = mdown_y - (m2) * mdown_x;
        //        for (int i = 30; i < 200; i++)
        //        {
        //            PointF testY = new PointF(i, (float)(((m2) * i) + b2));
        //            g.FillRectangle(Brushes.Green, new RectangleF(testY, new SizeF(2, 2)));
        //        }
        //        //find cut point
        //        double cutx = (b2 - b1) / (m1 - m2);
        //        //g.DrawLine(Pens.DeepPink,
        //        //    new PointF((float)cutx, 0),
        //        //    new PointF((float)cutx, 400));
        //        double cuty = (m1 * cutx) + b1;
        //        //g.DrawLine(Pens.DeepPink,
        //        //  new PointF((float)0, (float)cuty),
        //        //  new PointF((float)400, (float)cuty));
        //        //g.FillRectangle(Brushes.Magenta,
        //        //    new RectangleF((float)cutx, (float)cuty, 5, 5));

        //        PointF cutP = FindCutPoint(p0, p1, new PointF(mdown_x, mdown_y), cutAngle);
        //        g.FillRectangle(Brushes.Magenta,
        //             new RectangleF((float)cutP.X, (float)cutP.Y, 5, 5));

        //    }
        //}

        //static PointF FindCutPoint(PointF p0, PointF p1, PointF p2, float cutAngle)
        //{
        //    //a line from p0 to p1
        //    //p2 is any point
        //    //return p3 -> cutpoint on p0,p1

        //    //from line equation
        //    //y = mx + b ... (1)
        //    //from (1)
        //    //b = y- mx ... (2) 
        //    //----------------------------------
        //    //line1:
        //    //y1 = (m1 * x1) + b1 ...(3)            
        //    //line2:
        //    //y2 = (m2 * x2) + b2 ...(4)
        //    //----------------------------------
        //    //from (3),
        //    //b1 = y1 - (m1 * x1) ...(5)
        //    //b2 = y2 - (m2 * x2) ...(6)
        //    //----------------------------------
        //    //y1diff = p1.Y-p0.Y  ...(7)
        //    //x1diff = p1.X-p0.X  ...(8)
        //    //
        //    //m1 = (y1diff/x1diff) ...(9)
        //    //m2 = cutAngle of m1 ...(10)
        //    //
        //    //replace value (x1,y1) and (x2,y2)
        //    //we know b1 and b2         
        //    //----------------------------------              
        //    //at cutpoint of line1 and line2 => (x1,y1)== (x2,y2)
        //    //or find (x,y) where (3)==(4)
        //    //---------------------------------- 
        //    //at cutpoint, find x
        //    // (m1 * x1) + b1 = (m2 * x1) + b2  ...(11), replace x2 with x1
        //    // (m1 * x1) - (m2 * x1) = b2 - b1  ...(12)
        //    //  x1 * (m1-m2) = b2 - b1          ...(13)
        //    //  x1 = (b2-b1)/(m1-m2)            ...(14), now we know x1
        //    //---------------------------------- 
        //    //at cutpoint, find y
        //    //  y1 = (m1 * x1) + b1 ... (15), replace x1 with value from (14)
        //    //Ans: (x1,y1)
        //    //---------------------------------- 


        //    double x1diff = p1.X - p0.X;
        //    double y1diff = p1.Y - p0.Y;


        //    if (x1diff == 0)
        //    {
        //        //90 or 180 degree
        //        return new PointF(p1.X, p2.Y);
        //    }
        //    //------------------------------
        //    //
        //    //find slope 
        //    double m1 = y1diff / x1diff;
        //    //from (2) b = y-mx, and (5)
        //    //so ...
        //    double b1 = p0.Y - (m1 * p0.X);
        //    // 
        //    //from (10)
        //    //double invert_m = -(1 / slope_m);
        //    //double m2 = -1 / m1;   //rotate m1
        //    //---------------------
        //    double angle = Math.Atan2(y1diff, x1diff); //rad in degree 
        //                                               //double m2 = -1 / m1;

        //    double m2 = cutAngle == 90 ?
        //        //short cut
        //        (-1 / m1) :
        //        //or 
        //        Math.Tan(
        //        //radial_angle of original line + radial of cutAngle
        //        //return new line slope
        //        Math.Atan2(y1diff, x1diff) +
        //        DegreesToRadians(cutAngle)); //new m 
        //    //---------------------


        //    //from (6)
        //    double b2 = p2.Y - (m2) * p2.X;
        //    //find cut point

        //    //check if (m1-m2 !=0)
        //    double cutx = (b2 - b1) / (m1 - m2); //from  (14)
        //    double cuty = (m1 * cutx) + b1;  //from (15)
        //    return new PointF((float)cutx, (float)cuty);


        //    //------
        //    //at cutpoint of line1 and line2 => (x1,y1)== (x2,y2)
        //    //or find (x,y) where (3)==(4)
        //    //-----
        //    //if (3)==(4)
        //    //(m1 * x1) + b1 = (m2 * x2) + b2;
        //    //from given p0 and p1,
        //    //now we know m1 and b1, ( from (2),  b1 = y1-(m1*x1) )
        //    //and we now m2 since => it is a 90 degree of m1.
        //    //and we also know x2, since at the cut point x2 also =x1
        //    //now we can find b2...
        //    // (m1 * x1) + b1 = (m2 * x1) + b2  ...(5), replace x2 with x1
        //    // b2 = (m1 * x1) + b1 - (m2 * x1)  ...(6), move  (m2 * x1)
        //    // b2 = ((m1 - m2) * x1) + b1       ...(7), we can find b2
        //    //---------------------------------------------
        //}
    }
}
