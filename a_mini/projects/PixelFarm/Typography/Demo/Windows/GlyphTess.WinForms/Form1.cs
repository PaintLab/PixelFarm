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
            Test2();//update 
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
            Test2();
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
        void Test2()
        {
            g.Clear(Color.White);
            //
            PointF p0 = new PointF(50, 50);
            PointF p1 = new PointF(100, 120);
            g.DrawLine(Pens.Red, p0, p1);
            //

            //find slope 
            double m1 = (p1.Y - p0.Y) / (p1.X - p0.X);
            double b1 = FindB(p0, p1);
            //-------------------------------------

            //y = mx+b
            for (int i = 30; i < 200; i++)
            {
                PointF testY = new PointF(i, (float)((m1 * i) + b1));
                g.FillRectangle(Brushes.Blue, new RectangleF(testY, new SizeF(2, 2)));
            }
            //-------------------------------------
            //perpendicular line
            double m2 = -1 / m1;
            for (int n = 1; n <= 3; ++n)
            {
                for (int i = 30; i < 200; i++)
                {
                    PointF testY = new PointF(i, (float)(((m2) * i) + b1 + n * 100));
                    g.FillRectangle(Brushes.Blue, new RectangleF(testY, new SizeF(2, 2)));
                }
            }
            //-------------------------------------
            g.FillRectangle(Brushes.Red, new RectangleF(mdown_x, mdown_y, 3, 3));
            //-------------------------------------
            //
            //find b2
            //
            double b2 = mdown_y - (m2) * mdown_x;
            for (int i = 30; i < 200; i++)
            {
                PointF testY = new PointF(i, (float)(((m2) * i) + b2));
                g.FillRectangle(Brushes.Green, new RectangleF(testY, new SizeF(2, 2)));
            }
            //find cut point
            double cutx = (b2 - b1) / (m1 - m2);
            //g.DrawLine(Pens.DeepPink,
            //    new PointF((float)cutx, 0),
            //    new PointF((float)cutx, 400));
            double cuty = (m2 * cutx) + b2;
            //g.DrawLine(Pens.DeepPink,
            //  new PointF((float)0, (float)cuty),
            //  new PointF((float)400, (float)cuty));
            //g.FillRectangle(Brushes.Magenta,
            //    new RectangleF((float)cutx, (float)cuty, 5, 5));

            PointF cutP = FindPerpendicularCutPoint(p0, p1, new PointF(mdown_x, mdown_y));
            g.FillRectangle(Brushes.Magenta,
                 new RectangleF((float)cutP.X, (float)cutP.Y, 5, 5));


        }

        static PointF FindPerpendicularCutPoint(PointF p0, PointF p1, PointF p2)
        {
            //a line from p0 to p1
            //p2 is any point
            //return p3 -> cutpoint on p0,p1

            //find slope 
            double m1 = (p1.Y - p0.Y) / (p1.X - p0.X);
            double b1 = FindB(p0, p1);
            double m2 = -1 / m1;
            double b2 = p2.Y - (m2) * p2.X;
            //find cut point
            double cutx = (b2 - b1) / (m1 - m2);
            double cuty = (m2 * cutx) + b2;
            return new PointF((float)cutx, (float)cuty);
        }

        static double FindB(PointF p0, PointF p1)
        {
            //double invert_m = -(1 / slope_m);
            double slope_m = (p1.Y - p0.Y) / (p1.X - p0.X);
            //y = mx + b ...(1)
            //b = y- mx

            double b0 = p0.Y - (slope_m) * p0.X;
            double b1 = p1.Y - (slope_m) * p1.X;

            return b0;
        }
    }
}
