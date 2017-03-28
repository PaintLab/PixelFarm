//MIT, 2017, WinterDev
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
//
using PixelFarm.DrawingGL;
using Typography.OpenFont;
using Typography.Rendering;
using DrawingGL;
using DrawingGL.Text;

namespace BuildTextureFonts
{
    public partial class FormTess : Form
    {
        Graphics g;
        float[] glyphPoints2;
        TessTool tessTool = new TessTool();
        public FormTess()
        {
            InitializeComponent();
        }
        private void FormTess_Load(object sender, EventArgs e)
        {
            g = CreateGraphics();

            string testFont = "d:\\WImageTest\\DroidSans.ttf";
            using (FileStream fs = new FileStream(testFont, FileMode.Open, FileAccess.Read))
            {
                OpenFontReader reader = new OpenFontReader();
                Typeface typeface = reader.Read(fs);
                Glyph glyph = typeface.GetGlyphByIndex(typeface.LookupIndex('G'));
                //--
                GlyphPointF[] glyphPoints = glyph.GlyphPoints;
                //--
                var builder = new Typography.Rendering.GlyphPathBuilder(typeface);
                builder.BuildFromGlyphIndex(typeface.LookupIndex('G'), 256);

                var txToPath = new GlyphTranslatorToPath();
                var writablePath = new WritablePath();
                txToPath.SetOutput(writablePath);
                builder.ReadShapes(txToPath);
                //from contour to  
                var curveFlattener = new SimpleCurveFlattener();
                float[] flattenPoints = curveFlattener.Flatten(writablePath._points);

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

        float[] GetPolygonData()
        {
            return glyphPoints2;

            return new float[]
            {
                    10,10,
                    200,10,
                    100,100,
                    150,200,
                    20,200,
                    50,100
            };
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //show tess
            g.Clear(Color.White);

            float[] polygon1 = GetPolygonData();

            int nn = polygon1.Length;
            using (Pen pen1 = new Pen(Color.LightGray, 6))
            {
                int a = 0;
                PointF p0;
                PointF p1;
                for (int m = 2; m < nn;)
                {

                    p0 = new PointF(polygon1[m - 2], polygon1[m - 1]);
                    p1 = new PointF(polygon1[m], polygon1[m + 1]);

                    g.DrawLine(pen1, p0, p1);

                    //-----
                    g.DrawString(a.ToString(), this.Font, Brushes.Black, p0);


                    m += 2;
                    a++;
                }
                //-------
                //close polygon 
                p0 = new PointF(polygon1[nn - 2], polygon1[nn - 1]);
                p1 = new PointF(polygon1[0], polygon1[1]);
                g.DrawLine(pen1, p0, p1);
                g.DrawString(a.ToString(), this.Font, Brushes.Black, p0);

                //-------
            }


            int areaCount;
            float[] tessData = tessTool.TessPolygon(polygon1, out areaCount);
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
        }


    }
}
