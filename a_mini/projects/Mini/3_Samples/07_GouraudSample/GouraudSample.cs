//2014 BSD,WinterDev
//MatterHackers

using System;
using System.Diagnostics;

using MatterHackers.Agg.Image;
using MatterHackers.Agg.UI;
using MatterHackers.Agg.VertexSource;


using Mini;
namespace MatterHackers.Agg.Sample_Gouraud
{
    [Info(OrderCode = "07")]
    [Info("Gouraud shading. It's a simple method of interpolating colors in a triangle. There's no 'cube' drawn"
                + ", there're just 6 triangles. You define a triangle and colors in its vertices. When rendering, the "
                + "colors will be linearly interpolated. But there's a problem that appears when drawing adjacent "
                + "triangles with Anti-Aliasing. Anti-Aliased polygons do not 'dock' to each other correctly, there "
                + "visual artifacts at the edges appear. I call it “the problem of adjacent edges”. AGG has a simple"
                + " mechanism that allows you to get rid of the artifacts, just dilating the polygons and/or changing "
                + "the gamma-correction value. But it's tricky, because the values depend on the opacity of the polygons."
                + " In this example you can change the opacity, the dilation value and gamma. Also you can drag the "
                + "Red, Green and Blue corners of the “cube”.")]
    public class gouraud_application : DemoBase
    {
        double[] m_x = new double[3];
        double[] m_y = new double[3];
        double m_dx;
        double m_dy;
        int m_idx;

        Stopwatch stopwatch = new Stopwatch();

        public gouraud_application()
        {

            m_idx = (-1);
            m_x[0] = 57; m_y[0] = 60;
            m_x[1] = 369; m_y[1] = 170;
            m_x[2] = 143; m_y[2] = 310;

            this.DilationValue = 0.175;
            this.LinearGamma = 0.809;
            this.AlphaValue = 1;
        }

        [DemoConfig(MaxValue = 1)]
        public double DilationValue
        {
            get;
            set;
        }

        [DemoConfig(MaxValue = 1)]
        public double LinearGamma
        {
            get;
            set;
        }
        [DemoConfig(MaxValue = 1)]
        public double AlphaValue
        {
            get;
            set;
        }
        //template<class Scanline, class Ras> 
        public void render_gouraud(IImage backBuffer, Scanline sl, ScanlineRasterizer ras)
        {
            double alpha = this.AlphaValue;
            double brc = 1;
            Graphics2D graphics2D = NewGraphics2D();

#if SourceDepth24
            pixfmt_alpha_blend_rgb pf = new pixfmt_alpha_blend_rgb(backBuffer, new blender_bgr());
#else
            var image = new ChildImage(backBuffer, new BlenderBGRA());
#endif
            ClipProxyImage ren_base = new ClipProxyImage(image);

            //span_allocator span_alloc = new span_allocator();
            SpanGouraudRGBA span_gen = new SpanGouraudRGBA();

            ras.ResetGamma(new GammaLinear(0.0, this.LinearGamma));

            double d = this.DilationValue;

            // Six triangles
            double xc = (m_x[0] + m_x[1] + m_x[2]) / 3.0;
            double yc = (m_y[0] + m_y[1] + m_y[2]) / 3.0;

            double x1 = (m_x[1] + m_x[0]) / 2 - (xc - (m_x[1] + m_x[0]) / 2);
            double y1 = (m_y[1] + m_y[0]) / 2 - (yc - (m_y[1] + m_y[0]) / 2);

            double x2 = (m_x[2] + m_x[1]) / 2 - (xc - (m_x[2] + m_x[1]) / 2);
            double y2 = (m_y[2] + m_y[1]) / 2 - (yc - (m_y[2] + m_y[1]) / 2);

            double x3 = (m_x[0] + m_x[2]) / 2 - (xc - (m_x[0] + m_x[2]) / 2);
            double y3 = (m_y[0] + m_y[2]) / 2 - (yc - (m_y[0] + m_y[2]) / 2);


            span_gen.SetColor(ColorRGBAf.MakeColorRGBA(1, 0, 0, alpha),
                              ColorRGBAf.MakeColorRGBA(0, 1, 0, alpha),
                              ColorRGBAf.MakeColorRGBA(brc, brc, brc, alpha));

            span_gen.SetTriangle(m_x[0], m_y[0], m_x[1], m_y[1], xc, yc, d);
            ras.AddPath(new VertexStoreSnap(span_gen.MakeVxs()));

            ScanlineRasToDestBitmapRenderer sclineRasToBmp = new ScanlineRasToDestBitmapRenderer();
            sclineRasToBmp.GenerateAndRender(ren_base, ras, sl, span_gen);


            span_gen.SetColor(ColorRGBAf.MakeColorRGBA(0, 1, 0, alpha),
                              ColorRGBAf.MakeColorRGBA(0, 0, 1, alpha),
                             ColorRGBAf.MakeColorRGBA(brc, brc, brc, alpha));

            span_gen.SetTriangle(m_x[1], m_y[1], m_x[2], m_y[2], xc, yc, d);
            ras.AddPath(new VertexStoreSnap(span_gen.MakeVxs()));
            sclineRasToBmp.GenerateAndRender(ren_base, ras, sl, span_gen);


            span_gen.SetColor(ColorRGBAf.MakeColorRGBA(0, 0, 1, alpha),
                            ColorRGBAf.MakeColorRGBA(1, 0, 0, alpha),
                            ColorRGBAf.MakeColorRGBA(brc, brc, brc, alpha));
            span_gen.SetTriangle(m_x[2], m_y[2], m_x[0], m_y[0], xc, yc, d);
            ras.AddPath(new VertexStoreSnap(span_gen.MakeVxs()));
            sclineRasToBmp.GenerateAndRender(ren_base, ras, sl, span_gen);


            brc = 1 - brc;
            span_gen.SetColor(ColorRGBAf.MakeColorRGBA(1, 0, 0, alpha),
                            ColorRGBAf.MakeColorRGBA(0, 1, 0, alpha),
                           ColorRGBAf.MakeColorRGBA(brc, brc, brc, alpha));
            span_gen.SetTriangle(m_x[0], m_y[0], m_x[1], m_y[1], x1, y1, d);
            ras.AddPath(new VertexStoreSnap(span_gen.MakeVxs()));
            sclineRasToBmp.GenerateAndRender(ren_base, ras, sl, span_gen);


            span_gen.SetColor(ColorRGBAf.MakeColorRGBA(0, 1, 0, alpha),
                           ColorRGBAf.MakeColorRGBA(0, 0, 1, alpha),
                           ColorRGBAf.MakeColorRGBA(brc, brc, brc, alpha));
            span_gen.SetTriangle(m_x[1], m_y[1], m_x[2], m_y[2], x2, y2, d);

            ras.AddPath(new VertexStoreSnap(span_gen.MakeVxs()));
            sclineRasToBmp.GenerateAndRender(ren_base, ras, sl, span_gen);


            span_gen.SetColor(ColorRGBAf.MakeColorRGBA(0, 0, 1, alpha),
                            ColorRGBAf.MakeColorRGBA(1, 0, 0, alpha),
                            ColorRGBAf.MakeColorRGBA(brc, brc, brc, alpha));
            span_gen.SetTriangle(m_x[2], m_y[2], m_x[0], m_y[0], x3, y3, d);

            ras.AddPath(new VertexStoreSnap(span_gen.MakeVxs()));
            sclineRasToBmp.GenerateAndRender(ren_base, ras, sl, span_gen);
        }
        public override void Draw(Graphics2D g)
        {
            OnDraw(g);
        }
        public void OnDraw(Graphics2D graphics2D)
        {
            var widgetsSubImage = ImageHelper.CreateChildImage(graphics2D.DestImage, graphics2D.GetClippingRect());

            IImage backBuffer = widgetsSubImage;
#if SourceDepth24
            pixfmt_alpha_blend_rgb pf = new pixfmt_alpha_blend_rgb(backBuffer, new blender_bgr());
#else
            var pf = new ChildImage(backBuffer, new BlenderBGRA());
#endif
            ClipProxyImage ren_base = new ClipProxyImage(pf);
            ren_base.Clear(new ColorRGBAf(1.0, 1.0, 1.0).ToColorRGBA());

            ScanlineUnpacked8 sl = new ScanlineUnpacked8();
            ScanlineRasterizer ras = new ScanlineRasterizer();
#if true
            render_gouraud(backBuffer, sl, ras);
#else
            agg.span_allocator span_alloc = new span_allocator();
            span_gouraud_rgba span_gen = new span_gouraud_rgba(new rgba8(255, 0, 0, 255), new rgba8(0, 255, 0, 255), new rgba8(0, 0, 255, 255), 320, 220, 100, 100, 200, 100, 0);
            span_gouraud test_sg = new span_gouraud(new rgba8(0, 0, 0, 255), new rgba8(0, 0, 0, 255), new rgba8(0, 0, 0, 255), 320, 220, 100, 100, 200, 100, 0);
            ras.add_path(test_sg);
            renderer_scanlines.render_scanlines_aa(ras, sl, ren_base, span_alloc, span_gen);
            //renderer_scanlines.render_scanlines_aa_solid(ras, sl, ren_base, new rgba8(0, 0, 0, 255));
#endif


            ras.ResetGamma(new GammaNone());
            //m_dilation.Render(ras, sl, ren_base);
            //m_gamma.Render(ras, sl, ren_base);
            //m_alpha.Render(ras, sl, ren_base);

        }

        public override void MouseDown(int mx, int my, bool isRightButton)
        {
            int i;
            if (isRightButton)
            {
                ScanlineUnpacked8 sl = new ScanlineUnpacked8();
                ScanlineRasterizer ras = new ScanlineRasterizer();
                //stopwatch.Restart();
                stopwatch.Stop();
                stopwatch.Reset();
                stopwatch.Start();
                for (i = 0; i < 100; i++)
                {
                    //render_gouraud(sl, ras);
                }

                stopwatch.Stop();
                string buf;
                buf = "Time=" + stopwatch.ElapsedMilliseconds.ToString() + "ms";
                throw new NotImplementedException();
                //guiSurface.ShowSystemMessage(buf);
            }

            if (!isRightButton)
            {
                double x = mx;
                double y = my;

                for (i = 0; i < 3; i++)
                {
                    if (Math.Sqrt((x - m_x[i]) * (x - m_x[i]) + (y - m_y[i]) * (y - m_y[i])) < 10.0)
                    {
                        m_dx = x - m_x[i];
                        m_dy = y - m_y[i];
                        m_idx = (int)i;
                        break;
                    }
                }
                if (i == 3)
                {
                    if (AggMath.point_in_triangle(m_x[0], m_y[0],
                                              m_x[1], m_y[1],
                                              m_x[2], m_y[2],
                                              x, y))
                    {
                        m_dx = x - m_x[0];
                        m_dy = y - m_y[0];
                        m_idx = 3;
                    }

                }
            }
        }

        public override void MouseDrag(int mx, int my)
        {
            double x = mx;
            double y = my;

            if (m_idx == 3)
            {
                double dx = x - m_dx;
                double dy = y - m_dy;
                m_x[1] -= m_x[0] - dx;
                m_y[1] -= m_y[0] - dy;
                m_x[2] -= m_x[0] - dx;
                m_y[2] -= m_y[0] - dy;
                m_x[0] = dx;
                m_y[0] = dy;

            }
            else if (m_idx >= 0)
            {
                m_x[m_idx] = x - m_dx;
                m_y[m_idx] = y - m_dy;
            }

        }
        public override void MouseUp(int x, int y)
        {
            m_idx = -1;
        }



    }


}




