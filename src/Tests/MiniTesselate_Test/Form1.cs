//BSD 2014, WinterDev

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
           

            var alist = new MiniCollection.MaxFirstList<int>();
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

        void SaveToImage(string filename, List<Vertex> m_VertexList)
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

            //scale 10 + shift 20
            for (int i = 0; i < j; i++)
            {
                Vertex v = m_VertexList[i];
                m_VertexList[i] = new Vertex((v.m_X * 10) + 20, (v.m_Y * 10) + 20);
            }

            int lim = j - 2;

            for (int i = 0; i < lim; )
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
            SaveToImage(null, t01.resultVertexList);
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
            SaveToImage(null, t01.resultVertexList);
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
            SaveToImage(null, t01.resultVertexList);
            //--------------------------- 
        }
    }
}
