//BSD, 2014-2018, WinterDev

using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TessTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MatterHackers.Agg.Tests.TesselatorTests tt = new MatterHackers.Agg.Tests.TesselatorTests();
            tt.MatchesGLUTesselator();
        }


        private void button2_Click(object sender, EventArgs e)
        {


            var alist = new Tesselate.MaxFirstList<int>();
            alist.Add(16);
            alist.Add(4);
            alist.Add(14);
            alist.Add(7);

            alist.Add(9);
            alist.Add(3);
            alist.Add(2);
            alist.Add(8);
            alist.Add(1);

            alist.Add(16);
            alist.Add(4);
            alist.Add(14);
            alist.Add(7);

            alist.Add(9);
            alist.Add(3);
            alist.Add(2);
            alist.Add(8);
            alist.Add(1);

            var a = alist.FindMin();
        }

        void SaveToImage(string filename, List<Vertex> m_VertexList,
            float scaleImg = 1,
            int translateX = 0,
            int translateY = 0)
        {
            // ---------------
            // test
            //------------------ 
            int a = m_VertexList.Count;
            //test draw
            Bitmap bmp = new Bitmap(300, 300);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.White);
            int j = m_VertexList.Count;


            for (int i = 0; i < j; i++)
            {
                Vertex v = m_VertexList[i];
                m_VertexList[i] = new Vertex((v.m_X * scaleImg) + translateX, (v.m_Y * scaleImg) + translateY);
            }

            int lim = j - 2;

            for (int i = 0; i < lim;)
            {
                var v0 = m_VertexList[i];
                var v1 = m_VertexList[i + 1];
                var v2 = m_VertexList[i + 2];

                g.DrawLine(Pens.Black,
                    new PointF((float)v0.m_X, (float)v0.m_Y),
                    new PointF((float)v1.m_X, (float)v1.m_Y));
                g.DrawLine(Pens.Black,
                    new PointF((float)v1.m_X, (float)v1.m_Y),
                    new PointF((float)v2.m_X, (float)v2.m_Y));
                g.DrawLine(Pens.Black,
                    new PointF((float)v2.m_X, (float)v2.m_Y),
                    new PointF((float)v0.m_X, (float)v0.m_Y));
                i += 3;

            }
            g.Dispose();

            this.pictureBox1.Image = bmp;
            if (filename != null)
            {
                bmp.Save(filename);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //ref: http://www.songho.ca/opengl/gl_tessellation.html 
            //==============================================================================
            //song ho's tessellation examples 1
            //==============================================================================

            // // define concave quad data (vertices only)
            ////  0    2
            ////  \ \/ /
            ////   \3 /
            ////    \/
            ////    1
            //GLdouble quad1[4][3] = { {-1,3,0}, {0,0,0}, {1,3,0}, {0,2,0} };

            //// register callback functions
            //gluTessCallback(tess, GLU_TESS_BEGIN, (void (CALLBACK *)())tessBeginCB);
            //gluTessCallback(tess, GLU_TESS_END, (void (CALLBACK *)())tessEndCB);
            //gluTessCallback(tess, GLU_TESS_ERROR, (void (CALLBACK *)())tessErrorCB);
            //gluTessCallback(tess, GLU_TESS_VERTEX, (void (CALLBACK *)())tessVertexCB);

            //// tessellate and compile a concave quad into display list
            //// gluTessVertex() takes 3 params: tess object, pointer to vertex coords,
            //// and pointer to vertex data to be passed to vertex callback.
            //// The second param is used only to perform tessellation, and the third
            //// param is the actual vertex data to draw. It is usually same as the second
            //// param, but It can be more than vertex coord, for example, color, normal
            //// and UV coords which are needed for actual drawing.
            //// Here, we are looking at only vertex coods, so the 2nd and 3rd params are
            //// pointing same address.
            //glNewList(id, GL_COMPILE);
            //glColor3f(1,1,1);
            //gluTessBeginPolygon(tess, 0);                   // with NULL data
            //    gluTessBeginContour(tess);
            //        gluTessVertex(tess, quad1[0], quad1[0]);
            //        gluTessVertex(tess, quad1[1], quad1[1]);
            //        gluTessVertex(tess, quad1[2], quad1[2]);
            //        gluTessVertex(tess, quad1[3], quad1[3]);
            //    gluTessEndContour(tess);
            //gluTessEndPolygon(tess);
            //glEndList();

            TessTest.TessListener t01 = new TessTest.TessListener();
            Tesselate.Tesselator tess = new Tesselate.Tesselator();

            List<Vertex> vertexts = new List<Vertex>()
            {
                new Vertex(-1, 3),
                new Vertex(0, 0),
                new Vertex(1 ,3),
                new Vertex(0 ,2),
            };

            t01.Connect(vertexts, tess, Tesselate.Tesselator.WindingRuleType.Odd, true);


            tess.BeginPolygon();
            tess.BeginContour();

            int j = vertexts.Count;
            for (int i = 0; i < j; ++i)
            {
                Vertex v = vertexts[i];
                tess.AddVertex(v.m_X, v.m_Y, 0, i);
            }
            tess.EndContour();
            tess.EndPolygon();

            //---------------------------
            //save final images
            SaveToImage(null, t01.resultVertexList, 10, 20, 20);
            //---------------------------

        }
        private void button4_Click(object sender, EventArgs e)
        {
            //ref: http://www.songho.ca/opengl/gl_tessellation.html 
            //==============================================================================
            //song ho's tessellation examples 2
            //==============================================================================
            // define concave quad with a hole
            //  0--------3
            //  | 4----7 |
            //  | |    | |
            //  | 5----6 |
            //  1--------2
            //GLdouble quad2[8][3] = { {-2,3,0}, {-2,0,0}, {2,0,0}, { 2,3,0},
            //                         {-1,2,0}, {-1,1,0}, {1,1,0}, { 1,2,0} }; 
            //gluTessCallback(tess, GLU_TESS_BEGIN, (void (__stdcall*)(void))tessBeginCB);
            //gluTessCallback(tess, GLU_TESS_END, (void (__stdcall*)(void))tessEndCB);
            //gluTessCallback(tess, GLU_TESS_ERROR, (void (__stdcall*)(void))tessErrorCB);
            //gluTessCallback(tess, GLU_TESS_VERTEX, (void (__stdcall*)())tessVertexCB);

            //// tessellate and compile a concave quad into display list
            //glNewList(id, GL_COMPILE);
            //glColor3f(1,1,1);
            //gluTessBeginPolygon(tess, 0);                       // with NULL data
            //    gluTessBeginContour(tess);                      // outer quad
            //        gluTessVertex(tess, quad2[0], quad2[0]);
            //        gluTessVertex(tess, quad2[1], quad2[1]);
            //        gluTessVertex(tess, quad2[2], quad2[2]);
            //        gluTessVertex(tess, quad2[3], quad2[3]);
            //    gluTessEndContour(tess);
            //    gluTessBeginContour(tess);                      // inner quad (hole)
            //        gluTessVertex(tess, quad2[4], quad2[4]);
            //        gluTessVertex(tess, quad2[5], quad2[5]);
            //        gluTessVertex(tess, quad2[6], quad2[6]);
            //        gluTessVertex(tess, quad2[7], quad2[7]);
            //    gluTessEndContour(tess);
            //gluTessEndPolygon(tess);
            //glEndList();
            //============================================================================== 

            TessTest.TessListener t01 = new TessTest.TessListener();
            Tesselate.Tesselator tess = new Tesselate.Tesselator();
            List<Vertex> vertexts = new List<Vertex>()
            {
                new Vertex(-2,3),
                new Vertex(-2,0),
                new Vertex(2,0),
                new Vertex(2,3),               
                //------------------
                new Vertex(-1,2),
                new Vertex(-1,1),
                new Vertex(1,1),
                new Vertex(1,2),
            };
            //------------------

            t01.Connect(vertexts, tess, Tesselate.Tesselator.WindingRuleType.Odd, true);

            //polygon1 
            tess.BeginPolygon();
            //------------------------------------
            //contour1       
            tess.BeginContour();
            int start_at = 0;
            int endBefore = start_at + 4;
            for (int i = start_at; i < endBefore; ++i)
            {
                Vertex v = vertexts[i];
                tess.AddVertex(v.m_X, v.m_Y, 0, i);
            }
            tess.EndContour();
            //------------------------------------
            //contour 2
            tess.BeginContour();
            start_at = 4;
            endBefore = vertexts.Count;
            for (int i = start_at; i < endBefore; ++i)
            {
                Vertex v = vertexts[i];
                tess.AddVertex(v.m_X, v.m_Y, 0, i);
            }
            tess.EndContour();


            tess.EndPolygon();
            //---------------------------
            //save final images
            SaveToImage(null, t01.resultVertexList, 10, 20, 20);
            //--------------------------- 
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //ref: http://www.songho.ca/opengl/gl_tessellation.html 
            //==============================================================================
            //song ho's tessellation examples 2
            //==============================================================================
            //          // define self-intersecting star shape (with color)
            ////      0
            ////     / \
            ////3---+---+---2
            ////  \ |   | /
            ////   \|   |/
            ////    +   +
            ////    |\ /|
            ////    | + |
            ////    |/ \|
            ////    1   4
            //GLdouble star[5][6] = { { 0.0, 3.0, 0,  1, 0, 0},       // 0: x,y,z,r,g,b
            //                        {-1.0, 0.0, 0,  0, 1, 0},       // 1:
            //                        { 1.6, 1.9, 0,  1, 0, 1},       // 2:
            //                        {-1.6, 1.9, 0,  1, 1, 0},       // 3:
            //                        { 1.0, 0.0, 0,  0, 0, 1} };     // 4:

            //// register callback functions
            //// This polygon is self-intersecting, so GLU_TESS_COMBINE callback function
            //// must be registered. The combine callback will process the intersecting vertices.
            //gluTessCallback(tess, GLU_TESS_BEGIN, (void (__stdcall*)(void))tessBeginCB);
            //gluTessCallback(tess, GLU_TESS_END, (void (__stdcall*)(void))tessEndCB);
            //gluTessCallback(tess, GLU_TESS_ERROR, (void (__stdcall*)(void))tessErrorCB);
            //gluTessCallback(tess, GLU_TESS_VERTEX, (void (__stdcall*)(void))tessVertexCB2);
            //gluTessCallback(tess, GLU_TESS_COMBINE, (void (__stdcall*)(void))tessCombineCB);

            //// tessellate and compile a concave quad into display list
            //// Pay attention to winding rules if multiple contours are overlapped.
            //// The winding rules determine which parts of polygon will be filled(interior)
            //// or not filled(exterior). For each enclosed region partitioned by multiple
            //// contours, tessellator assigns a winding number to the region by using
            //// given winding rule. The default winding rule is GLU_TESS_WINDING_ODD,
            //// but, here we are using non-zero winding rule to fill the middle area.
            //// BTW, the middle region will not be filled with the odd winding rule.
            //gluTessProperty(tess, GLU_TESS_WINDING_RULE, GLU_TESS_WINDING_NONZERO);
            //glNewList(id, GL_COMPILE);
            //gluTessBeginPolygon(tess, 0);                   // with NULL data
            //    gluTessBeginContour(tess);
            //        gluTessVertex(tess, star[0], star[0]);
            //        gluTessVertex(tess, star[1], star[1]);
            //        gluTessVertex(tess, star[2], star[2]);
            //        gluTessVertex(tess, star[3], star[3]);
            //        gluTessVertex(tess, star[4], star[4]);
            //    gluTessEndContour(tess);
            //gluTessEndPolygon(tess);
            //glEndList(); 
            //gluDeleteTess(tess);        // safe to delete after tessellation

            //======================================================
            //GLdouble star[5][6] = { { 0.0, 3.0, 0,  1, 0, 0},       // 0: x,y,z,r,g,b
            //                        {-1.0, 0.0, 0,  0, 1, 0},       // 1:
            //                        { 1.6, 1.9, 0,  1, 0, 1},       // 2:
            //                        {-1.6, 1.9, 0,  1, 1, 0},       // 3:
            //                        { 1.0, 0.0, 0,  0, 0, 1} };     // 4:

            TessTest.TessListener t01 = new TessTest.TessListener();
            Tesselate.Tesselator tess = new Tesselate.Tesselator();
            List<Vertex> vertexts = new List<Vertex>()
            { 
                //use only coord (not include rgb color)
                new Vertex(0,3),
                new Vertex(-1,0),
                new Vertex(1.6,1.9),
                new Vertex(-1.6,1.9),
                new Vertex(1,0)
            };
            //------------------

            t01.Connect(vertexts, tess, Tesselate.Tesselator.WindingRuleType.NonZero, true);

            //polygon1 
            tess.BeginPolygon();
            //------------------------------------
            //contour1       
            tess.BeginContour();
            int start_at = 0;
            int endBefore = vertexts.Count;
            for (int i = start_at; i < endBefore; ++i)
            {
                Vertex v = vertexts[i];
                tess.AddVertex(v.m_X, v.m_Y, 0, i);
            }
            tess.EndContour();
            //------------------------------------

            tess.EndPolygon();
            //---------------------------
            //save final images
            SaveToImage(null, t01.resultVertexList, 10, 20, 20);
            //--------------------------- 
        }



        struct BorderVertices
        {

            public PointF A;
            public PointF B;
            public PointF C;
            public PointF D;
            //
            public PointF P;
            public PointF Q;
            public PointF R;
            public PointF S;
            //
            public PointF P1;
            public PointF Q1;
            public PointF R1;
            public PointF S1;

        }
        class SimpleRectBorderBuilder
        {
            //sum of both inner-outer border***
            public float LeftBorderWidth { get; set; }
            public float TopBorderHeight { get; set; }
            public float RightBorderWidth { get; set; }
            public float BottomBorderHeight { get; set; }

            public void SetBorderWidth(float leftBorderW, float topBorderH, float rightBorderW, float bottomBorderH)
            {
                LeftBorderWidth = leftBorderW;
                TopBorderHeight = topBorderH;
                RightBorderWidth = rightBorderW;
                BottomBorderHeight = bottomBorderH;
            }
            public void SetBorderWidth(float allside)
            {
                LeftBorderWidth =
                TopBorderHeight =
                RightBorderWidth =
                BottomBorderHeight = allside;
            }
            public void BuilderBorderForRect(float left, float top, float width, float height, out BorderVertices borderVerices)
            {
                borderVerices = new BorderVertices();
                //outer vertices
                borderVerices.A = new PointF(left, top);
                borderVerices.B = new PointF(left + width, top);
                borderVerices.C = new PointF(left + width, top + height);
                borderVerices.D = new PointF(left, top + height);
                //----------
                //inner vertices
                borderVerices.P = new PointF(left + LeftBorderWidth, top + TopBorderHeight);
                //borderVerices.P1 = new PointF(left, top + TopBorderHeight);
                //
                borderVerices.Q = new PointF(left + width - RightBorderWidth, top + TopBorderHeight);
                // borderVerices.Q1 = new PointF(left + width, top + TopBorderHeight);
                //
                borderVerices.R = new PointF(left + width - RightBorderWidth, top + height - BottomBorderHeight);
                //borderVerices.R1 = new PointF(left + width, top + height - BottomBorderHeight);
                //
                borderVerices.S = new PointF(left + LeftBorderWidth, top + height - BottomBorderHeight);
                // borderVerices.S1 = new PointF(left, top + height - BottomBorderHeight);
                //------------ 
            }
            static void AppendCoords(PointF p0, List<float> output)
            {
                output.Add(p0.X);
                output.Add(p0.Y);
            }
            static void AppendCoords(PointF p0, PointF p1, PointF p2, List<float> output)
            {
                output.Add(p0.X);
                output.Add(p0.Y);
                //
                output.Add(p1.X);
                output.Add(p1.Y);
                //
                output.Add(p2.X);
                output.Add(p2.Y);
            }
            public void BuilderBorderForRect(float left, float top, float width, float height, out BorderVertices b, out float[] coords)
            {
                //left,top,width,
                b = new BorderVertices();
                //outer vertices
                b.A = new PointF(left, top);
                b.B = new PointF(left + width, top);
                b.C = new PointF(left + width, top + height);
                b.D = new PointF(left, top + height);
                //----------
                //inner vertices
                b.P = new PointF(left + LeftBorderWidth, top + TopBorderHeight);
                //b.P1 = new PointF(left, top + TopBorderHeight);
                //
                b.Q = new PointF(left + width - RightBorderWidth, top + TopBorderHeight);
                //b.Q1 = new PointF(left + width, top + TopBorderHeight);
                //
                b.R = new PointF(left + width - RightBorderWidth, top + height - BottomBorderHeight);
                //b.R1 = new PointF(left + width - RightBorderWidth, top + height);
                //
                b.S = new PointF(left + LeftBorderWidth, top + height - BottomBorderHeight);
                //b.S1 = new PointF(left, top + height - BottomBorderHeight);
                //------------ 

                List<float> coordList = new List<float>();
                AppendCoords(b.A, coordList);
                AppendCoords(b.D, coordList);
                AppendCoords(b.C, coordList);
                AppendCoords(b.B, coordList);

                AppendCoords(b.P, coordList);
                AppendCoords(b.S, coordList);
                AppendCoords(b.R, coordList);
                AppendCoords(b.Q, coordList);

                coords = coordList.ToArray();
            }
            public void BuilderBorderForRect4(float left, float top, float width, float height, out BorderVertices b, out float[] coords)
            {
                //left,top,width,
                b = new BorderVertices();
                //outer vertices
                b.A = new PointF(left, top);
                b.B = new PointF(left + width, top);
                b.C = new PointF(left + width, top - height);
                b.D = new PointF(left, top - height);
                //----------
                //inner vertices
                b.P = new PointF(left + LeftBorderWidth, top - TopBorderHeight);
                //b.P1 = new PointF(left, top + TopBorderHeight);
                //
                b.Q = new PointF(left + width - RightBorderWidth, top - TopBorderHeight);
                //b.Q1 = new PointF(left + width, top + TopBorderHeight);
                //
                b.R = new PointF(left + width - RightBorderWidth, top - height + BottomBorderHeight);
                //b.R1 = new PointF(left + width - RightBorderWidth, top + height);
                //
                b.S = new PointF(left + LeftBorderWidth, top - height + BottomBorderHeight);
                //b.S1 = new PointF(left, top + height - BottomBorderHeight);
                //------------ 

                List<float> coordList = new List<float>();
                AppendCoords(b.A, coordList);
                AppendCoords(b.D, coordList);
                AppendCoords(b.C, coordList);
                AppendCoords(b.B, coordList);

                AppendCoords(b.P, coordList);
                AppendCoords(b.S, coordList);
                AppendCoords(b.R, coordList);
                AppendCoords(b.Q, coordList);

                coords = coordList.ToArray();
            }
            public void BuilderBorderForRect2(float left, float top, float width, float height, out BorderVertices b, out float[] coords)
            {
                b = new BorderVertices();
                //outer vertices
                b.A = new PointF(left, top);
                b.B = new PointF(left + width, top);
                b.C = new PointF(left + width, top + height);
                b.D = new PointF(left, top + height);
                //----------
                //inner vertices
                b.P = new PointF(left + LeftBorderWidth, top + TopBorderHeight);
                b.P1 = new PointF(left, top + TopBorderHeight);
                //
                b.Q = new PointF(left + width - RightBorderWidth, top + TopBorderHeight);
                b.Q1 = new PointF(left + width, top + TopBorderHeight);
                //
                b.R = new PointF(left + width - RightBorderWidth, top + height - BottomBorderHeight);
                b.R1 = new PointF(left + width - RightBorderWidth, top + height);
                //
                b.S = new PointF(left + LeftBorderWidth, top + height - BottomBorderHeight);
                b.S1 = new PointF(left, top + height - BottomBorderHeight);
                //------------ 

                List<float> coordList = new List<float>();
                AppendCoords(b.A, coordList);
                AppendCoords(b.D, coordList);
                AppendCoords(b.C, coordList);
                AppendCoords(b.B, coordList);
                //AppendCoords(b.B, coordList);
                //// 
                //AppendCoords(b.Q, coordList);
                AppendCoords(b.P, coordList);
                AppendCoords(b.Q, coordList);
                AppendCoords(b.R, coordList);
                AppendCoords(b.S, coordList);


                coords = coordList.ToArray();

            }
            public void BuilderBorderForRect1(float left, float top, float width, float height, out BorderVertices b, out float[] coords)
            {
                b = new BorderVertices();
                //outer vertices
                b.A = new PointF(left, top);
                b.B = new PointF(left + width, top);
                b.C = new PointF(left + width, top + height);
                b.D = new PointF(left, top + height);
                //----------
                //inner vertices
                b.P = new PointF(left + LeftBorderWidth, top + TopBorderHeight);
                b.P1 = new PointF(left, top + TopBorderHeight);
                //
                b.Q = new PointF(left + width - RightBorderWidth, top + TopBorderHeight);
                b.Q1 = new PointF(left + width, top + TopBorderHeight);
                //
                b.R = new PointF(left + width - RightBorderWidth, top + height - BottomBorderHeight);
                b.R1 = new PointF(left + width - RightBorderWidth, top + height);
                //
                b.S = new PointF(left + LeftBorderWidth, top + height - BottomBorderHeight);
                b.S1 = new PointF(left, top + height - BottomBorderHeight);
                //------------ 

                List<float> coordList = new List<float>();
                AppendCoords(b.A, coordList);
                AppendCoords(b.B, coordList);
                AppendCoords(b.C, coordList);
                AppendCoords(b.D, coordList);
                //

                AppendCoords(b.P, coordList);
                AppendCoords(b.S, coordList);
                AppendCoords(b.R, coordList);
                AppendCoords(b.Q, coordList);

                coords = coordList.ToArray();
                //List<float> coordList = new List<float>();
                //AppendCoords(b.A, b.P1, b.B, coordList);
                //AppendCoords(b.P1, b.B, b.Q1, coordList);

                //AppendCoords(b.Q1, b.Q, b.C, coordList);
                //AppendCoords(b.Q, b.C, b.R1, coordList);

                //AppendCoords(b.R1, b.R, b.D, coordList);
                //AppendCoords(b.R, b.D, b.S1, coordList);

                //AppendCoords(b.S1, b.S, b.P1, coordList);
                //AppendCoords(b.S, b.P1, b.P, coordList);

                //coords = coordList.ToArray();


                ////_g.DrawPolygon(Pens.Red, new PointF[]
                ////{
                ////    borderVerices.A,borderVerices.B,borderVerices.C,borderVerices.D,
                ////});
                ////_g.DrawPolygon(Pens.Red, new PointF[]
                ////{
                ////    borderVerices.P,borderVerices.Q,borderVerices.R,borderVerices.S,
                ////});
                //////---------------------------
                ////_g.DrawLine(Pens.Red, borderVerices.A, borderVerices.Q1);
                ////_g.DrawLine(Pens.Red, borderVerices.Q1, borderVerices.R);
                ////_g.DrawLine(Pens.Red, borderVerices.R1, borderVerices.D);
                ////_g.DrawLine(Pens.Red, borderVerices.S, borderVerices.P1);


            }
        }


        private void button7_Click(object sender, EventArgs e)
        {
            SimpleRectBorderBuilder builder = new SimpleRectBorderBuilder();
            builder.SetBorderWidth(5);
            //builder.RightBorderWidth = 2;
            builder.BuilderBorderForRect(0, 0, 30, 30, out BorderVertices b1, out float[] coords);
            //
            TessTest.TessListener t01 = new TessTest.TessListener();
            Tesselate.Tesselator tess = new Tesselate.Tesselator();
            List<Vertex> vertexts = new List<Vertex>();
            for (int i = 0; i < coords.Length;)
            {
                vertexts.Add(new Vertex(coords[i], coords[i + 1]));
                i += 2;
            }
            //------------------

            t01.Connect(vertexts, tess, Tesselate.Tesselator.WindingRuleType.Odd, true);

            //polygon1 
            tess.BeginPolygon();
            //------------------------------------
            //contour1       
            tess.BeginContour();
            int start_at = 0;
            int endBefore = start_at + 4;
            for (int i = start_at; i < endBefore; ++i)
            {
                Vertex v = vertexts[i];
                tess.AddVertex(v.m_X, v.m_Y, 0, i);
            }
            tess.EndContour();
            //------------------------------------
            //contour 2
            tess.BeginContour();
            start_at = 4;
            endBefore = vertexts.Count;
            for (int i = start_at; i < endBefore; ++i)
            {
                Vertex v = vertexts[i];
                tess.AddVertex(v.m_X, v.m_Y, 0, i);
            }
            tess.EndContour();


            tess.EndPolygon();
            //---------------------------
            //save final images
            SaveToImage(null, t01.resultVertexList, 1, 0, 0);
            //--------------------------- 

        }
    }
}
